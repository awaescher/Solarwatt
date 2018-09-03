using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SundaysApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var from = DateTime.Today.AddDays(-7);
            var to = DateTime.Today;
            var sundays = await new SundaysFunctionService().Get(null, from, to);
            Console.WriteLine(sundays?.Count() ?? 0);
        }
    }
}
