using System;
using System.IO;
using System.Security.Cryptography;

namespace SundaysApp.Services
{
    public class AesCryptoService : ICryptoService
    {
        public string Encrypt(string value, string password)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = CreateKey(password);

                byte[] encrypted = AesEncryptStringToBytes(value, aes.Key, aes.IV);
                return Convert.ToBase64String(encrypted) + ";" + Convert.ToBase64String(aes.IV);
            }
        }

        public string Decrypt(string encryptedValue, string password)
        {
            string iv = encryptedValue.Substring(encryptedValue.IndexOf(';') + 1, encryptedValue.Length - encryptedValue.IndexOf(';') - 1);
            encryptedValue = encryptedValue.Substring(0, encryptedValue.IndexOf(';'));

            return AesDecryptStringFromBytes(Convert.FromBase64String(encryptedValue), CreateKey(password), Convert.FromBase64String(iv));
        }

        private byte[] CreateKey(string password, int passwordBytes = 32)
        {
            byte[] salt = new byte[] { 10, 70, 60, 20, 40, 20, 20, 80 };
            int iterations = 400;
            var keyGenerator = new Rfc2898DeriveBytes(password, salt, iterations);
            return keyGenerator.GetBytes(passwordBytes);
        }

        private byte[] AesEncryptStringToBytes(string plainText, byte[] password, byte[] iv)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException(nameof(password));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = password;
                aes.IV = iv;

                using (var memoryStream = new MemoryStream())
                {
                    using (var encryptor = aes.CreateEncryptor())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (var streamWriter = new StreamWriter(cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }
                        }
                    }
                    encrypted = memoryStream.ToArray();
                }
            }

            return encrypted;
        }

        private static string AesDecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            string plaintext = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var memoryStream = new MemoryStream(cipherText))
                {
                    using (var decryptor = aes.CreateDecryptor())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(cryptoStream))
                            {
                                plaintext = streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
