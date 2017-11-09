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
using System.Net.Http;

namespace Solarwatt.Api.Repositories
{
	public class LiveWebserviceExportRepository : IExportRepository
	{
		private CookieContainer _cookieContainer;

		public LiveWebserviceExportRepository(ISolarwattConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		public async Task<bool> Initialize()
		{
			_cookieContainer = new CookieContainer();

			AuthzReqHash = new Random(DateTime.UtcNow.Millisecond).Next(700000000, 799999999).ToString();

			await Login();

			return !string.IsNullOrWhiteSpace(AccessToken);
		}

		private HttpClient CreateClient(Uri baseAddress)
		{
			var handler = new HttpClientHandler()
			{
				Proxy = Connection.Proxy,
				PreAuthenticate = true,
				UseDefaultCredentials = false,
				CookieContainer = _cookieContainer,
				AutomaticDecompression = DecompressionMethods.GZip
			};
			var client = new HttpClient(handler);
			client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
			client.DefaultRequestHeaders.Add("DNT", "1");
			client.Timeout = TimeSpan.FromSeconds(10);
			client.BaseAddress = baseAddress;

			return client;
		}

		private async Task<bool> Login()
		{
			var loginResult = await PostLogin();

			var redirect = await GetLoginRedirect(loginResult);

			await GetRedir(redirect);

			AccessToken = await GetAccessTokenFromContextJs();

			return !string.IsNullOrEmpty(AccessToken);
		}

		private async Task<LoginResponse> PostLogin()
		{
			var client = CreateClient(new Uri("https://auth.energy-manager.de/login"));

			var items = new Dictionary<string, string>();

			items.Add("Host", "auth.energy-manager.de");
			items.Add("Accept", "application/json, text/plain, */*");
			items.Add("Content-Type", "application/x-www-form-urlencoded");
			items.Add("Accept-Language", "de-DE,de;q=0.8");
			items.Add("Accept-Encoding", "gzip, deflate, br");
			items.Add("Referer", $"https://auth.energy-manager.de/index.html?AuthzReqHash={AuthzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");

			items.Add("username", Connection.UserName);
			items.Add("password", Connection.Password);
			items.Add("autologin", "false");
			items.Add("channel", "solarwatt");
			items.Add("originalRequest", "" +
							"/authorize?response_type=code&amp;" +
							"redirect_uri=https://desktop.energy-manager.de/rest/auth/auth_grant&amp;" +
							"state=&amp;" +
							"client_id=kiwigrid.desktop&amp;" +
							"overrideRedirectUri=true");

			var content = new FormUrlEncodedContent(items);

			var response = await client.PostAsync(client.BaseAddress, content);
			var responseString = await response.Content.ReadAsStringAsync();

			SessionId = _cookieContainer.GetCookies(client.BaseAddress)[0].Value;

			return TinyJson.JSONParser.FromJson<LoginResponse>(responseString);
		}

		private async Task<Uri> GetLoginRedirect(LoginResponse loginResult)
		{
			const string BASE_URL = "https://auth.energy-manager.de";
			string redirectUri = BASE_URL + loginResult.redirectUri;

			var client = CreateClient(new Uri(redirectUri));

			var items = new Dictionary<string, string>();

			client.DefaultRequestHeaders.Add("Host", "auth.energy-manager.de");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Referer", $"https://auth.energy-manager.de/index.html?AuthzReqHash={AuthzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");
			client.DefaultRequestHeaders.Add("Accept-Language", "de-DE,de;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

			_cookieContainer.Add(new Uri(BASE_URL), new Cookie("session_id", SessionId));

			var response = await client.GetAsync(client.BaseAddress);

			return response.RequestMessage.RequestUri;
		}

		private async Task<bool> GetRedir(Uri redirect)
		{
			var client = CreateClient(redirect);

			client.DefaultRequestHeaders.Add("Host", "desktop.energy-manager.de");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Referer", $"https://auth.energy-manager.de/index.html?AuthzReqHash={AuthzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");
			client.DefaultRequestHeaders.Add("Accept-Language", "de-DE,de;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

			var response = await client.GetAsync(client.BaseAddress);

			return await Task.FromResult(response != null);
		}

		private async Task<string> GetAccessTokenFromContextJs()
		{
			var client = CreateClient(new Uri("https://desktop.energy-manager.de/js/context.js"));

			client.DefaultRequestHeaders.Add("Host", "desktop.energy-manager.de");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Referer", "https://desktop.energy-manager.de/index.html");
			client.DefaultRequestHeaders.Add("Accept-Language", "de-DE,de;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
		
			var responseString = await client.GetStringAsync(client.BaseAddress);

			var tokenMatch = Regex.Match(responseString, @"accessToken:\s*'(?<token>.*)'");
			return tokenMatch.Groups["token"].Value;
		}

		public Task<IEnumerable<string>> GetExport(DateTime day, int minutesInterval = 15) => GetExport(day, day, minutesInterval);

		public async Task<IEnumerable<string>> GetExport(DateTime from, DateTime to, int minutesInterval = 15)
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
			var client = CreateClient(new Uri(url));

			client.DefaultRequestHeaders.Add("Host", "solarwatt-exportbackend.kiwigrid.com");
			client.DefaultRequestHeaders.Add("Accept", "*/*");
			client.DefaultRequestHeaders.Add("Accept-Language", "de-DE,de;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
			client.DefaultRequestHeaders.Add("Origin", "https://export.energy-manager.de");
			client.DefaultRequestHeaders.Add("Referer", "https://export.energy-manager.de/index.html");
			client.DefaultRequestHeaders.Add("token", AccessToken);

			var content = new StringContent(json);
			content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

			var response = await client.PostAsync(client.BaseAddress, content);
			var responseString = await response.Content.ReadAsStringAsync();

			return responseString.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
		}

		public ISolarwattConnection Connection { get; }

		protected string SessionId { get; set; }

		protected string AccessToken { get; private set; }

		private string AuthzReqHash { get; set; }
	}
}
