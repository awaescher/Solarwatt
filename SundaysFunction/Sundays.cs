using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Solarwatt.Api;
using Solarwatt.Api.Connection;
using Solarwatt.Api.Repositories;

namespace SundaysFunction
{
	public static class Sundays
	{
		[FunctionName("Sundays")]
		public static HttpResponseMessage Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "user/{user}/password/{password}/devicelocation/{deviceLocation}/devicename/{deviceName}")]
			HttpRequestMessage req,
			string user,
			string password,
			string deviceLocation,
			string deviceName,
			TraceWriter log)
		{
			var connection = new SolarwattConnection()
			{
				UserName = user,
				Password = password,
				DeviceLocation = deviceLocation,
				DeviceName = deviceName
			};

			var provider = new SolarwattSundayProvider(
				new LiveWebserviceExportRepository(connection),
				new SolarwattExportSundayConverter()
				);

			var export = provider.Get(DateTime.Today.AddDays(-7), DateTime.Today);

			// Fetching the name from the path parameter in the request URL
			return req.CreateResponse(HttpStatusCode.OK, export);
		}
	}
}
