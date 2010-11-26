using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using MvvmFoundation.Wpf;
using QuickImageUpload.Services;
using System.Text;

namespace QuickImageUpload.ViewModels
{
    class ShellViewModel : ObservableObject
    {
        protected RelayCommand _copyImageCommand;
        protected RelayCommand _pasteImageCommand;
        protected Dispatcher _dispatcher;
        protected string _notification;
        private RelayCommand _aboutCommand;
        protected ObservableCollection<UploadedImage> _uploadedImages;

        public string Notification { 
            get { return _notification; }
            set {
                _notification = value;
                RaisePropertyChanged("Notification");
            }
        }

        public ICollectionView UploadedImages { get; protected set; }

        public ShellViewModel()
        {
            SelectImagesCommand = new RelayCommand(() => SelectImage());
            _uploadedImages = new ObservableCollection<UploadedImage>();
            UploadedImages = CollectionViewSource.GetDefaultView(_uploadedImages);
            _dispatcher = Dispatcher.CurrentDispatcher;
            Notification = "Please select images to upload or get it from the clipboard";
        }

        public ICommand SelectImagesCommand
        {
            get;
            protected set;
        }

        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null) {
                    _copyImageCommand = new RelayCommand(() =>
                    {
                        var dataObj = new DataObject();
                        UploadedImage img = (UploadedImage)UploadedImages.CurrentItem;
                        Clipboard.Clear();
                        dataObj.SetData(DataFormats.Text, img.DirectLink);

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(img.DirectLink);
                        bitmapImage.EndInit();

                        dataObj.SetData(DataFormats.Bitmap, bitmapImage);

                        Clipboard.SetDataObject(dataObj);
                        Notification = "Image & Direct Link Copied to Clipboard";
                    }, () => UploadedImages.CurrentPosition > -1);
                }
                return _copyImageCommand;
            }
        }

        public ICommand PasteImageCommand
        {
            get {
                if (_pasteImageCommand == null)
                {
                    _pasteImageCommand = new RelayCommand(() =>
                    {
                        var img = Clipboard.GetImage();
                        var encoder = new JpegBitmapEncoder();
                        var tmpFileName = Path.GetTempFileName();
                        var outStream = new FileStream(tmpFileName, FileMode.Create);
                        encoder.Frames.Add(BitmapFrame.Create(img));
                        encoder.Save(outStream);
                        outStream.Close();
                        byte[] imageData = File.ReadAllBytes(tmpFileName);
                        Task.Factory.StartNew(() => UploadImage(Path.GetFileName(tmpFileName), "image/jpg", imageData));
                    }, () => Clipboard.ContainsImage());
                }
                return _pasteImageCommand;
            } 
        }

        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null) {
                    _aboutCommand = new RelayCommand(delegate
                    {
                        var aboutView = new QuickImageUpload.Views.AboutView();
                        aboutView.Show();
                    });
                }
                return _aboutCommand;
            }
        }

        protected void SelectImage()
        {
            string[] fileNames = new string[0];
            var dialogService = new DialogService();
            fileNames = dialogService.GetOpenFileDialog("Select Images to Open", "Image Files|*.jpg;*.jpeg;*.png;*.gif");
            Parallel.ForEach(fileNames, img =>
            {
                byte[] imageBytes = File.ReadAllBytes(img);
                Task.Factory.StartNew(() => UploadImage(Path.GetFileName(img), "image/" + Path.GetExtension(img).Substring(1), imageBytes));
            });
        }

        public void UploadImage(string filepath)
        {
            string filename = Path.GetFileName(filepath);
            string contentType = "image/" + Path.GetExtension(filepath).Substring(1).ToLower();
            byte[] imageData = File.ReadAllBytes(filepath);
            UploadImage(filename, contentType, imageData);
        }

        public void UploadImage(string filename, string contentType, byte[] image)
        {
            var req = (HttpWebRequest)WebRequest.Create("http://api.imgur.com/2/upload");
            var bound = "-------------" + DateTime.Now.Ticks.ToString();
            var tmplField = "--" + bound + "\r\nContent-Disposition: form-data; name='{0}'\r\n\r\n{1}\r\n";
            var tmplFile = "--" + bound + "\r\nContent-Disposition: form-data; name='{0}'; filename='{1}'\r\nContent-Type={2}\r\n\r\n";
            
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=" + bound;
            req.AllowWriteStreamBuffering = false;

            Notification = "Uploading Image " + filename + " ...";

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
            #endregion write upload data to memory stream
            memStream.Position = 0;
            req.ContentLength = memStream.Length;

            try
            {
                using (var reqStream = req.GetRequestStream())
                {
                    BinaryWriter reqWriter = new BinaryWriter(reqStream);
                    byte[] buffer = new byte[640]; // 50KB Buffer
                    int read = 0, bytesRead = 0;
                    while ((read = memStream.Read(buffer, 0, buffer.Length)) > 0) {
                        reqWriter.Write(buffer, 0, read);
                        bytesRead += read;
                        Debug.WriteLine("Percent Done: " + ((double)bytesRead / memStream.Length * 100) + "% " + DateTime.Now);
                    }
                    reqWriter.Flush();

                    // close stream writers
                    memBW.Close();
                }

                Debug.WriteLine("Getting Response " + DateTime.Now);
                var res = req.GetResponse();
                Debug.WriteLine("Got Response " + DateTime.Now);
                using (var resStream = res.GetResponseStream())
                {
                    Debug.WriteLine("Using Response Stream: " + DateTime.Now);
                    XDocument doc = XDocument.Load(resStream);
                    var link = from imgurLink in doc.Descendants("imgur_page")
                                from directLink in doc.Descendants("original")
                                select new UploadedImage
                                {
                                    Link = imgurLink.Value,
                                    DirectLink = directLink.Value
                                };
                    _dispatcher.Invoke(new Action(() => _uploadedImages.Add(link.FirstOrDefault())));
                    Notification = "Image '" + filename + "' Successfully Uploaded";
                }
            }
            catch (Exception e)
            {
                _dispatcher.Invoke(new Action(() => _uploadedImages.Add(new UploadedImage { Error = e.Message })));
                Notification = "Image '" + filename + "' Upload Failed";
            }
            Debug.WriteLine("Finish: " + DateTime.Now);
        }
    }
}
