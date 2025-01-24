using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphProj;

/// <summary>
/// Interaction logic for _2_2.xaml
/// </summary>
public partial class _2_2 : UserControl
{
    public _2_2()
    {
        InitializeComponent();
    }

    public void Solve()
    {
        if (StepSlider == null || ToleranceSlider == null)
        {
            return;
        }

        WpfPlot1.Plot.Clear();

        // Параметри задачі
        double L = 1.0; // Довжина інтервалу
        double T = 1.0; // Часовий інтервал
        int Nx = (int)ToleranceSlider.Value; // Кількість вузлів по x
        int Nt = (int)StepSlider.Value; // Кількість кроків по часу
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
        foreach (var time in timesToPlot)
        {
            var dict = new Dictionary<double, double>();
            int index = (int)(time / dt);
            for (int i = 0; i <= Nx; i++)
            {
                dict.Add(x[i], u[index, i]);
            }

            WpfPlot1.Plot.Add.Scatter(dict.Keys.ToArray(), dict.Values.ToArray());
        }

        WpfPlot1.Refresh();
    }

    // Метод для розв'язання системи лінійних рівнянь A * x = b
    double[] SolveLinearSystem(double[,] A, double[] b)
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

    private void ToleranceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ToleranceSliderLabel.Content = ToleranceSlider.Value;
        Solve();
    }

    private void StepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        StepSliderLabel.Content = StepSlider.Value;
        Solve();
    }
}
