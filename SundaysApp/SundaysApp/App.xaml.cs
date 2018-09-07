using System;
using FreshMvvm;
using SundaysApp.PageModels;
using SundaysApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SundaysApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var ioc = FreshIOC.Container;

            ioc.Register<IAuthService, PersistentAuthService>();
            ioc.Register<ISundayService, SundaysFunctionService>();
            ioc.Register<ICryptoService, AesCryptoService>();

            var page = FreshPageModelResolver.ResolvePageModel<MainPageModel>();
            var basicNavContainer = new FreshNavigationContainer(page);
            MainPage = basicNavContainer;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
