---
title: "Практическая работа 3 — Численное интегрирование методом Гаусса-3"
author: "Лушников Николай"
geometry: margin=2.5cm
fontsize: 12pt
mainfont: "Times New Roman"
monofont: "Consolas"
lang: ru
header-includes:
  - \usepackage{amsmath}
  - \usepackage{amssymb}
---

# 1. Постановка задачи

**Вариант 9.** Требуется вычислить определённый интеграл:

$$\int_{1}^{8} \frac{5\sqrt{x+24}}{(x+24)^2 \cdot \sqrt{x}} \, dx$$

Метод: **квадратура Гаусса-Лежандра с 3 узлами** (Гаусс-3).

Реализация: десктопное приложение на C# (Windows Forms, .NET 8.0) с прогресс-баром и многопоточностью.

---

# 2. Теоретическая часть

## 2.1 Идея численного интегрирования

Если аналитически (через первообразную) взять интеграл сложно, его можно **приближённо** вычислить, заменив интеграл взвешенной суммой значений функции в специально подобранных точках:

$$\int_{a}^{b} f(x)\,dx \;\approx\; \sum_{i=1}^{n} w_i \cdot f(x_i)$$

где $x_i$ — **узлы** (точки, в которых вычисляем функцию), $w_i$ — **веса** (коэффициенты важности каждого узла).

## 2.2 Квадратура Гаусса-Лежандра

Формулы Гаусса — самые точные из всех формул с фиксированным числом узлов. Формула Гаусса с $n$ узлами **точна** для полиномов степени до $2n - 1$.

Базовая формула определена на **стандартном отрезке** $[-1,\; 1]$:

$$\int_{-1}^{1} f(t)\,dt \;\approx\; \sum_{i=1}^{n} w_i \cdot f(t_i)$$

Узлы $t_i$ — это корни полинома Лежандра $P_n(t)$, а веса вычисляются по формуле:

$$w_i = \frac{2}{\bigl(1 - t_i^2\bigr) \cdot \bigl[P_n'(t_i)\bigr]^2}$$

## 2.3 Гаусс-3: конкретные узлы и веса

Для $n = 3$ полином Лежандра имеет вид $P_3(t) = \tfrac{1}{2}(5t^3 - 3t)$. Его корни и соответствующие веса:

| Узел $t_i$ | Значение | Вес $w_i$ | Значение |
|:---:|:---:|:---:|:---:|
| $t_1$ | $-\sqrt{3/5} \approx -0{,}7746$ | $w_1$ | $5/9 \approx 0{,}5556$ |
| $t_2$ | $0$ | $w_2$ | $8/9 \approx 0{,}8889$ |
| $t_3$ | $+\sqrt{3/5} \approx +0{,}7746$ | $w_3$ | $5/9 \approx 0{,}5556$ |

Формула точна для полиномов степени $\leq 5$, поскольку $2 \cdot 3 - 1 = 5$.

## 2.4 Замена переменных: от $[-1,\;1]$ к $[a,\;b]$

Формула Гаусса работает на $[-1,\;1]$. Чтобы вычислить интеграл на произвольном $[a,\;b]$, выполняем линейную замену:

$$x = \frac{a+b}{2} + \frac{b-a}{2} \cdot t, \qquad dx = \frac{b-a}{2}\,dt$$

Подставляя, получаем **итоговую формулу Гаусса-3 на отрезке** $[a,\;b]$:

$$\boxed{\;\int_{a}^{b} f(x)\,dx \;\approx\; \frac{b-a}{2} \sum_{i=1}^{3} w_i \cdot f\!\left(\frac{a+b}{2} + \frac{b-a}{2} \cdot t_i\right)\;}$$

Обозначения, используемые в коде:

- $\dfrac{a+b}{2}$ — **середина отрезка** (переменная `midpoint`)
- $\dfrac{b-a}{2}$ — **полуширина отрезка**, она же **якобиан** (переменная `detJ`)

## 2.5 Составная формула (разбиение на сегменты)

Один сегмент $[a,\;b]$ даёт приемлемую точность только для гладких функций на коротких отрезках. Для повышения точности **разбиваем** $[a,\;b]$ на $N$ равных частей шириной $h$:

$$h = \frac{b - a}{N}, \qquad [a_k,\; b_k] = [a + kh,\;\; a + (k+1)h], \quad k = 0, 1, \ldots, N-1$$

На каждом маленьком сегменте применяем Гаусс-3 и складываем результаты:

$$\int_{a}^{b} f(x)\,dx \;=\; \sum_{k=0}^{N-1} \int_{a_k}^{b_k} f(x)\,dx \;\approx\; \sum_{k=0}^{N-1} \frac{h}{2} \sum_{i=1}^{3} w_i \cdot f\!\left(\frac{a_k + b_k}{2} + \frac{h}{2} \cdot t_i\right)$$

Больше сегментов $N$ — выше точность. В программе по умолчанию $N = 1000$.

## 2.6 Погрешность

Для составной формулы Гаусса-3 с $N$ сегментами порядок погрешности:

$$R \sim O(h^6) = O\!\left(\frac{1}{N^6}\right)$$

Сравнение с другими методами:

| Метод | Порядок погрешности |
|:---|:---:|
| Прямоугольников | $O(h^2)$ |
| Трапеций | $O(h^2)$ |
| Симпсона | $O(h^4)$ |
| **Гаусс-3** | $O(h^6)$ |

---

# 3. Структура проекта

```
Practica3/
  Practica3.sln                  -- файл решения Visual Studio
  Practica3/
    Program.cs                   -- точка входа (запуск формы)
    GaussIntegralCalculator.cs   -- математическое ядро (Гаусс-3)
    Form1.cs                     -- логика UI + многопоточность
    Form1.Designer.cs            -- авто-генерированная разметка формы
    Practica3.csproj             -- конфигурация проекта (.NET 8.0, WinForms)
```

---

# 4. Разбор кода

## 4.1 GaussIntegralCalculator.cs — математическое ядро

Это единственный файл, содержащий всю математику.

### Узлы и веса (строки 13–16)

```csharp
private static readonly double[] GaussPoints =
    { -Math.Sqrt(3.0 / 5.0), 0.0, Math.Sqrt(3.0 / 5.0) };

private static readonly double[] GaussWeights =
    { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };
```

Массивы $t_i$ и $w_i$ из таблицы в разделе 2.3. Модификатор `static readonly` означает, что значения вычисляются один раз при загрузке класса.

### Вычисление интеграла на одном сегменте (строки 48–66)

```csharp
private double CalculateSegmentIntegral(Func<double, double> f,
                                        double a, double b)
{
    double sum = 0.0;
    double detJ = (b - a) / 2.0;       // якобиан
    double midpoint = (a + b) / 2.0;    // середина отрезка

    for (int i = 0; i < GaussPoints.Length; i++)
    {
        double x = midpoint + detJ * GaussPoints[i]; // t_i -> x_i
        sum += GaussWeights[i] * f(x);               // w_i * f(x_i)
    }

    return detJ * sum;  // умножаем на якобиан
}
```

Это прямая реализация формулы из раздела 2.4:

$$\frac{b-a}{2} \sum_{i=1}^{3} w_i \cdot f\!\left(\frac{a+b}{2} + \frac{b-a}{2} \cdot t_i\right)$$

### Составная формула — цикл по сегментам (строки 22–45)

```csharp
public double CalculateIntegral(Func<double, double> f,
                                double a, double b, int segments)
{
    double segmentWidth = (b - a) / segments;  // h
    double sum = 0.0;

    for (int i = 0; i < segments; i++)
    {
        double segmentA = a + i * segmentWidth;
        double segmentB = segmentA + segmentWidth;
        sum += CalculateSegmentIntegral(f, segmentA, segmentB);
        IterationSolved?.Invoke(this,
            new IterationSolvedEventArgs(i, sum));
    }
    return sum;
}
```

Цикл по $k = 0, 1, \ldots, N-1$: на каждом шаге вычисляется интеграл на маленьком сегменте и накапливается общая сумма.

### Событие IterationSolved (строки 4, 7, 19)

```csharp
public record IterationSolvedEventArgs(int Iteration, double Sum);
public delegate void IterationSolved(object sender,
                                     IterationSolvedEventArgs args);
public event IterationSolved? IterationSolved;
```

После обработки каждого сегмента калькулятор генерирует событие с номером итерации и текущей накопленной суммой. Это позволяет обновлять прогресс-бар в UI.

---

## 4.2 Form1.cs — интерфейс и многопоточность

### Зачем нужен отдельный поток?

Если выполнять 1000 итераций в UI-потоке, окно «зависнет» — не будет перерисовываться. Поэтому вычисления запускаются в **фоновом потоке** (`Thread`).

### Запуск вычислений (строки 22–54)

```csharp
private void btnCalculate_Click(object sender, EventArgs e)
{
    if (_calculationRunning) { /* уже считается — выходим */ }

    double a = (double)numLowerLimit.Value;
    double b = (double)numUpperLimit.Value;
    int segments = (int)numSegments.Value;

    if (b <= a) { /* ошибка: верхний предел <= нижнего */ }

    _calculationRunning = true;
    btnCalculate.Enabled = false;
    progressBar.Value = 0;

    _calculationThread = new Thread(
        () => CalculateIntegral(a, b, segments));
    _calculationThread.Start();
}
```

### Подставляемая функция — Вариант 9 (строки 68–72)

```csharp
var result = calculator.CalculateIntegral(
    x => 5 * Math.Sqrt(x + 24)
       / (Math.Pow(x + 24, 2) * Math.Sqrt(x)),
    a, b, segments);
```

Лямбда-выражение `x => ...` — это подынтегральная функция:

$$f(x) = \frac{5\sqrt{x+24}}{(x+24)^2 \cdot \sqrt{x}}$$

Передаётся как `Func<double, double>` — делегат, принимающий и возвращающий `double`.

### Безопасное обновление UI из фонового потока (строки 83–106)

```csharp
private void UpdateProgress(int progress, double currentValue)
{
    if (InvokeRequired)
    {
        Invoke(new Action(
            () => UpdateProgress(progress, currentValue)));
        return;
    }
    progressBar.Value = progress;
    lblResult.Text = $"Текущее значение: {currentValue:F16}";
}
```

В Windows Forms нельзя обращаться к элементам управления из чужого потока. `InvokeRequired` проверяет, находимся ли мы в UI-потоке. Если нет — `Invoke(...)` перебрасывает вызов в UI-поток.

---

## 4.3 Form1.Designer.cs — визуальная разметка

Файл авто-генерируется Visual Studio. Определяет следующие элементы:

| Элемент | Тип | Назначение |
|:---|:---|:---|
| `numLowerLimit` | NumericUpDown | Нижний предел (по умолчанию 1) |
| `numUpperLimit` | NumericUpDown | Верхний предел (по умолчанию 8) |
| `numSegments` | NumericUpDown | Число сегментов (по умолчанию 1000) |
| `btnCalculate` | Button | Кнопка «Вычислить» |
| `progressBar` | ProgressBar | Индикатор прогресса |
| `lblResult` | Label | Отображение результата |

Форма имеет фиксированный размер 284$\times$211, размещается по центру экрана, заголовок — «Калькулятор интегралов».

---

# 5. Как всё работает вместе

```
Пользователь нажимает "Вычислить"
         |
         v
btnCalculate_Click():
  - Читает a, b, N из полей ввода
  - Блокирует кнопку
  - Создаёт Thread -> CalculateIntegral(a, b, N)
         |
         v
CalculateIntegral() [фоновый поток]:
  - Создаёт GaussIntegralCalculator
  - Подписывается на событие IterationSolved
  - Вызывает calculator.CalculateIntegral(f, a, b, N)
         |
         v
GaussIntegralCalculator.CalculateIntegral():
  for k = 0 .. N-1:
    - Границы сегмента: [a + k*h, a + (k+1)*h]
    - CalculateSegmentIntegral():
        - Замена переменных [-1,1] -> [ak, bk]
        - Вычисление f в 3 точках Гаусса
        - Взвешенная сумма * якобиан
    - sum += результат сегмента
    - Событие -> UpdateProgress()
         |                    |
         v                    v
  return sum         progressBar обновляется
         |           lblResult показывает
         v           промежуточную сумму
UpdateResult():
  lblResult = "Результат: ..."
  Разблокировать кнопку
```

## Пример ручного вычисления (1 сегмент, $[1,\;8]$)

Вычислим параметры замены переменных:

$$\text{midpoint} = \frac{1+8}{2} = 4{,}5, \qquad \text{detJ} = \frac{8-1}{2} = 3{,}5$$

Три точки Гаусса, отображённые на отрезок $[1,\;8]$:

$$x_1 = 4{,}5 + 3{,}5 \cdot (-0{,}7746) \approx 1{,}789$$

$$x_2 = 4{,}5 + 3{,}5 \cdot 0 = 4{,}5$$

$$x_3 = 4{,}5 + 3{,}5 \cdot (+0{,}7746) \approx 7{,}211$$

Приближённое значение интеграла:

$$\int_{1}^{8} f(x)\,dx \;\approx\; 3{,}5 \cdot \left[\frac{5}{9} \cdot f(1{,}789) + \frac{8}{9} \cdot f(4{,}5) + \frac{5}{9} \cdot f(7{,}211)\right]$$

С 1000 сегментами точность значительно выше — погрешность порядка $10^{-16}$.

---

# Как сконвертировать в PDF

Установить [Pandoc](https://pandoc.org/) и LaTeX-движок (MiKTeX или TeX Live), затем выполнить:

```bash
pandoc DOCUMENTATION.md -o documentation.pdf --pdf-engine=xelatex
```
