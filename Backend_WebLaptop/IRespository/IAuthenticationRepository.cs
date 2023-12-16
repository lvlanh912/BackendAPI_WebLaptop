using Backend_WebLaptop.Model;
using System.Data;

namespace Backend_WebLaptop.IRespository
{
    public interface IAuthenticationRepository
    {
        /// <summary>
        /// Login and create jwt token role=1 is user role=2 is admin
        /// </summary>
        /// <returns></returns>
        Task<string> CreatetokenForUser(Account entity, string browser,string ipaddress, int role);

        Task SendLinkResetPassword(string email);
        Task ResetPassword(string accessToken);
        Task GetConfirmEmail(string email);
        Task<bool> ConfirmEmail(string? email,int otp);
    }
}
