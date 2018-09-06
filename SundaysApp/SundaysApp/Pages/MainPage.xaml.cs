using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SundaysApp.Services;
using Xamarin.Forms;
using Microcharts;
using SkiaSharp;
using Sundays.Model;
using System.Threading;

namespace SundaysApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as PageModels.MainPageModel).PropertyChanged += Handle_PropertyChanged;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as PageModels.MainPageModel).PropertyChanged -= Handle_PropertyChanged;
            base.OnDisappearing();
        }

        protected void Handle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("Sundays"))
                return;

            generationChartView.Chart = generationChartView.Chart ?? new BarChart() {  };
            consumptionChartView.Chart = consumptionChartView.Chart ?? new BarChart() { };

            var dateFormat = Thread.CurrentThread.CurrentUICulture.DateTimeFormat;

            var sundays = (BindingContext as PageModels.MainPageModel)
                .Sundays.Where(d => d.Date <= DateTime.Today)
                .ToList();

            var generationEntries = sundays.Select(d =>
                new Microcharts.Entry(d.PowerGenerationTotalWh)
                {
                    ValueLabel = (d.PowerGenerationTotalWh / 1000).ToString("F1") + " kWh",
                    Label = dateFormat.GetShortestDayName(d.Date.DayOfWeek),
                    Color = SKColor.Parse(d.Date == DateTime.Today ? "#b0f9a4" : "#90D585")
                }
            );

            var consumptionEntries = sundays.Select(d =>
                new Microcharts.Entry(-1 * d.PowerConsumptionWh)
                {
                    ValueLabel = (-1 * d.PowerConsumptionWh / 1000).ToString("F1") + " kWh",
                    // Label = dateFormat.GetShortestDayName(d.Date.DayOfWeek), // on week scale's enough from the first chart
                Color = SKColor.Parse(d.Date == DateTime.Today ? "#f9ada4" : "#d58d85")
                }
            );

            generationChartView.Chart.Entries = generationEntries;
            consumptionChartView.Chart.Entries = consumptionEntries;

            EnsureSameScale(generationChartView.Chart as BarChart, consumptionChartView.Chart as BarChart);
        }

        private void EnsureSameScale(BarChart generationChart, BarChart consumptionChart)
        {
            if (generationChart.MaxValue > Math.Abs(consumptionChart.MinValue))
                consumptionChart.MinValue = -1 * generationChart.MaxValue;
            else
                generationChart.MaxValue = Math.Abs(consumptionChart.MinValue);
        }
    }
}
