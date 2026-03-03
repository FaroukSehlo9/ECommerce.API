using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.API.Utilities
{
    public static class WebUiUtility
    {
        // الـ IV ثابت للطول 16 bytes
        private static readonly byte[] initVectorBytes = Encoding.UTF8.GetBytes("tu79Pear340t79u2");

        // طول المفتاح AES-256
        private const int keysize = 256;

        // Passphrase للتشفير (يمكن تغييره لاحقاً)
        private static readonly string passPhrase = "&$#$%$";

        #region Encryption / Decryption

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var aes = Aes.Create())
            {
                aes.KeySize = keysize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using var key = new Rfc2898DeriveBytes(passPhrase, initVectorBytes, 1000);
                aes.Key = key.GetBytes(keysize / 8);
                aes.IV = initVectorBytes;

                using var encryptor = aes.CreateEncryptor();
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cs.FlushFinalBlock();
                }

                // تحويل Base64 وتهيئة للـ URL
                return Convert.ToBase64String(ms.ToArray())
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace('=', ',');
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            // إعادة الرموز الأصلية
            cipherText = cipherText.Replace('-', '+').Replace('_', '/').Replace(',', '=');
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.KeySize = keysize;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using var key = new Rfc2898DeriveBytes(passPhrase, initVectorBytes, 1000);
                aes.Key = key.GetBytes(keysize / 8);
                aes.IV = initVectorBytes;

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(cipherBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        #endregion

        #region Validation

        // التحقق من رقم الهاتف الدولي
        public static bool ValidPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return false;

            string pattern = @"^(?:\+|00)\d{6,13}$";
            return Regex.IsMatch(phone, pattern, RegexOptions.CultureInvariant);
        }

        // التحقق من البريد الإلكتروني
        public static bool ValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        #endregion
    }
}