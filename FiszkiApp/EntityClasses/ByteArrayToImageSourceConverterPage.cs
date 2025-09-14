using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FiszkiApp.EntityClasses
{
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] imageAsBytes && imageAsBytes.Length > 0)
            {
                var stream = new MemoryStream(imageAsBytes);
                return ImageSource.FromStream(() => stream);
            }

            return "avataricon2.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}