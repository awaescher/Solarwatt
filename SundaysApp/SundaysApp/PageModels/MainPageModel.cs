using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using Sundays.Model;
using SundaysApp.Services;
using Xamarin.Forms;

namespace SundaysApp.PageModels
{
    public class MainPageModel : FreshBasePageModel
    {
        private List<Sunday> _sundays;
        private bool _isLoading;

        public MainPageModel(ISundayService sundayService)
        {
            SundayService = sundayService ?? throw new ArgumentNullException(nameof(sundayService));

            RefreshCommand = new Command(async () => await LoadWeek());
            IsLoading = false;
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (!SundayService.IsConfigured)
            {
                await CoreMethods.PushPageModel<LoginPageModel>(null, modal: true, animate: true);
            }

            if (SundayService.IsConfigured)
                await LoadWeek();
        }

        private async Task LoadWeek()
        {
            if (IsLoading)
                return;

            IsLoading = true;

            var from = DateTime.Today.AddDays(-7);
            var to = DateTime.Today;
            Sundays = (await SundayService.Get(from, to)).ToList();

            IsLoading = false;
        }

        public List<Sunday> Sundays
        {
            get => _sundays;
            set
            {
                _sundays = value;
                RaisePropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public ISundayService SundayService { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FinishedLoading));
            }
        }

        public bool FinishedLoading => !IsLoading;
    }
}