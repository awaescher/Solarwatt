using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Solarwatt.Console
{
	internal class SolarwattConnector
	{
		private RestClient _client;
		private string _username;

		public void Login(string username, string password)
		{
			_username = username;

			string url = "https://auth.energy-manager.de/login";
			_client = new RestClient(url) { CookieContainer = new CookieContainer() };
			_client.BaseUrl = new Uri(url);
			var request = new RestRequest(Method.POST);

			request.AddHeader("Host", "auth.energy-manager.de");
			request.AddHeader("Accept", "application/json, text/plain, */*");
			request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
			request.AddHeader("Accept-Language", "de-DE,de");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");

			request.AddParameter("username", username);
			request.AddParameter("password", password);
			request.AddParameter("autologin", "false");
			request.AddParameter("channel", "solarwatt");
			request.AddParameter("originalRequest", "" +
				"/authorize?response_type=code&amp;" +
				"redirect_uri=https://desktop.energy-manager.de/rest/auth/auth_grant&amp;" +
				"state=&amp;" +
				"client_id=kiwigrid.desktop&amp;" +
				"overrideRedirectUri=true");

			_client.Timeout = 10 * 1000;

			var response = _client.Execute(request);
		}

		public void GetExport(string deviceLocation)
		{
			string json = @"
{'columnInformation':[{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromGrid']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromStorage']}],'label':'Gesamt-Stromverbrauch ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromProducers']}],'label':'Stromerzeugung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromStorage']}],'label':'Netzeinspeisung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromGrid']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromGrid']}],'label':'Stromzukauf aus dem Netz ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromStorage']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromStorage']}],'label':'Batterieversorgung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromGrid']}],'label':'Batterieaufladung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:solarwatt:myreserve:bc:a30b000a5e5d','tags':['StateOfCharge'],'function':'TWA'}],'label':'Ladezustand: MyReserve ','precision':2,'baseUnit':'%','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkBufferedFromProducers']}],'label':'Eigennutzung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkOutFromStorage']}],'label':'Netzeinspeisung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromProducers']},{'guid':'urn:kiwigrid:location:%DEVICE_LOCATION%','tags':['WorkConsumedFromStorage']}],'label':'Strom-Selbstversorgung aus PV ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:pvplant:%DEVICE_LOCATION%','tags':['WorkACOut']}],'label':'Stromerzeugung: Wäscher, Bad Schussenried ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''},{'tagsByDevice':[{'guid':'urn:kiwigrid:pvplant:%DEVICE_LOCATION%','tags':['WorkACOut']}],'label':'Gesamt-Stromerzeugung ','precision':2,'baseUnit':'Wh','decimalUnitPrefix':''}],'outputFileConfig':{'delimiter':';','fileName':'overall_pvwaescher_1508873449626.csv','timeZoneId':'Europe/Berlin','timezone':'Europe/Berlin','dateFormat':'DD.MM.YYYY HH:mm:ss','replaceForNull':'---','decimalSeparator':','},'aggregationConfig':{'resolution':'PT15M','from':1506808800,'to':1508873449,'function':'INC'}}
".Replace("'", "\"").Replace("%DEVICE_LOCATION%", deviceLocation);

			string url = "https://solarwatt-exportbackend.kiwigrid.com/v1.0/export";
			_client.BaseUrl = new Uri(url);
			var request = new RestRequest(Method.POST);

			request.AddHeader("Host", "solarwatt-exportbackend.kiwigrid.com");
			request.AddHeader("Accept", "application/json, text/plain, */*");
			request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
			request.AddHeader("Content-Type", "application/json; charset=UTF-8");
			request.AddHeader("Accept", "*/*");
			request.AddHeader("Accept-Language", "de-DE,de");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("Origin", "https://export.energy-manager.de");
			request.AddHeader("Referer", "https://export.energy-manager.de/index.html");
			request.AddHeader("token", GetToken(_username));

			request.AddParameter("application/json", json, ParameterType.RequestBody);

			_client.Timeout = 10 * 1000;

			var response = _client.Execute(request);
		}

		private string GetToken(string username)
		{
			string content = @"
{'client_id':'solarwatt.export','user_id':'%USER_NAME%','client_type':'application','session_id':'0af66c12-4355-4016-9eed-7eb783f570f5','session':{'type':'normal'},'scope':['FileService.read_files','FileService.manage_files','manage_devices','read_app_info','read_tagvalue_history','read_users','manage_users','read_devices','impex_tagvalue_history'],'expiration':1508525001931,'channel':'solarwatt','accessible_channels':['solarwatt']}.8AiiTt3ESm9zT7JH9zNhj2EXGDg=
".Replace("'", "\"").Replace("%USER_NAME%", username);

			var bytes = Encoding.UTF8.GetBytes(content);
			return Convert.ToBase64String(bytes);
		}
	}
}
