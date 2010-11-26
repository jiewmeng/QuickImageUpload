using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuickImageUpload.Services
{
    interface IDialogService
    {
        string GetNewFileDialog(string title, string filter);
        string[] GetOpenFileDialog(string title, string filter);
        string GetSaveFileDialog(string title, string filter);
        MessageBoxResult GetMessageBox(string title, string caption, MessageBoxButton msgBoxButtons, MessageBoxImage msgBoxIcon);
    }
}
