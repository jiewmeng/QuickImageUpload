using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows;

namespace QuickImageUpload.Services
{
    class DialogService : IDialogService
    {
        public string GetNewFileDialog(string title, string filter)
        {
            throw new NotImplementedException();
        }

        public string[] GetOpenFileDialog(string title, string filter)
        {
            string[] fileNames = new string[0];
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = true;
            dialog.Title = title;
            dialog.Filter = filter;
            if ((bool)dialog.ShowDialog())
            {
                fileNames = dialog.FileNames;
            }
            return fileNames;
        }

        public string GetSaveFileDialog(string title, string filter)
        {
            throw new NotImplementedException();
        }

        public MessageBoxResult GetMessageBox(string title, string caption, MessageBoxButton msgBoxButtons, MessageBoxImage msgBoxIcon)
        {
            return MessageBox.Show(title, caption, msgBoxButtons, msgBoxIcon);
        }
    }
}
