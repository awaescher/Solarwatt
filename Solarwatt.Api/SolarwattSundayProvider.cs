using Solarwatt.Api.Repositories;
using Sundays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solarwatt.Api
{
	public class SolarwattSundayProvider : ISundayProvider
	{
		private bool _firstRun = true;

		public SolarwattSundayProvider(IExportRepository repository, SundayConverter converter)
		{
			Repository = repository ?? throw new ArgumentNullException(nameof(repository));
			Converter = converter ?? throw new ArgumentNullException(nameof(converter));
		}

		public IEnumerable<Sunday> Get(DateTime from, DateTime to)
		{
			if (_firstRun)
				Repository.Initialize();

			_firstRun = false;

			var exportStrings = Repository.GetExport(from, to, 60);
			var exportRows = Converter.Convert(exportStrings).ToList();

			// that's specific to Solarwatt:
			// for today, we cannot use higher intervals without losing data
			// so for today, we have to go to a 15min interval while for the
			// other days we had 60min to not get too much data records
			// So: we have to remove today's rows and query them again with a 15min interval
			if (from.Date <= DateTime.Today && to.Date >= DateTime.Today)
			{
				exportRows.RemoveAll(r => r.Date.Date == DateTime.Today);

				var todayStrings = Repository.GetExport(DateTime.Today, 15);
				var todayRows = Converter.Convert(todayStrings);

				exportRows.AddRange(todayRows);
			}

			return Converter.MergeByDate(exportRows);
		}

		public IExportRepository Repository { get; }

		public SundayConverter Converter { get; }
	}
}
