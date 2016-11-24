using System.Threading.Tasks;

namespace VTS.Core.CrossCutting
{
    public interface INetworkReachability
    {
        bool IsOnlineMode { get; }
        Task<bool> IsReachableHost(string host, int msTimeout); 
    }
}
