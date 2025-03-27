using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DemoMVC.WebUi.Helper
{
    static public class AESCrypto
    {
        private static readonly string Key = "jhja2537AhjsDa84"; // 16 chars for AES-128
        private static readonly string IV = "ghdts5FkG6S3dghG"; // 16 chars for AES

        public static string Encrypt(int userId, int testId)
        {
            string plainText = $"{userId}:{testId}"; // Store both UserId and TestId

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128; // AES-128 (valid for 16-char key)
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(inputBytes, 0, inputBytes.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static (int userId, int examId) Decrypt(string encryptedToken)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128; // AES-128 (valid for 16-char key)
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encryptedToken)))
                using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    string decryptedText = sr.ReadToEnd();
                    var parts = decryptedText.Split(':');
                    return (int.Parse(parts[0]), int.Parse(parts[1]));
                }
            }
        }
    }
}
