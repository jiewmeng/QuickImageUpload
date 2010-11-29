using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace QuickImageUpload.ViewModels
{
    public class UploadedImage
    {
        public string Link { get; set; }
        public string DirectLink { get; set; }
        public Exception Error { get; set; }
    }
}
