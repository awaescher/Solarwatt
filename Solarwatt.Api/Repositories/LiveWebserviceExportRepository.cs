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

namespace Solarwatt.Api.Repositories
{
	public class LiveWebserviceExportRepository : IExportRepository
	{
		private RestClient _client;

		public LiveWebserviceExportRepository(ISolarwattConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		public void Initialize()
		{
			Login();
		}

		private void CreateClient()
		{
			_client = new RestClient() { CookieContainer = new CookieContainer() };
			_client.Proxy = Connection.Proxy;
			_client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
			_client.Timeout = 10 * 1000;

			AuthzReqHash = new Random(DateTime.UtcNow.Millisecond).Next(700000000, 799999999).ToString();
		}

		private bool Login()
		{
			CreateClient();

			var loginResult = PostLogin();

			var redirect = GetLoginRedirect(loginResult);

			GetRedir(redirect);

			AccessToken = GetAccessTokenFromContextJs();

			return !string.IsNullOrEmpty(AccessToken);
		}

		private LoginResponse PostLogin()
		{
			_client.BaseUrl = new Uri("https://auth.energy-manager.de/login");
			var request = new RestRequest(Method.POST);
			request.AddHeader("Host", "auth.energy-manager.de");
			request.AddHeader("Accept", "application/json, text/plain, */*");
			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("Referer", $"https://auth.energy-manager.de/index.html?AuthzReqHash={AuthzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");

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
			return SimpleJson.DeserializeObject<LoginResponse>(response.Content);
		}

		private Uri GetLoginRedirect(LoginResponse loginResult)
		{
			string redirectUri = "https://auth.energy-manager.de" + loginResult.redirectUri;
			var sessionId = _client.CookieContainer.GetCookies(_client.BaseUrl)[0].Value;

			_client.BaseUrl = new Uri(redirectUri);
			var request = new RestRequest(Method.GET);

			request.AddHeader("Host", "auth.energy-manager.de");
			request.AddHeader("Upgrade-Insecure-Requests", "1");
			request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.AddHeader("Referer", $"https://auth.energy-manager.de/index.html?AuthzReqHash={AuthzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("DNT", "1");

			request.AddCookie("session_id", sessionId);

			var response = _client.Execute(request);

			return response.ResponseUri;
		}


		private void GetRedir(Uri redirect)
		{
			_client.BaseUrl = redirect;
			var request = new RestRequest(Method.GET);

			request.AddHeader("Host", "desktop.energy-manager.de");
			request.AddHeader("Upgrade-Insecure-Requests", "1");
			request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.AddHeader("Referer", $"https://auth.energy-manager.de/index.html?AuthzReqHash={AuthzReqHash}&appEntryUri=https%3A%2F%2Fdesktop.energy-manager.de%2F");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("DNT", "1");

			_client.Execute(request);
		}

		private string GetAccessTokenFromContextJs()
		{
			_client.BaseUrl = new Uri("https://desktop.energy-manager.de/js/context.js");
			var request = new RestRequest(Method.GET);

			request.AddHeader("Host", "desktop.energy-manager.de");
			request.AddHeader("Upgrade-Insecure-Requests", "1");
			request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			request.AddHeader("Referer", "https://desktop.energy-manager.de/index.html");
			request.AddHeader("Accept-Language", "de-DE,de;q=0.8");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("DNT", "1");

			var response = _client.Execute(request);

			var tokenMatch = Regex.Match(response.Content, @"accessToken:\s*'(?<token>.*)'");
			return tokenMatch.Groups["token"].Value;
		}

		public IEnumerable<string> GetExport(DateTime day, int minutesInterval = 15) => GetExport(day, day, minutesInterval);

		public IEnumerable<string> GetExport(DateTime from, DateTime to, int minutesInterval = 15)
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

			return response.Content.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
		}
		
		public ISolarwattConnection Connection { get; }

		protected string AccessToken { get; private set; }

		private string AuthzReqHash { get; set; }
	}
}
