using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using Microcharts;
using System.Threading;

namespace Sundays.Client.App
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			var entries = new[]
			{
				new Microcharts.Entry(10957)
				{
					Label = "Wednesday",
					ValueLabel = "10,9",
					Color = SKColor.Parse("#266489")
				},
				new Microcharts.Entry(12427)
				{
					Label = "Thursday",
					ValueLabel = "12,4",
					Color = SKColor.Parse("#68B9C0")
				},
				new Microcharts.Entry(14430)
					{
					Label = "Friday",
					ValueLabel = "14,4",
					Color = SKColor.Parse("#90D585")
				},
				new Microcharts.Entry(17114)
					{
					Label = "Saturday",
					ValueLabel = "17,1",
					Color = SKColor.Parse("#90D585")
				},
				new Microcharts.Entry(11986)
					{
					Label = "Sunday",
					ValueLabel = "11,9",
					Color = SKColor.Parse("#90D585")
				},
				new Microcharts.Entry(12804)
					{
					Label = "Monday",
					ValueLabel = "12,8",
					Color = SKColor.Parse("#90D585")
				},
				new Microcharts.Entry(10813)
					{
					Label = "Tuesday",
					ValueLabel = "10,8",
					Color = SKColor.Parse("#90D585")
				}
			};

			var chart = new BarChart() { Entries = entries, BackgroundColor = SKColor.Empty };
			consumptionChartView.Chart = chart;

			entries = new[]
			{
				new Microcharts.Entry(12088)
				{
					Label = "Wednesday",
					ValueLabel = "12,0",
					Color = SKColor.Parse("#266489")
				},
				new Microcharts.Entry(14629)
				{
					Label = "Thursday",
					ValueLabel = "14,6",
					Color = SKColor.Parse("#68B9C0")
				},
				new Microcharts.Entry(16978)
					{
					Label = "Friday",
					ValueLabel = "16,9",
					Color = SKColor.Parse("#90D585")
				},
				new Microcharts.Entry(20410)
					{
					Label = "Saturday",
					ValueLabel = "20,4",
					Color = SKColor.Parse("#90D585")
				},
				new Microcharts.Entry(1964)
					{
					Label = "Sunday",
					ValueLabel = "1,9",
					Color = SKColor.Parse("#90D585")
				},
				new Microcharts.Entry(4904)
					{
					Label = "Monday",
					ValueLabel = "4,9",
					Color = SKColor.Parse("#90D585")
				},
				new Microcharts.Entry(1354)
					{
					Label = "Tuesday",
					ValueLabel = "1,3",
					Color = SKColor.Parse("#90D585")
				}
			};

			chart = new BarChart() { Entries = entries, BackgroundColor = SKColor.Empty };
			generationChartView.Chart = chart;
		}
	}
}
