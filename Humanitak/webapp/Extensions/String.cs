using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SmartAdminMvc.Extensions {
    public static partial class Extensions {
        public static string FirstCharToUpper(this string input) {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string Md5Hash(this string str) {
            using (var md5 = new MD5CryptoServiceProvider())
                return string.Join(string.Empty, md5.ComputeHash(Encoding.UTF8.GetBytes(str)).Select(b => b.ToString("X2")));
        }

        public static string ToBase64(this string text) {
            if (text == null)
                return null;

            var textAsBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        public static string FromBase64(string text) {
            string outVal;
            return TryParseBase64(text, out outVal) ? outVal : "";
        }

        private static bool TryParseBase64(this string encodedText, out string decodedText) {
            if (encodedText == null) {
                decodedText = null;
                return false;
            }

            try {
                var textAsBytes = Convert.FromBase64String(encodedText);
                decodedText = Encoding.UTF8.GetString(textAsBytes);
                return true;
            }
            catch (Exception) {
                decodedText = null;
                return false;
            }
        }
    }
}
