using MmnDotNetSdk.Models;

namespace MmnDotNetSdk
{
    public class MmnClient
    {
        public readonly MmnNodeClient NodeClient;
        public readonly ZkProveClient ZkProveClient;

        public MmnClient(Config config)
        {
            NodeClient = new MmnNodeClient(config.Endpoint);
            ZkProveClient = new ZkProveClient(config.ZkProveEndpoint);
        }
    }
}
