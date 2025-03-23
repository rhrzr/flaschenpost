using System.Net.Sockets;
using FlaschenpostToDo.Domain;

namespace FlaschenpostToDo.Data.Network;

public class NetworkStatusController : INetworkStatusController
{
    public async Task<bool> IsApiServerReachableAsync(CancellationToken cancellationToken = default)
    {
        var isReachable = false;
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(Settings.ApiAddress, Settings.Port, cancellationToken);
            isReachable = true;
        }
        catch (Exception)
        {
            // ignored
        }

        return isReachable;
    }

    public bool IsDeviceOnline()
    {
        return Connectivity.Current.NetworkAccess >= NetworkAccess.Internet;
    }
}