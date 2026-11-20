using OBDPredictiveMaintenance.Data;

namespace OBDPredictiveMaintenance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("OBD-II Predictive Maintenance");
            Console.WriteLine();

            string filePath = @"D:\Egyetem\Egyetem\5. Félév\Projektmunka1\Project\data\exp1_14drivers_14cars_dailyRoutes.csv";

            var reader = new CsvDataReader();
            var preprocessor = new DataPreprocessor();

            // Eredeti adatok beolvasása
            var originalRecords = reader.ReadVehicleData(filePath, "car11");

            // Adatok előfeldolgozása
            var processedRecords = preprocessor.Process(originalRecords);

            // Eredmények
            Console.WriteLine($"Eredeti rekordok: {originalRecords.Count}");
            Console.WriteLine($"Feldolgozott rekordok: {processedRecords.Count}");
            Console.WriteLine($"Kiszűrt rekordok: {originalRecords.Count - processedRecords.Count}");

            Console.WriteLine();
            Console.WriteLine("Első 5 feldolgozott rekord:");

            foreach (var record in processedRecords.Take(5))
            {
                Console.WriteLine(
                    $"{record.Timestamp} | " +
                    $"RPM: {record.EngineRpm} | " +
                    $"Speed: {record.Speed} | " +
                    $"Coolant: {record.EngineCoolantTemp} | " +
                    $"Load: {record.EngineLoad} | " +
                    $"Throttle: {record.ThrottlePos} | " +
                    $"MAF: {record.Maf}");
            }

            Console.ReadKey();
        }
    }
}
