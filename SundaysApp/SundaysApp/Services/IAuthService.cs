using System.Threading.Tasks;
using SundaysApp.Model;

namespace SundaysApp.Services
{
    public interface IAuthService
    {
        void SetAuth(Auth auth);

        Auth GetAuth();
    }
}

