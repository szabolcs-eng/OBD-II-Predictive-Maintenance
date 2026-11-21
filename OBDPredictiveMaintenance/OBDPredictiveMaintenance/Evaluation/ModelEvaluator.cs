using OBDPredictiveMaintenance.Models;

namespace OBDPredictiveMaintenance.Evaluation
{
    public class ModelEvaluator
    {
        public EvaluationResult Evaluate(List<SyntheticTelemetryRecord> expected, List<AnomalyResult> actual)
        {
            if (expected.Count != actual.Count)
            {
                throw new ArgumentException("A két lista elemszáma nem egyezik.");
            }

            int tp = 0;
            int fp = 0;
            int fn = 0;
            int tn = 0;

            for (int i = 0; i < expected.Count; i++)
            {
                bool expectedAnomaly = expected[i].IsAnomaly;
                bool detectedAnomaly = actual[i].IsAnomaly;

                if (expectedAnomaly && detectedAnomaly)
                    tp++;
                else if (!expectedAnomaly && detectedAnomaly)
                    fp++;
                else if (expectedAnomaly && !detectedAnomaly)
                    fn++;
                else
                    tn++;
            }

            double precision = tp + fp > 0 ? (double)tp / (tp + fp) : 0;

            double recall = tp + fn > 0 ? (double)tp / (tp + fn) : 0;

            double f1Score = precision + recall > 0 ? 2 * precision * recall / (precision + recall) : 0;

            return new EvaluationResult
            {
                TruePositive = tp,
                FalsePositive = fp,
                FalseNegative = fn,
                TrueNegative = tn,
                Precision = precision,
                Recall = recall,
                F1Score = f1Score
            };
        }
    }
}
