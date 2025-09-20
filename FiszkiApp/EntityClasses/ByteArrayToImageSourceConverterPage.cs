using System.Globalization;

namespace FiszkiApp.EntityClasses
{
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public bool UseFallback { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
            {
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }

            if (UseFallback)
            {
                foreach (var dict in Application.Current?.Resources.MergedDictionaries ?? Enumerable.Empty<ResourceDictionary>())
                {
                    if (dict.TryGetValue("AvatarImage", out var avatarObj))
                    {
                        if (avatarObj is string avatarFile && !string.IsNullOrWhiteSpace(avatarFile))
                            return ImageSource.FromFile(avatarFile);

                        if (avatarObj is ImageSource imgSrc)
                            return imgSrc;
                    }
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}