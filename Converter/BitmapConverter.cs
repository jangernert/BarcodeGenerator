using Avalonia.Data.Converters;
using System;
using Avalonia.Media.Imaging;
using System.Globalization;
using System.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

namespace BarcodeGenerator.Converter
{
    public class BitmapConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }

            MemoryStream memStream = new MemoryStream();
            ((Image)value).Save(memStream, ImageFormat.Png);
            memStream.Seek(0, SeekOrigin.Begin);
            return new Avalonia.Media.Imaging.Bitmap(memStream);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
