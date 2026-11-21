namespace OBDPredictiveMaintenance.Models
{
    public class AnomalyResult
    {
        public int RecordIdx { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsAnomaly { get; set; }
        public double Score { get; set; }
        public List<string> DetectedVariables { get; set; } = new List<string>();
    }
}
