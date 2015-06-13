namespace GrandSeal.Editor.Logic.MVVM
{
    using System.IO;
    using System.Windows.Media.Imaging;

    using CarbonCore.Utils.Compat.IO;

    public static class WPFUtilities
    {
        public static BitmapImage FileToImage(CarbonPath path)
        {
            if (path == null || !path.Exists)
            {
                return null;
            }

            Stream stream = new FileStream(path.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
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
