using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sundays;

namespace SundaysApp
{
    public class SundaysFunctionService
    {
        private HttpClient _httpClient;
        private JsonSerializer _serializer;

        private const string URL = "https://sundaysfunctionapp.azurewebsites.net/api/from/{FROM}/to/{TO}/user/{USER}/password/{PASSWORD}/devicelocation/{DEVICELOCATION}/devicename/{DEVICENAME}code={FUNCTIONCODE}";

        public async Task<IEnumerable<Sunday>> Get(SundaysAuth auth, DateTime from, DateTime to)
        {
            _httpClient = _httpClient ?? CreateHttpClient();
            _serializer = _serializer ?? new JsonSerializer();

            var url = URL
                .Replace("{FROM}", from.Date.ToString("MM-dd-yyyy"))
                .Replace("{TO}", to.Date.ToString("MM-dd-yyyy"))
                .Replace("{USER}", auth.UserName)
                .Replace("{PASSWORD}", auth.Password)
                .Replace("{DEVICELOCATION}", auth.DeviceLocation)
                .Replace("{DEVICENAME}", auth.DeviceName)
                .Replace("{FUNCTIONCODE}", "lol");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {
                return _serializer.Deserialize<IEnumerable<Sunday>>(json);
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            return client;
        }
    }
}