using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Utils
{
    public static class HashUtils
    {
        private static readonly SHA1 HashProvider = SHA1.Create();
        private static readonly MD5 Md5Provider = MD5.Create();

        public static string BuildResourceHash(string path)
        {
            lock (HashProvider)
            {
                byte[] hashData = HashProvider.ComputeHash(Encoding.UTF8.GetBytes(path));
                return Convert.ToBase64String(hashData);
            }
        }

        public static int CombineObjectHashes(object[] data)
        {
            var hashCodes = new byte[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                hashCodes[i] = BitConverter.GetBytes(data[i] == null ? 0 : data[i].GetHashCode());
            }

            return FNV.Combine(hashCodes);
        }

        public static int CombineHashes<T>(T[] data)
            where T : struct 
        {
            if (data == null)
            {
                return 0;
            }

            var hashCodes = new byte[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                hashCodes[i] = BitConverter.GetBytes(data[i].GetHashCode());
            }

            return FNV.Combine(hashCodes);
        }

        public static byte[] GetMd5(Stream source)
        {
            source.Position = 0;
            lock (Md5Provider)
            {
                return Md5Provider.ComputeHash(source);
            }
        }

        public static string Md5ToString(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public static string GetSHA1FileName(string data)
        {
            var sha = SHA1.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash).Replace('/', '-');
        }

        public static string GetFNVFileName(int value)
        {
            var fnv = FNV.Create();
            int hash = fnv.Compute(BitConverter.GetBytes(value));
            return hash.ToString(CultureInfo.InvariantCulture).Replace('-', 'N');
        }
    }
}
