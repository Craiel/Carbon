using System.IO;
using System.Windows.Media.Imaging;

namespace Carbed.Logic.MVVM
{
    public static class WPFUtilities
    {
        public static BitmapImage FileToImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return null;
            }

            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = new BitmapImage();
            result.CacheOption = BitmapCacheOption.OnDemand;
            result.BeginInit();
            result.StreamSource = stream;
            result.EndInit();
            return result;
        }
    }
}
