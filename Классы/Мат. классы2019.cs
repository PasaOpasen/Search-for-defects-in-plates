using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods;
using Excel = Microsoft.Office.Interop.Excel;
using static Computator.NET.Core.NumericalCalculations.FunctionRoot;

namespace МатКлассы
{
    /// <summary>
    /// Комплексная функция многих переменных
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public delegate Complex CnToCFunction(CVectors v);
    /// <summary>
    /// Матричная функция от векторного аргумента
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public delegate CSqMatrix CVecToCMatrix(CVectors v);
    /// <summary>
    /// Функция R->C
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate Complex RToC(double x);

    public delegate double MethodR(Func<double, double> f, double beg, double end, double eps, uint N);

    /// <summary>
    /// Класс расширений для всяких методов
    /// </summary>
    public static partial class Expendator
    {
        /// <summary>
        /// Преобразование метода ()=>void в пригодный для использования после оператора await
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Task ToTask(this Action t) => Task.Run(t);
        /// <summary>
        /// Преобразование метода ()=>T в пригодный для использования после оператора await
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <remarks>Функцию типа f(x,y)=>T, x=fixX, y=fixY, нужно вызывать примерно так: await( ()=>f(fixX,fixY) ).ToTask()</remarks>
        public static Task ToTask<T>(this Func<T> t) => Task.Run(t);

        /// <summary>
        /// Кубический сплайн по сеточной функции с коэффициентами условий на границе
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="is0outcut">Должна ли функция равняться 0 вне отрезка задания</param>
        /// <returns></returns>
        public static RealFunc ToSpline(this NetFunc f, double a = 0, double b = 0, bool is0outcut = true) => Polynom.CubeSpline(f.Points, a, b, is0outcut);

        /// <summary>
        /// Сумма комплексного массива
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Complex Sum(this Complex[] mas)
        {
            Complex sum = 0;
            for (int i = 0; i < mas.Length; i++)
                sum += mas[i];
            return sum;
        }
        /// <summary>
        /// Сумма комплексных массивов
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Complex[] Adding(this Complex[] mas, Complex[] mas2)
        {
            Complex[] sum = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                sum[i] = mas[i] + mas2[i];
            return sum;
        }
        /// <summary>
        /// Сумма комплексных массивов
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Complex[] Add(Complex[] mas, Complex[] mas2)
        {
            Complex[] sum = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                sum[i] = mas[i] + mas2[i];
            return sum;
        }
        /// <summary>
        /// Действительная часть комплексного массива
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double[] Re(this Complex[] m)
        {
            double[] res = new double[m.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = m[i].Re;
            return res;
        }
        /// <summary>
        /// Действительная часть комплексного массива
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double[] ReOf(Complex[] m)
        {
            double[] res = new double[m.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = m[i].Re;
            return res;
        }

        /// <summary>
        /// Вывести пустую строку
        /// </summary>
        public static void EmptyLine() => "".Show();

        /// <summary>
        /// Примерный максимум модуля функции на отрезке
        /// </summary>
        /// <param name="f"></param>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static double MaxApproxAbs(ComplexFunc f, double beg, double end, double step = 0.01)
        {
            double m(double c) => f(c).Abs;
            double max = m(beg), tmp;
            while (beg <= end)
            {
                beg += step;
                tmp = m(beg);
                if (tmp > max)
                    max = tmp;  //max.Show();           
            }
            return max;
        }

        /// <summary>
        /// Объединение двух массивов с удалением одного из двух близких (ближе eps) друг к другу элементов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static double[] Distinct(double[] a, double[] b, double eps = 1e-6)
        {
            double[] m = Expendator.Union(a, b);
            Array.Sort(m);
            List<double> l = new List<double>();
            l.Add(m[0]);
            int k = 0;
            for (int i = 1; i < m.Length; i++)
                if (m[i] - m[k] >= eps)
                {
                    l.Add(m[i]);
                    k = i;
                }
            return l.ToArray();
        }

        /// <summary>
        /// Деление комплексного массива на комплексное число
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public static Complex[] Div(this Complex[] mas, Complex coef)
        {
            Complex[] res = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                res[i] = mas[i] / coef;
            return res;
        }
        /// <summary>
        /// Умножение комплексного массива на комплексное число
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public static Complex[] Multiply(this Complex[] mas, Complex coef)
        {
            Complex[] res = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                res[i] = mas[i] * coef;
            return res;
        }
        /// <summary>
        /// Умножение комплексного массива на комплексное число
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public static Complex[] Mult(Complex[] mas, Complex coef)
        {
            Complex[] res = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                res[i] = mas[i] * coef;
            return res;
        }
        /// <summary>
        /// Умножение пары комплексных чисел на комплексное числл
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public static Tuple<Complex, Complex> Mult(Tuple<Complex, Complex> mas, Complex coef) => new Tuple<Complex, Complex>(mas.Item1 * coef, mas.Item2 * coef);

        /// <summary>
        /// Записать массив векторов в файл
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mas"></param>
        public static void WriteInFile(string name = "1", params Vectors[] mas)
        {
            StreamWriter f = new StreamWriter(name + ".txt");
            for (int i = 0; i < mas.Length; i++)
            {
                for (int j = 0; j < mas[i].Deg - 1; j++)
                    f.Write(mas[i][j] + " ");
                f.WriteLine(mas[i][mas[i].Deg - 1]);
            }
            f.Close();
        }

        /// <summary>
        /// Запустить процесс и выполнить какие-то действия по его окончанию
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="act"></param>
        public static void StartProcess(string fileName, Action act)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.EnableRaisingEvents = true;

            process.Exited += (sender, e) => act();
            process.Start();
        }

        /// <summary>
        /// Дубликат массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static T[] Dup<T>(this T[] mas) where T : struct
        {
            T[] res = new T[mas.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = mas[i];
            return res;
        }
        /// <summary>
        /// Дубликат массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static T[] dup<T>(this T[] mas) where T : Idup<T>
        {
            T[] res = new T[mas.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = mas[i].dup;
            return res;
        }
        /// <summary>
        /// Срез массива
        /// </summary>
        /// <param name="mas"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static T[] Slice<T>(this T[] mas,int i, int j) where T : Idup<T>
        {
            T[] res = new T[j - 1 + 1];
            for (int s = 0; s < res.Length; s++)
                res[s] = mas[i + s].dup;
            return res;
        }


        public static int ToInt32(this string s) => Convert.ToInt32(s);
        public static int ToInt32(this object s) => Convert.ToInt32(s);

        public static Complex[] ToComplex(this double[] m) => new CVectors(m).ComplexMas;

        /// <summary>
        /// Преобразовать строку в массив действительных чисел
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double[] ToDoubleMas(this string s)
        {
            string[] st = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            double[] res = new double[st.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] =Convert.ToDouble( st[i]);
            st = null;
            return res;
        }
        /// <summary>
        /// Преобразовать число в строку, из которой его можно воспроизвести
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToRString(this double s) => s.ToString("r");

        /// <summary>
        /// Создать и заполнить массив алгебраической прогрессией
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static double[] Seq(double from, double to, int count, bool withto = true)
        {
            double[] res = new double[count];
            double h;
            if (withto)
                h = (to - from) / (count - 1);
            else
                h = (to - from) / (count);

            for (int i = 0; i < count; i++)
                res[i] = from + i * h;
            return res;
        }

        /// <summary>
        /// Применяет функцию к массиву и возвращает массив результатов
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="mas"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T1[] Map<T1,T2>(this T2[] mas,Func<T2,T1> func)
        {
            T1[] res = new T1[mas.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = func(mas[i]);
            return res;
        }

        /// <summary>
        /// Возвращает пару индексов массива, между элементами которого находится искомый элемент либо пару одинаковых индексов, когда элемент совпадает с каким-то элементом массива
        /// </summary>
        /// <param name="w"></param>
        /// <param name="el"></param>
        /// <returns></returns>
        public static Tuple<int,int> BinarySearch(double[] w,double el)
        {
            int i = 0,j=w.Length-1,c;
            if (el < w[0]) return new Tuple<int, int>(i, i);
            if (el > w[j]) return new Tuple<int, int>(j, j);

            while (j - i > 1)
            {
                c = (i + j) / 2;
                if (w[c] == el) return new Tuple<int, int>(c, c);
                if (el > w[c]) i = c;
                else j = c;
            }
            return new Tuple<int, int>(i, j);
        }

        /// <summary>
        /// Найти период в массиве
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static int GetPeriod<T>(T[] mas)
        {
            bool f = true;
            for (int k = 1; k <= mas.Length / 2; k++)
            {
                f = true;
                for (int i = 0; i < k; i++)
                {
                    int s = 0;
                    while (i + (k) * (s+1) < mas.Length)
                    {
                        if (!mas[i + k * s].Equals(mas[i + (k ) * (s+1)]))
                        {
                            f = false;
                            goto z1;
                        }
                        s++;
                    }

                }
            z1:
                if (f) return k;
            }
            return 0;
        }

        public static CVectors ToCVector(this Complex[] m) => new CVectors(m);

        /// <summary>
        /// Записать одно слово в файл
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="word"></param>
        public static void WriteStringInFile(string filename,string word)
        {
            using (StreamWriter f = new StreamWriter(filename))
                f.WriteLine(word);
        }
        /// <summary>
        /// Записать массив слов в файл
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="word"></param>
        public static void WriteInFile(string filename, string[] word)
        {
            using (StreamWriter f = new StreamWriter(filename))
                for (int i = 0; i < word.Length; i++)
                    f.WriteLine(word[i]);
        }

        /// <summary>
        /// Прочесть все строки файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string[] GetStringArrayFromFile(string filename,bool withoutEmpty=false)
        {
            string[] st;
            using (StreamReader f = new StreamReader(filename))
                st = f.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (withoutEmpty)
                st = st.Where(n => n.Length > 0).ToArray();
            return st;
        }

        /// <summary>
        /// Получить первую строку из файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetWordFromFile(string filename) => GetStringArrayFromFile(filename, true)[0];

        /// <summary>
        /// Определяет директорию, считанную из файла и возвращает ответ о её существовании
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool IfDirectoryExists(string filename,out string directory)
        {
            directory = GetWordFromFile(filename);
            return Directory.Exists(directory);
        }

        /// <summary>
        /// Скопировать набор файлов из одной директории в другую, сохраняя имена
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="filenames"></param>
        public static void CopyFiles(string from,string to,params string[] filenames)
        { 
            for (int i = 0; i < filenames.Length; i++)
                File.Copy(Path.Combine(from, filenames[i]), Path.Combine(to, filenames[i]), true);
        }
    }

    /// <summary>
    /// Класс функции, осуществляющей отображение Ck -> Cn
    /// </summary>
    public class CkToCnFunc
    {
        /// <summary>
        /// Делегат, отождествляемый с унитарным отображением
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public delegate CVectors VecToVec(CVectors v);

        private CnToCFunction[] FuncMas;
        private VecToVec func = null;

        /// <summary>
        /// Размерность области значений
        /// </summary>
        public int EDimention => FuncMas.Length;

        /// <summary>
        /// Значение функции от вектора через индексатор
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public CVectors this[CVectors v]
        {
            get
            {
                if (func == null)
                {
                    CVectors res = new CVectors(EDimention);
                    for (int i = 0; i < EDimention; i++)
                        res[i] = FuncMas[i](v);
                    return res;
                }
                else
                    return func(v);
            }
        }
        /// <summary>
        /// Функция отдельного измерения
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CnToCFunction this[int i] => new CnToCFunction(FuncMas[i]);
        /// <summary>
        /// Метод, возвращающий значение функции от вектора
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public CVectors Value(CVectors v) => this[v];
        /// <summary>
        /// Значение функции от вектора
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public CVectors Value(params Complex[] c)
        {
            CVectors v = new CVectors(c);
            return this[v];
        }

        /// <summary>
        /// Задание функции как совокупности комплексных функций многих переменных
        /// </summary>
        /// <param name="mas"></param>
        public CkToCnFunc(params CnToCFunction[] mas)
        {
            FuncMas = new CnToCFunction[mas.Length];
            for (int i = 0; i < FuncMas.Length; i++)
                FuncMas[i] = new CnToCFunction(mas[i]);
        }
        /// <summary>
        /// Задать унитарную функции как произведение унитарной функции на комплексную матрицу
        /// </summary>
        /// <param name="M"></param>
        /// <param name="F"></param>
        public CkToCnFunc(CSqMatrix M, CkToCnFunc F)
        {
            this.FuncMas = new CnToCFunction[F.EDimention];
            for (int i = 0; i < this.EDimention; i++)
                this.FuncMas[i] = (CVectors v) => M.GetLine(i) * F.Value(v);
        }
        /// <summary>
        /// Задать унитарную функции как произведение унитарной функции на кматричную функцию
        /// </summary>
        /// <param name="M"></param>
        /// <param name="F"></param>
        public CkToCnFunc(CVecToCMatrix M, CkToCnFunc F)
        {
            this.FuncMas = null;
            func = (CVectors v) =>
              {
                  CSqMatrix Mat = M(v);
                  CVectors Vec = F.Value(v);
                  CVectors res = new CVectors(Vec.Degree);

                  for (int i = 0; i < this.EDimention; i++)
                      res[i] = new Complex(Mat.GetLine(i) * Vec);
                  return res;
              };

        }
        /// <summary>
        /// Задать функцию через делегат отображения
        /// </summary>
        /// <param name="f"></param>
        public CkToCnFunc(VecToVec f) { this.func = new VecToVec(f); }

        /// <summary>
        /// Тип каррирования
        /// </summary>
        public enum CarringType
        {
            /// <summary>
            /// По первым аргументам
            /// </summary>
            FirstArgs,
            /// <summary>
            /// По последним аргументам
            /// </summary>
            LastArgs
        }
        /// <summary>
        /// Каррирование отображения в соответствии с параметрами
        /// </summary>
        /// <param name="C">Параметр каррирования</param>
        /// <param name="c">Фиксированные аргументы</param>
        /// <returns></returns>
        public CkToCnFunc CarrByFirstOrLastArgs(CarringType C = CarringType.LastArgs, params Complex[] c)
        {
            if (func == null)
            {
                CnToCFunction[] mas = new CnToCFunction[EDimention];

                switch (C)
                {
                    case CarringType.FirstArgs:
                        for (int i = 0; i < mas.Length; i++)
                            mas[i] = (CVectors v) => this.FuncMas[i](new CVectors(Expendator.Union(c, v.ComplexMas)));
                        break;
                    default:
                        for (int i = 0; i < mas.Length; i++)
                            mas[i] = (CVectors v) => this.FuncMas[i](new CVectors(Expendator.Union(v.ComplexMas, c)));
                        break;
                }
                return new CkToCnFunc(mas);
            }
            else
            {
                VecToVec h;
                switch (C)
                {
                    case CarringType.FirstArgs:
                        h = (CVectors v) => func(new CVectors(Expendator.Union(c, v.ComplexMas)));
                        break;
                    default:
                        h = (CVectors v) => func(new CVectors(Expendator.Union(v.ComplexMas, c)));
                        break;
                }
                return new CkToCnFunc(h);
            }

        }

        /// <summary>
        /// Интеграл от отображения по одному аргументу (другие зафиксированы)
        /// </summary>
        /// <param name="beforeArg">Фиксированные аргументы до изменяемого</param>
        /// <param name="afterArg">Фиксированные аргументы после изменяемого</param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="tm"></param>
        /// <param name="tp"></param>
        /// <param name="eps"></param>
        /// <param name="pr"></param>
        /// <param name="gr"></param>
        /// <returns></returns>
        public CVectors IntegralAmoutOneArg(CVectors beforeArg, CVectors afterArg, double t1, double t2, double t3, double t4, double tm, double tp, double eps, double pr, double gr)
        {
            FuncMethods.DefInteg.GaussKronrod.ComplexVectorFunc tmp = (Complex x, int N) => this.Value(Expendator.Union(beforeArg.ComplexMas, new Complex[] { x }, afterArg.ComplexMas)).ComplexMas;
            return new CVectors(FuncMethods.DefInteg.GaussKronrod.DINN5_GK(tmp, t1, t2, t3, t4, tm, tp, eps, pr, gr, this.EDimention));
        }
    }

    /// <summary>
    /// Класс оптимизации функции двух аргументов
    /// </summary>
    public static class OptimizationDCompFunc
    {
        /// <summary>
        /// Поиск минимума функции на прямоугольнике, чьи стороны параллельны осям координат
        /// </summary>
        /// <param name="f">Оптимизируемая функция</param>
        /// <param name="x0"></param>
        /// <param name="X"></param>
        /// <param name="y0"></param>
        /// <param name="Y"></param>
        /// <param name="nodescount">Корень из числа точек, берущихся в прямоугольнике (нижняя граница, потому что если прямоугольник слишком далёк от квадрата, надо брять другое соотношение)</param>
        /// <param name="eps">Погрешность поиска</param>
        /// <param name="ogr">Через сколько максимально итераций нужно закончить цикл, если последние ogr итераций подряд точка максимума не изменялась</param>
        /// <returns></returns>
        public static Tuple<double, double> GetMaxOnRectangle(Func<double, double, double> f, double x0, double X, double y0, double Y, int nodescount = 10, double eps = 1e-7, int ogr = 3, bool useGradient = false, bool parallel = true)
        {
            double max = f(x0, y0);//max.Show();
            Tuple<double, double> res = new Tuple<double, double>(x0, y0);
            double x = (X - x0).Abs(), y = (Y - y0).Abs();
            int nodescI, nodescJ;
            if (x > y)
            {
                nodescJ = nodescount;
                nodescI = (int)(nodescount * (x / y));
            }
            else
            {
                nodescI = nodescount;
                nodescJ = (int)(nodescount * (x / y));
            }
            double[,] mas = new double[nodescI, nodescJ];
            int k = 0;

            while (((X - x0) * (Y - y0)).Abs() > eps && k <= ogr)
            {
                double xstep = (X - x0) / (nodescI /*+ 1*/);
                double ystep = (Y - y0) / (nodescJ /*+ 1*/);

                k++;
                if (!parallel)
                    for (int i = 0; i < nodescI; i++)
                        for (int j = 0; j < nodescJ; j++)
                        {
                            mas[i, j] = f(x0 + xstep * i, y0 + ystep * j);
                            if (mas[i, j] > max)
                            {
                                k = 0;
                                max = mas[i, j];//max.Show();
                                res = new Tuple<double, double>(x0 + xstep * i, y0 + ystep * j);
                            }
                        }
                else
                {
                    //параллельная версия
                    double[] maxmas = new double[nodescI];
                    Tuple<double, double>[] resmas = new Tuple<double, double>[nodescI];
                    for (int i = 0; i < nodescI; i++)
                    {
                        maxmas[i] = max;
                        resmas[i] = new Tuple<double, double>(res.Item1, res.Item2);
                    }

                    Parallel.For(0, nodescI, (int i) =>
                    {
                        for (int j = 0; j < nodescJ; j++)
                        {
                            mas[i, j] = f(x0 + xstep * i, y0 + ystep * j);
                            if (mas[i, j] > maxmas[i])
                            {
                                k = 0;
                                maxmas[i] = mas[i, j];//max.Show();
                                resmas[i] = new Tuple<double, double>(x0 + xstep * i, y0 + ystep * j);
                            }
                        }
                    });

                    max = maxmas.Max();
                    int tmp = Array.IndexOf(maxmas, max);
                    res = new Tuple<double, double>(resmas[tmp].Item1, resmas[tmp].Item2);
                }



                //double x = x0 + (X - x0) / 2;
                //double y = y0 + (Y - y0) / 2;
                //if (res.Item1 > x) x0 = x;
                //else X = x;
                //if (res.Item2 > y) y0 = y;
                //else Y = y;
                x = (X - x0) / 2;
                double x2 = x / 2, p1p = res.Item1 + x2, p1m = res.Item1 - x2;
                y = (Y - y0) / 2;
                double y2 = y / 2, p2p = res.Item2 + y2, p2m = res.Item2 - y2;

                if (p1m < x0) X = x0 + x;
                else if (p1p > X) x0 = X - x;
                else
                {
                    x0 = p1m;
                    X = p1p;
                }
                if (p2m < y0) Y = y0 + y;
                else if (p2p > Y) y0 = Y - y;
                else
                {
                    y0 = p2m;
                    Y = p2p;
                }

            }

            if (useGradient)
            {
                Complex point = new Complex(res.Item1, res.Item2);
                ComplexFunc cf = (Complex a) => f(a.Re, a.Im);
                Gradient(cf, ref point, eps: eps);
                res = new Tuple<double, double>(point.Re, point.Im);
            }

            return res;
        }

        /// <summary>
        /// Метод градиентного спуска к максимуму по модулю от функции
        /// </summary>
        /// <param name="f">Функция комплексного переменного</param>
        /// <param name="point">Начальная точка</param>
        /// <param name="alp">Коэффициент метода</param>
        /// <param name="maxcount">Максимальное число итераций</param>
        /// <param name="eps">Погрешность</param>
        private static void Gradient(ComplexFunc f, ref Complex point, double alp = 0.01, int maxcount = 100, double eps = 1e-14)
        {
            DefInteg.Residue.eps = eps;
            Complex gr = DefInteg.Residue.Derivative(f, point);
            Complex fp = f(point);
            int count = 0;
            while (gr.Abs > eps && count <= maxcount && alp > 10 * eps)
            {
                Complex p2 = point - alp * gr;
                Complex fp2 = f(p2);
                if (fp2.Abs > fp.Abs)
                {
                    point = new Complex(p2);
                    fp = new Complex(fp2);
                    gr = DefInteg.Residue.Derivative(f, p2);
                }
                else
                {
                    alp /= 2;
                }
                count++;
            }
            //point = new Complex(fp);
        }
    }

    /// <summary>
    /// Класс методов поиска корней
    /// </summary>
    public static class Roots
    {
        /// <summary>
        /// Простой поиск корней комплексной функции на действительном отрезке методом дихотомии
        /// </summary>
        /// <param name="f"></param>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static Vectors MyHalfc(ComplexFunc f, double beg, double end, double step = 0.01, double eps = 1e-12)
        {
            List<double> list = new List<double>();
            double a = beg, a2 = beg + step, b = end, t1, t2, s;
            Complex fa = f(a), fa2 = f(a2), fc;
            while (a < b)
            {
                if (fa.Abs < eps) list.Add(a);
                else
                if (Math.Sign(fa.Re) * Math.Sign(fa2.Re) <= 0 && Math.Sign(fa.Im) * Math.Sign(fa2.Im) <= 0)//написал условие именно так, чтобы избежать переполнения
                {
                    t1 = a; t2 = a2;
                    while (t2 - t1 > eps)
                    {
                        s = (t1 + t2) / 2;
                        fc = f(s);//fc.Show();
                        if (fc.Abs < eps)
                            break;
                        if (Math.Sign(fa.Re) * Math.Sign(fc.Re) <= 0 && Math.Sign(fa.Im) * Math.Sign(fc.Im) <= 0)
                            t2 = s;
                        else t1 = s;
                    }
                    s = (t1 + t2) / 2;
                    if (f(s).Abs < 3) list.Add(s);
                }
                a = a2;
                a2 += step;
                fa = new Complex(fa2);
                fa2 = f(a2);
            }
            return new Vectors(list.ToArray());
        }
        /// <summary>
        /// Простой поиск корней как поиск минимумов модуля функции (которые должны быть равны 0)
        /// </summary>
        /// <param name="f"></param>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static Vectors RootsByMinAbs(ComplexFunc f, double beg, double end, double step = 0.01, double eps = 1e-12)
        {
            RealFunc fabs = (double c) => f(c).Abs;
            List<double> list = new List<double>();

            double a = beg, a2 = beg + step, t1, t2, s, d1, d2, ds;
            double fa = fabs(a), fa2 = fabs(a2), fc;

            double der(double c) => fabs(c + eps) - fabs(c - eps);

            while (a < end)
            {
                d1 = der(a);
                d2 = der(a2);
                //$"{d1} {d2}".Show();
                if (fa < eps) list.Add(a);
                else if (fa2 < eps) list.Add(a2);
                else if (d1 < 0 && d2 > 0)
                {
                    t1 = a; t2 = a2;
                    while (t2 - t1 > eps)
                    {
                        //$"{d1} {d2}".Show();
                        s = (t1 + t2) / 2;
                        fc = fabs(s);//fc.Show();
                        ds = der(s);
                        if (fc < eps) break;
                        if (ds > 0)
                        { t2 = s; d2 = ds; }
                        else { t1 = s; d1 = ds; }
                    }
                    s = (t1 + t2) / 2;
                    list.Add(s);
                }

                a = a2;
                a2 += step;
                fa = fa2;
                fa2 = fabs(a2);
            }

            for (int i = 0; i < list.Count; i++)
                if (fabs(list[i]) > eps)
                {
                    list.RemoveAt(i);
                    i--;
                }

            return new Vectors(list.Distinct().ToArray());
        }

        /// <summary>
        /// Метод локального поиска корня
        /// </summary>
        public enum MethodRoot
        {
            Brent,
            Broyden,
            Bisec,
            Secant,
            NewtonRaphson,
            /// <summary>
            /// Комбинация методов Brent, Secant и Broyden
            /// </summary>
            Combine
            //Halfc
        }
        /// <summary>
        /// Поиск корней одним из специальных методов
        /// </summary>
        /// <param name="f"></param>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <param name="eps"></param>
        /// <param name="m">Метод</param>
        /// <param name="withMuller">Дополнять ли корни корнями метода парабол</param>
        /// <returns></returns>
        public static Vectors OtherMethod(ComplexFunc f, double beg, double end, double step = 0.01, double eps = 1e-12, MethodRoot m = MethodRoot.Brent, bool withMuller = false,int countpoles=2)
        {
            List<double> list = new List<double>();
            Func<double, double> func = (double c) => f(c).ReIm;
            double a = beg, b = beg + step;
            Complex fa = f(a), fb = f(b);
            MethodR g;
            //MethodR half = (Func<double, double> ff, double begf, double endf, double epsf, uint N) => Optimization.Halfc((double c) => f(c).ReIm, begf, endf, step, eps);
            switch (m)
            {
                case MethodRoot.Brent:
                    g = BrentMethod;
                    break;
                case MethodRoot.Bisec:
                    g = bisectionMethod;
                    break;
                case MethodRoot.Secant:
                    g = secantMethod;
                    break;
                case MethodRoot.NewtonRaphson:
                    g = secantNewtonRaphsonMethod;
                    break;
                default:
                    g = BroydenMethod;
                    break;
            }

            if (withMuller)
                while (a < end)
                {
                    if (fa.Re * fb.Re <= 0 && fa.Im * fb.Im <= 0)
                    {
                        if (m == MethodRoot.Combine)
                        {
                            list.Add(BrentMethod(func, a, b, 1e-12, N: 1000));
                            list.Add(BroydenMethod(func, a, b, 1e-12, N: 1000));
                            list.Add(secantMethod(func, a, b, 1e-12, N: 1000));
                        }
                        else
                            list.Add(g(func, a, b, 1e-12, N: 1000));
                        //Optimization.Muller(f, a, new Complex((a + b) / 2, 0), new Complex(b, 0)).Re.Show();
                        list.Add(Optimization.Muller(f, a, new Complex((a + b) / 2, 0.01), new Complex((a + b) / 2, -0.01)).Re);
                    }
if (list.Count == countpoles) break;

                    a = b;
                    b += step;
                    fa = new Complex(fb);
                    fb = f(b);              
                }
            else
                while (a < end)
                {
                    if (fa.Re * fb.Re <= 0 && fa.Im * fb.Im <= 0)
                    {
                        if (m == MethodRoot.Combine)
                        {
                            list.Add(BrentMethod(func, a, b, 1e-12, N: 1000));
                            list.Add(BroydenMethod(func, a, b, 1e-12, N: 1000));
                            list.Add(secantMethod(func, a, b, 1e-12, N: 1000));
                        }
                        else
                            list.Add(g(func, a, b, 1e-12, N: 1000));
                    }

                    if (list.Count == countpoles) break;
                    a = b;
                    b += step;
                    fa = new Complex(fb);
                    fb = f(b);
                }

            //new Vectors(list.ToArray()).Show();

            return new Vectors(list.Distinct().Where(n => !Double.IsNaN(n) && f(n).Abs <= eps && n >= beg && n <= end).ToArray());
        }
    }

    /// <summary>
    /// Некоторые специальные функции
    /// </summary>
    public static class SpecialFunctions
    {
        /// <summary>
        /// Функция Бесселя
        /// </summary>
        /// <param name="a">Порядок</param>
        /// <param name="x">Аргумент</param>
        /// <returns></returns>
        public static Complex MyBessel(double a, Complex x)
        {
            if (x.Im.Abs() < 1e-16) return Computator.NET.Core.Functions.SpecialFunctions.BesselJν(a, x.Re);
            ComplexFunc f = (Complex c) => Complex.Cos(a * c - x * Complex.Sin(c));
            return FuncMethods.DefInteg.GaussKronrod.MySimpleGaussKronrod(f, 0, Math.PI, 61, true, 5) / Math.PI;
        }

        /// <summary>
        /// Функция Ханкеля первого рода
        /// </summary>
        /// <param name="a"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Complex Hankel(double a, double z) => Computator.NET.Core.Functions.SpecialFunctions.Hankel1(a, z);
    }

    /// <summary>
    /// Класс разных методов, которые я затем использую в комбинации со скриптами
    /// </summary>
    public static class ForScripts
    {
        /// <summary>
        /// Создать файлы для последующего создания поверхностей. 
        /// </summary>
        /// <param name="x0">Минимальное значение аргумента х</param>
        /// <param name="X">Максимальное значение аргумента х</param>
        /// <param name="argcount">Число точек по аргументу</param>
        /// <param name="filename">Базовое имя файла</param>
        /// <param name="f">Массив функций, от которых нужны поверхности</param>
        /// <param name="filter">Фильтр принадлежности области, на которой надо рисовать</param>
        public static void MakeFilesForSurfaces(double x0, double X, double y0, double Y, int argcount, string filename, Functional[] f, Func<Point, bool> filter,bool parallel=false)
        {

            int xc = argcount, yc = argcount;

            double hx = (X - x0) / (xc - 1), hy = (Y - y0) / (yc - 1);

            double[] xa = new double[xc], ya = new double[yc];

            for (int i = 0; i < xc; i++)
                xa[i] = x0 + hx * i;
            for (int i = 0; i < yc; i++)
                ya[i] = y0 + hy * i;

            double[][,] val = new double[f.Length][,];
            for (int i = 0; i < f.Length; i++)
            {
                val[i] = new double[xc, yc];

                if(!parallel)
                for (int ix = 0; ix < xc; ix++)
                    for (int iy = 0; iy < yc; iy++)
                    {
                        Point t = new Point(xa[ix], ya[iy]), o = new Point(0);

                        if (filter(t))
                            val[i][ix, iy] = f[i](t);
                        else val[i][ix, iy] = Double.NaN;
                    }
                else             
                    Parallel.For(0, xc, (int ix) => 
                    {
                        for (int iy = 0; iy < yc; iy++)
                        {
                            Point t = new Point(xa[ix], ya[iy]), o = new Point(0);

                            if (filter(t))
                                val[i][ix, iy] = f[i](t);
                            else val[i][ix, iy] = Double.NaN;
                        }
                    });
                

            }

            StreamWriter args = new StreamWriter(filename + "(arg).txt");
            StreamWriter vals = new StreamWriter(filename + "(vals).txt");

            for (int i = 0; i < xc; i++)
                args.WriteLine($"{xa[i]} {ya[i]}");

            for (int i = 0; i < xc; i++)
                for (int j = 0; j < yc; j++)
                {
                    string st = "";
                    for (int s = 0; s < f.Length; s++)
                        st += (val[s][i, j] + " ");
                    vals.WriteLine(st.Replace("NaN", "NA"));
                }


            args.Close();
            vals.Close();
        }
    }

    /// <summary>
    /// B-сплайны дефекта 1
    /// </summary>
    public class BSpline
    {
        /// <summary>
        /// Массив узлов
        /// </summary>
        private CVectors xk;

        private int m => xk.Degree - 1;

        /// <summary>
        /// Создать сплайн по массиву узлов
        /// </summary>
        /// <param name="mas"></param>
        public BSpline(CVectors mas) { xk = mas.dup; }

    }

}