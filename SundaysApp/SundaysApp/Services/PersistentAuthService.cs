using System;
using System.IO;
using Foundation;
using Newtonsoft.Json;
using SundaysApp.Model;
using Xamarin.Essentials;

namespace SundaysApp.Services
{
    public class PersistentAuthService : IAuthService
    {
        public PersistentAuthService(ICryptoService cryptoService)
        {
            CryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
        }

        public void SetAuth(Auth auth)
        {
            var authFile = GetFileName();

            try
            {
                var fileContent = JsonConvert.SerializeObject(auth);

                if (!string.IsNullOrEmpty(fileContent))
                    fileContent = CryptoService.Encrypt(fileContent, GetUltimat3Passwprd());

                File.WriteAllText(authFile, fileContent);
            }
            catch
            {
                if (File.Exists(authFile))
                    File.Delete(authFile);
            }
        }

        public Auth GetAuth()
        {
            try
            {
                var fileManager = new NSFileManager();
                var appGroupContainer = fileManager.GetContainerUrl("group.net.sodacore.sundays.sharing");
                var appGroupContainerPath = appGroupContainer?.Path ?? "";

                Console.WriteLine($"Reading {GetFileName()}");
                var fileContent = File.ReadAllText(GetFileName());

                Console.WriteLine($"Content lenght: {fileContent?.Length}");
                if (!string.IsNullOrEmpty(fileContent))
                    fileContent = CryptoService.Decrypt(fileContent, GetUltimat3Passwprd());

                Console.WriteLine($"Deserializing");
                var auth = JsonConvert.DeserializeObject<Auth>(fileContent);

                Console.WriteLine($"Auth invalid: {auth?.IsValid.ToString() ?? "{null}"}");
                if (auth?.IsValid == false)
                    return null;

                return auth;
            }
            catch(Exception ex)
            {
                // well, for a fun app that's okay, I think.
                // The user will have to enter his login once more ...
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static string GetFileName() => Path.Combine(FileSystem.AppDataDirectory, "current.auth");

        private static string GetUltimat3Passwprd() => "c'mon_its_not_about_S3cur!ty_here-just_hiding_plain_text";

        public ICryptoService CryptoService { get; }
    }
}