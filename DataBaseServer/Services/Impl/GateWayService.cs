using System.Threading.Tasks;
using Grpc.Core;
using GRPCService.GRPCProto;

namespace DataBaseServer.Services
{
    public class GateWayService : IGateWayService
    {
        private readonly Channel channel;
        private readonly GateWay.GateWayClient client;
        public GateWayService(string gatewayUri)
        {
            channel = new Channel(gatewayUri, ChannelCredentials.Insecure);
            client = new GateWay.GateWayClient(channel);
        }

        public async Task SendJobInfo(JobInfo jobInfo)
        {
            await client.PostJobInfoAsync(jobInfo);
        }
        
        public async void Dispose()
        {
            await channel.ShutdownAsync();
        }
    }
}