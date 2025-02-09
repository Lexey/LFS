# InSim.Client

It is a mininalistic client for the LFS InSim targeting .NET 8. Maintains the connection and supports only 3 incoming informational packet types.

Possible usage:
```C#
var cancellationTokenSource = new CancellationTokenSource();
var inSimClient = new Client(new Parameters("localhost", 29999, ProtocolVersion.V8), cancellationTokenSource.Token);
inSimClient.RaceStartedPacketReceived += p =>
{
    Logger.Debug("Race started packet received");
    try
    {
        // process packet
    }
    catch (Exception e)
    {
        Logger.Error(e, "Failed to queue Race started packet");
    }
};
inSimClient.SplitCompletedPacketReceived += p =>
{
    Logger.Debug("Split completed packet received");
    try
    {
        // process packet
    }
    catch (Exception e)
    {
        Logger.Error(e, "Failed to queue Split completed packet");
    }
};
inSimClient.LapCompletedPacketReceived += p =>
{
    Logger.Debug("Lap completed packet received");
    try
    {
        // process packet
    }
    catch (Exception e)
    {
        Logger.Error(e, "Failed to queue Lap completed packet");
    }
};

...

// Shutdown:
cancellationTokenSource.Cancel();
inSimClient.Dispose();
```
