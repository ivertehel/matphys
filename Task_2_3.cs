﻿using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace MatPhysTasks;

public class Task_2_3
{
    public static void Solve()
    {
        int N = 100;
        double[] x = Enumerable.Range(0, N + 1).Select(i => i / (double)N).ToArray();
        double[] u = new double[N + 1]; // Розв'язок (значення в вузлах)

        double[,] K = new double[N + 1, N + 1]; // Матриця жорсткості
        double[] F = new double[N + 1]; // Вектор правої частини

        for (int i = 0; i < N; i++)
        {
            double h = x[i + 1] - x[i];
            double mid = (x[i] + x[i + 1]) / 2;

            // Локальний вклад в жорсткість
            double a = 4 * Math.Sin(Math.PI * mid) + 1;
            double c = Math.Cos(Math.PI * mid);

            K[i, i] += a / h + c * h / 3;
            K[i, i + 1] -= a / h - c * h / 6;
            K[i + 1, i] -= a / h - c * h / 6;
            K[i + 1, i + 1] += a / h + c * h / 3;

            // Локальний вклад в праву частину
            double f = -4 * Math.Cos(2 * Math.PI * mid);
            F[i] += f * h / 2;
            F[i + 1] += f * h / 2;
        }

        // Умови на межі
        K[0, 0] += 1e10; // Симулюємо u'(0) = 0
        F[0] = 0;

        K[N, N] = 1e10;  // Симулюємо u(1) = 1
        F[N] = 1e10;

        u = SolveLinearSystem(K, F);

        var result = new Dictionary<double, double>();
        for (int i = 0; i <= N; i++)
        {
            Console.WriteLine($"x = {x[i]}, u = {u[i]}");
            result.Add(x[i], u[i]);
        }

        SaveToFile("task_2_3.csv", result);
    }

    static double[] SolveLinearSystem(double[,] A, double[] b)
    {
        int n = b.Length;
        double[] x = new double[n];
        for (int k = 0; k < n; k++)
        {
            for (int i = k + 1; i < n; i++)
            {
                double factor = A[i, k] / A[k, k];
                for (int j = k; j < n; j++)
                {
                    A[i, j] -= factor * A[k, j];
                }

                b[i] -= factor * b[k];
            }
        }

        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = b[i] / A[i, i];
            for (int j = i - 1; j >= 0; j--)
            {
                b[j] -= A[j, i] * x[i];
            }
        }

        return x;
    }

    static void SaveToFile(string fileName, Dictionary<double, double> points)
    {
        var configPersons = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";"
        };

        using (var streamWriter = new StreamWriter(fileName))
        using (var csvWriter = new CsvWriter(streamWriter, configPersons))
        {
            csvWriter.WriteRecords(points.Select(s => new Row
            { 
                f_1_x = s.Key,
                f_1_y1 = s.Value
            }));
        }
    }
}