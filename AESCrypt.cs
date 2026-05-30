using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AzeuServices_V1
{
    public static class AESCrypt
    {
        // Built-in variable for the encryption key
        private static readonly string _internalDefaultKey = "azeu_mark";

        /// <summary>
        /// Encrypts a string using AES-256.
        /// </summary>
        public static string Encrypt(string plainText, string key = null)
        {
            if (string.IsNullOrEmpty(plainText)) return null;

            // Use internal key if none is provided
            string selectedKey = string.IsNullOrEmpty(key) ? _internalDefaultKey : key;

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                // Generate a consistent 32-byte key from the string
                aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(selectedKey));
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Decrypts an AES-256 encrypted Base64 string. 
        /// Returns null or throws if decryption fails.
        /// </summary>
        public static string Decrypt(string cipherText, string key = null)
        {
            if (string.IsNullOrEmpty(cipherText)) return null;

            // Use internal key if none is provided
            string selectedKey = string.IsNullOrEmpty(key) ? _internalDefaultKey : key;

            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(selectedKey));
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                // Decryption failed (likely because the data wasn't encrypted)
                return null;
            }
        }
    }
}