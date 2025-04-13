using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shamia.API.Config;
using Shamia.API.Hubs;
using Shamia.API.MiddleWare;
using Shamia.API.Services;
using Shamia.API.Services.interFaces;
using Shamia.API.Services.InterFaces;
using Shamia.DataAccessLayer;
using Shamia.DataAccessLayer.Entities;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<EmailServiceConfigruation>(builder.Configuration.GetSection("EmailServiceConfigruation"));
builder.Services.Configure<GoogleAuthConfiguration>(builder.Configuration.GetSection("AuthConfigruation:Google"));

builder.Services.Configure<FatoorahConfig>(builder.Configuration.GetSection("Fatoorah"));
    
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


//builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<FatoorahConfig>>().Value);


builder.Services.AddScoped<IFatoorahService, FatoorahService>();


builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Secret)),
    ValidIssuer = jwtConfig.ValidIssuer,
    // change in production
    ValidateIssuer = false,
    ValidAudience = jwtConfig.ValidAudience,
    // change in production
    ValidateAudience = false,
    RequireExpirationTime = true,
    //ValidateLifetime = true,
    ClockSkew = TimeSpan.FromMinutes(jwtConfig.ClockSkewMinutes),
};

builder.Services.AddSingleton<TokenValidationParameters>(tokenValidationParameters);
builder.Services.AddSingleton<JwtConfig>(jwtConfig);




builder.Services.AddProjectServices(builder.Configuration);

builder.Services.AddIdentityCore<User>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 6;    
})
    .AddEntityFrameworkStores<ShamiaDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(2);
});


builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IJwtHandlerService, JwtHandlerService>();

builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IFatoorahService, FatoorahService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<NotificationHub>();

builder.Services.AddSignalR();

builder.Services.AddHttpClient("FatoorahClient", client =>
{
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Enforce TLS 1.2 or higher for this client
    SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13
}); 


//builder.Services.AddScoped<MyFatoorahService>();
//builder.Services.AddScoped<IFatoorahService, FatoorahService>();


builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = tokenValidationParameters;
    });

builder.Services.AddRouting(opt =>
{
    opt.LowercaseUrls = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                //.AllowCredentials()
               );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<NotificationHub>("/notificationhub");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseCors("AllowAll");

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.MapControllers();

app.Run();
