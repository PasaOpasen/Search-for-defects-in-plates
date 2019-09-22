using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods;
using МатКлассы;

namespace МатКлассы
{
    /// <summary>
    /// Класс с вейвлетным преобразованием
    /// </summary>
    public sealed class Wavelet: IDisposable
    {
        private static readonly double sqrt2pi = Math.Sqrt(2 * Math.PI);
        private static readonly double frac1sqrt2pi = 1.0 / sqrt2pi;
        private static readonly Func<Complex, Complex> sigma =
            (Complex z) => SumsAndLimits.Sum(1, n => 
            {
                int nsqr = n * n;
                return Complex.Sin(Math.PI * z * nsqr) / nsqr;
                }, eps);
        public static DefInteg.GaussKronrod.NodesCount countNodes = DefInteg.GaussKronrod.NodesCount.GK61;

        /// <summary>
        /// Масштабный множитель
        /// </summary>
        public double k;
        /// <summary>
        /// Частота
        /// </summary>
        public double w;
        /// <summary>
        /// Материнский вейвлет
        /// </summary>
        private Func<double,Complex> Mother;
        /// <summary>
        /// Фурье-образ материнского вейвлета
        /// </summary>
        private Func<Complex,Complex> FMother;
        /// <summary>
        /// Тип исходного вейвлета
        /// </summary>
        public Wavelets Type { get; }

        /// <summary>
        /// Допустимая погрешность
        /// </summary>
        public static double eps = 1e-15;

        /// <summary>
        /// Перечисление доступных вейвлетов
        /// </summary>
        public enum Wavelets : byte
        {
            /// <summary>
            /// Гауссов вейвлет первого порядка
            /// </summary>
            WAVE,
            /// <summary>
            /// Мексиканская шляпа
            /// </summary>
            MHAT,
            /// <summary>
            /// "difference of gaussians"
            /// </summary>
            DOG,
            /// <summary>
            /// "Littlewood & Paley"
            /// </summary>
            LP,
            /// <summary>
            /// Хаар-вейвлет
            /// </summary>
            HAAR,
            /// <summary>
            /// Французская шляпа
            /// </summary>
            FHAT,
            /// <summary>
            /// Вейвлет Морле
            /// </summary>
            Morlet
        }

        /// <summary>
        /// Создание вейвлета по масштабному множителю с указанием вейвлета из перечисления
        /// </summary>
        /// <param name="W"></param>
        /// <param name="ww"></param>
        /// <param name="k"></param>
        public Wavelet(Wavelets W = Wavelets.MHAT, double k = -1, double ww = 1)
        {
            this.w = ww;
            this.k = k;
            this.Type = W;
            switch (W)
            {
                case Wavelets.WAVE:
                    this.Mother = (double t) => -t * Math.Exp(-t*t / 2);
                    this.FMother = (Complex w) => Complex.I * w * sqrt2pi * Complex.Exp(-w * w / 2);
                    break;
                case Wavelets.MHAT:
                    this.Mother = (double t) => { double sqr = t*t; return (1 - sqr) * Math.Exp(-sqr / 2); };
                    this.FMother = (Complex w) =>
                    {
                        var sqr = -w * w;
                        return sqr * sqrt2pi * Complex.Exp(sqr / 2);
                    };
                        break;
                case Wavelets.DOG:
                    this.Mother = (double t) => { double sqr = -t*t/2; return Math.Exp(sqr) - 0.5 * Math.Exp(sqr / 4); };
                    this.FMother = (Complex w) =>
                    {
                        var sqr = -w * w;
                        return sqrt2pi * (Complex.Exp(sqr / 2) - Complex.Exp(2 * sqr));
                    };
                    break;
                case Wavelets.LP:
                    this.Mother = t => { double pt = t * Math.PI; return (Math.Sin(2 * pt) - Math.Sin(pt)) / pt; };
                    this.FMother = (Complex w) => 
                    {
                        double tmp = w.Abs;
                        if (tmp <= 2 * Math.PI && tmp >= Math.PI)
                            return frac1sqrt2pi;
                        return 0;
                    };
                    break;
                case Wavelets.HAAR:
                    this.Mother = t =>
                    {
                        if (t >= 0)
                        {
                            if (t <= 0.5) return 1;
                            if (t <= 1) return -1;
                            return 0;
                        }
                        return 0;
                    };
                    this.FMother = (Complex w) => 4 * Complex.I * Complex.Exp(Complex.fracI2 * w) / w * Complex.Sin(w / 4).Sqr();
                    break;
                case Wavelets.FHAT:
                    this.Mother = t =>
                    {
                        double q = t.Abs();
                        if (q <= 1.0 / 3) return 1;
                        if (q <= 1) return -0.5;
                        return 0;
                    };
                    this.FMother = (Complex w) => 4 * Complex.Sin(w / 3).Pow(3) / w;
                    break;
                case Wavelets.Morlet:
                    this.Mother = t => Math.Exp(-t.Sqr() / 2) * Complex.Exp(Complex.I * w * t);
                    this.FMother = (Complex w) => sigma(w) * sqrt2pi * Complex.Exp(-(w - this.w).Sqr() / 2);
                    break;
            }
        }
        /// <summary>
        /// Создать вейвлет
        /// </summary>
        /// <param name="W"></param>
        /// <param name="k"></param>
        /// <param name="ww"></param>
        /// <returns></returns>
        public static Wavelet Create(Wavelets W = Wavelets.MHAT, double k = -1, double ww = 1) => new Wavelet(W, k, ww);

        /// <summary>
        /// Функция, получившаяся при последнем анализе 
        /// </summary>
        public Func<double,double,Complex> ResultMemoized = null;
        private Memoize<Point, Complex> Resultmems = null;

        /// <summary>
        /// Вейвлет-образ указанной функции
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public Func<double,double,Complex> GetAnalys(Func<double,double> f)
        {
            Func<double,double,Complex> s = (double a, double b) =>
             {                
                 Func<Complex,Complex> F1 = (Complex t) => f(t.Re) * this.Mother((t.Re - b) / a).Conjugate;
                 Func<Complex,Complex> F2 = (Complex t) => f(-t.Re) * this.Mother((-t.Re - b) / a).Conjugate;
                 double con = 1.0 / Math.Sqrt(Math.Abs(a));
                 Complex 
                 t1 = DefInteg.GaussKronrod.DINN_GK(F1, 0, 0, 0, 0, 0, 0, eps: eps, nodesCount: countNodes), 
                 t2 = DefInteg.GaussKronrod.DINN_GK(F2, 0, 0, 0, 0, 0, 0, eps: eps, nodesCount: countNodes);

                 return con * (t1 + t2);
             };
            Resultmems = new Memoize<Point, Complex>((Point p) => s(p.x, p.y));
            ResultMemoized = new Func<double,double,Complex>((double a, double b) => Resultmems.Value(new Point(a, b)));
            return ResultMemoized;
        }
        /// <summary>
        /// Обратное вейвлет-преобразование указанной функции
        /// </summary>
        /// <param name="F"></param>
        /// <returns></returns>
        public Func<double,double> GetSyntesis(Func<double,double,Complex> F = null)
        {
            //вычисление коэффициента С
            //надо добавить какое-нибудь ограничение на <inf 
            Complex C;
            if (this.Type == Wavelets.LP) C = Math.Log(2) / Math.PI;
            else
                C = DefInteg.GaussKronrod.DINN_GKwith0Full((Complex w) =>
                {
                    if (w == 0) return 0;
                    return this.FMother(w).Abs.Sqr() / w.Abs;
                }, 
                eps: eps, nodesCount:countNodes);
            C *= Math.Sqrt(2);

            Func<double,double> GetRes(Func<Point,Complex> func)=> 
                (double t) => 
                (DefInteg.DoubleIntegralIn_FULL(
                    (Point p) => (this.Mother((t - p.y) / p.x) * func(p) / p.x / p.x).Re, 
                    eps: eps, parallel: true, M: DefInteg.Method.GaussKronrod61, changestepcount: 0, a: 1, b: 10) / C).Re;

            //задание промежуточных переменных
            if (F != null)
            {
                Memoize<Point, Complex> f = new Memoize<Point, Complex>((Point p) => F(p.x, p.y));

                return GetRes(f.Value);
            }
            else
                return GetRes(p => ResultMemoized(p.x, p.y));
        }



        public void Dispose()
        {
            Resultmems.Dispose();
        }
    }
}