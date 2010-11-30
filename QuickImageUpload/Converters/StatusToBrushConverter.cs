using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WorkQueueLib;
using System.Windows.Media;

namespace QuickImageUpload.Converters
{
    class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = (WorkStatus)value;
            SolidColorBrush brush = Brushes.Green;
            switch (status)
            {
                case WorkStatus.Cancelled:
                    brush = Brushes.DarkGray;
                    break;
                case WorkStatus.Error:
                    brush = Brushes.Red;
                    break;
            }
            return brush;   
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
