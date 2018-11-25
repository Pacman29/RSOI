using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthOptions
{
    public static class SetupAuthServices
    {
        public static void Setup(IServiceCollection services , IConfiguration configuration)
        {
            
            var accountOptions = new AuthOptions()
            {
                ISSUER = "authServer",
                AUDIENCE = "GateWay"
            };
            var jwtTokenGenerator = new JwtTokenGenerator(accountOptions);
            services.AddSingleton<IJwtTokenGenerator>(jwtTokenGenerator);
            services.AddSingleton<AuthOptions>(accountOptions);
            configuration.GetSection("Security:Tokens").Bind(accountOptions);
            services.Configure<AuthOptions>(options => configuration.GetSection("Security:Tokens").Bind(options));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = accountOptions.GetParameters();
                });
        }
    }
}