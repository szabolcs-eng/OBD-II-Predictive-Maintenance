namespace OBDPredictiveMaintenance.Models
{
    public class TelemetryRecord
    {
        public DateTime Timestamp { get; set; }
        public double? EngineRpm { get; set; }
        public double? Speed { get; set; }
        public double? EngineCoolantTemp { get; set; }
        public double? EngineLoad { get; set; }
        public double? ThrottlePos { get; set; }
        public double? Maf { get; set; }
    }
}
