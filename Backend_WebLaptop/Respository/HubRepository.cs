using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Backend_WebLaptop.Respository
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HubRepository : Hub, IHubRepository
    {
        private static Dictionary<string, string> ListUserConected = new Dictionary<string, string>();
        private readonly IChatRepository _chat;
        private readonly ISessionRepository _session;
        public HubRepository(IChatRepository chat, ISessionRepository session)
        {
            _chat = chat;
            _session = session;
        }

        

        public override Task OnConnectedAsync()
        {
            string userId = Context.User?.FindFirst("Id")?.Value!;
            string connectionId = Context.ConnectionId;
            //lấy role
            var role = Context.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            //Thêm vào Danh sách
            if (role == "Admin")
                ListUserConected["Admin"] = connectionId;
            else
                ListUserConected[userId] = connectionId;
            

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //khi ngắt kết nối xoá khỏi Idictonary
            string userId = Context.User?.FindFirst("Id")?.Value!;
            var role = Context.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            if(role=="Admin")
                ListUserConected.Remove("Admin");
            else if (ListUserConected.ContainsKey(userId))
            {
                // Xoá thông tin userId và connectionId khi kết nối bị ngắt
                ListUserConected.Remove(userId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task SendMessageToAdmin(string message)
        {
                var accounId = Context.User!.FindFirst("Id")!.Value;
                //thêm nội dung chat vào cơ sở dữ liệu
                var chat= await _chat.AddChat(accounId, message, false);
            //nếu admin đang connect ->Gửi cho admin
            if (ListUserConected.TryGetValue("Admin", out var ConnectionId))
            {
                //Gửi thông điệp cho admin
                await Clients.Client(ConnectionId).SendAsync("ReceiveMessage", new { message = chat, accountId = accounId });
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task SendMessageToUser(string userId, string message)
        {
            var chat = await _chat.AddChat(userId, message, true);
            if (ListUserConected.TryGetValue(userId, out var ConnectionId))
            {
                //Gửi thông điệp cho Client
                await Clients.Client(ConnectionId).SendAsync("ReceiveMessage", chat);
            }
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task AdminDisconnectChat(string userId)
        {
            if (ListUserConected.TryGetValue(userId, out var ConnectionId))
            {
                //Gửi thông điệp cho user
                await Clients.Client(ConnectionId).SendAsync("Disconnect");
            }
            //xoá chat trên database
            await _chat.DeleteChat(userId);
        }

        [Authorize(Roles = "Member")]
        [ServiceFilter(typeof(SessionAuthor))]
        public async Task UserDisconnectChat()
        {
            var accounId = Context.User!.FindFirst("Id")!.Value;
            if (ListUserConected.TryGetValue("Admin", out var ConnectionId))
            {
                //Gửi thông điệp cho admin
                await Clients.Client(ConnectionId).SendAsync("Disconnect",accounId);
            }
            //xoá chat trên database
            await _chat.DeleteChat(accounId);
        }
    }
}
