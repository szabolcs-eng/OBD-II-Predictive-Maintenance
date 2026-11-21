using OBDPredictiveMaintenance.AnomalyDetection;
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
            var detector = new MadDetector();

            var originalRecords = reader.ReadVehicleData(filePath, "car11");

            var processedRecords = preprocessor.Process(originalRecords);

            var syntheticRecords = generator.Generate(processedRecords, 100);

            var detectionInput = syntheticRecords
                .Select(record => record.Data)
                .ToList();

            var detectionResults = detector.Detect(detectionInput);

            int detectedCount = detectionResults.Count(result => result.IsAnomaly);

            Console.WriteLine($"Vizsgált rekordok: {detectionResults.Count}");
            Console.WriteLine($"Detektált anomáliák: {detectedCount}");

            Console.WriteLine();
            Console.WriteLine("Néhány detektált anomália:");

            foreach (var result in detectionResults
                .Where(result => result.IsAnomaly)
                .Take(10))
            {
                Console.WriteLine(
                    $"{result.Timestamp} | " +
                    $"Score: {result.Score:F2} | " +
                    $"Változók: {string.Join(", ", result.DetectedVariables)}");
            }

            Console.ReadKey();
        }
    }
}
