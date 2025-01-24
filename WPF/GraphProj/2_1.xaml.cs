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
/// Interaction logic for _2_1.xaml
/// </summary>
public partial class _2_1 : UserControl
{
    public _2_1()
    {
        InitializeComponent();
    }

    public void Solve()
    {
        if (StepSlider == null || ToleranceSlider == null)
        {
            return;
        }

        ToleranceSliderLabel.Content = ToleranceSlider.Value;
        StepSliderLabel.Content = StepSlider.Value;

        double tolerance = 10^((int)ToleranceSlider.Value); // Точність для пошуку lambda
        double h = StepSlider.Value; // Крок інтегрування
        int maxIterations = 100;
        WpfPlot1.Plot.Clear();
        List<Dictionary<double, double>> results = new List<Dictionary<double, double>>();
        for (int i = 1; i <= 3; i++)
        {
            double lambda = FindEigenvalue(h, tolerance, maxIterations, i);

            // Обчислення власної функції
            var eigenFunction = SolveEigenFunction(h, lambda, i);

            WpfPlot1.Plot.Add.Scatter(eigenFunction.Keys.ToArray(), eigenFunction.Values.ToArray());
            WpfPlot1.Refresh();
        }
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