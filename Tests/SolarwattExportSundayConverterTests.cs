using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Solarwatt.Api;
using Sundays;
using Sundays.Model;

namespace Tests
{
	public class SolarwattExportSundayConverterTests
	{
		public class MergeByDateMethod
		{
			private SundayConverter _converter;

			[SetUp]
			public void Setup()
			{
				_converter = new SolarwattExportSundayConverter();
			}

			[Test]
			public void Sums_Up_Values_Per_Day()
			{
				var records = new List<Sunday>();

				for (int i = 0; i < 10; i++)
				{
					records.Add(new Sunday()
					{
						Date = new DateTime(2018, 12, 31),
						PowerConsumptionWh = 1,
						PowerGenerationWh = 2,
						FeedInWh = 3,
						PurchaseWh = 4,
						BatterySupplyWh = 5,
						BatteryChargeWh = 6,
						BatteryChargePercent = 7,
						PrivateUseWh = 8,
						FeedIn2Wh = 9,
						PrivateUseFromPvWh = 10,
						PowerGenerationAtLocationWh = 11,
						PowerGenerationTotalWh = 12
					});
				}

				var merged = _converter.MergeByDate(records).ToList();

				var dayEntry = merged.Single();

				dayEntry.Date.Should().Be(new DateTime(2018, 12, 31));
				dayEntry.PowerConsumptionWh.Should().Be(10);
				dayEntry.PowerGenerationWh.Should().Be(20);
				dayEntry.FeedInWh.Should().Be(30);
				dayEntry.PurchaseWh.Should().Be(40);
				dayEntry.BatterySupplyWh.Should().Be(50);
				dayEntry.BatteryChargeWh.Should().Be(60);
				// dayEntry.BatteryChargePercent <-- extra test, no sum of percent
				dayEntry.PrivateUseWh.Should().Be(80);
				dayEntry.FeedIn2Wh.Should().Be(90);
				dayEntry.PrivateUseFromPvWh.Should().Be(100);
				dayEntry.PowerGenerationAtLocationWh.Should().Be(110);
				dayEntry.PowerGenerationTotalWh.Should().Be(120);
			}

			[Test]
			public void Takes_Last_Available_BatteryPercent_Per_Day()
			{
				var records = new List<Sunday>();

				for (int hour = 1; hour <= 20; hour++)
				{
					records.Add(new Sunday()
					{
						Date = new DateTime(2018, 12, 30, hour, 0, 0),
						BatteryChargePercent = hour <= 10 ? hour * 8 : (float?)null // 10 * 8 is highest -> 80%
					});
				}

				for (int hour = 1; hour <= 20; hour++)
				{
					records.Add(new Sunday()
					{
						Date = new DateTime(2018, 12, 31, hour, 0, 0),
						BatteryChargePercent = hour <= 10 ? hour * 5 : (float?)null // 10 * 5 is highest -> 50%
					});
				}

				var merged = _converter.MergeByDate(records).ToList();

				merged.Count.Should().Be(2);

				merged[0].BatteryChargePercent.Should().Be(80);
				merged[1].BatteryChargePercent.Should().Be(50);
			}

			[Test]
			public void Takes_Last_Available_BatteryPercent_In()
			{
				var records = new List<Sunday>();

				for (int hour = 10; hour >= 1; hour--)
				{
					records.Add(new Sunday()
					{
						Date = new DateTime(2018, 12, 30, hour, 0, 0),
						BatteryChargePercent = hour <= 10 ? hour * 8 : (float?)null
					});
				}

				// no matter what values we have, set the latest one and check for this value
				// this should be the result for the day
				records.OrderByDescending(r => r.Date).First().BatteryChargePercent = 33.3f;
				
				var merged = _converter.MergeByDate(records).ToList();
				merged[0].BatteryChargePercent.Should().Be(33.3f);
			}
		}
	}
}
