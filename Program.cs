using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VenusERP_Persistence;
using VenusERP_Application;
using System.Text;
using VenusERP_API.Data;
using EInvoiceKSADemo.Helpers.Zatca;
using VenusERP_API.Controllers.Reports;
using VenusERP_API.Controllers.Operations;
using VenusERP_API.RealTimeHub;
using VenusERP_Persistence.Context;
using PharmacyIntegrationTrackCore;
using VenusERP_API.Providers;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddHttpContextAccessor();
builder.Services.AddPersistence();
builder.Services.AddApplication();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddZatcaServices(configuration);
builder.Services.AddScoped<PrintFactory>();
builder.Services.AddScoped<RSD>();
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt auth header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
});
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<ApplicationDBContext>(opt =>
{
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

SharedData.APIUrl = configuration.GetSection("Zatca")["url"];

//JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

var app = builder.Build();


RsdSharedData.IsProduction = bool.Parse(builder.Configuration["Rsd:IsProduction"]);
if (RsdSharedData.IsProduction)
{
    RsdSharedData.BaseUrl = builder.Configuration["Rsd:ProductionUrl"];
}
else
{
    RsdSharedData.BaseUrl = builder.Configuration["Rsd:TestUrl"];
}


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();



app.UseAuthentication();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();

