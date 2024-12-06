using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace MatPhysTasks;

public class Task_2_2
{
    public static void Solve()
    {
        // Параметри задачі
        double L = 1.0; // Довжина інтервалу
        double T = 1.0; // Часовий інтервал
        int Nx = 50; // Кількість вузлів по x
        int Nt = 100; // Кількість кроків по часу
        double dx = L / Nx;
        double dt = T / Nt;

        // Сітка по x
        double[] x = new double[Nx + 1];
        for (int i = 0; i <= Nx; i++)
        {
            x[i] = i * dx;
        }

        // Початкова умова
        double[,] u = new double[Nt + 1, Nx + 1];
        for (int i = 0; i <= Nx; i++)
        {
            u[0, i] = x[i] * (1 - x[i]);
        }

        double alpha = dt / (2 * dx * dx);
        double[,] A = new double[Nx - 1, Nx - 1];
        double[,] B = new double[Nx - 1, Nx - 1];

        for (int i = 0; i < Nx - 1; i++)
        {
            A[i, i] = 1 + 2 * alpha;
            B[i, i] = 1 - 2 * alpha;
            if (i > 0)
            {
                A[i, i - 1] = -alpha;
                B[i, i - 1] = alpha;
            }

            if (i < Nx - 2)
            {
                A[i, i + 1] = -alpha;
                B[i, i + 1] = alpha;
            }
        }

        // Доданок x
        double[] f = new double[Nx - 1];
        for (int i = 0; i < Nx - 1; i++)
        {
            f[i] = x[i + 1] * dt;
        }

        // Розв'язок на кожному часовому кроці
        for (int n = 0; n < Nt; n++)
        {
            // Вектор b
            double[] b = new double[Nx - 1];
            for (int i = 0; i < Nx - 1; i++)
            {
                b[i] = f[i];
                for (int j = 0; j < Nx - 1; j++)
                {
                    b[i] += B[i, j] * u[n, j + 1];
                }
            }

            // Розв'язання системи A * u_next = b
            double[] uNext = SolveLinearSystem(A, b);

            // Оновлення значень u
            for (int i = 0; i < Nx - 1; i++)
            {
                u[n + 1, i + 1] = uNext[i];
            }
        }

        // Вивід результатів для заданих моментів часу
        double[] timesToPlot = { 0.3, 0.5, 1.0 };
        var result = new List<Dictionary<double, double>>();
        foreach (var time in timesToPlot)
        {
            var dict = new Dictionary<double, double>();
            int index = (int)(time / dt);
            Console.WriteLine($"t = {time}");
            for (int i = 0; i <= Nx; i++)
            {
                dict.Add(x[i], u[index, i]);
                Console.WriteLine($"x = {x[i]}, u = {u[index, i]}");
            }

            result.Add(dict);
            Console.WriteLine();
        }

        SaveToFile("results.csv", result);

        Console.ReadKey();
    }

    // Метод для розв'язання системи лінійних рівнянь A * x = b
    static double[] SolveLinearSystem(double[,] A, double[] b)
    {
        int n = b.Length;
        double[] x = new double[n];
        double[] tempB = (double[])b.Clone();
        double[,] tempA = (double[,])A.Clone();

        // Прямий хід методу Гаусса
        for (int k = 0; k < n; k++)
        {
            for (int i = k + 1; i < n; i++)
            {
                double factor = tempA[i, k] / tempA[k, k];
                for (int j = k; j < n; j++)
                {
                    tempA[i, j] -= factor * tempA[k, j];
                }
                tempB[i] -= factor * tempB[k];
            }
        }

        // Зворотний хід
        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = tempB[i];
            for (int j = i + 1; j < n; j++)
            {
                x[i] -= tempA[i, j] * x[j];
            }
            x[i] /= tempA[i, i];
        }

        return x;
    }

    static void SaveToFile(string fileName, List<Dictionary<double, double>> points)
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