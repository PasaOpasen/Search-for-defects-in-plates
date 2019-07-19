using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods;

/// <summary>
/// Библиотека математических классов, написанная Опасным Пасей (Дмитрией Пасько/Деметрием Паскалём).
/// Начал писать примерно в конце февраля 2018-го, с класса полиномов.
/// С конца января 2019-го пишу продолжение библиотеки (МатКлассы),
/// в текущую библиотеку иногда добавляю новый функционал.
/// Сильные стороны библиотеки: классы комплексный чисел, векторов, полиномов,
/// матриц, методов интегрирования, графов (особое внимание), СЛАУ, методы расширения
/// Недостатки: мало где заботился об исключениях, содержимое методов почти не комментрируется,
/// в классе СЛАУ из-за диплома, вышедшего с С++, есть слишком сложные низкоуровневые методы
/// и путаница из-за тесной связи с классом кривых, 
/// класс вероятностей начал из эксперимента и почти ничего не написал,
/// очень много открытых полей и методов,
/// почти не проводил тестирование,
/// но большинство методов использовались в визуальных приложениях
/// и так были отлажены
/// Всё написал сам, кроме 3-5% кода, взятого из открытых источников
/// 
/// ------------Контакты:
/// Telegram: 8 961 519 36 46 (на звонки не отвечаю)
/// Mail:     qtckpuhdsa@gmail.com
/// Discord:  Пася Опасен#3065
/// VK:       https://vk.com/roman_disease
/// Steam:    https://vk.com/away.php?to=https%3A%2F%2Fsteamcommunity.com%2Fid%2FPasaOpasen&cc_key=
///      Активно пользуюсь всеми указанными сервисами
/// </summary>
namespace МатКлассы
{
    #region Делегаты и перечисления
    /// <summary>
    /// Действительные функции действительного аргумента
    /// </summary>
    /// <param name="x">Аргумент - действительное число</param>
    /// <returns></returns>
    public delegate double RealFunc(double x);
    /// <summary>
    /// Комплексная функция комплексного аргумента
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate Complex ComplexFunc(Complex x);
    /// <summary>
    /// Действительные функции от точки
    /// </summary>
    /// <param name="x">Аргумент - пара действительных чисел (x,y), реализованная как точка Point</param>
    /// <returns></returns>
    public delegate double Functional(Point x);
    /// <summary>
    /// Действительная функция двух переменных
    /// </summary>
    /// <param name="u"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate double DRealFunc(double x, double u);
    /// <summary>
    /// Комплекснозначная функция двух действительных переменных
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public delegate Complex DComplexFunc(double x, double z);
    /// <summary>
    /// Комплексная функция двух переменных
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public delegate Complex DoubleComplexFunc(Complex a, Complex b);
    /// <summary>
    /// Вектор-функция от вектора и параметра
    /// </summary>
    /// <param name="x"></param>
    /// <param name="u"></param>
    /// <returns></returns>
    public delegate Vectors VRealFunc(double x, Vectors u);
    /// <summary>
    /// Вектор-функция
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate Vectors VectorFunc(double x);
    /// <summary>
    /// Функция из Rn в Rn
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public delegate Vectors VectorToVector(Vectors v);
    /// <summary>
    /// Функция двух векторов, выдающая вектор
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public delegate Vectors TwoVectorToVector(Vectors a, Vectors b);
    /// <summary>
    /// Действительная функция векторного аргумента
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public delegate double AntiVectorFunc(Vectors v);
    /// <summary>
    /// Действительная функция комплексного переменного
    /// </summary>
    /// <param name="z"></param>
    /// <returns></returns>
    public delegate double RealFuncOfCompArg(Complex z);
    /// <summary>
    /// Действительная функция трёх аргументов, необходимая для вычисления площади сегментов с параметрами tx, ty при радиусе кривой r
    /// </summary>
    /// <param name="tx"></param>
    /// <param name="ty"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public delegate double TripleFunc(double tx, double ty, double r);
    /// <summary>
    /// Функция многих аргументов
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate double MultiFunc(params double[] x);
    /// <summary>
    /// Действительная функция из какой-то системы функций
    /// </summary>
    /// <param name="x">Аргумент</param>
    /// <param name="k">Номер функции в системе</param>
    /// <returns></returns>
    public delegate double SequenceFunc(double x, int k);
    /// <summary>
    /// Действительная функция от точки из системы функций
    /// </summary>
    /// <param name="z"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    public delegate double SeqPointFunc(Point z, int k);
    /// <summary>
    /// Полином из системы полиномов
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public delegate Polynom SequencePol(int k);
    /// <summary>
    /// Функция, возвращающая точку в зависимости от параметра
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public delegate Point PointFunc(double t);
    /// <summary>
    /// Функция, возвращающая точку в зависимости от двух параметров
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public delegate Point DPointFunc(double t, double r);

    /// <summary>
    /// Перечисление "род": для криволинейных интегралов, полиномов Чебышёва и т.д.
    /// </summary>
    public enum Kind { FirstKind, SecondKind };
    /// <summary>
    /// Ортогональные функции, ортонормированные, неортогональные
    /// </summary>
    public enum SequenceFuncKind { Orthogonal, Orthonormal, Other };
    #endregion

    /// <summary>
    /// Класс для расширения всяких методов
    /// </summary>
    public static partial class Expendator
    {
        /// <summary>
        /// Перевести действительную функцию комплексного переменного в функционал
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Functional ToFunctional(RealFuncOfCompArg f)
        {
            return (Point z) =>
            {
                return f(new Complex(z.x, z.y));
            };
        }
        /// <summary>
        /// Перевести функционал в действительную функцию комплексного переменного
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static RealFuncOfCompArg ToRealFuncOfCompArg(Functional f)
        {
            return (Complex z) =>
            {
                return f(new Point(z.Re, z.Im));
            };
        }
        /// <summary>
        /// Перевести функционал в функцию комплесного переменного
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ComplexFunc ToCompFunc(Functional f)
        {
            return (Complex z) => f(new Point(z.Re, z.Im));
        }
        /// <summary>
        /// Преобразовать действительную функцию комплексного переменного в комплексную функцию
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static ComplexFunc ToCompFunc(RealFuncOfCompArg f) => (Complex z) => f(z);

        /// <summary>
        /// Минимальное из кучи
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double Min(params double[] c)
        {
            double min = Math.Min(c[0], c[1]);
            for (int i = 2; i < c.Length; i++) min = Math.Min(min, c[i]);
            return min;
        }

        /// <summary>
        /// Максимальное из кучи
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double Max(params double[] c)
        {
            double max = Math.Max(c[0], c[1]);
            for (int i = 2; i < c.Length; i++) max = Math.Max(max, c[i]);
            return max;
        }
        /// <summary>
        /// Максимальное из кучи
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int Max(params int[] c)
        {
            int max = Math.Max(c[0], c[1]);
            for (int i = 2; i < c.Length; i++) max = Math.Max(max, c[i]);
            return max;
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static double Min(MultiFunc F)
        {
            return -1;
        }

        public static double Max(MultiFunc F)
        {
            MultiFunc e = (double[] x) => -F(x);
            return -Min(e);
        }

        /// <summary>
        /// Вывести число на консоль
        /// </summary>
        /// <param name="i"></param>
        public static void Show<T>(this T i) => Console.WriteLine(i.ToString());

        public static void Show<T>(this T[] t)
        {
            for (int i = 0; i < t.Length; i++)
                Console.WriteLine(Convert.ToString(t[i]));
        }

        public static double Sum(SequenceFunc f, double x, int N)
        {
            double sum = 0;
            for (int i = 0; i < N; i++)
                sum += f(x, i);
            return sum;
        }

        public static double Abs(this double x) => Math.Abs(x);

        /// <summary>
        /// Более точное среднее арифметическое для двух чисел
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex Average(Complex a, Complex b)
        {
            if (Math.Sign(a.Re) == Math.Sign(b.Re) && Math.Sign(a.Im) == Math.Sign(b.Im))
                return a + (b - a) / 2;
            return (a + b) / 2;
        }
        /// <summary>
        /// Переводит строку в действительное число через конвертер
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ToDouble(this string s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch (Exception e)
            {
                s = s.Replace('.', ',');
                try
                {
                    return Convert.ToDouble(s);
                }
                catch
                {
                    throw e;
                }
            }
        }

        public static string Swap(this string s, char a, char b)
        {
            var mas = s.ToCharArray();
            int ai = s.IndexOf(a), bi = s.IndexOf(b);
            char tmp = mas[ai];
            mas[ai] = mas[bi];
            mas[bi] = tmp;
            return new string(mas);
        }

        public static double Reverse(this double val)
        {
            return 1.0 / val;
        }
        public static Complex Reverse(this Complex val)
        {
            double abs = val.Abs;
            if (Double.IsNaN(abs) || Double.IsInfinity(abs)) return 0;
            return 1.0 / val;
        }
        public static double Sqr(this double val) => val * val;
        public static Complex Sqr(this Complex val) => val * val;
        public static double Pow(this double v, double deg) => Math.Pow(v, deg);
        public static Complex Pow(this Complex v, double deg) => Complex.Pow(v, deg);

        /// <summary>
        /// Выдаёт массив действительных частей элементов комплексного массива
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] ToDoubleMas(this Complex[] c)
        {
            double[] res = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                res[i] = c[i].Re;
            return res;
        }
        /// <summary>
        /// Переводит произвольный массив в массив действительных чисел через конвертер
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] ToDoubleMas<T>(this T[] c)
        {
            double[] res = new double[c.Length];
            for (int i = 0; i < c.Length; i++)
                res[i] = Convert.ToDouble(c[i]);
            return res;
        }

        /// <summary>
        /// Переводит произвольный массив в массив строк через конвертер
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string[] ToStringMas<T>(this T[] c)
        {
            string[] res = new string[c.Length];
            for (int i = 0; i < c.Length; i++)
                res[i] = Convert.ToString(c[i]);
            return res;
        }
        /// <summary>
        /// Конкатенация двух массивов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T[] Union<T>(T[] a, T[] b)
        {
            T[] res = new T[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
                res[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                res[i + a.Length] = b[i];
            return res;
        }

        public static T[] Union<T>(params T[][] c)
        {
            //возможно, надо будет как-то преобразовать массив с
            int d2 = c.GetLength(1);
            int len = 0;
            for (int i = 0; i < d2; i++)
                len += c[i].Length;
            T[] res = new T[len];
            len = 0;
            for (int i = 0; i < d2; i++)
                for (int j = 0; j < c[i].Length; j++)
                    res[len++] = c[i][j];

            return res;
        }

        public class Compar : Comparer<double>
        {
            /// <summary>
            /// Компаратор по модулю
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public override int Compare(double x, double y)
            {
                return x.Abs().CompareTo(y.Abs());
            }

        }
        public class ComparPointTres<Tres> : Comparer<Tuple<Point, Tres>>
        {
            public override int Compare(Tuple<Point, Tres> x, Tuple<Point, Tres> y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
        }

        /// <summary>
        /// Размерность дробной части (количество знаков после запятой)
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int DimOfFractionalPath(this double d)
        {
            string s = d.ToString();
            if (s.Contains("E-"))
            {
                int i = s.IndexOf('E');
                int deg = (int)s.Substring(i + 2).ToDouble();
                if (s.Contains(',')) deg--;
                return i + deg - 1;
            }
            else
            {
                int i = s.IndexOf(',');
                if (i <= 0) return 0;
                int deg = s.Substring(i + 1).Length;
                return deg;
            }
        }

        public static decimal ToDecimal(this double i) => Convert.ToDecimal(i);
        public static float ToFloat(this double i) => Convert.ToSingle(i);
        public static double ToDouble(this int i) => (double)i;
        public static int ToInt(this double i) => (int)i;

        /// <summary>
        /// Среднее двух целых чисел (по правилам целочисленного деления)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Average(int a, int b) => (a + b) / 2;
        /// <summary>
        /// Сокращение числа на период
        /// </summary>
        /// <param name="d"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static double ToPeriod(this double d,double period)
        {
            //int k = (d < 0) ? 1 : 0;
            //int f = (int)Math.Floor(d.Abs() / period);
            //return d - Math.Sign(d) * period * (f + k);
            int f = (int)Math.Floor(d / period);
            return d - f * period;
            return d;
        }

        /// <summary>
        /// Перевод матрицы в массив System.Numerics.Complex
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static System.Numerics.Complex[,] ToSystemNumComplex(this CSqMatrix mat)
        {
            System.Numerics.Complex[,] mas = new System.Numerics.Complex[mat.ColCount, mat.ColCount];
            for (int i = 0; i < mat.ColCount; i++)
                for (int j = 0; j < mat.ColCount; j++)
                    mas[i, j] = new System.Numerics.Complex(mat[i, j].Re, mat[i, j].Im);
            return mas;
        }
        /// <summary>
        /// Перевод в матрицу массива System.Numerics.Complex
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static CSqMatrix ToCSqMatrix(this System.Numerics.Complex[,] mas)
        {
            int k = mas.GetLength(0);
            Complex[,] res = new Complex[k, k];
            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++)
                    res[i, j] = new Complex(mas[i, j].Real, mas[i, j].Imaginary);
            return new CSqMatrix(res);
        }

        /// <summary>
        /// Перевод числа в строку с обрезанием дробной части (оставить только n знаков после запятой)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ToString(this double d,int n)
        {
            n++;
            string s = d.ToString();
            int p = s.IndexOf(',');
            int e = s.IndexOf('E');
            if (e <= 0) e = s.Length;
            if (p > 0)
            {
                n = Math.Min(n, s.Length - p);
                return s.Substring(0, p + n) + s.Substring(e, s.Length - e);
            }
            else return s;

        }
    }

    /// <summary>
    /// Критерии принятия решений в условиях неопределённости
    /// </summary>
    public static class UnderUncertainty
    {
        private static void MAX(Vectors v, out int k)
        {
            double m = v[0];
            k = 0;
            for (int i = 1; i < v.n; i++)
                if (v[i] > m) { m = v[i]; k = i; }
        }
        private static void MIN(Vectors v, out int k)
        {
            double m = v[0];
            k = 0;
            for (int i = 1; i < v.n; i++)
                if (v[i] < m) { m = v[i]; k = i; }
        }

        /// <summary>
        /// Критерий среднего выйгрыша
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="q">Вектор вероятностей</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void AverageGain(Matrix S, out Vectors v, Vectors q = null)
        {
            v = new Vectors(S.n);
            if (q == null)
            {
                q = new Vectors(S.m);
                double w = 1.0 / q.n;
                for (int i = 0; i < q.n; i++) q[i] = w;
            }
            for (int i = 0; i < S.n; i++)
            {
                for (int j = 0; j < S.m; j++) v[i] += S[i, j] * q[j];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию среднего значения с вектором вероятностей " + q.ToString() + " оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий минимакса
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void MiniMax(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp; MAX(c, out tmp);
                v[i] = S[i, tmp];
            }

            int s; MIN(v, out s);
            Console.WriteLine("По критерию минимакса оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий максимакса
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void MaxiMax(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp; MAX(c, out tmp);
                v[i] = S[i, tmp];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию максимакса оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий Лапласа
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Laplas(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            Vectors q = new Vectors(S.n);
            double w = 1.0 / S.m;
            for (int i = 0; i < q.n; i++) q[i] = w;

            for (int i = 0; i < S.n; i++)
            {
                for (int j = 0; j < S.m; j++) v[i] += S[i, j];
                v[i] *= q[i];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Лапласа оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий Вальда (максимин)
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Vald(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp; MIN(c, out tmp);
                v[i] = S[i, tmp];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Вальда (максимина) оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий Сэвиджа
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Savage(Matrix S, out Vectors v)
        {
            Vectors v0 = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp; MAX(c, out tmp);
                v0[i] = S[i, tmp];
            }
            Matrix M = new Matrix(S.n, S.m);
            for (int i = 0; i < M.n; i++)
                for (int j = 0; j < M.m; j++)
                    M[i, j] = v0[i] - S[i, j];//M.PrintMatrix();

            v = new Vectors(M.n);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = M.GetLine(i);
                int tmp; MAX(c, out tmp);
                v[i] = M[i, tmp];
            }
            int s; MIN(v, out s);
            Console.WriteLine("По критерию Сэвиджа оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
        /// <summary>
        /// Критерий Гурвица
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        /// <param name="a">Коэффициент оптимизма</param>
        public static void Hurwitz(Matrix S, out Vectors v, double a = 0.5)
        {
            v = new Vectors(S.n);
            Vectors l = new Vectors(v), r = new Vectors(v);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp1, tmp2;
                MAX(c, out tmp1);
                MIN(c, out tmp2);
                l[i] = S[i, tmp1]; r[i] = S[i, tmp2];
                v[i] = a * l[i] + (1 - a) * r[i];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Гурвица с параметром {2} оптимальным является решение {0} (со значением {1}).", s + 1, v[s], a);
        }
        /// <summary>
        /// Критерий Ходжа-Лемана
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        /// <param name="a">Коэффициент метода (вес)</param>
        /// <param name="q">Вектор вероятностей</param>
        public static void HodgeLeman(Matrix S, out Vectors v, double a = 0.5, Vectors q = null)
        {
            v = new Vectors(S.n);
            if (q == null)
            {
                q = new Vectors(S.m);
                double w = 1.0 / q.n;
                for (int i = 0; i < q.n; i++) q[i] = w;
            }
            Vectors l = new Vectors(v), r = new Vectors(v);
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                int tmp1, tmp2;
                for (int j = 0; j < S.m; j++) l[i] += S[i, j] * q[j];
                MIN(c, out tmp2);
                r[i] = S[i, tmp2];
                v[i] = a * l[i] + (1 - a) * r[i];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Ходжа-Лемана с параметром {2} и вектором вероятностей {3} оптимальным является решение {0} (со значением {1}).", s + 1, v[s], a, q.ToString());
        }
        /// <summary>
        /// Критерий Гермейера
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Germeier(Matrix S, out Vectors v, Vectors q = null)
        {
            v = new Vectors(S.n);
            if (q == null)
            {
                q = new Vectors(S.m);
                double w = 1.0 / q.n;
                for (int i = 0; i < q.n; i++) q[i] = w;
            }
            for (int i = 0; i < S.n; i++)
            {
                Vectors c = S.GetLine(i);
                for (int j = 0; j < S.m; j++) c[j] *= q[j];
                int tmp; MIN(c, out tmp);
                v[i] = S[i, tmp];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию Гермейера с вектором вероятностей {2} оптимальным является решение {0} (со значением {1}).", s + 1, v[s], q.ToString());
        }
        /// <summary>
        /// Критерий произведений
        /// </summary>
        /// <param name="S">Матрица решений</param>
        /// <param name="v">Дополнительный столбец</param>
        public static void Powers(Matrix S, out Vectors v)
        {
            v = new Vectors(S.n);
            for (int i = 0; i < S.n; i++)
            {
                v[i] = 1;
                for (int j = 0; j < S.m; j++)
                    v[i] *= S[i, j];
            }

            int s; MAX(v, out s);
            Console.WriteLine("По критерию произведений оптимальным является решение {0} (со значением {1}).", s + 1, v[s]);
        }
    }

    /// <summary>
    /// Вероятности
    /// </summary>
    public class Probability
    {
        /// <summary>
        /// Абстрактный класс случайной величины
        /// </summary>
        public abstract class RandVal
        {
            /// <summary>
            /// Математическое ожидание
            /// </summary>
            public abstract double M { get; }
            /// <summary>
            /// Дисперсия
            /// </summary>
            public abstract double Dis { get; }
        }

        /// <summary>
        /// Дискретная случайная величина
        /// </summary>
        public class DisRandVal : RandVal
        {
            /// <summary>
            /// Значения случайной величины
            /// </summary>
            Vectors X;
            /// <summary>
            /// Значения вероятностей
            /// </summary>
            Vectors p;
            /// <summary>
            /// Функция распределения
            /// </summary>
            RealFunc F = null;

            //Конструкторы

            /// <summary>
            /// Конструктор по массиву вероятностей
            /// </summary>
            /// <param name="a"></param>
            public DisRandVal(double[] a)
            {
                if (!ProbOne(a)) throw new Exception("Сумма элементов в массиве не равна 1");
                X = new Vectors(a.Length);
                p = new Vectors(a.Length);
                for (int i = 0; i < a.Length; i++)
                {
                    X[i] = i + 1;
                    p[i] = a[i];
                }
            }
            /// <summary>
            /// Конструктор по умолчанию
            /// </summary>
            /// <param name="n"></param>
            public DisRandVal(int n)
            {
                double[] a = new double[n];
                X = new Vectors(n); p = new Vectors(n);
                double val = 1.0 / n;
                for (int i = 0; i < n; i++) a[i] = val;
                DisRandVal r = new DisRandVal(a);
                this.X = r.X;
                this.p = r.p;
            }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="r"></param>
            public DisRandVal(DisRandVal r) { X = new Vectors(r.X); p = new Vectors(r.p); }
            /// <summary>
            /// Чтение из файла
            /// </summary>
            /// <param name="fs"></param>
            public DisRandVal(StreamReader fs)
            {
                string s = fs.ReadLine();
                string[] st = s.Split(' ');
                int n = st.Length;
                this.X = new Vectors(n); this.p = new Vectors(n);
                for (int i = 0; i < n; i++) X[i] = Convert.ToDouble(st[i]);
                s = fs.ReadLine();
                st = s.Split(' ');
                for (int i = 0; i < n; i++) p[i] = Convert.ToDouble(st[i]);
                if (!ProbOne(this.p.vector)) throw new Exception("Сумма элементов в массиве не равна 1");
                fs.Close();
            }

            //Свойства
            /// <summary>
            /// Функция распределения дискретной случайной величины
            /// </summary>
            public RealFunc FDist
            {
                get
                {
                    return (double x) =>
                {
                    if (x < this.X[0]) return 0;
                    if (x > this.X[p.n - 1]) return 1;
                    double k = this.p[0];
                    for (int i = 1; i < this.p.n; i++)
                    {
                        if (x <= this.X[i]) return k;
                        k += this.p[i];
                    }
                    return k;
                };
                }
            }

            //методы
            /// <summary>
            /// Подходит ли массив под массив вероятностей
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            private bool ProbOne(double[] k)
            {
                double sum = 0;
                for (int i = 0; i < k.Length; i++) sum += k[i];
                if (sum == 1) return true;
                return false;
            }
            /// <summary>
            /// Проиллюстрировать
            /// </summary>
            public void Show()
            {
                this.X.PrintMatrix();
                this.p.PrintMatrix();
            }
            /// <summary>
            /// Мат. ожидание этой СВ
            /// </summary>
            /// <returns></returns>
            public override double M
            {
                get
                {
                    DisRandVal R = new DisRandVal(this);
                    return DisRandVal.MatExp(R);
                }
            }
            /// <summary>
            /// Дисперсия
            /// </summary>
            public override double Dis
            {
                get { return DisRandVal.Dispersion(this); }
            }
            /// <summary>
            /// Мат. ожидание СВ
            /// </summary>
            /// <param name="R"></param>
            /// <returns></returns>
            public static double MatExp(DisRandVal R)
            {
                double sum = 0;
                for (int i = 0; i < R.X.n; i++) sum += R.X[i] * R.p[i];
                return sum;
            }
            /// <summary>
            /// Дисперсия
            /// </summary>
            /// <param name="R"></param>
            /// <returns></returns>
            public static double Dispersion(DisRandVal R) { return CenM(R, 2); }
            /// <summary>
            /// Начальный момент
            /// </summary>
            /// <param name="R"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static double BegM(DisRandVal R, int n) { return MatExp((R) ^ n); }
            /// <summary>
            /// Центральный момент
            /// </summary>
            /// <param name="R"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static double CenM(DisRandVal R, int n) { return MatExp((R - R.M) ^ n); }
            /// <summary>
            /// По неравенству Чебышева вероятность того, что случайная величина отклонится от мат. ожидания не менее чем на eps
            /// </summary>
            /// <param name="R"></param>
            /// <param name="eps"></param>
            public static void NerCheb(DisRandVal R, double eps) { Console.WriteLine("<= {0}", R.Dis / eps / eps); }

            //операторы
            /// <summary>
            /// Смещение СВ
            /// </summary>
            /// <param name="A"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            public static DisRandVal operator -(DisRandVal A, double m)
            {
                DisRandVal M = new DisRandVal(A);
                for (int i = 0; i < M.X.n; i++) M.X[i] -= m;
                return M;
            }
            /// <summary>
            /// Случайная величина в степени
            /// </summary>
            /// <param name="A"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static DisRandVal operator ^(DisRandVal A, int n)
            {
                DisRandVal M = new DisRandVal(A);
                for (int i = 0; i < M.X.n; i++) M.X[i] = Math.Pow(M.X[i], n);
                return M;
            }
        }

        /// <summary>
        /// Непрерывная случайная величина
        /// </summary>
        public class ConRandVal : RandVal
        {
            /// <summary>
            /// Тип распределения (нормальное, равномерное, пуассоновское, экспоненциальное и т. д.)
            /// </summary>
            public enum BasisDistribution { Normal, Uniform, Puasson, Exp, Other };

            /// <summary>
            /// Вспомогательная функция
            /// </summary>
            private RealFunc x = (double t) => { return t; };

            /// <summary>
            /// Функция распределения
            /// </summary>
            private RealFunc F = null;
            /// <summary>
            /// Плотность распределения
            /// </summary>
            private RealFunc f = null;
            private BasisDistribution TypeValue = BasisDistribution.Other;
            /// <summary>
            /// Пока не известные мат. ожидание и дисперсия
            /// </summary>
            private double? m = null, d = null;

            //Конструкторы
            /// <summary>
            /// Конструктор по функции распределения и плотности распределения
            /// </summary>
            /// <param name="A"></param>
            /// <param name="a"></param>
            public ConRandVal(RealFunc A, RealFunc a) { F = A; f = a; }//по обеим функциям
                                                                       /// <summary>
                                                                       /// Конструктор только по плотности распределению
                                                                       /// </summary>
                                                                       /// <param name="a"></param>
            public ConRandVal(RealFunc a) { f = a;/* F = (double t) => { return DefInteg.};*/ }//по плотности распределения
                                                                                               /// <summary>
                                                                                               /// Конструктор копирования
                                                                                               /// </summary>
                                                                                               /// <param name="S"></param>
            public ConRandVal(ConRandVal S) { this.f = S.f; this.F = S.F; this.x = S.x; this.TypeValue = S.TypeValue; }
            /// <summary>
            /// Конструктор по одному из основных распределений с двумя аргументами
            /// </summary>
            /// <param name="Type"></param>
            /// <param name="m"></param>
            /// <param name="D"></param>
            public ConRandVal(BasisDistribution Type, double m, double D)
            {
                switch (Type)
                {
                    //Нормальное распределение
                    case BasisDistribution.Normal:
                        this.f = (double s) => { return 1.0 / Math.Sqrt(1 * Math.PI * D) * Math.Exp(-1.0 / 2 / D * (s - m) * (s - m)); };
                        this.m = m;
                        this.d = D;
                        this.F = (double x) => { return FuncMethods.DefInteg.Simpson((double t) => { return Math.Exp(-t * t / 2); }, 0, x); };
                        return;
                    //Равномерное распределение
                    case BasisDistribution.Uniform:
                        this.f = (double s) => { return 1.0 / (D - m); };
                        this.m = (D + m) / 2;
                        this.d = (D - m) * (D - m) / 12;
                        this.F = (double s) =>
                        {
                            if (s < m) return 0;
                            if (m < s && s <= D) return (s - m) / (D - m);
                            return 1;
                        };
                        return;
                    //Распределение Пуассона
                    case BasisDistribution.Puasson:
                        int m_new = (int)m;
                        double tmp = Math.Exp(-D);
                        this.f = (double s) => { return Math.Pow(D, m_new) / Combinatorik.P(m_new) * tmp; };
                        this.m = D;
                        this.d = D;
                        return;
                    default:
                        throw new Exception("Такого конструктора не существует");

                }
            }
            /// <summary>
            /// Конструктор по параметру экспоненциального распределния
            /// </summary>
            /// <param name="l"></param>
            public ConRandVal(double l)
            {
                this.f = (double s) =>
                {
                    if (s < 0) return 0;
                    return l * Math.Exp(-l * s);
                };
                this.m = 1 / l;
                this.d = 1 / l / l;

                this.F = (double s) =>
                {
                    if (s < 0) return 0;
                    return 1 - Math.Exp(-l * s);
                };
            }
            /// <summary>
            /// Конструктор нормального распределения по умолчанию
            /// </summary>
            public ConRandVal()
            {
                ConRandVal T = new ConRandVal(BasisDistribution.Normal, 0, 1);
                this.f = T.f; this.m = T.m; this.d = T.d;
            }

            //Операторы
            /// <summary>
            /// Случайная величина в степени
            /// </summary>
            /// <param name="a"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static ConRandVal operator ^(ConRandVal a, int n)
            {
                ConRandVal S = new ConRandVal(a);
                S.x = (double t) => { return Math.Pow(t, n); };
                return S;
            }
            /// <summary>
            /// Сдвиг случайной величины
            /// </summary>
            /// <param name="A"></param>
            /// <param name="m"></param>
            /// <returns></returns>
            public static ConRandVal operator -(ConRandVal A, double m)
            {
                ConRandVal S = new ConRandVal(A);
                S.x = (double t) => { return t - m; };
                return S;
            }

            //Методы
            /// <summary>
            /// Мат. ожидание
            /// </summary>
            /// <param name="R"></param>
            /// <returns></returns>
            public static double MatExp(ConRandVal R)
            {
                RealFunc xf = (double t) => { return R.x(t) * R.f(t); };
                return FuncMethods.DefInteg.ImproperFirstKind(xf);
            }
            /// <summary>
            /// Дисперсия
            /// </summary>
            /// <param name="R"></param>
            /// <returns></returns>
            public static double Dispersion(ConRandVal R) { return /*CenM(R, 2);*/ MatExp(R ^ 2) - R.M * R.M; }
            /// <summary>
            /// Начальный момент
            /// </summary>
            /// <param name="R"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static double BegM(ConRandVal R, int n) { return MatExp((R) ^ n); }
            /// <summary>
            /// Центральный момент
            /// </summary>
            /// <param name="R"></param>
            /// <param name="n"></param>
            /// <returns></returns>
            public static double CenM(ConRandVal R, int n) { return MatExp((R - R.M) ^ n); }
            /// <summary>
            /// По неравенству Чебышева вероятность того, что случайная величина отклонится от мат. ожидания не менее чем на eps
            /// </summary>
            /// <param name="R"></param>
            /// <param name="eps"></param>
            public static void NerCheb(ConRandVal R, double eps) { Console.WriteLine("<= {0}", R.Dis / eps / eps); }
            /// <summary>
            /// Вывести на консоль информацию о случайной величине
            /// </summary>
            public void Show()
            {
                Console.WriteLine("Мат. ожидание: {0} ; дисперсия: {1} ; тип распределения: {2}", this.M, this.Dis, this.TypeValue);
            }
            /// <summary>
            /// Вероятность попадания случайной величины в интервал
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public double P(double a, double b)
            {
                if (this.F != null) return F(b) - F(a);
                return FuncMethods.DefInteg.Simpson(this.f, a, b);
            }

            //Свойства
            /// <summary>
            /// Мат. ожидание
            /// </summary>
            public override double M
            {
                get
                {
                    if (this.m != null) return (double)this.m;
                    ConRandVal R = new ConRandVal(this);
                    this.m = ConRandVal.MatExp(R);
                    return (double)this.m;
                }
            }
            /// <summary>
            /// Дисперсия
            /// </summary>
            public override double Dis
            {
                get
                {
                    if (this.d != null) return (double)this.d;
                    this.d = ConRandVal.Dispersion(this);
                    return (double)this.d;
                }
            }

            //Константы класса
        }
    }

    //-----------------------------------------чисто для курсача
    /// <summary>
    /// Класс базисных точек
    /// </summary>
    public class BasisPoint : Point
    {
        public BasisPoint() : base(0) { }
        public BasisPoint(double a) : base(a) { }
        public BasisPoint(double a, double b) : base(a, b) { }
        public BasisPoint(Point p) : base(p) { }

        /// <summary>
        /// Функция базисного потенциала, сцепленного с точкой z
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public double PotentialF(BasisPoint z)
        {
            return Math.Log(1.0 / Point.Eudistance(this, z));
        }
        /// <summary>
        /// Функция базисного потенциала, сцепленного с точкой z
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public double PotentialF(Point z)
        {
            return Math.Log(1.0 / Point.Eudistance(this, z));
        }
        /// <summary>
        /// Функция второго базисного потенциала, сцепленного с точкой z
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public double BPotentialF(Point z)
        {
            double r = Point.Eudistance(this, z);
            return Math.Log(1.0 / r)*r*r;
        }

        //public static explicit operator Point(BasisPoint p)=>
    }
}

