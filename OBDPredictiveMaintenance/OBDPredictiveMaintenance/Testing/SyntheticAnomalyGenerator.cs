using OBDPredictiveMaintenance.Models;

namespace OBDPredictiveMaintenance.Testing
{
    public class SyntheticAnomalyGenerator
    {
        private readonly Random random = new Random(42);

        public List<SyntheticTelemetryRecord> Generate(List<TelemetryRecord> records, int anomalyCount)
        {
            if (anomalyCount < 0 || anomalyCount > records.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(anomalyCount));
            }

            var result = records.Select(record => new SyntheticTelemetryRecord
                {
                    Data = CopyRecord(record),
                    IsAnomaly = false,
                    AnomalyType = null
                }).ToList();

            var selectedIndexes = Enumerable
                .Range(0, result.Count)
                .OrderBy(_ => random.Next())
                .Take(anomalyCount)
                .ToList();

            foreach (int index in selectedIndexes)
            {
                var item = result[index];

                int anomalyType = random.Next(5);

                switch (anomalyType)
                {
                    case 0:
                        item.Data.EngineRpm += 2500;
                        item.AnomalyType = "RPM spike";
                        break;

                    case 1:
                        item.Data.Speed += 100;
                        item.AnomalyType = "Speed spike";
                        break;

                    case 2:
                        item.Data.EngineCoolantTemp += 35;
                        item.AnomalyType = "Coolant temperature spike";
                        break;

                    case 3:
                        item.Data.EngineLoad = Math.Min(item.Data.EngineLoad!.Value + 50, 100);
                        item.AnomalyType = "Engine load increase";
                        break;

                    case 4:
                        item.Data.Maf *= 4;
                        item.AnomalyType = "MAF spike";
                        break;
                }

                item.IsAnomaly = true;
            }

            return result;
        }

        private static TelemetryRecord CopyRecord(TelemetryRecord record)
        {
            return new TelemetryRecord
            {
                Timestamp = record.Timestamp,
                EngineRpm = record.EngineRpm,
                Speed = record.Speed,
                EngineCoolantTemp = record.EngineCoolantTemp,
                EngineLoad = record.EngineLoad,
                ThrottlePos = record.ThrottlePos,
                Maf = record.Maf
            };
        }
    }
}
