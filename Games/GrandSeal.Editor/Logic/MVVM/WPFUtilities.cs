using System.IO;
using System.Windows.Media.Imaging;

namespace GrandSeal.Editor.Logic.MVVM
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
            var result = new BitmapImage { CacheOption = BitmapCacheOption.OnDemand };
            result.BeginInit();
            result.StreamSource = stream;
            result.EndInit();
            return result;
        }

        public static BitmapImage DataToImage(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            Stream stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;
            var result = new BitmapImage { CacheOption = BitmapCacheOption.OnDemand };
            result.BeginInit();
            result.StreamSource = stream;
            result.EndInit();
            return result;
        }
    }
}
