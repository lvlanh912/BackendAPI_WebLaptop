using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IAuthenticationRepository
    {
        /// <summary>
        /// Create jwt token 
        /// </summary>
        /// <returns></returns>
        Task<string> Createtoken(Account entity);
        /// <summary>
        /// Check token in database if valid
        /// </summary>
        /// <returns>true or false</returns>
        Task<bool> CheckValidToken(string jwttoken);
    }
}
