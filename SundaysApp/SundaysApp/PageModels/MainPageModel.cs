using System;
using System.Collections.Generic;
using System.Linq;
using FreshMvvm;
using Sundays.Model;
using SundaysApp.Services;

namespace SundaysApp.PageModels
{
    public class MainPageModel : FreshBasePageModel
    {
        private List<Sunday> _sundays;

        public MainPageModel(ISundayService sundayService)
        {
            SundayService = sundayService ?? throw new ArgumentNullException(nameof(sundayService));
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            var from = DateTime.Today.AddDays(-7);
            var to = DateTime.Today;
            Sundays = (await SundayService.Get(from, to)).ToList();
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

        public ISundayService SundayService { get; }
    }
}