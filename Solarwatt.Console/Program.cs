//using RestSharp;
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
			var connector = new SolarwattConnector();
			connector.Login("your_user", "your_pass");
			connector.GetExport("your_device_location"); // like XXX00-000000000:0
		}
	}
}
