using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Core.Utils
{
    public static class HashUtils
    {
        private static readonly SHA1 HashProvider = SHA1.Create();

        public static string BuildResourceHash(string path)
        {
            byte[] hashData = HashProvider.ComputeHash(Encoding.UTF8.GetBytes(path));
            return Convert.ToBase64String(hashData);
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
