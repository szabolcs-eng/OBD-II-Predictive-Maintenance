using OBDPredictiveMaintenance.Data;
using OBDPredictiveMaintenance.Testing;

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
            var generator = new SyntheticAnomalyGenerator();

            // 1. Adatok beolvasása
            var originalRecords = reader.ReadVehicleData(filePath, "car11");

            // 2. Előfeldolgozás
            var processedRecords = preprocessor.Process(originalRecords);

            // 3. Szintetikus anomáliák előállítása
            var syntheticRecords = generator.Generate(processedRecords, 100);

            // 4. Eredmények
            int anomalyCount = syntheticRecords.Count(record => record.IsAnomaly);

            int normalCount = syntheticRecords.Count(record => !record.IsAnomaly);

            Console.WriteLine($"Eredeti rekordok: {originalRecords.Count}");
            Console.WriteLine($"Feldolgozott rekordok: {processedRecords.Count}");

            Console.WriteLine();
            Console.WriteLine("Szintetikus adatok:");
            Console.WriteLine($"Összes rekord: {syntheticRecords.Count}");
            Console.WriteLine($"Normál rekordok: {normalCount}");
            Console.WriteLine($"Anomáliák: {anomalyCount}");

            Console.WriteLine();
            Console.WriteLine("Néhány mesterséges anomália:");

            foreach (var record in syntheticRecords
                .Where(record => record.IsAnomaly)
                .Take(10))
            {
                Console.WriteLine(
                    $"{record.Data.Timestamp} | " +
                    $"{record.AnomalyType} | " +
                    $"RPM: {record.Data.EngineRpm} | " +
                    $"Speed: {record.Data.Speed} | " +
                    $"Coolant: {record.Data.EngineCoolantTemp} | " +
                    $"Load: {record.Data.EngineLoad} | " +
                    $"MAF: {record.Data.Maf}");
            }

            Console.ReadKey();
        }
    }
}
