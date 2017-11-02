using System;
using System.Collections.Generic;

namespace Solarwatt.Api.Repositories
{
	public interface IExportRepository
	{
		void Initialize();

		IEnumerable<string> GetExport(DateTime day, int minutesInterval = 15);

		IEnumerable<string> GetExport(DateTime from, DateTime to, int minutesInterval = 15);
	}
}