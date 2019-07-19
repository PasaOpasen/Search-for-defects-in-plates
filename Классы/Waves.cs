using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using static МатКлассы.Number;
using МатКлассы;
using System.Collections.Generic;

namespace МатКлассы
{
    /// <summary>
    /// Класс, обеспечивающий исследование колебаний
    /// </summary>
    public static class Waves
    {
        /// <summary>
        /// Нормаль
        /// </summary>
        public class Normal2D
        {
            /// <summary>
            /// Вектор нормали
            /// </summary>
            public Point n;
            /// <summary>
            /// Позиция приложения
            /// </summary>
            public Point Position;
            private double coef = 1;

            
            /// <summary>
            /// Создание вектора нормали к точке на окружности
            /// </summary>
            /// <param name="center">Центр окружности</param>
            /// <param name="position">Декартовы координаты точки на окружности</param>
            /// <param name="coefficent">Коэффициент умножения нормали</param>
            public Normal2D(Point center, Point position, double coefficent = 1)
            {
                double dx = position.x - center.x;
                double dy = position.y - center.y;
                double div = Math.Sqrt(dx * dx + dy * dy);
                this.coef = coefficent;
                this.n = new Point(dx / div * coef, dy / div * coef);
                this.Position = new Point(position);
            }
            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="coef"></param>
            /// <param name="normal"></param>
            /// <param name="position"></param>
            public Normal2D(double coef,Point normal,Point position)
            {
                this.n = new Point(normal.x*coef,normal.y*coef);
                this.Position = new Point(position);
                this.coef = coef;
            }

            /// <summary>
            /// Возвращает массив нормалей как массив точек
            /// </summary>
            /// <param name="mas"></param>
            /// <returns></returns>
            public static Point[] NormalsToPoins(Normal2D[] mas)
            {
                Point[] res = new Point[mas.Length];
                for (int i = 0; i < mas.Length; i++)
                    res[i] = new Point(mas[i].n);
                return res;
            }

            /// <summary>
            /// Умножить нормаль на число
            /// </summary>
            /// <param name="s"></param>
            /// <param name="d"></param>
            /// <returns></returns>
            public static Normal2D operator *(Normal2D s, double d) => new Normal2D(s.coef * d, s.n, s.Position);
            public override string ToString()
            {
                return $"({Position.x}; {Position.y}) -> [{n.x}; {n.y}] ";
            }

            /// <summary>
            /// Угол относительно оси Х и точки, из которой исходит нормаль
            /// </summary>
            public double Corner => new Complex(n.x, n.y).Arg;

            public override bool Equals(object obj)
            {
                Normal2D v = (Normal2D)obj;
                return n.Equals(v.n)&&Position.Equals(v.Position);
            }

            public override int GetHashCode()
            {
                var hashCode = 2114770179;
                hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(n);
                hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(Position);
                return hashCode;
            }
        }

        /// <summary>
        /// Окружность (пьезоэлемент)
        /// </summary>
        public class Circle
        {
            /// <summary>
            /// Центр окружности
            /// </summary>
            public Point center;
            /// <summary>
            /// Радиус окружности
            /// </summary>
            public double radius;
            public Circle(Point center, double radius) { this.center = new Point(center); this.radius = radius; }
            public Circle(Circle c) : this(c.center, c.radius) { }

            /// <summary>
            /// Возврат нормали в точке по аргументу
            /// </summary>
            /// <param name="arg"></param>
            /// <returns></returns>
            public Normal2D GetNormal(double arg,double len=1)
            {
                Point pos = new Point(center.x + radius * Math.Cos(arg), center.y + radius * Math.Sin(arg));
                return new Normal2D(center, pos, len);
            }

            /// <summary>
            /// Создержит ли круг точку
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public bool ContainPoint(Point p) => Point.Eudistance(p, this.center) < this.radius;

            /// <summary>
            /// Возвращает массив точек на окружности
            /// </summary>
            /// <param name="args">Углы точек относительно центра окружности и оси X</param>
            /// <param name="weights">Веса точек (по умолчанию единичные)</param>
            /// <returns></returns>
            public Normal2D[] GetNormalsOnCircle(double[] args, double[] weights = null)
            {
                Normal2D[] res = new Normal2D[args.Length];
                for (int i = 0; i < res.Length; i++)
                    res[i] = new Normal2D(this.center, new Point(center.x + radius * Math.Cos(args[i]), center.y + radius * Math.Sin(args[i])), (weights == null) ? 1 : weights[i]);
                return res;
            }
            /// <summary>
            /// Возвращает массив равномерно рассположенных по окружности нормалей
            /// </summary>
            /// <param name="count"></param>
            /// <param name="weights"></param>
            /// <returns></returns>
            public Normal2D[] GetNormalsOnCircle(int count, double[] weights = null)
            {
                if (weights == null)
                {
                    weights = new double[count];
                    double w = 2.0 * Math.PI / count;
                    for (int i = 0; i < count; i++)
                        weights[i] = w;
                }

                double h = 2 * Math.PI / (count /*- 1*/);
                double[] args = new double[count];
                for (int i = 0; i < count; i++)
                    args[i] = i * h;
                //$"{args[0]} {args[count-1]- 2 * Math.PI}".Show();
                //new Vectors(args).Show();
                //new Vectors(weights).Show();

                return GetNormalsOnCircle(args, weights);
            }

            /// <summary>
            /// Записать поле (массивы аргументов x,y) и массивы значений Re ur, Im ur, Abs ur, Re uz, Im uz, Abs us в файл (чтобы потом нарисовать графики)
            /// </summary>
            /// <param name="filename">Имя файла</param>
            /// <param name="title">То, что должно быть позже написано над графиками</param>
            /// <param name="F">Функция (x,y,normal) -> (ur, uz)</param>
            /// <param name="circle">Окружность, относительно которой всё происходит</param>
            /// <param name="x0">Начало отрезка по х</param>
            /// <param name="X">Конец отрезка по х</param>
            /// <param name="xcount">Число точек по х</param>
            /// <param name="y0">Начало отрезка по у</param>
            /// <param name="Y">Конец отрезка по у</param>
            /// <param name="ycount">Число точек по у</param>
            /// <param name="k">Массив для отслеживания прогресса</param>
            public static void FieldToFile(string filename, Func<double, double, Tuple<Complex, Complex>> F, double x0, double X, int xcount, double y0, double Y, int ycount, /*Circle circle,*/ IProgress<int> progress/*ref int[] k*/, System.Threading.CancellationToken token, Func<Point, bool> Filter, string title = "", int normalscount = 100)
            {
                int[] k = new int[xcount * ycount];
                double[] x = Expendator.Seq(x0, X, xcount);
                double[] y = Expendator.Seq(y0, Y, ycount);
                Complex[,] ur = new Complex[xcount, ycount], uz = new Complex[xcount, ycount];
                //Point[] Nmas = Waves.Normal2D.NormalsToPoins(circle.GetNormalsOnCircle(normalscount));

                //нахождение массивов
                Parallel.For(0, xcount, (int i) =>
                {
                    // for(int i=0;i<xcount;i++)
                    for (int j = 0; j < ycount; j++)
                    {
                        if (token.IsCancellationRequested) return;
                        if (!Filter(new Point(x[i], y[j])))//больше или равно, потому что в массивах изначально нули
                        {
                            var tmp = F(x[i], y[j]);//if (Double.IsNaN((tmp.Item1 + tmp.Item2).Abs) || Double.IsInfinity((tmp.Item1 + tmp.Item2).Abs)) tmp = new Tuple<Complex, Complex>(0, 0);
                            ur[i, j] = new Complex(tmp.Item1);
                            uz[i, j] = new Complex(tmp.Item2);
                        }
                        else//иначе типа NA
                        {
                            ur[i, j] = new Complex(Double.NaN);
                            uz[i, j] = new Complex(Double.NaN);
                        }
                        k[(i) * ycount + j] = 1;

                        progress.Report(k.Sum());
                    }
                });
                //-------------------------------------------------------------------------------------
                //запись в файлы
                StreamWriter fs = new StreamWriter(filename);
                string se = filename.Substring(0, filename.Length - 4);//-.txt
                StreamWriter ts = new StreamWriter(se + "(title).txt");
                //StreamWriter ds = new StreamWriter(se + "(dim).txt");
                StreamWriter xs = new StreamWriter(se + "(x).txt");
                StreamWriter ys = new StreamWriter(se + "(y).txt");

                ts.WriteLine($"{title}");
                //fs.WriteLine($"dim {xcount} {ycount}");

                xs.WriteLine("x");
                for (int i = 0; i < xcount; i++)
                    xs.WriteLine(x[i]);

                ys.WriteLine("y");
                for (int i = 0; i < ycount; i++)
                    ys.WriteLine(y[i]);

                fs.WriteLine("urRe urIm urAbs uzRe uzIm uzAbs");
                for (int i = 0; i < xcount; i++)
                    for (int j = 0; j < ycount; j++)
                        if (Double.IsNaN(ur[i, j].Abs) || Double.IsNaN(uz[i, j].Abs))
                            fs.WriteLine("NA NA NA NA NA NA");
                        else
                            fs.WriteLine($"{ur[i, j].Re} {ur[i, j].Im} {ur[i, j].Abs} {uz[i, j].Re} {uz[i, j].Im} {uz[i, j].Abs}");

                fs.Close();
                ts.Close();
                xs.Close();
                ys.Close();
            }

            /// <summary>
            /// Записать поле (массивы аргументов x,y) и массивы значенийur, ur, uz в файл (чтобы потом нарисовать графики)
            /// </summary>
            /// <param name="filename">Имя файла</param>
            /// <param name="title">То, что должно быть позже написано над графиками</param>
            /// <param name="F">Функция (x,y,normal) -> (ur, uz)</param>
            /// <param name="x0">Начало отрезка по х</param>
            /// <param name="X">Конец отрезка по х</param>
            /// <param name="xcount">Число точек по х</param>
            /// <param name="y0">Начало отрезка по у</param>
            /// <param name="Y">Конец отрезка по у</param>
            /// <param name="ycount">Число точек по у</param>
            /// <param name="k">Массив для отслеживания прогресса</param>
            public static void FieldToFile(string filename, Func<double, double,Tuple<double, double>> F, double x0, double X, int xcount, double y0, double Y, int ycount, IProgress<int> progress, System.Threading.CancellationToken token, Func<Point, bool> Filter, string title = "",bool parallel=true)
            {
                int[] k = new int[xcount * ycount];
                double[] x = Expendator.Seq(x0, X, xcount);
                double[] y = Expendator.Seq(y0, Y, ycount);
                double[,] ur = new double[xcount, ycount], uz = new double[xcount, ycount];

                //нахождение массивов
                if(parallel)
                Parallel.For(0, xcount, (int i) =>
                {
                    for (int j = 0; j < ycount; j++)
                    {
                        if (token.IsCancellationRequested) return;
                        if (!Filter(new Point(x[i], y[j])))//больше или равно, потому что в массивах изначально нули
                        {
                            var tmp = F(x[i], y[j]);
                            ur[i, j] = tmp.Item1;
                            uz[i, j] = tmp.Item2;
                        }
                        else//иначе типа NA
                        {
                            ur[i, j] = Double.NaN;
                            uz[i, j] = Double.NaN;
                        }
                        k[(i) * ycount + j] = 1;

                        progress.Report(k.Sum());
                    }
                });
                else
                    for(int i=0;i<xcount;i++)
                    {
                        for (int j = 0; j < ycount; j++)
                        {
                            if (token.IsCancellationRequested) return;
                            if (!Filter(new Point(x[i], y[j])))//больше или равно, потому что в массивах изначально нули
                            {
                                var tmp = F(x[i], y[j]);
                                ur[i, j] = tmp.Item1;
                                uz[i, j] = tmp.Item2;
                            }
                            else//иначе типа NA
                            {
                                ur[i, j] = Double.NaN;
                                uz[i, j] = Double.NaN;
                            }
                            k[(i) * ycount + j] = 1;

                            progress.Report(k.Sum());
                        }
                    }

                //-------------------------------------------------------------------------------------
                //запись в файлы
                StreamWriter fs = new StreamWriter(filename);
                string se = filename.Substring(0, filename.Length - 4);//-.txt
                StreamWriter ts = new StreamWriter(se + "(title).txt");
                //StreamWriter ds = new StreamWriter(se + "(dim).txt");
                StreamWriter xs = new StreamWriter(se + "(x).txt");
                StreamWriter ys = new StreamWriter(se + "(y).txt");
                StreamWriter info = new StreamWriter(se + "(info).txt");
                //info.WriteLine($"ur(x,{t})");


                ts.WriteLine($"{title}");
                //fs.WriteLine($"dim {xcount} {ycount}");

                xs.WriteLine("x");
                for (int i = 0; i < xcount; i++)
                    xs.WriteLine(x[i]);

                ys.WriteLine("y");
                for (int i = 0; i < ycount; i++)
                    ys.WriteLine(y[i]);

                fs.WriteLine("ur uz");
                for (int i = 0; i < xcount; i++)
                    for (int j = 0; j < ycount; j++)
                        if (Double.IsNaN(ur[i, j].Abs()) || Double.IsNaN(uz[i, j].Abs()))
                            fs.WriteLine("NA NA");
                        else
                            fs.WriteLine($"{ur[i, j]} {uz[i, j]}");

                fs.Close();
                ts.Close();
                xs.Close();
                ys.Close();
                info.Close();
            }
        }
    

        /// <summary>
        /// Окружность с вырезом, представимым как круг с центром на большой окружности
        /// </summary>
        public class DCircle
        {

            public static DCircle Example = new DCircle(new Point(1, 1), 15, 5);

            /// <summary>
            /// Окружности
            /// </summary>
            private Circle circle1, circle2;
            /// <summary>
            /// Аргумент, определяющий положение центра меньшей окружности
            /// </summary>
            private double arg;
            /// <summary>
            /// Центральные половинные углы окружностей в радианах
            /// </summary>
            private double alp1, alp2;

            public int n1=90, n2=30;

            public double Radius => circle1.radius;
            public Point Center => circle1.center;
            public Circle BigCircle => new Circle(circle1);

            public Tuple<double, double, double> DiamsAndArg => new Tuple<double, double, double>(circle1.radius*2,circle2.radius*2,arg);

            /// <summary>
            /// Окружность с вырезом
            /// </summary>
            /// <param name="center">Центр большей окружности</param>
            /// <param name="diam1">Диамерт большей окружности</param>
            /// <param name="diam2">Диаметр меньшей окружности</param>
            /// <param name="arg">Угол в радианах, определяющий положение центра меньшей окружности</param>
            public DCircle(Point center, double diam1=1.6,double diam2=0.5, double arg = 4.8,int n1=90,int n2=30)
            {                
                this.arg = arg;

                double r1 = diam1 / 2, r2 = diam2 / 2;
                circle1 = new Circle(center, r1);
                circle2 = new Circle(new Point(/*-*/center.x + r1 /** Math.Cos(arg)*/, /*-*/center.y + 0/*r1 * Math.Sin(arg)*/), r2);

                double tmp = r2 / r1;
                alp1 = Math.Acos(1.0-0.5*tmp*tmp);//alp1.Show();
                alp2 = Math.Acos(0.5 * tmp);//alp2.Show();

                this.n1 = n1;
                this.n2 = n2;
            }

            public DCircle(DCircle dc)
            {
                this.alp1 = dc.alp1;
                this.arg = dc.arg;
                this.alp2 = dc.alp2;
                this.circle1 = new Circle(dc.circle1);
                this.circle2 = new Circle(dc.circle2);

                this.n1 = dc.n1;
                this.n2 = dc.n2;
            }

            /// <summary>
            /// Содержит ли окружность с вырезом точку в своей внутренности
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public bool ContainPoint(Point p)
            {
                p = p.Turn(circle1.center, -arg);
                return Point.Eudistance(circle1.center, p) < circle1.radius && Point.Eudistance(circle2.center, p) > circle2.radius;
            }

            /// <summary>
            /// Возвращает нормаль для точки на плоскости
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public Point GetNormal(Point p,double len=0.1,double eps=0.01)
            {
                p = p.Turn(circle1.center,-arg);
                Point nil = new Point(0);
                double corner = new Complex(p.x-circle1.center.x, p.y-circle1.center.y).Arg;//corner.Show();
                if (corner < -alp1-eps || corner > alp1+eps) 
                    return circle1.GetNormal(corner,len).n.Turn(nil, arg);//sec1.Show();sec2.Show();

                double d1 = Point.Eudistance(circle1.center, p); //circle2.center.Show();
                double d2 = Point.Eudistance(circle2.center, p);

                if (d2 == 0) return new Point(Math.Cos(arg)*len, Math.Sin(arg)*len);
                Normal2D res=new Normal2D(circle2.center, p,len);//res.n.Show(); circle1.radius.Show();
                if (d1 > circle1.radius+eps)
                    return res.n.Turn(nil, arg);
                return (res * (-1)).n.Turn(nil, arg);
            }

            /// <summary>
            /// Возвращает массив точек и массив нормалей в этих точках
            /// </summary>
            /// <param name="n1">Число точек на большей окружности</param>
            /// <param name="n2">Число точек на меньшей окружности</param>
            /// <returns></returns>
            public Tuple<Point[],Point[]> DrawMasses(int n1=100, int n2 = 10,double len=0.1,double eps=0.00001)
            {
                double h1 = (Math.PI * 2 - 2 * alp1-2*eps) / (n1 - 1);
                double h2 = (2 * alp2) / (n2 - 1),tmp;

                Point[] p = new Point[n1 + n2];
                Point[] n = new Point[n1 + n2];
                for(int i = 0; i < n1; i++)
                {
                    tmp = alp1 + eps + i * h1;
                    p[i] = new Point(circle1.radius * Math.Cos(tmp)+circle1.center.x, circle1.radius * Math.Sin(tmp)+circle1.center.y).Turn(circle1.center, arg);
                    n[i] = GetNormal(p[i],0.05*circle1.radius);
                }
                for(int i = 0; i < n2; i++)
                {
                    tmp =-Math.PI+ alp2 - i * h2;
                    p[n1 + i] = new Point(circle2.radius * Math.Cos(tmp) + circle2.center.x, circle2.radius * Math.Sin(tmp)+ circle2.center.y).Turn(circle1.center, arg);
                    n[n1 + i] = GetNormal(p[n1 + i],0.2*circle2.radius);
                }
                return new Tuple<Point[], Point[]>(p, n);
            }

            /// <summary>
            /// Возвращает массив точек и массив нормалей в этих точках
            /// </summary>
            /// <returns></returns>
            public Normal2D[] GetNormalsOnDCircle(double eps=0.01)
            {
                double m1 = 2 * Math.PI * circle1.radius / n1;
                double m2 = 0.85 * 2 * circle2.radius*alp2 / n2;

                Debug.WriteLine($"m1 = {m1} m2 = {m2}");

                double h1 = (Math.PI * 2 - 2 * alp1 - 2 * eps) / (n1 - 1);
                double h2 = (2 * alp2) / (n2 - 1), tmp;

                Point[] p = new Point[n1 + n2];
                Point[] n = new Point[n1 + n2];
                Normal2D[] res = new Normal2D[n1 + n2];
                for (int i = 0; i < n1; i++)
                {
                    tmp = alp1 + eps + i * h1;
                    p[i] = new Point(circle1.radius * Math.Cos(tmp) + circle1.center.x, circle1.radius * Math.Sin(tmp) + circle1.center.y).Turn(circle1.center, arg);
                    n[i] = GetNormal(p[i],1);
                    res[i] = new Normal2D(m1, n[i], p[i]);
                }
                for (int i = 0; i < n2; i++)
                {
                    tmp = -Math.PI + alp2 - i * h2;
                    p[n1 + i] = new Point(circle2.radius * Math.Cos(tmp) + circle2.center.x, circle2.radius * Math.Sin(tmp) + circle2.center.y).Turn(circle1.center, arg);
                    n[n1 + i] = GetNormal(p[n1 + i], 1);
                    res[n1 + i] = new Normal2D(m2, n[n1 + i], p[n1 + i]);
                }
                return res;
            }
        }

        /// <summary>
        /// Поворот точки на угол
        /// </summary>
        /// <param name="p"></param>
        /// <param name="corner"></param>
        /// <returns></returns>
        public static Point Turn(this Point p,Point center,double corner)
            {
                double sin = Math.Sin(corner);
                double cos = Math.Cos(corner);
            Point t = new Point(p.x - center.x, p.y - center.y);
                return new Point(t.x * cos - t.y * sin+ center.x, t.x * sin + t.y * cos+ center.y);
            }
    }
}