using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using SundaysApp.Model;
using SundaysApp.Services;
using Xamarin.Forms;

namespace SundaysApp.PageModels
{
    public class LoginPageModel : FreshBasePageModel
    {
        private bool _lastAttemptFailed;

        public LoginPageModel(IAuthService authService)
        {
            AuthService = authService ?? throw new ArgumentNullException(nameof(authService));

            Auth = new Auth();
            LoginCommand = new Command(async () => await PerformLogin());

            LastAttemptFailed = false;
        }

        private async Task PerformLogin()
        {
            LastAttemptFailed = false;

            var service = new SundaysFunctionService(new DirectAuthService(Auth));
            if (service.IsConfigured)
            {
                var testResult = await service.Get(DateTime.Today, DateTime.Today);

                var couldLogin = testResult.Any();
                LastAttemptFailed = !couldLogin;
                if (couldLogin)
                {
                    AuthService.SetAuth(Auth);
                    await CoreMethods.PopPageModel(modal: true, animate: true);
                }
            }
        }

        public ICommand LoginCommand { get; }

        public bool LastAttemptFailed
        {
            get => _lastAttemptFailed;
            set 
            {
                _lastAttemptFailed = value;
                RaisePropertyChanged();
            }
        }

        public Auth Auth { get; private set; }

        public IAuthService AuthService { get; }

        class DirectAuthService : IAuthService
        {
            public DirectAuthService(Auth auth)
            {
                Auth = auth;
            }

            public Auth Auth { get; private set; }

            public Auth GetAuth() => Auth;

            public void SetAuth(Auth value) => Auth = value;
        }
    }
}

