using System.Threading;

namespace Practica3
{
    public partial class Form1 : Form
    {
        // Поток для вычислений
        private Thread? _calculationThread;

        // Флаг — идёт ли сейчас расчёт
        private bool _calculationRunning;

        // Объект для синхронизации потоков
        private readonly object _lockObject = new();

        public Form1()
        {
            InitializeComponent();
        }

        // Обработчик нажатия кнопки "Вычислить"
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            // Не даём запустить расчёт повторно
            if (_calculationRunning)
            {
                MessageBox.Show("Вычисление уже выполняется", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Считываем параметры из полей ввода
            double a = (double)numLowerLimit.Value;
            double b = (double)numUpperLimit.Value;
            int segments = (int)numSegments.Value;

            // Проверяем, что верхний предел больше нижнего
            if (b <= a)
            {
                MessageBox.Show("Верхний предел должен быть больше нижнего", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Блокируем кнопку и сбрасываем прогресс
            _calculationRunning = true;
            btnCalculate.Enabled = false;
            progressBar.Value = 0;
            lblResult.Text = "Вычисление...";

            // Запускаем вычисление в отдельном потоке
            _calculationThread = new Thread(() => CalculateIntegral(a, b, segments));
            _calculationThread.Start();
        }

        // Метод, который выполняется в фоновом потоке
        private void CalculateIntegral(double a, double b, int segments)
        {
            var calculator = new GaussIntegralCalculator();

            // Подписываемся на событие — обновляем прогресс после каждого сегмента
            calculator.IterationSolved += (s, args) =>
            {
                int progress = (args.Iteration + 1) * 100 / segments;
                UpdateProgress(progress, args.Sum);
            };

            // Вариант 9: f(x) = 5*sqrt(x+24) / ((x+24)^2 * sqrt(x))
            // Пределы интегрирования: от 1 до 8
            var result = calculator.CalculateIntegral(
                x => 5 * Math.Sqrt(x + 24) / (Math.Pow(x + 24, 2) * Math.Sqrt(x)),
                a, b, segments);

            // Показываем итоговый результат
            if (_calculationRunning)
            {
                UpdateResult(result);
                _calculationRunning = false;
            }
        }

        // Обновление прогресс-бара из фонового потока (безопасно через Invoke)
        private void UpdateProgress(int progress, double currentValue)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(progress, currentValue)));
                return;
            }

            progressBar.Value = progress;
            lblResult.Text = $"Текущее значение: {currentValue:F16}";
        }

        // Вывод итогового результата (безопасно через Invoke)
        private void UpdateResult(double result)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateResult(result)));
                return;
            }

            lblResult.Text = $"Результат: {result:F16}";
            btnCalculate.Enabled = true;
        }

        // При закрытии формы — дожидаемся завершения потока
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _calculationRunning = false;
            _calculationThread?.Join();
        }
    }
}
