using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sundays
{
	public abstract class SundayConverter
	{
		public abstract IEnumerable<Sunday> Convert(object value);

		public IEnumerable<Sunday> MergeByDate(IEnumerable<Sunday> rows)
		{
			var result = new List<Sunday>();

			foreach (var row in rows.OrderBy(r => r.Date).ToArray())
			{
				row.Date = row.Date.Date;
				var existingRow = result.SingleOrDefault(e => e.Date == row.Date);

				if (existingRow == null)
				{
					result.Add(row);
				}
				else
				{
					//existingRow.Date = Convert.ToDateTime(values[0]),
					existingRow.PowerConsumptionWh += row.PowerConsumptionWh;
					existingRow.PowerGenerationWh += row.PowerGenerationWh;
					existingRow.FeedInWh += row.FeedInWh;
					existingRow.PurchaseWh += row.PurchaseWh;
					existingRow.BatterySupplyWh += row.BatterySupplyWh;
					existingRow.BatteryChargeWh += row.BatteryChargeWh;
					existingRow.BatteryChargePercent += row.BatteryChargePercent;
					existingRow.PrivateUseWh += row.PrivateUseWh;
					existingRow.FeedIn2Wh += row.FeedIn2Wh;
					existingRow.PrivateUseFromPvWh += row.PrivateUseFromPvWh;
					existingRow.PowerGenerationAtLocationWh += row.PowerGenerationAtLocationWh;
					existingRow.PowerGenerationTotalWh += row.PowerGenerationTotalWh;
				}
			}

			return result;
		}
	}
}
