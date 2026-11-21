using OBDPredictiveMaintenance.Models;

namespace OBDPredictiveMaintenance.AnomalyDetection
{
    public class MadDetector
    {
        private const double Threshold = 3.5;
        private const double ScalingFactor = 0.6745;

        public List<AnomalyResult> Detect(List<TelemetryRecord> records)
        {
            var statistics = new List<(string Name, Func<TelemetryRecord, double?> Selector, double Median, double Mad)>();

            var variables = new List<(string Name, Func<TelemetryRecord, double?> Selector)>
        {
            ("EngineRpm", r => r.EngineRpm),
            ("Speed", r => r.Speed),
            ("EngineCoolantTemp", r => r.EngineCoolantTemp),
            ("EngineLoad", r => r.EngineLoad),
            ("ThrottlePos", r => r.ThrottlePos),
            ("Maf", r => r.Maf)
        };

            foreach (var variable in variables)
            {
                var values = records
                    .Select(variable.Selector)
                    .Where(value => value.HasValue)
                    .Select(value => value!.Value)
                    .ToList();

                double median = CalculateMedian(values);

                var deviations = values
                    .Select(value => Math.Abs(value - median))
                    .ToList();

                double mad = CalculateMedian(deviations);

                statistics.Add((variable.Name, variable.Selector, median, mad));
            }

            var results = new List<AnomalyResult>();

            for (int i = 0; i < records.Count; i++)
            {
                var record = records[i];

                double maxScore = 0;
                var detectedVariables = new List<string>();

                foreach (var statistic in statistics)
                {
                    double? value = statistic.Selector(record);

                    if (!value.HasValue || statistic.Mad == 0)
                        continue;

                    double score = Math.Abs(ScalingFactor * (value.Value - statistic.Median) / statistic.Mad);

                    if (score > maxScore)
                        maxScore = score;

                    if (score > Threshold)
                        detectedVariables.Add(statistic.Name);
                }

                results.Add(new AnomalyResult
                {
                    RecordIdx = i,
                    Timestamp = record.Timestamp,
                    IsAnomaly = detectedVariables.Count > 0,
                    Score = maxScore,
                    DetectedVariables = detectedVariables
                });
            }

            return results;
        }

        private static double CalculateMedian(List<double> values)
        {
            if (values.Count == 0)
                return 0;

            var sorted = values.OrderBy(value => value).ToList();

            int middle = sorted.Count / 2;

            if (sorted.Count % 2 == 0)
            {
                return (sorted[middle - 1] + sorted[middle]) / 2.0;
            }

            return sorted[middle];
        }
    }
}
