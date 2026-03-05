using Serilog;
using Serilog.Events;

namespace SocietyLedger.Shared.Logging
{
    public static class SerilogConfiguration
    {
        public static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/Society_ledger-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
