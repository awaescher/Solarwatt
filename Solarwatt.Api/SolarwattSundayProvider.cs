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

			var exportStrings = Repository.GetExport(from, to);
			var exportRows = Converter.Convert(exportStrings);

			return Converter.MergeByDate(exportRows);
		}

		public IExportRepository Repository { get; }

		public SundayConverter Converter { get; }
	}
}
