using System;
namespace SundaysApp.Services
{
    public interface ICryptoService
    {
        string Encrypt(string value, string password);

        string Decrypt(string value, string password);
    }
}
