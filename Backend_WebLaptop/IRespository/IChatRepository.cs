using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IChatRepository
    {
        public Task<Message> AddChat(string accountId, string message,bool isAdmin);
        public Task DeleteChat(string accountId);
        public Task SendMessageToUser(string accountId, string message);
        /// <summary>
        /// Return all chat
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="type"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<PagingResult<Chat>> GetAllChat(int pageindex, int pagesize, string sort);
        Task<Chat> GetChat(string accountId);

    }
}
