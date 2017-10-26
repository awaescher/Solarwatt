using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Solarwatt.Api.Connection
{
	public class DirtyHardcodedTestConnection : ISolarwattConnection
	{
		private string[] _data;

		public DirtyHardcodedTestConnection()
		{
			try
			{
				// sorry folks, I put that data outside of the repo for reasons :)
				_data = System.IO.File.ReadAllLines(@"C:\Temp\Pv\cred.txt");
			}
			catch { }

			if (_data?.Length != 3)
				_data = null;
		}

		public string Username => _data?.ElementAt(0) ?? "";

		public string Password => _data?.ElementAt(1) ?? "";

		public string DeviceLocation => _data?.ElementAt(2) ?? "";

		public string ProxyUserDomain { get; set; }

		public string ProxyUser { get; set; }

		public string ProxyPassword { get; set; }

		public IWebProxy Proxy
		{
			get
			{
				IWebProxy proxy = null;

				var hasProxyUserDomain = !string.IsNullOrEmpty(ProxyUserDomain);
				var hasProxyUser = !string.IsNullOrEmpty(ProxyUser);

				if (hasProxyUserDomain && hasProxyUser)
				{
					proxy = WebRequest.GetSystemWebProxy();
					proxy.Credentials = new NetworkCredential(ProxyUser, ProxyPassword ?? "", ProxyUserDomain);
				}

				return proxy;
			}
		}
	}
}
