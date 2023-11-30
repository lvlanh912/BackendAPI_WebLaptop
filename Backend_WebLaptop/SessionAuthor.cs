using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop;

public class SessionAuthor : IAuthorizationFilter
{
    private readonly ISessionRepository _session;
    public SessionAuthor(ISessionRepository session)
    {
        _session = session;
    }
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        // Kiểm tra điều kiện tùy chỉnh ở đây
        if (!await _session.CheckValidSession(context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ","")))
        {
            context.Result = new UnauthorizedResult();
        }
    }

}
