namespace OneLine.Security.Infrastructure.Options;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";
    public int MaxRequestsPerMinutePerIp { get; set; } = 60;
    public int MaxRequestsPerMinutePerUser { get; set; } = 100;
    public int MaxFailedLoginAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 15;
}
