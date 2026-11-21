using OBDPredictiveMaintenance.Models;

namespace OBDPredictiveMaintenance.AnomalyDetection
{
    public class MadDetector
    {
        private static readonly Dictionary<string, double> Thresholds = new()
        {
            ["EngineRpm"] = 3.5,
            ["Speed"] = 3.5,
            ["EngineCoolantTemp"] = 8.0,
            ["EngineLoad"] = 3.5,
            ["ThrottlePos"] = 5.0,
            ["Maf"] = 3.5
        };
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

                Console.WriteLine($"{variable.Name}: Median = {median:F2}, MAD = {mad:F2}");
            }

            var results = new List<AnomalyResult>();

            var variableDetectionCounts = statistics.ToDictionary(stat => stat.Name, _ => 0);

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

                    if (score > Thresholds[statistic.Name])
                    {
                        detectedVariables.Add(statistic.Name);
                        variableDetectionCounts[statistic.Name]++;
                    }
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

            Console.WriteLine();
            Console.WriteLine("Anomáliák változónként:");
            Console.WriteLine("------------------------");

            foreach (var item in variableDetectionCounts)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
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
