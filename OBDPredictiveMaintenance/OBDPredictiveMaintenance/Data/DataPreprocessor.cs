using OBDPredictiveMaintenance.Models;

namespace OBDPredictiveMaintenance.Data
{
    public class DataPreprocessor
    {
        public List<TelemetryRecord> Process(List<TelemetryRecord> records)
        {
            return records.Where(IsValidRecord).OrderBy(record => record.Timestamp).ToList();
        }

        private static bool IsValidRecord(TelemetryRecord record)
        {
            return
                IsValidNumber(record.EngineRpm) &&
                IsValidNumber(record.Speed) &&
                IsValidNumber(record.EngineCoolantTemp) &&
                IsValidNumber(record.EngineLoad) &&
                IsValidNumber(record.ThrottlePos) &&
                IsValidNumber(record.Maf);
        }

        private static bool IsValidNumber(double? value)
        {
            return value.HasValue && double.IsFinite(value.Value);
        }
    }
}
