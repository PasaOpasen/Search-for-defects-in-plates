using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace МатКлассы
{
    /// <summary>
    /// Реализация алгоритма роя частиц и подобных
    /// </summary>
    public static class BeeHiveAlgorithm
    {
        /// <summary>
        /// Многомерная парабола
        /// </summary>
        public static readonly Func<Vectors, double> Parabol = (Vectors v) =>v.DistNorm; 
        /// <summary>
        /// Функция Растригина
        /// </summary>
        public static readonly Func<Vectors, double> Rastr = (Vectors v) => {
            double s = 10 * v.Deg;
            for (int i = 0; i < v.Deg; i++)
                s += v[i] * v[i] - 10 * Math.Cos(2 * Math.PI * v[i]);

            return s;
        };
        /// <summary>
        /// Функция Швеля
        /// </summary>
        public static readonly Func<Vectors, double> Shvel = (Vectors v) => {
            double s = 0;
            for (int i = 0; i < v.Deg; i++)
                s += -v[i] * Math.Sin(Math.Sqrt(v[i].Abs()));

            return s;
        };

        /// <summary>
        /// Параметры шага для роя
        /// </summary>
        //tex:Каждая частица в рое делает примерно следующий шаг: $v_{i+1} = w v_i + \varphi_p \cdot random_1 (p_i - x_i) + \varphi_g \cdot random_2 (g_i - x_i)$
        public static double w=0.3, fp=2, fg=5;

        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="n">Размерность области определения целевой функции</param>
        /// <param name="min">Минимальное возможное значение каждого аргумента</param>
        /// <param name="max">Максимальное возможное значение каждого аргумента</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static Tuple<Vectors,double> GetGlobalMin(Func<Vectors,double> f,int n=1,double min=-1e12,double max=1e12,double eps=1e-10,int countpoints=1000,int maxcountstep = 100,Vectors center=null,int maxiter=150)
        {
            Vectors minimum = new Vectors(n, min);
            Vectors maximum = new Vectors(n, max);
           
            Hive hive;
                if(center==null) hive=new Hive(minimum, maximum, f, countpoints);
                else hive = new Hive(minimum+center, maximum+center, f, countpoints,center);

            return Gets(hive, eps, maxcountstep, maxiter);

        }
        /// <summary>
        /// Получить минимум функции, посчитанный роевым методом
        /// </summary>
        /// <param name="f">Целевая функция</param>
        /// <param name="n">Размерность области определения целевой функции</param>
        /// <param name="min">Минимальное возможное значение каждого аргумента</param>
        /// <param name="max">Максимальное возможное значение каждого аргумента</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="countpoints">Количество пчёл в рое</param>
        /// <param name="maxcountstep">Максимальное число итераций метода</param>
        /// <returns></returns>
        public static Tuple<Vectors, double> GetGlobalMin(Func<Vectors, double> f, Vectors minimum,Vectors maximum, double eps = 1e-10, int countpoints = 1000, int maxcountstep = 100, int maxiter = 150)
        {
            return Gets(new Hive(minimum , maximum , f, countpoints),eps,maxcountstep,maxiter);
        }
        public static Tuple<Vectors, double> Gets(Hive hive, double eps = 1e-10, int maxcountstep = 100, int maxiter = 150)
        {
            if (maxiter <= 0) maxiter = Int32.MaxValue;
            double e = hive.val;
            int c = maxcountstep,k=0;

            Debug.WriteLine($"Погрешность после инициализации пчёл:  {e}");
            while (e>eps && maxcountstep > 0 && hive.Radius>eps)
            {
                hive.MakeStep(w,fp,fg);
                k++;
                if (hive.val < e)
                {
                    Debug.WriteLine($"Hive method (iter {k}):  {e} ---> {hive.val}");
                    e = hive.val;                   
                    maxcountstep = c;
                }
                else
                maxcountstep--;
                //Debug.WriteLine( $"c = {maxcountstep}  val = {hive.val}");
                if (k == maxiter) break;
            }
            return new Tuple<Vectors, double>(hive.g, hive.val);
        }

        /// <summary>
        /// Рой пчёл
        /// </summary>
        public sealed class Hive
        {
            /// <summary>
            /// Массив пчёл
            /// </summary>
            Bee[] bees;
            /// <summary>
            /// Наилучшее положение в рое
            /// </summary>
            public Vectors g { get;private set;  }
            /// <summary>
            /// Значение целевой функции в наилучшем положении
            /// </summary>
            public double val { get; private set; }

            /// <summary>
            /// Радиус роя как наибольшее расстояние между наилучшим положением в рое и наилучшими положениями отдельных частиц
            /// </summary>
            public double Radius
            {
                get
                {
                    double d = 0,di;
                    for(int i=0;i<bees.Length;i++)
                    {
                        di = (g - bees[i].p).EuqlidNorm;
                        if (di > d)
                            d = di;
                    }
                    return d;
                }
            }
            private Func<Vectors, double> func;

            /// <summary>
            /// Попытаться обновить наилучшее положение
            /// </summary>
            /// <param name="gnew"></param>
            public void UpdateG(Vectors gnew)
            {
                double v = func(gnew);
                if (v < val)
                {
                    val = v;
                    g = gnew.dup;
                }
            }

            /// <summary>
            /// Сгенерировать рой частиц
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            /// <param name="count"></param>
            /// <param name="v"></param>
            public Hive(Vectors min, Vectors max, Func<Vectors, double> f,int count = 1000,params Vectors[] v)
            {
                this.func = new Func<Vectors, double>(f);

                bees = new Bee[count+v.Length];
                //for (int i = 0; i < count; i++)
                    Parallel.For(0, count, (int i) => 
                    { 
                    bees[i] = new Bee(min, max, f);
                    });

                for (int i = count; i < count + v.Length; i++)
                    bees[i] = new Bee(v[i-count],min,max,f);

                g = bees[0].p.dup;
                val = bees[0].bestval;
                ReCount();
            }

            private void ReCount()
            {
                for(int i=0;i<bees.Length;i++)
                    if (bees[i].bestval < val)
                    {
                        val = bees[i].bestval;
                        g = bees[i].p.dup;
                    }
            }

            /// <summary>
            /// Сделать шаг по дискретному времени
            /// </summary>
            /// <param name="w"></param>
            /// <param name="fp"></param>
            /// <param name="fg"></param>
            /// <param name="parallel"></param>
            public void MakeStep(double w=0.3, double fp=2, double fg=5,bool parallel=true)
            {
                Parallel.For(0, bees.Length, (int i) => { 
                //for(int i=0;i<bees.Length;i++)
                //{
                    bees[i].RecalcV(w, fp, fg, this.g);
                    bees[i].Move();
                //}
                });
                if (!parallel)
                    for (int i = 0; i < bees.Length; i++)
                    {
                        bees[i].ReCount();
                    }
                else
                    Parallel.For(0, bees.Length, (int i) => {
                        bees[i].ReCount();
                    });

                ReCount();
            }
        }

        /// <summary>
        /// Классы пчелы
        /// </summary>
        public sealed class Bee
        {
            /// <summary>
            /// Текущее положение частицы
            /// </summary>
            Vectors x;
            /// <summary>
            /// Наилучшее положение частицы
            /// </summary>
            public Vectors p { get; private set; }
            /// <summary>
            /// Текущая скорость частицы
            /// </summary>
            Vectors v;

            /// <summary>
            /// Значение целевой функции в наилучшем положении
            /// </summary>
            public double bestval { get; private set; }
            /// <summary>
            /// Целевая функция
            /// </summary>
            Func<Vectors, double> f;

            /// <summary>
            /// Создать частицу в окне решений
            /// </summary>
            /// <param name="min">Минимальные возможные значения положения</param>
            /// <param name="max">Максимальные возможные значения положения</param>
            /// <param name="f">Целевая функция</param>
            public Bee(Vectors min,Vectors max, Func<Vectors, double> f)
            {
                var r = new MathNet.Numerics.Random.CryptoRandomSource();

                x = new Vectors(min);
                for (int i = 0; i < x.Deg; i++)
                {
                  x[i] += r.NextDouble() * (max[i] - min[i]); 
                }
               // v = null;f = null;bestval = double.MaxValue;p = null;  

                WhenX(min, max, f);
            }

            /// <summary>
            /// Задать пчелу по известному начальному положению
            /// </summary>
            /// <param name="x"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            public Bee(Vectors x, Vectors min, Vectors max, Func<Vectors, double> f)
            {
               this.x = x.dup;
                WhenX(min, max, f);
            }
            /// <summary>
            /// Задать наилучшее положение и случайные скорости, когда x уже известно
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="f"></param>
            public void WhenX(Vectors min, Vectors max, Func<Vectors, double> f)
            {
                var r = new MathNet.Numerics.Random.CryptoRandomSource();
                p = new Vectors(x);
                this.f = new Func<Vectors, double>(f);
                bestval = f(p);

                Vectors vmax = (max - min), vmin = -vmax;
                v = new Vectors(vmin);
                for (int i = 0; i < x.Deg; i++)
                    v[i] += r.NextDouble() * (vmax[i] - vmin[i]);
            }

            /// <summary>
            /// Переопределить скорость
            /// </summary>
            /// <param name="w">Коэффициент инерции</param>
            /// <param name="fp">Весовой коэффициент для p</param>
            /// <param name="fg">Весовой коэффициент для g</param>
            /// <param name="g">Наилучшее положение по рою</param>
            public void RecalcV(double w, double fp,double fg,Vectors g)
            {
                var r = new MathNet.Numerics.Random.CryptoRandomSource();
                double fi = fg + fp;

                Vectors rp=new Vectors(v.Deg), rg = new Vectors(v.Deg);
                for(int i=0;i<v.Deg;i++)
                {
                    rp[i] = r.NextDouble();
                    rg[i] = r.NextDouble();
                }

                v =2*w * (v + fp * Vectors.CompMult(rp, p - x) + fg * Vectors.CompMult(rg, g - x))/Math.Abs(2-fi-Math.Sqrt(fi*(fi-4)));
            }

            /// <summary>
            /// Сделать шаг по скорости
            /// </summary>
            public void Move()
            {
                x =x+v;
            }

            /// <summary>
            /// Переопределить наилучшее положение частицы, если можно
            /// </summary>
            public void ReCount()
            {
                double t = f(x);
                if(t<bestval)
                {
                    bestval = t;
                    p = x.dup;
                }
            }
        }


        /// <summary>
        /// Оптимизация методом пчелиной колонии
        /// </summary>
        /// <param name="f">Оптимизируемая функция</param>
        /// <param name="min">Нижняя граница области решений</param>
        /// <param name="max">Верхняя граница области решений</param>
        /// <param name="n">Размерность области решений</param>
        /// <param name="s">Общее число пчёл</param>
        /// <param name="p">Число пчёл, выбранных для последующего исследования (p меньше s)</param>
        /// <param name="e">Число особо исследуемых пчёл (e меньше p)</param>
        /// <param name="sp">Число вспомогательных пчёл для пчёл из p</param>
        /// <param name="se">Число вспомогательных пчёл для пчёл из e</param>
        /// <param name="delta">Радиус окрестности</param>
        /// <param name="eps">Допустимая погрешность</param>
        /// <param name="maxcount">Максимальное число итераций</param>
        /// <returns></returns>
        public static Tuple<Vectors, double> GetGlobalMin(Func<Vectors, double> f, Vectors min, Vectors max,int n = 1, int s = 1000,int p=300,int e=100,int sp=50,int se=100,double delta=1.0,  double eps = 1e-10, int maxcount = 10)
        {
            SBee[] mas = SBee.Create(f, min, max, s);
            int k = 0;

            while(SBee.GetBest(mas).v>eps && maxcount > 0 && k<3)
            {
                double old = SBee.GetBest(mas).v;
                SBee.MakeStep(ref mas, f, min, max, n, p, e, sp, se, delta);
                double now = SBee.GetBest(mas).v;
                if (now < old)
                {
                    Debug.WriteLine($"\tColony: {old} ---> {now}");
                    k = 0;
                }
                else k++;
                

                maxcount--;
            }

            SBee res = SBee.GetBest(mas);
            return new Tuple<Vectors, double>(res.x, res.v);
        }

        /// <summary>
        /// Класс упрощённой пчелы
        /// </summary>
        private class SBee : IComparable
        {
            public Vectors x { get; private set; }
            public double v { get; private set; }

            public SBee(Vectors vec,double f)
            {
                v = f;
                x = vec.dup;
            }

            public SBee(Vectors vec, Func<Vectors, double> f) : this(vec, f(vec)) { }

            public SBee(SBee be) : this(be.x, be.v) { }

            public int CompareTo(object obj)
            {
                return v.CompareTo(((SBee)(obj)).v);
            }

            /// <summary>
            /// Наилучшая пчела в массиве (у которой наименьшее значение)
            /// </summary>
            /// <param name="mas"></param>
            /// <returns></returns>
            public static SBee GetBest(SBee[] mas)
            {
                int i = -1;
                double d = Double.MaxValue;
                for(int k=0;k<mas.Length;k++)
                    if (mas[k].v < d)
                    {
                        d = mas[k].v;
                        i = k;
                    }
                return mas[i];
            }

            /// <summary>
            /// Получить случайный массив упрощённых пчёл
            /// </summary>
            /// <param name="f"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="count">Число пчёл</param>
            /// <returns></returns>
            public static SBee[] Create(Func<Vectors, double> f, Vectors min, Vectors max,int count=100,bool withaverage=true)
            {

                SBee[] res = new SBee[count];
                Vectors[] tmp = new Vectors[count];

                Parallel.For(0, count, (int i) => {
                    tmp[i] = Vectors.Create(min, max);
 });

                for (int i = 0; i < count; i++)
                {
                res[i] = new SBee(tmp[i], f(tmp[i]));
                }

                if (withaverage)
                    res[res.Length - 1] = new SBee((max + min) / 2, f);

                return res;
            }

            /// <summary>
            /// Сделать шаг по алгоритму пчелиной колонии
            /// </summary>
            /// <param name="mas"></param>
            /// <param name="f"></param>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <param name="n"></param>
            /// <param name="p"></param>
            /// <param name="e"></param>
            /// <param name="sp"></param>
            /// <param name="se"></param>
            /// <param name="delta"></param>
            public static void MakeStep(ref SBee[] mas, Func<Vectors, double> f, Vectors min, Vectors max, int n = 1, int p = 300, int e = 100, int sp = 50, int se = 100, double delta = 1.0)
            {
                Array.Sort(mas);

                Vectors[] t;

                t = new Vectors[se];
                for(int i = 0; i < e; i++)
                {
                    SBee it = new SBee(mas[i]),tmp;
                    Vectors cent = it.x.dup;

                    
                    Parallel.For(0, t.Length, (int u) => {
                        t[u] = Vectors.Create(cent, delta);
                    });

                    for (int j = 0; j < se; j++)
                    {
                        tmp = new SBee(t[j], f);
                        if (tmp.v < it.v)
                            it = new SBee(tmp);
                    }
                    mas[i] = new SBee(it);
                }

                t = new Vectors[sp];
                for (int i = e; i < p; i++)
                {
                    SBee it = new SBee(mas[i]), tmp;
                    Vectors cent = it.x.dup;

                    
                    Parallel.For(0, t.Length, (int u) => {
                        t[u] = Vectors.Create(cent, delta);
                    });

                    for (int j = 0; j < sp; j++)
                    {
                        tmp = new SBee(t[j], f);
                        if (tmp.v < it.v)
                            it = new SBee(tmp);
                    }
                    mas[i] = new SBee(it);
                }



                t = new Vectors[mas.Length - p];
                Parallel.For(0, t.Length, (int i) => {
                    t[i] = Vectors.Create(min, max);
                });

                for (int i = p; i < mas.Length; i++)
                    mas[i] = new SBee(t[i-p], f);

            }
        }
    }
}