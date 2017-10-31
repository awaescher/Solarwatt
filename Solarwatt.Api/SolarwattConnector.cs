using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Solarwatt.Api.Connection;
using System.Text.RegularExpressions;
using Solarwatt.Api.Helper;
using Solarwatt.Api.Dto;

namespace Solarwatt.Api
{
	public class SolarwattConnector
	{
		private RestClient _client;

		public SolarwattConnector(ISolarwattConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		private void CreateClient()
		{
			_client = new RestClient() { CookieContainer = new CookieContainer() };
			_client.Proxy = Connection.Proxy;
			_client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
			_client.Timeout = 10 * 1000;
		}

		public void Login()
		{
			string authzReqHash = new Random(DateTime.UtcNow.Millisecond).Next(700000000, 799999999).ToString();
			string url = "https://auth.energy-manager.de/login";

			CreateClient();

			/*
			POST https://auth.energy-manager.de/login HTTP/1.1
			Host: auth.energy-manager.de
			Connection: keep-alive
			Content-Length: 326
			Accept: application/json, text/plain, *-*
			Origin: https://auth.energy-manager.de
			User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36
			Content-Type: application/x-www-form-urlencoded
			DNT: 1
			Referer: https://auth.energy-manager.de/index.html?authzReqHash=753969061&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F
			Accept-Encoding: gzip, deflate, br
			Accept-Language: de-DE,de;q=0.8
			Cookie: session_id=MTkyLjE2OC4yMjQuMTAyIGQ0ODZhNDgwLTYwZDktNDUyZi1iZDQ4LTJiZTNlYjRkYjRmZQ==

			username=XXXXX&password=XXXXX&autologin=false&channel=solarwatt&originalRequest=%2Fauthorize%3Fresponse_type%3Dcode%26amp%3Bredirect_uri%3Dhttps%253A%252F%252Fdesktop.energy-manager.de%252Frest%252Fauth%252Fauth_grant%26amp%3Bstate%3D%26amp%3Bclient_id%3Dkiwigrid.desktop%26amp%3BoverrideRedirectUri%3Dtrue
			*/

			_client.BaseUrl = new Uri(url);
			var request = new RestRequest(Method.POST);
			
			request.AddHeader("Host", "auth.energy-manager.de");
			request.AddHeader("Accept", "application/json, text/plain, */*");
			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("Referer", $"https://auth.energy-manager.de/index.html?authzReqHash={authzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");

			request.AddHeader("DNT", "1");
			request.AddParameter("username", Connection.UserName);
			request.AddParameter("password", Connection.Password);
			request.AddParameter("autologin", "false");
			request.AddParameter("channel", "solarwatt");
			request.AddParameter("originalRequest", "" +
							"/authorize?response_type=code&amp;" +
							"redirect_uri=https://desktop.energy-manager.de/rest/auth/auth_grant&amp;" +
							"state=&amp;" +
							"client_id=kiwigrid.desktop&amp;" +
							"overrideRedirectUri=true");

			var response = _client.Execute(request);

			var loginResult = SimpleJson.DeserializeObject<LoginResponse>(response.Content);

			/*
			GET https://auth.energy-manager.de/authorize?response_type=code&amp;state=&amp;client_id=kiwigrid.desktop&amp;overrideRedirectUri=true&amp;redirect_uri=https%3A%2F%2Fdesktop.energy-manager.de%2Frest%2Fauth%2Fauth_grant HTTP/1.1
			Host: auth.energy-manager.de
			Connection: keep-alive
			Upgrade-Insecure-Requests: 1
			User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36
			Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*; q = 0.8
			Referer: https://auth.energy-manager.de/index.html?authzReqHash={authzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F
						Accept - Encoding: gzip, deflate, br
			Accept - Language: de - DE,de; q = 0.8,en - US; q = 0.6,en; q = 0.4,lb; q = 0.2
			Cookie: session_id = MTkyLjE2OC4yMjQuMTAyIGI5NzdjYTliLTJmNDQtNDNiMS1hNjFhLWEzNTJiMzMzM2JmZA ==
			*/

			string redirectUri = "https://auth.energy-manager.de" + loginResult.redirectUri;
			var sessionId = _client.CookieContainer.GetCookies(_client.BaseUrl)[0].Value;

			_client.BaseUrl = new Uri(redirectUri);
			request = new RestRequest(Method.GET);

			request.AddHeader("Host", "auth.energy-manager.de");
			request.AddHeader("Upgrade-Insecure-Requests", "1");
			request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.AddHeader("Referer", $"https://auth.energy-manager.de/index.html?authzReqHash={authzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("DNT", "1");

			request.AddCookie("session_id", sessionId);

			response = _client.Execute(request);

			/*
			GET https://desktop.energy-manager.de/rest/auth/auth_grant?code=5d917a53d60a5e479d617fc05e49587d HTTP/1.1
			Host: desktop.energy-manager.de
			Connection: keep-alive
			Upgrade-Insecure-Requests: 1
			User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36
			Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*-*;q=0.8
			Referer: https://auth.energy-manager.de/index.html?authzReqHash={authzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F
			Accept-Encoding: gzip, deflate, br
			Accept-Language: de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4,lb;q=0.2
			 */

			_client.BaseUrl = new Uri(response.ResponseUri.AbsoluteUri);
			request = new RestRequest(Method.GET);

			request.AddHeader("Host", "desktop.energy-manager.de");
			request.AddHeader("Upgrade-Insecure-Requests", "1");
			request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.AddHeader("Referer", $"https://auth.energy-manager.de/index.html?authzReqHash={authzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("DNT", "1");

			response = _client.Execute(request);

			/*

			GET https://desktop.energy-manager.de/js/context.js HTTP/1.1
			Host: desktop.energy-manager.de
			Connection: keep-alive
			User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36
			Accept: +/+
			Referer: https://desktop.energy-manager.de/index.html
			Accept-Encoding: gzip, deflate, br
			Accept-Language: de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4,lb;q=0.2
			Cookie: webcore_sid="MTkyLjE2OC4yMjQuMTAyIDlhYWY0NTE1LTYxYTItNGRlNS04NzIxLWRiMjU4M2I5MzVjOQ=="
			*/

			url = "https://desktop.energy-manager.de/js/context.js";
			_client.BaseUrl = new Uri(url);
			request = new RestRequest(Method.GET);

			request.AddHeader("Host", "desktop.energy-manager.de");
			request.AddHeader("Upgrade-Insecure-Requests", "1");
			request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.AddHeader("Referer", "https://desktop.energy-manager.de/index.html");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("DNT", "1");

			response = _client.Execute(request);

			var tokenMatch = Regex.Match(response.Content, @"accessToken:\s*'(?<token>.*)'");
			AccessToken = tokenMatch.Groups["token"].Value;
		}

		public string GetTotals(DateTime day) => GetTotals(day, day);

		public string GetTotals(DateTime from, DateTime to) => GetExport(from, to, 24 * 60);

		public string GetTodayTotal() => GetExport(DateTime.Today, 1440);

		public string GetExport(DateTime day, int minutesInterval = 15) => GetExport(day, day, minutesInterval);

		public string GetExport(DateTime from, DateTime to, int minutesInterval = 15)
		{
			string json = @"{'columnInformation':[{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromGrid']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromStorage']}],'label':'Gesamt-Stromverbrauch ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromProducers']}],'label':'Stromerzeugung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromStorage']}],'label':'Netzeinspeisung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromGrid']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromGrid']}],'label':'Stromzukauf aus dem Netz ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromStorage']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromStorage']}],'label':'Batterieversorgung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromGrid']}],'label':'Batterieaufladung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:solarwatt:myreserve:bc:a30b000a5e5d','tags':['StateOfCharge'],'function':'TWA'}],'label':'Ladezustand: MyReserve ','precision':2,'baseUnit':'%','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromProducers']}],'label':'Eigennutzung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromStorage']}],'label':'Netzeinspeisung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromStorage']}],'label':'Strom-Selbstversorgung aus PV ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:pvplant:%DEVICE_LOCATION%','tags':['WorkACOut']}],'label':'Stromerzeugung: %DEVICE_NAME% ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:pvplant:%DEVICE_LOCATION%','tags':['WorkACOut']}],'label':'Gesamt-Stromerzeugung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''}],'outputFileConfig':{'delimiter':';','fileName':'overall_%USER_NAME%_%EXPORT_DATE%.csv','timeZoneId':'Europe/Berlin','timezone':'Europe/Berlin','dateFormat':'DD.MM.YYYY HH:mm:ss','replaceForNull':'---','decimalSeparator':','},'aggregationConfig':{'resolution':'PT%INTERVAL_MINUTES%M','from':%FROM%,'to':%TO%,'function':'INC'}}"
.Replace("'", "\"")
.Replace("%DEVICE_LOCATION%", Connection.DeviceLocation)
.Replace("%USER_NAME%", Connection.UserName)
.Replace("%DEVICE_NAME%", Connection.DeviceName)
.Replace("%FROM%", from.ToUnixTimeStamp(DateRange.Begin).ToString())
.Replace("%TO%", to.ToUnixTimeStamp(DateRange.End).ToString())
.Replace("%EXPORT_DATE%", DateTime.Now.ToUnixTimeStamp().ToString() + "572") // add any 3 digits to mimic milliseconds
.Replace("%INTERVAL_MINUTES%", minutesInterval.ToString());

			string url = "https://solarwatt-exportbackend.kiwigrid.com/v1.0/export";
			_client.BaseUrl = new Uri(url);
			var request = new RestRequest(Method.POST);

			request.AddHeader("Host", "solarwatt-exportbackend.kiwigrid.com");
			request.AddHeader("Accept", "application/json, text/plain, */*");

			request.AddHeader("Content-Type", "application/json; charset=UTF-8");
			request.AddHeader("Accept", "*/*");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("Origin", "https://export.energy-manager.de");
			request.AddHeader("Referer", "https://export.energy-manager.de/index.html");
			request.AddHeader("token", AccessToken);

			request.AddParameter("application/json", json, ParameterType.RequestBody);

			var response = _client.Execute(request);
			return response.Content;
		}
		
		public ISolarwattConnection Connection { get; }

		protected string AccessToken { get; private set; }
	}
}
