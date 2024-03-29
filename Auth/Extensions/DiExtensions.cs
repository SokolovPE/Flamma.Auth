﻿using System.Text;
using EasyCaching.Core.Configurations;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using Flamma.Auth.Common.Interfaces;
using Flamma.Auth.Common.Services;
using Flamma.Auth.Data.Access.Extensions;
using Flamma.Auth.Interfaces;
using Flamma.Auth.Models;
using Flamma.Auth.Services;
using Flamma.Auth.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Flamma.Auth.Extensions;

/// <summary>
///     Dependency Injection helpers
/// </summary>
public static class DiExtensions
{
    /// <summary>
    ///     Add core service containers
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection)
    {
        // Add services
        serviceCollection.AddDataAccess();
        
        serviceCollection.AddScoped<IAccountManager, Services.CoreAccountManager>();
        serviceCollection.AddSingleton<IDateProvider, BaseDateProvider>();
        serviceCollection.AddSingleton<IHasher, Hasher>();
        serviceCollection.AddSingleton<IJwtManager, Services.JwtManager>();
        
        // Add validators
        serviceCollection.AddScoped<IValidator<Models.RegisterRequest>, RegisterRequestValidator>();
        
        return serviceCollection;
    }

    /// <summary>
    ///     Add Jwt Authentication
    /// </summary>
    public static IServiceCollection AddJwt(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddAuthorization();
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return serviceCollection;
    }

    /// <summary>
    ///     Add Cache
    /// </summary>
    public static IServiceCollection AddCache(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var endpoints = configuration.GetSection("CacheConfiguration").Get<CacheEndpointInfo[]>();
        serviceCollection.AddEasyCaching(opts =>
        {
            foreach (var endpoint in endpoints)
            {
                opts.UseRedis(config =>
                {
                    config.DBConfig.Endpoints.Add(new ServerEndPoint(endpoint.Url, endpoint.Port));
                }, endpoint.Alias).WithSystemTextJson(endpoint.Alias);
            }
        });
        return serviceCollection;
    }
}