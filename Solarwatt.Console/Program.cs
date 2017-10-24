using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solarwatt.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://auth.energy-manager.de/login";
            var client = new RestClient(url);
            client.BaseUrl = new Uri(url);
            var request = new RestRequest(Method.POST);

            request.AddHeader("Host", "auth.energy-manager.de");
            request.AddHeader("Accept", "application/json, text/plain, */*");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept-Language", "de-DE,de");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");

            request.AddParameter("username", "pvwaescher");
            request.AddParameter("password", "");
            request.AddParameter("autologin", "false");
            request.AddParameter("channel", "solarwatt");
            request.AddParameter("originalRequest", "/authorize?response_type=code&amp;redirect_uri=https%3A%2F%2Fdesktop.energy-manager.de%2Frest%2Fauth%2Fauth_grant&amp;state=&amp;client_id=kiwigrid.desktop&amp;overrideRedirectUri=true");
            client.Timeout = 30 * 1000;

            var response = client.Execute(request);

        }
    }
}
