using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using MvvmFoundation.Wpf;
using QuickImageUpload.Services;
using WorkQueueLib;

namespace QuickImageUpload.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        #region Fields
        protected RelayCommand _selectImageCommand;
        protected RelayCommand _copyImageCommand;
        protected RelayCommand _pasteImageCommand;
        protected Dispatcher _dispatcher;
        protected string _notification;
        private RelayCommand _aboutCommand; 
        #endregion

        #region Properties
        public string Notification
        {
            get { return _notification; }
            set
            {
                _notification = value;
                RaisePropertyChanged("Notification");
            }
        }

        public WorkQueue<string, UploadedImage> UploadQueue { get; protected set; }
        public WorkItem<string, UploadedImage> SelectedWorkItem { get; set; } 
        #endregion

        public ShellViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Notification = "Please select images to upload or get it from the clipboard";

            #region Queued Uploads

            Action<BackgroundWorker, DoWorkEventArgs> doWork = (worker, args) => {
                // get work item from argument
                var item = (WorkItem<string, UploadedImage>)args.Argument;
                Debug.WriteLine(item.Worker);
                item.Status = WorkStatus.Processing;

                // init params needed
                string filename = item.Args;
                string contentType = "image/" + Path.GetExtension(filename).Substring(1).ToLower();
                byte[] image = File.ReadAllBytes(filename);

                // init HTTPWebRequest stuff
                var req = (HttpWebRequest)WebRequest.Create("http://api.imgur.com/2/upload");
                var bound = "-------------" + DateTime.Now.Ticks.ToString();
                var tmplField = "--" + bound + "\r\nContent-Disposition: form-data; name='{0}'\r\n\r\n{1}\r\n";
                var tmplFile = "--" + bound + "\r\nContent-Disposition: form-data; name='{0}'; filename='{1}'\r\nContent-Type={2}\r\n\r\n";
            
                req.Method = "POST";
                req.ContentType = "multipart/form-data; boundary=" + bound;
                req.AllowWriteStreamBuffering = false;

                #region write upload data to memory stream
                // variables
                UTF8Encoding encoder = new UTF8Encoding();
                MemoryStream memStream = new MemoryStream();
                BinaryWriter memBW = new BinaryWriter(memStream, encoder);

                // write fields
                memBW.Write(encoder.GetBytes(string.Format(tmplField, "key", "c06f4d0cdf6f2cc652635a08be34973d")));
                memBW.Write(encoder.GetBytes(string.Format(tmplField, "type", "file")));
                memBW.Write(encoder.GetBytes(string.Format(tmplFile, "image", filename, contentType)));
                memBW.Flush();
            
                // write image
                memBW.Write(image);
                memBW.Flush();

                // write closing
                memBW.Write(encoder.GetBytes("\r\n--" + bound + "--"));
                memBW.Flush();

                memStream.Position = 0;
                req.ContentLength = memStream.Length;
                #endregion write upload data to memory stream

                try
                {
                    using (var reqStream = req.GetRequestStream())
                    {
                        BinaryWriter reqWriter = new BinaryWriter(reqStream);
                        byte[] buffer = new byte[640]; // 50KB Buffer
                        int read = 0, bytesRead = 0;
                        while ((read = memStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (worker.CancellationPending)
                            {
                                item.Status = WorkStatus.Cancelled;
                                args.Cancel = true;
                                return;
                            }

                            reqWriter.Write(buffer, 0, read);
                            bytesRead += read;
                            item.Progress = (double)bytesRead / memStream.Length * 100;
                        }
                        reqWriter.Flush();

                        // close stream writers
                        memBW.Close();
                    }

                    var res = req.GetResponse();
                    using (var resStream = res.GetResponseStream())
                    {
                        XDocument doc = XDocument.Load(resStream);
                        var uploadedImage = (from imgurLink in doc.Descendants("imgur_page")
                                             from directLink in doc.Descendants("original")
                                             select new UploadedImage
                                             {
                                                 Link = imgurLink.Value,
                                                 DirectLink = directLink.Value
                                             }).FirstOrDefault();
                        item.Result = uploadedImage;
                        item.Status = WorkStatus.Finished;
                    }
                }
                catch (WebException e)
                {
                    if (e.Status != WebExceptionStatus.RequestCanceled)
                    {
                        item.Status = WorkStatus.Error;
                        item.Result = new UploadedImage { Error = e };
                    }
                }
                catch (Exception e)
                {
                    item.Status = WorkStatus.Error;
                    item.Result = new UploadedImage { Error = e };
                }

                item.Status = (item.Status == WorkStatus.Processing) ? WorkStatus.Finished : item.Status;
            };
            UploadQueue = new WorkQueue<string, UploadedImage>(1, doWork); 
            #endregion
        }

        #region ICommands
        public ICommand SelectImagesCommand
        {
            get
            {
                if (_selectImageCommand == null)
                {
                    _selectImageCommand = new RelayCommand(() => SelectImage());
                }
                return _selectImageCommand;
            }
        }

        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                {
                    _copyImageCommand = new RelayCommand(() =>
                    {
                        var dataObj = new DataObject();
                        UploadedImage img = SelectedWorkItem.Result;
                        Clipboard.Clear();
                        dataObj.SetData(DataFormats.Text, img.DirectLink);

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(img.DirectLink);
                        bitmapImage.EndInit();

                        dataObj.SetData(DataFormats.Bitmap, bitmapImage);

                        Clipboard.SetDataObject(dataObj);
                        Notification = "Image & Direct Link Copied to Clipboard";
                    }, () =>
                    {
                        if (SelectedWorkItem != null && SelectedWorkItem.Status == WorkStatus.Finished)
                            return true;
                        return false;
                    });
                }
                return _copyImageCommand;
            }
        }

        public ICommand PasteImageCommand
        {
            get
            {
                if (_pasteImageCommand == null)
                {
                    _pasteImageCommand = new RelayCommand(() =>
                    {
                        var img = Clipboard.GetImage();
                        var encoder = new JpegBitmapEncoder();
                        var tmpFileName = Path.GetTempFileName() + ".jpg"; // just add a .jpg so that can preview image when user double click on the image
                        var outStream = new FileStream(tmpFileName, FileMode.Create);
                        encoder.Frames.Add(BitmapFrame.Create(img));
                        encoder.Save(outStream);
                        outStream.Close();

                        UploadQueue.AddWork(new WorkItem<string, UploadedImage>(tmpFileName));
                        UploadQueue.DoNext();
                    }, () => Clipboard.ContainsImage());
                }
                return _pasteImageCommand;
            }
        }

        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                {
                    _aboutCommand = new RelayCommand(delegate
                    {
                        var aboutView = new QuickImageUpload.Views.AboutView();
                        aboutView.Show();
                    });
                }
                return _aboutCommand;
            }
        } 
        #endregion

        #region Helpers
        protected void SelectImage()
        {
            string[] fileNames = new string[0];
            var dialogService = new DialogService();
            fileNames = dialogService.GetOpenFileDialog("Select Images to Open", "Image Files|*.jpg;*.jpeg;*.png;*.gif");
            foreach (string filename in fileNames)
                UploadQueue.AddWork(new WorkItem<string, UploadedImage>(filename));
            UploadQueue.DoNext();
        } 
        #endregion
    }
}
