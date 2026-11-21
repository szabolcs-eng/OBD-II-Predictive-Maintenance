namespace OBDPredictiveMaintenance.Models
{
    public class EvaluationResult
    {
        public int TruePositive { get; set; }
        public int FalsePositive { get; set; }
        public int FalseNegative { get; set; }
        public int TrueNegative { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
    }
}
