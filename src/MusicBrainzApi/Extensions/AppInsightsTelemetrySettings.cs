namespace MusicBrainzApi.Extensions
{
    public class AppInsightsTelemetrySettings
    {
        public bool BodyLoggingEnabled { get; set; }

        public bool HeaderLoggingEnabled { get; set; }

        public bool SerilogSelfLogEnabled { get; set; }
    }
}