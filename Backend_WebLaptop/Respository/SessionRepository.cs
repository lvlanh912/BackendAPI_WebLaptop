using Backend_WebLaptop.Configs;
using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


       public async Task<List<Session>> GetAllSession(string accountId)
        {
            var rs = await _sessions.FindSync(e => e.AccounId == accountId).ToListAsync();
            return rs;
        }

       public async Task<Session> Insert(Session entity)
        {
           await _sessions.InsertOneAsync(entity);
            return entity;
        }
        public async Task RemoveSession(string sessionId, string accountId)
        {

           var rs= await _sessions.DeleteOneAsync(e => e.Id == sessionId&&e.AccounId==accountId);
            if (rs.DeletedCount == 0)
                throw new Exception("Không tồn tại phiên đăng nhập để xoá");
        }

        public async Task RemoveSessionByValue(string sessionValue)
        {
          var rs = await _sessions.FindOneAndDeleteAsync(e=>e.Value==sessionValue.Replace("Bearer ",string.Empty)) ?? throw new Exception("invalid session");
        }
    }
}
