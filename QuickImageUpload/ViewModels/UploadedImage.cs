using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace QuickImageUpload.ViewModels
{
    class UploadedImage
    {
        public string Link { get; set; }
        public string DirectLink { get; set; }
        public string Error { get; set; }
    }
}
