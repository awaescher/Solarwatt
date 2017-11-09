using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Solarwatt.Api.Connection
{
	public class SolarwattConnection : ISolarwattConnection
	{
		public string DeviceName { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

		public string DeviceLocation { get; set; }

		public string ProxyUserDomain { get; set; }

		public string ProxyUser { get; set; }

		public string ProxyPassword { get; set; }

		public IWebProxy Proxy { get; set; }
	}
}
