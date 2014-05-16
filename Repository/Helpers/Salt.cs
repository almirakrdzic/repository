using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DigitalLibraryService.Helpers
{
    public static class Salt
    {
        public static byte[] GenerateSaltValue(int size)
        {
            UnicodeEncoding utf16 = new UnicodeEncoding();

            if (utf16 != null)
            {

                Random random = new Random(unchecked((int)DateTime.Now.Ticks));

                if (random != null)
                {

                    byte[] saltValue = new byte[size];

                    random.NextBytes(saltValue);


                    return saltValue;
                }
            }

            return null;
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  // SHA1.Create()
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}