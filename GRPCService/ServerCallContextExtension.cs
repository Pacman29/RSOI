using System;
using System.Linq;
using Grpc.Core;

namespace GRPCService
{
    public static class ServerCallContextExtension
    {
        public static string GetAccessToken(this ServerCallContext context)
        {
            var authHeader = context.RequestHeaders.FirstOrDefault(h =>
                h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase));
            return authHeader?.Value.Substring("Bearer".Length).Trim();
        }
    }
}