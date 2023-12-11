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
        Task<string> Createtoken(Account entity, string browser,string ipaddress, int role);
        /// <summary>
        /// Check token in database if valid
        /// </summary>
        /// <returns>true or false</returns>
        Task<bool> CheckValidToken(string jwttoken);
    }
}
