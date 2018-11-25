using Grpc.Core;

namespace GRPCService
{
    public static class MetadataExtension
    {
        public static Metadata SetAccessToken(this Metadata metadata, string token)
        {
            metadata = metadata ?? new Metadata();
            metadata.Add("Authorization",$"Bearer {token}");
            return metadata;
        }
    }
}