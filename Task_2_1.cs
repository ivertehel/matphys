using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace MatPhysTasks;

public class Task_2_1
{
    public static void Solve()
    {
        double tolerance = 1e-6; // Точність для пошуку lambda
        double h = 0.01; // Крок інтегрування
        int maxIterations = 100;

        List<Dictionary<double, double>> results = new List<Dictionary<double, double>>();
        for (int i = 1; i <= 3; i++)
        {
            double lambda = FindEigenvalue(h, tolerance, maxIterations, i);
            Console.WriteLine($"lambda{i}: {lambda}");

            // Обчислення власної функції
            var eigenFunction = SolveEigenFunction(h, lambda, i);

            results.Add(eigenFunction);
        }

        SaveFunctionsToFile("eigenfunction.csv", results);

        Console.ReadKey();
    }

    // Пошук власного значення
    static double FindEigenvalue(double h, double tolerance, int maxIterations, int mode)
    {
        double lower = 0, upper = 20; // Початковий інтервал для lambda
        double mid = 0;

        for (int iter = 0; iter < maxIterations; iter++)
        {
            mid = (lower + upper) / 2.0;
            double yEnd = SolveBVP(h, mid, mode);

            if (Math.Abs(yEnd) < tolerance) 
            { 
                break;
            }

            // Визначення напрямку пошуку
            double test = SolveBVP(h, lower, mode);
            if (Math.Sign(yEnd) != Math.Sign(test))
            {
                upper = mid;
            }
            else
            { 
                lower = mid;
            }
        }

        return mid;
    }

    // Розв'язання крайової задачі
    static double SolveBVP(double h, double lambda, int mode)
    {
        double x = 0;
        double y1 = 0;
        double y2 = mode;
        double endX = 1;

        while (x < endX)
        {
            if (x + h > endX)
            {
                // Корекція кроку, щоб не перейти межу
                h = endX - x;
            }

            // Метод Рунге-Кутти 4-го порядку
            double k1_y1 = h * y2;
            double k1_y2 = h * ((5 + Math.Exp(x) - lambda) * y1);

            double k2_y1 = h * (y2 + 0.5 * k1_y2);
            double k2_y2 = h * ((5 + Math.Exp(x + 0.5 * h) - lambda) * (y1 + 0.5 * k1_y1));

            double k3_y1 = h * (y2 + 0.5 * k2_y2);
            double k3_y2 = h * ((5 + Math.Exp(x + 0.5 * h) - lambda) * (y1 + 0.5 * k2_y1));

            double k4_y1 = h * (y2 + k3_y2);
            double k4_y2 = h * ((5 + Math.Exp(x + h) - lambda) * (y1 + k3_y1));

            y1 += (k1_y1 + 2 * k2_y1 + 2 * k3_y1 + k4_y1) / 6.0;
            y2 += (k1_y2 + 2 * k2_y2 + 2 * k3_y2 + k4_y2) / 6.0;

            x += h;
        }

        return y1; // Повертаємо значення y1 в кінцевій точці
    }

    static Dictionary<double, double> SolveEigenFunction(double h, double lambda, int mode)
    {
        var points = new Dictionary<double, double>();

        double x = 0, y1 = 0, y2 = mode; // Початкові умови
        double endX = 1; // Кінець інтегрування

        while (x < endX)
        {
            if (x + h > endX) h = endX - x; // Корекція кроку

            points.Add(x, y1); // Додаємо точку в словник

            // Метод Рунге-Кутти 4-го порядку
            double k1_y1 = h * y2;
            double k1_y2 = h * ((5 + Math.Exp(x) - lambda) * y1);

            double k2_y1 = h * (y2 + 0.5 * k1_y2);
            double k2_y2 = h * ((5 + Math.Exp(x + 0.5 * h) - lambda) * (y1 + 0.5 * k1_y1));

            double k3_y1 = h * (y2 + 0.5 * k2_y2);
            double k3_y2 = h * ((5 + Math.Exp(x + 0.5 * h) - lambda) * (y1 + 0.5 * k2_y1));

            double k4_y1 = h * (y2 + k3_y2);
            double k4_y2 = h * ((5 + Math.Exp(x + h) - lambda) * (y1 + k3_y1));

            y1 += (k1_y1 + 2 * k2_y1 + 2 * k3_y1 + k4_y1) / 6.0;
            y2 += (k1_y2 + 2 * k2_y2 + 2 * k3_y2 + k4_y2) / 6.0;

            x += h;
        }

        return points;
    }

    static void SaveFunctionsToFile(string fileName, List<Dictionary<double, double>> points)
    {
        var configPersons = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        List<Row> table = new List<Row>();
        for (int i = 0; i < points[0].Count; i++)
        {
            table.Add(new Row
            {
                f_1_x = points[0].ElementAt(i).Key,
                f_1_y1 = points[0].ElementAt(i).Value,
                f_2_x = points[1].ElementAt(i).Key,
                f_2_y1 = points[1].ElementAt(i).Value,
                f_3_x = points[2].ElementAt(i).Key,
                f_3_y1 = points[2].ElementAt(i).Value
            });
        }

        using (var streamWriter = new StreamWriter(fileName))
        using (var csvWriter = new CsvWriter(streamWriter, configPersons))
        {
            csvWriter.WriteRecords(table);
        }
    }
}

public class Row 
{
    [Index(0)]
    public double f_1_x { get; set; }

    [Index(1)]
    public double f_1_y1 { get; set; }

    [Index(2)]
    public double f_2_x { get; set; }

    [Index(3)]
    public double f_2_y1 { get; set; }

    [Index(4)]
    public double f_3_x { get; set; }

    [Index(5)]
    public double f_3_y1 { get; set; }
}