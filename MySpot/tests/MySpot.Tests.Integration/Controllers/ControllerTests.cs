using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySpot.Application.DTO;
using MySpot.Application.Security;
using MySpot.Infrastructure.Auth;
using MySpot.Infrastructure.DAL;
using MySpot.Infrastructure.Time;

namespace MySpot.Tests.Integration.Controllers
{
    public abstract class ControllerTests : IClassFixture<OptionsProvider>
    {
        private readonly IAuthenticator _authenticator;
        protected HttpClient Client { get; }
        protected JwtDto Authorize(Guid userId,string role)
        {
            var jwt = _authenticator.CreateToken(userId, role);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.AccessToken);

            return jwt;
        }
        public ControllerTests(OptionsProvider optionsProvider) 
        {
            var authOptions = optionsProvider.Get<AuthOptions>("auth");
            _authenticator = new Authenticator(new OptionsWrapper<AuthOptions>(authOptions), new Clock());

            var app =  new MySpotTestApp(ConfigureServices);
            Client = app.Client;
            //var postgresOptions = optionsProvider.Get<PostgresOptions>("postgres");
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {

        }
    }
}
