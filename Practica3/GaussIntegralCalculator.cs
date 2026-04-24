namespace Practica3;

// Аргументы события — номер итерации и текущая сумма
public record IterationSolvedEventArgs(int Iteration, double Sum);

// Делегат для события завершения итерации
public delegate void IterationSolved(object sender, IterationSolvedEventArgs args);

// Класс для вычисления интеграла методом Гаусса-3
public class GaussIntegralCalculator
{
    // Точки Гаусса для метода Гаусса-3 (3 точки)
    private static readonly double[] GaussPoints = { -Math.Sqrt(3.0 / 5.0), 0.0, Math.Sqrt(3.0 / 5.0) };

    // Веса для метода Гаусса-3
    private static readonly double[] GaussWeights = { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };

    // Событие, которое вызывается после вычисления каждого сегмента
    public event IterationSolved? IterationSolved;

    // Основной метод — считает интеграл функции f на отрезке [a, b] с разбиением на segments частей
    public double CalculateIntegral(Func<double, double> f, double a, double b, int segments)
    {
        if (segments <= 0)
            throw new ArgumentException("Количество сегментов должно быть положительным");

        // Ширина одного сегмента
        double segmentWidth = (b - a) / segments;
        double sum = 0.0;

        for (int i = 0; i < segments; i++)
        {
            // Границы текущего сегмента
            double segmentA = a + i * segmentWidth;
            double segmentB = segmentA + segmentWidth;

            // Прибавляем интеграл по текущему сегменту
            sum += CalculateSegmentIntegral(f, segmentA, segmentB);

            // Сообщаем о прогрессе
            IterationSolved?.Invoke(this, new IterationSolvedEventArgs(i, sum));
        }

        return sum;
    }

    // Вычисляет интеграл на одном сегменте [a, b] по формуле Гаусса-3
    private double CalculateSegmentIntegral(Func<double, double> f, double a, double b)
    {
        double sum = 0.0;

        // Якобиан перехода от [-1, 1] к [a, b]
        double detJ = (b - a) / 2.0;
        double midpoint = (a + b) / 2.0;

        // Суммируем по всем точкам Гаусса
        for (int i = 0; i < GaussPoints.Length; i++)
        {
            // Переводим точку Гаусса из [-1, 1] в [a, b]
            double x = midpoint + detJ * GaussPoints[i];
            sum += GaussWeights[i] * f(x);
        }

        // Умножаем на якобиан
        return detJ * sum;
    }
}
