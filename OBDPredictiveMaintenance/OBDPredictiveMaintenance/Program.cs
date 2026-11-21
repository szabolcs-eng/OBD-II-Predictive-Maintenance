using OBDPredictiveMaintenance.AnomalyDetection;
using OBDPredictiveMaintenance.Data;
using OBDPredictiveMaintenance.Evaluation;
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
            var evaluator = new ModelEvaluator();

            // 1. Adatok beolvasása
            var originalRecords = reader.ReadVehicleData(filePath, "car11");

            // 2. Előfeldolgozás
            var processedRecords = preprocessor.Process(originalRecords);

            // 3. Szintetikus anomáliák
            var syntheticRecords = generator.Generate(processedRecords, 100);

            // 4. Anomáliadetektálás
            var detectionInput = syntheticRecords
                .Select(record => record.Data)
                .ToList();

            var detectionResults = detector.Detect(detectionInput);

            // 5. Kiértékelés
            var evaluation = evaluator.Evaluate(syntheticRecords, detectionResults);

            // 6. Eredmények megjelenítése
            Console.WriteLine("MAD algoritmus kiértékelése");
            Console.WriteLine("---------------------------");

            Console.WriteLine($"TP (helyes találat): {evaluation.TruePositive}");
            Console.WriteLine($"FP (téves riasztás): {evaluation.FalsePositive}");
            Console.WriteLine($"FN (nem felismert anomália): {evaluation.FalseNegative}");
            Console.WriteLine($"TN (helyesen normális): {evaluation.TrueNegative}");

            Console.WriteLine();

            Console.WriteLine($"Precision: {evaluation.Precision:P2}");
            Console.WriteLine($"Recall: {evaluation.Recall:P2}");
            Console.WriteLine($"F1-score: {evaluation.F1Score:F4}");

            Console.ReadKey();
        }
    }
}
