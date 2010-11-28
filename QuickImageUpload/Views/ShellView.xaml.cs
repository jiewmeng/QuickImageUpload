using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using QuickImageUpload.ViewModels;
using System.Diagnostics;
using WorkQueueLib;
using System.Windows.Controls;

namespace QuickImageUpload.Views
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            this.DataContext = new ShellViewModel();
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            var vm = (ShellViewModel)this.DataContext;
            // get the image files from filelist
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] imageFormats = new[] {"jpg", "jpeg", "png", "gif"};
                var images = from img in files
                             where imageFormats.Contains(System.IO.Path.GetExtension(img).Substring(1).ToLower())
                             select img;
                Parallel.ForEach(images, img =>
                {
                    Task.Factory.StartNew(() => vm.UploadImage(img));
                });
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var item = (WorkItem<string, UploadedImage>)((ListBoxItem)sender).Content;
            switch (item.Status)
            {
                case WorkStatus.Finished:
                    Process.Start(item.Result.DirectLink);
                    break;
                case WorkStatus.Pending:
                case WorkStatus.Processing:
                    Process.Start(item.Args);
                    break;
            }
        }
    }
}
