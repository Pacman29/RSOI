using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using AuthOptions;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace GRPCService
{
    public class AuthServerGrpc : Authorize.AuthorizeBase
    {
        private readonly string _serverName;
        private readonly string _password;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthServerGrpc(string serverName, string password, IJwtTokenGenerator jwtTokenGenerator)
        {
            _serverName = serverName;
            _password = password;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        
        public override async Task<AuthResponse> Login(ServerInfo request, ServerCallContext context)
        {
            if(request.ServerName == _serverName && request.Password == _password)
                return new AuthResponse()
                {
                    ServerName = request.ServerName,
                    Token = new JwtSecurityTokenHandler().WriteToken(
                        _jwtTokenGenerator.GenerateJwtToken("0",request.ServerName))
                };
            context.Status = new Status(StatusCode.Unauthenticated, "Login incorrect");
            return default(AuthResponse);
        }
    }
}