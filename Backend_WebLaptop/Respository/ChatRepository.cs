using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Net;
using System.Xml.Linq;

namespace Backend_WebLaptop.Respository
{
    public class ChatRepository:IChatRepository
    {
        private readonly IMongoCollection<Chat> _chat;
        public ChatRepository(IDatabaseService databaseService)
        {
            _chat = databaseService.Get_Chats_Collections();
        }

        public Task DeleteAllChat(string accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<PagingResult<Chat>> GetAllChat(int pageindex, int pagesize,string sort)
        {
            //sort
            var result = await _chat.FindSync(e => true).ToListAsync();
            result = sort switch
            {
                "date" => result.OrderBy(e => e.CreateAt).ToList(),
                _ => result.OrderByDescending(e => e.CreateAt).ToList()

            };
            return new PagingResult<Chat>
            {
                Items = result.Skip((pageindex - 1) * pagesize).Take(pagesize),
                PageIndex = pageindex,
                PageSize = pagesize,
                TotalCount = result.Count
            };
        }

        public Task SendMessageToUser(string accountId, string message)
        {
            throw new NotImplementedException();
        }

        public async Task<Message> AddChat(string accountId, string message,bool isAdmin)
        {
            var Message = new Message { isAdmin = isAdmin, Content = message };
            //kiểm tra chưa có chat thì tạo mới
            var chat =await _chat.FindSync(e => e.AccountId == accountId).FirstOrDefaultAsync();
            if(chat is null)
            {
                await _chat.InsertOneAsync(new Chat { AccountId = accountId });
                chat= await _chat.FindSync(e => e.AccountId == accountId).FirstOrDefaultAsync();
            }
            //thêm vào đoạn chat
            var update = Builders<Chat>.Update.Push(e => e.Messages, Message)
                .Set(e=>e.CreateAt,DateTime.Now);
            await _chat.UpdateOneAsync(e => e.AccountId == accountId, update);
            return Message;
        }

        public Task<Chat> GetChat(string accountId)
        {
            return _chat.FindSync(e => e.AccountId == accountId).FirstOrDefaultAsync();
        }
    }
}
