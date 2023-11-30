using Backend_WebLaptop.Configs;
using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backend_WebLaptop.Respository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly IMongoCollection<Session> _sessions;
        public SessionRepository(IDatabaseService database)
        {
            _sessions = database.Get_Sessions_Collections();
        }

        public async Task<bool> CheckValidSession(string jwtToken)
        {
            var rs =await _sessions.FindSync(e => e.Value == jwtToken).FirstOrDefaultAsync();
            return rs != null;
        }

        async Task<List<Session>> ISessionRepository.GetAllSession(string accountId)
        {
            var rs = await _sessions.FindSync(e => e.AccounId == accountId).ToListAsync();
            return rs;
        }

        async Task<Session> ISessionRepository.Insert(Session entity)
        {
           await _sessions.InsertOneAsync(entity);
            return entity;
        }
        async Task<bool> ISessionRepository.RemoveSession(string sessionId)
        {
           var rs= await _sessions.DeleteOneAsync(e => e.Id == sessionId);
            if (rs.DeletedCount == 0)
                throw new Exception("Không tồn tại phiên đăng nhập để xoá");
            return true;
        }
    }
}
