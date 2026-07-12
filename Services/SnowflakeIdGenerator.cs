using UrlShortener.Interfaces;
using UrlShortener.Utilities;

namespace UrlShortener.Services;

public class SnowFlakeIdGenerator : IIdGenerator
{

    private readonly long _epoch;
    private readonly long machineId;

    public SnowFlakeIdGenerator(IConfiguration configuration)
    {
        _epoch = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
        var machineIdStr = configuration["SnowflakeSettings:MachineId"] ?? throw new InvalidOperationException("Machine Id not configured.");
        machineId = long.Parse(machineIdStr);
    }
    private long LastTimeStamp = -1;
    private long Sequence = 0;
    private readonly Lock _lock = new();
    public long GenerateId()
    {
        lock (_lock)
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _epoch;

            if (LastTimeStamp == currentTime)
            {
                Sequence++;
                if (Sequence > Constant.SequenceMaxLimit)
                {
                    while (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _epoch == currentTime)
                    {
                    }
                    currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _epoch;
                    Sequence = 0;
                    LastTimeStamp = currentTime;

                }
            }
            else
            {
                LastTimeStamp = currentTime;
                Sequence = 0;
            }

            return (long)((ulong)(currentTime << 22) | (ulong)(machineId << 12) | (ulong)Sequence);
        }
    }
}