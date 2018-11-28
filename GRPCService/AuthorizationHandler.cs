using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace GRPCService
{
    public class AuthorizationHandler
    {
        private readonly string _target;
        private readonly ChannelCredentials _credentials;
        private readonly IEnumerable<ChannelOption> _options;
        public string ServerName { get; }
        public string Password { get; }
        
        private readonly object _threadLock = new object();

        private string _token = "";
        public string Token
        {
            get
            {
                lock (_threadLock)
                {
                    return _token;
                }   
            }
        }

        public AuthorizationHandler(string serverName, string password,
            string target, 
            ChannelCredentials credentials, 
            IEnumerable<ChannelOption> options)
        {
            _target = target;
            _credentials = credentials;
            _options = options;
            ServerName = serverName;
            Password = password;
        }

        public Func<Task<string>> GetAuthorizeFunc()
        {
            return async () =>
            {
                var channel =  new Channel(_target, _credentials, _options);
                var authClient = new Authorize.AuthorizeClient(channel);
                try
                {
                    var result = await authClient.LoginAsync(new ServerInfo()
                    {
                        Password = Password,
                        ServerName = ServerName
                    });
                    if (result.Token != "")
                    {
                        lock (_threadLock)
                        {
                            _token = result.Token;
                            return _token;
                        }
                    }

                    return "";
                }
                catch (RpcException e)
                {
                    return "";
                }
            };
        }
    }
}