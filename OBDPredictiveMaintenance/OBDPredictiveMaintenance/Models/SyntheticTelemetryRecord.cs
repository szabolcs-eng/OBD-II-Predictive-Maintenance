namespace OBDPredictiveMaintenance.Models
{
    public class SyntheticTelemetryRecord
    {
        public TelemetryRecord Data { get; set; } = new TelemetryRecord();
        public bool IsAnomaly { get; set; }
        public string? AnomalyType { get; set; }
    }
}
