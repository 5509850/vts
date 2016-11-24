using Plugin.Connectivity;
using System.Threading.Tasks;
using VTS.Core.CrossCutting;

namespace VTS.Core.Business.Services
{
    public class NetworkReachability : INetworkReachability
    {
        public bool IsOnlineMode
        {
            get
            {
                return CrossConnectivity.Current.IsConnected;
            }
        }
        public async Task<bool> IsReachableHost(string host, int msTimeout)
        {
            return await CrossConnectivity.Current.IsReachable(host, msTimeout);
        }
    }
}