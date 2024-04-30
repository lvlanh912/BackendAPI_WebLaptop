
using Backend_WebLaptop.Configs;
using Backend_WebLaptop.Configs.OnlinePayment;
using Backend_WebLaptop.Database;
using Backend_WebLaptop.IRespository;
using Backend_WebLaptop.Respository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = 102400000;
});
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddLogging();
builder.Services.Configure<DatabaseConfig>(
    builder.Configuration.GetSection("Database")
);
builder.Services.Configure<MailConfig>(
    builder.Configuration.GetSection("EmailConfig")
);
builder.Services.Configure<AuthenticationConfig>(
    builder.Configuration.GetSection("Authentication")
);
builder.Services.Configure<FrontendConfig>(
    builder.Configuration.GetSection("Frontend")
);
builder.Services.Configure<VNPayconfig>(
    builder.Configuration.GetSection("OnlinePayment:VnPay")
);
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy",
        builder => builder
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                            .AllowAnyOrigin()
                          );
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IEmailSendRepository, EmailSendRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentOnlineRepository, PaymentOnlineRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IShippingAddressRepository, ShippingAddressRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUploadImageRepository, UploadImageRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
//Đăng ký dịch vụ kiểm tra session
builder.Services.AddScoped<SessionAuthor>();
//builder.Services.AddScoped<IStatusOrderingRepository, StatusOrderingRepository>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<INewRepository, NewsRepository>();
//Thêm middleWare xác thực jwt token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {

        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,//kiểm tra token còn thời gian sử dụng nữa hay không
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/Hub")))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});



var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("MyCorsPolicy");
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx => {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Content-Type", "image/jpeg");
        ctx.Context.Response.Headers.Append("Accept-Ranges", "bytes");
    },
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseWebSockets();
app.MapHub<HubRepository>("Hub");

app.Run();
