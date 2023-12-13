namespace Backend_WebLaptop.IRespository
{
    public interface IHubRepository
    {
        public Task SendMessage(string user, string message);
        public Task SendMessageToAdmin(string message);
        public Task SendMessageToUser(string userId,  string message);
    }
}
