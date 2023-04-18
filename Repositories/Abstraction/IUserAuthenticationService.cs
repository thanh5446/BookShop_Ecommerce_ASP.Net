using Assignment.Models.DBO;

namespace Assignment.Repositories.Abstraction
{
    public interface IUserAuthenticationService
    {
        public  Task<Status> LoginAsync(LoginModel model);
        public  Task<Status> RegistrationAsync(RegistrationModel model);
        public  Task TaskLogoutAsync();
    }
}
