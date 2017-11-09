using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solarwatt.Api.Repositories
{
	public interface IExportRepository
	{
		Task<bool> Initialize();

		Task<IEnumerable<string>> GetExport(DateTime day, int minutesInterval = 15);

		Task<IEnumerable<string>> GetExport(DateTime from, DateTime to, int minutesInterval = 15);
	}
}