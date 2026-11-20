using Microsoft.VisualBasic.FileIO;
using OBDPredictiveMaintenance.Models;
using System.Globalization;

namespace OBDPredictiveMaintenance.Data
{
    public class CsvDataReader
    {
        public List<TelemetryRecord> ReadVehicleData(string filePath, string vehicleId)
        {
            var records = new List<TelemetryRecord>();

            using var parser = new TextFieldParser(filePath);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;

            string[] headers = parser.ReadFields() ?? throw new InvalidOperationException("A CSV fejléc nem olvasható.");

            var columnIndexes = headers
                .Select((name, index) => new { name, index })
                .ToDictionary(x => x.name, x => x.index);

            while (!parser.EndOfData)
            {
                string[]? fields = parser.ReadFields();

                if (fields == null)
                    continue;

                if (fields[columnIndexes["VEHICLE_ID"]] != vehicleId)
                    continue;

                var record = new TelemetryRecord
                {
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(fields[columnIndexes["TIMESTAMP"]])).DateTime,

                    EngineRpm = ParseNumber(fields[columnIndexes["ENGINE_RPM"]]),

                    Speed = ParseNumber(fields[columnIndexes["SPEED"]]),

                    EngineCoolantTemp = ParseNumber(fields[columnIndexes["ENGINE_COOLANT_TEMP"]]),

                    EngineLoad = ParseNumber(fields[columnIndexes["ENGINE_LOAD"]]),

                    ThrottlePos = ParseNumber(fields[columnIndexes["THROTTLE_POS"]]),

                    Maf = ParseNumber(fields[columnIndexes["MAF"]])
                };

                records.Add(record);
            }

            return records.OrderBy(record => record.Timestamp).ToList();
        }

        private static double? ParseNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string normalized = value
                .Trim()
                .Replace("%", "")
                .Replace(",", ".");

            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            return null;
        }
    }
}
