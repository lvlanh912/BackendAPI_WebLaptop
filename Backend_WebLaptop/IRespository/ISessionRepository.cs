using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface ISessionRepository
    {
        public Task<Session> Insert(Session entity);
        public Task<List<Session>> GetAllSession(string accountId);
        public Task RemoveSession(string sessionId,string accountId);
        public Task RemoveSessionByValue(string sessionValue);
        public Task<bool> CheckValidSession(string jwtToken);
    }
}
