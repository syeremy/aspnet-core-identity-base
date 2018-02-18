using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syeremy.Fundamentals.Data;

namespace Syeremy.Fundamentals.Middleware
{
    public class CreateUserMIddleware
    {
        RequestDelegate _next;


        //IServiceProvider _serviceProvider;
        //UserManager<ApplicationUser> _userManager;
        ILogger<CreateUserMIddleware> _logger;

        public IConfiguration Configuration { get; }





        public CreateUserMIddleware(RequestDelegate next, ILogger<CreateUserMIddleware> logger, IConfiguration configuration)
        {
            _next = next;


            //_userManager = userManager;
            Configuration = configuration;

            // -- Creating Manually a User Manager 
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext(optionsBuilder.Options));
            IPasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
            var validator = new UserValidator<ApplicationUser>();
            var validators = new List<UserValidator<ApplicationUser>> { validator };
            var userManager = new UserManager<ApplicationUser>(userStore, null, hasher, validators, null, null, null, null, null);

            var x  = userManager.CreateAsync(new ApplicationUser("syeremy"), "p@ssw0rd");
            var result = x.Result;

            // --

            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);

        }
    }

    public static class MyMiddlewareExtensions
    {

        public static IApplicationBuilder UseCreateUserMIddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CreateUserMIddleware>();
        }
    }
}
