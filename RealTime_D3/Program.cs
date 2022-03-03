using Microsoft.AspNetCore.Identity;
using RealTime_D3.Extensions;
using RealTime_D3.Models;
using Npgsql;
using Serilog;
using Npgsql.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using RealTime_D3.Hubs;
using RealTime_D3.Configurations;
using RealTime_D3.Contracts;
using RealTime_D3.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var connString = builder.Configuration.GetConnectionString("postgresql");

builder.Services.AddDbContext<RealtimeDbContext>(options => options.UseNpgsql(connString));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", opt => opt
        .WithOrigins(
        //"https://localhost:7275", 
        "http://127.0.0.1:5500/"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        //.SetIsOriginAllowed((host) => true)
        );
});
builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddIdentityCore<ApiUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<RealtimeDbContext>()
                .AddDefaultTokenProviders();
//The AddDefaultTokenProviders extension method will do exactly that, add the required token providers to enable the token generation in our project. But there is one more thing we have to configure.
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
   opt.TokenLifespan = TimeSpan.FromHours(2));
// for our password reset token is to be valid for a limited time:2 hours
builder.Services.AddEndpointsApiExplorer();

builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));


builder.Services.AddScoped<ITbllogRepository, TbllogRepository>();
builder.Services.AddScoped<IRealtimeLogRepository, RealtimeLogRepository>();



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<LogHub>("/loghub");



});

app.Run();

app.UsePostgreSQLBroker();
