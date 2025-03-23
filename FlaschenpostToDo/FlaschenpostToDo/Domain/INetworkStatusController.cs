namespace FlaschenpostToDo.Domain;

public interface INetworkStatusController
{
    public Task<bool> IsApiServerReachableAsync(CancellationToken cancellationToken = default);
    public bool IsDeviceOnline();
}