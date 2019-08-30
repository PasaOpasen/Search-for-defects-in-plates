using System;
using System.Collections.Generic;
using System.IO;

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
    /// <summary>
    /// Точки на плоскости
    /// </summary>
    public class Point : IComparable, ICloneable, Idup<Point>
    {
        /// <summary>
        /// Начало координат в нуле
        /// </summary>
        public static readonly Point Zero;

        //координаты
        /// <summary>
        /// Первая координата точки
        /// </summary>
        public double x = 0;

        /// <summary>
        /// Вторая координата точки
        /// </summary>
        public double y = 0;

        static Point()
        {
            Zero = new Point(0, 0);
        }

        //конструкторы
        /// <summary>
        /// Точка с нулевыми координатами
        /// </summary>
        public Point() { x = 0; y = 0; }

        /// <summary>
        /// Точка с одинаковыми координатами
        /// </summary>
        /// <param name="a"></param>
        public Point(double a) { x = a; y = a; }

        /// <summary>
        /// Точка по своим координатам
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Point(double a, double b) { x = a; y = b; }

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="p"></param>
        public Point(Point p) : this(p.x, p.y) { }

        /// <summary>
        /// Расстояние от точки до (0,0)
        /// </summary>
        public double Abs => new Number.Complex(x, y).Abs;

        /// <summary>
        /// Дубликат точки
        /// </summary>
        public Point dup => new Point(this);

        public Point Swap => new Point(this.y, this.x);
        public static Point Add(Point p, double d) => new Point(p.x + d, p.y + d);

        public static Point Add(Point p, Point d) => new Point(p.x + d.x, p.y + d.y);

        /// <summary>
        /// Центр множества точек как их взвешенная сумма
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Point Center(Point[] mas)
        {
            double x = mas[0].x, y = mas[0].y;
            for (int i = 1; i < mas.Length; i++)
            {
                x += mas[i].x;
                y += mas[i].y;
            }
            return new Point(x / mas.Length, y / mas.Length);
        }

        /// <summary>
        /// Евклидово расстояние между точками
        /// </summary>
        /// <param name="z"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static double Eudistance(Point z, Point w)
        {
            return Math.Sqrt((z.x - w.x) * (z.x - w.x) + (z.y - w.y) * (z.y - w.y));
        }

        /// <summary>
        /// Возвращает координаты нижнего левого и верхнего правого угла прямоугольника, сожержащего все точки массива
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        public static Tuple<Point, Point> GetBigRect(Point[] mas)
        {
            Point min = mas[0].dup;
            Point max = min.dup;
            for (int i = 1; i < mas.Length; i++)
            {
                if (min.x > mas[i].x)
                {
                    min.x = mas[i].x;
                }

                if (min.y > mas[i].y)
                {
                    min.y = mas[i].y;
                }

                if (max.x < mas[i].x)
                {
                    max.x = mas[i].x;
                }

                if (max.y < mas[i].y)
                {
                    max.y = mas[i].y;
                }
            }

            return new Tuple<Point, Point>(min, max);
        }

        /// <summary>
        /// Перевести массив чисел в последовательность точек на плоскости
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Point[] GetSequence(double[] c)
        {
            Point[] p = new Point[c.Length];
            for (int i = 0; i < c.Length; i++)
            {
                p[i] = new Point(i, c[i]);
            }

            return p;
        }

        public static implicit operator Point(Number.Complex e)=>new Point(e.Re, e.Im);

        public static Point operator -(Point p) => new Point(-p.x, -p.y);

        public static bool operator !=(Point a, Point b)
        {
            /*if (Convert.IsDBNull((object)b) || Convert.IsDBNull((object)a)) return false;*/
            return !(a == b);
        }

        /// <summary>
        /// Скалярное произведение точек как векторов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double operator *(Point a, Point b) => a.x * b.x + a.y * b.y;

        public static Point operator *(double s, Point p) => new Point(s * p.x, s * p.y);

        public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);

        public static bool operator <(Point a, Point b) //функция компоратора
        {
            if (a.y < b.y)
            {
                return true; //cравнение по второй координате
            }
            else if (a.y > b.y)
            {
                return false;
            }
            else
            {
                return a.x < b.x; //если вторые координаты равны, сравнение по первой координате
            }
        }

        //public static bool operator ==(Point a, Point b) => (new Complex(a) - new Complex(b)).Abs == 0;
        public static bool operator ==(Point a, Point b) { /*if (a == null || b == null) return false;bool tmp = false; try { tmp=(a.x == b.x) && (a.y == b.y); } catch(NullReferenceException e){ }return tmp;*/  return (a.x == b.x) && (a.y == b.y); }

        /// <summary>
        /// Сравнение точек по установленной по умолчанию упорядоченности
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator >(Point a, Point b)
        {
            return (b < a);
        }

        //методы
        /// <summary>
        /// Набор n+1 точек на графике функции f, разбитых равномерно на отрезке от a до b
        /// </summary>
        /// <param name="f"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point[] Points(RealFunc f, int n, double a, double b)
        {
            double h = (b - a) / n;
            Point[] points = new Point[n + 1];
            for (int i = 0; i <= n; i++)
            {
                points[i] = new Point(a + h * i, f(a + h * i));
            }

            return points;
        }

        /// <summary>
        /// Вывести массив точек, через которые проходит функция
        /// </summary>
        /// <param name="f">Функция, заданная на отрезке</param>
        /// <param name="h">Шаг обхода отрезка</param>
        /// <param name="a">Начало отрезка</param>
        /// <param name="b">Конец отрезка</param>
        /// <returns></returns>
        public static Point[] Points(RealFunc f, double h, double a, double b)
        {
            int n = (int)((b - a) / h);
            Point[] points = new Point[n];
            for (int i = 0; i < n; i++)
            {
                points[i] = new Point(a + h * i, f(a + h * i));
            }

            return points;
        }

        /// <summary>
        /// Считать массив точек из файла
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static Point[] Points(StreamReader fs)
        {
            string s = fs.ReadLine();
            string[] st = s.Split(' ');
            int n = Convert.ToInt32(st[0]);
            Point[] p = new Point[n];

            for (int k = 0; k < n; k++)
            {
                s = fs.ReadLine();
                st = s.Split(' ');//в аргументах указывается массив символов, которым разделяются числа
                p[k] = new Point(Convert.ToDouble(st[0]), Convert.ToDouble(st[1]));
            }

            fs.Close();
            return p;
        }

        /// <summary>
        /// Массив точек, через которые проходит функция, по массиву абцисс эти точек
        /// </summary>
        /// <param name="f"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Point[] Points(RealFunc f, double[] c)
        {
            Point[] p = new Point[c.Length];
            for (int i = 0; i < c.Length; i++)
            {
                p[i] = new Point(c[i], f(c[i]));
            }

            return p;
        }

        /// <summary>
        /// Генерация массива точек по списку точек
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        public static Point[] Points(List<Point> L)
        {
            Point[] P = new Point[L.Count];
            for (int i = 0; i < P.Length; i++)
            {
                P[i] = new Point(L[i]);
            }

            return P;
        }

        //то же самое, только отдельными массивами выводятся первые и вторые координаты точек (сделано для рисования в Chart)
        public static double[] PointsX(RealFunc f, int n, double a, double b)
        {
            Point[] p = new Point[Point.Points(f, n, a, b).Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point(Point.Points(f, n, a, b)[i]);
            }

            double[] x = new double[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                x[i] = p[i].x;
            }

            return x;
        }

        public static double[] PointsX(RealFunc f, double h, double a, double b)
        {
            Point[] p = new Point[Point.Points(f, h, a, b).Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point(Point.Points(f, h, a, b)[i]);
            }

            double[] x = new double[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                x[i] = p[i].x;
            }

            return x;
        }

        public static double[] PointsY(RealFunc f, int n, double a, double b)
        {
            Point[] p = new Point[Point.Points(f, n, a, b).Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point(Point.Points(f, n, a, b)[i]);
            }

            double[] y = new double[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                y[i] = p[i].y;
            }

            return y;
        }
        public static double[] PointsY(RealFunc f, double h, double a, double b)
        {
            Point[] p = new Point[Point.Points(f, h, a, b).Length];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point(Point.Points(f, h, a, b)[i]);
            }

            double[] y = new double[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                y[i] = p[i].y;
            }

            return y;
        }
        /// <summary>
        /// Показать массив точек на консоли
        /// </summary>
        /// <param name="f"></param>
        public static void Show(Point[] f)
        {
            //for (int i = 0; i < f.Length; i++) Console.Write("{0} \t", f[i].x); Console.WriteLine();
            //for (int i = 0; i < f.Length; i++) Console.Write("{0} \t", f[i].y); Console.WriteLine();
            for (int i = 0; i < f.Length; i++)
            {
                Console.WriteLine(f[i].ToString());
            }
        }

        public object Clone()
        {
            //throw new NotImplementedException();
            return (object)new Point(this);
        }

        public int CompareTo(object obj)
        {
            Point p = (Point)obj;
            if (this.x < p.x)
            {
                return -1;
            }

            if (this.x == p.x)
            {
                if (this.y < p.y)
                {
                    return -1;
                }

                if (this.y == p.y)
                {
                    return 0;
                }
            }
            return 1;
            //return x.CompareTo(obj);
        }

        public override bool Equals(object obj)
        {
            var point = obj as Point;
            return /*point != null &&*/ x == point.x && y == point.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Показать координаты точки на консоли
        /// </summary>
        public void Show() { Console.WriteLine(this.ToString()); }

        /// <summary>
        /// Строковое изображение точки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0} , {1})", this.x, this.y);
        }
    }
}