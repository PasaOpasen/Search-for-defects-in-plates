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
    public class Wavelet
    {
        private static double sqrt2pi = Math.Sqrt(2 * Math.PI);
        private static readonly ComplexFunc sigma = (Complex z) => SumsAndLimits.Sum(1, n => Complex.Sin(Math.PI * z * n * n) / n / n, eps);

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
        private RToC Mother;
        /// <summary>
        /// Фурье-образ материнского вейвлета
        /// </summary>
        private ComplexFunc FMother;

        private Wavelets type;
        /// <summary>
        /// Тип исходного вейвлета
        /// </summary>
        public Wavelets Type => this.type;

        /// <summary>
        /// Половина длины отрезка интегрирования
        /// </summary>
        private double N = 20;

        public static double eps = 1e-15;

        /// <summary>
        /// Перечисление доступных вейвлетов
        /// </summary>
        public enum Wavelets
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
        /// <param name="w"></param>
        /// <param name="k"></param>
        public Wavelet(Wavelets W = Wavelets.MHAT, double k = -1, double ww = 1)
        {
            this.k = k;
            this.type = W;
            switch (W)
            {
                case Wavelets.WAVE:
                    this.Mother = (double t) => -t * Math.Exp(-t.Sqr() / 2);
                    this.FMother = (Complex w) => Complex.I * w * sqrt2pi * Complex.Exp(-w * w / 2);
                    break;
                case Wavelets.MHAT:
                    this.Mother = (double t) => { double sqr = t.Sqr(); return (1 - sqr) * Math.Exp(-sqr / 2); };
                    this.FMother = (Complex w) => -w * w * sqrt2pi * Complex.Exp(-w * w / 2);
                    break;
                case Wavelets.DOG:
                    this.Mother = (double t) => { double sqr = t.Sqr(); return Math.Exp(-sqr / 2) - 0.5 * Math.Exp(-sqr / 8); };
                    this.FMother = (Complex w) => sqrt2pi * (Complex.Exp(-w * w / 2) - Complex.Exp(-2 * w * w));
                    break;
                case Wavelets.LP:
                    this.Mother = t => { double pt = t * Math.PI; return (Math.Sin(2 * pt) - Math.Sin(pt)) / pt; };
                    this.FMother = (Complex w) => { if (w.Abs <= 2 * Math.PI && w.Abs >= Math.PI) return 1.0 / sqrt2pi; return 0; };
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
                    this.FMother = (Complex w) => 4 * Complex.I * Complex.Exp(Complex.I * w / 2) / w * Complex.Sin(w / 4).Sqr();
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
        /// Функция, получившаяся при последнем анализе 
        /// </summary>
        public DComplexFunc LastAnalResult = null;
        private Memoize<Point, Complex> MemofLastAnal = null;
        /// <summary>
        /// Набор значений последнего результата анализа
        /// </summary>
        public ConcurrentDictionary<Point, Complex> Dic => MemofLastAnal.dic;

        /// <summary>
        /// Вейвлет-образ указанной функции
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public DComplexFunc GetAnalys(RealFunc f)
        {
            DComplexFunc s = (double a, double b) =>
             {
                 DefInteg.GaussKronrod.NodesCount count = DefInteg.GaussKronrod.NodesCount.GK61;

                 ComplexFunc F1 = (Complex t) => f(t.Re) * this.Mother((t.Re - b) / a).Conjugate;
                 ComplexFunc F2 = (Complex t) => f(-t.Re) * this.Mother((-t.Re - b) / a).Conjugate;
                 double con = 1.0 / Math.Sqrt(a.Abs()) /*Math.Pow(a.Abs(), this.k/2)*/;
                 Complex t1 = 0, t2 = 0;
                 Parallel.Invoke(
                     () => t1 = DefInteg.GaussKronrod.DINN_GK(F1, 0, 0, 0, 0, 0, 0, eps: eps, nodesCount: count),
                     () => t2 = DefInteg.GaussKronrod.DINN_GK(F2, 0, 0, 0, 0, 0, 0, eps: eps, nodesCount: count)
                     );
                 //return con*DefInteg.GaussKronrod.ParallelGaussKronrod(F1,-N,N,61,10);
                 return con * (t1 + t2);
             };
            MemofLastAnal = new Memoize<Point, Complex>((Point p) => s(p.x, p.y));
            LastAnalResult = new DComplexFunc((double a, double b) => MemofLastAnal.Value(new Point(a, b)));
            return LastAnalResult;
        }
        /// <summary>
        /// Обратное вейвлет-преобразование указанной функции
        /// </summary>
        /// <param name="F"></param>
        /// <returns></returns>
        public RealFunc GetSyntesis(DComplexFunc F = null)
        {
            //вычисление коэффициента С
            //надо добавить какое-нибудь ограничение на <inf 
            Complex C;
            if (this.Type == Wavelets.LP) C = Math.Log(2) / Math.PI;//2*DefInteg.GaussKronrod.MySimpleGaussKronrod((Complex w) =>
            //     {
            //         Complex fi = this.FMother(w);
            //         return fi.Abs.Sqr() / w.Abs;
            //     }, Math.PI, 2 * Math.PI);
            else
                C = DefInteg.GaussKronrod.DINN_GKwith0Full((Complex w) =>
                {
                    if (w == 0) return 0;
                    //Complex fi = DefInteg.GaussKronrod.DINN_GKwith0Full(t=>this.Mother(t.Re)*Complex.Exp(-Complex.I*w*t.Re));//fi.Show();
                    Complex fi = this.FMother(w);
                    return fi.Abs.Sqr() / w.Abs;
                }, eps: eps);
            C *= Math.Sqrt(2);
            //C.Show();
            //задание промежуточных переменных
            if (F != null)
            {
                Memoize<Point, Complex> f = new Memoize<Point, Complex>((Point p) => F(p.x, p.y));
                //выдача двойного интеграла
                return (double t) => (DefInteg.DoubleIntegralIn_FULL((Point p) => (this.Mother((t - p.y) / p.x) * f.Value(p)/*F(p.x, p.y)*/ / p.x / p.x/*Math.Pow(p.x.Abs(), this.k+3)*/).Re, eps: eps, parallel: true, M: DefInteg.Method.GaussKronrod61, changestepcount: 0, a: 1, b: 10) / C).Re;
            }
            else
                return (double t) => (DefInteg.DoubleIntegralIn_FULL((Point p) => (this.Mother((t - p.y) / p.x) * LastAnalResult(p.x, p.y)/*F(p.x, p.y)*/ / p.x / p.x/*Math.Pow(p.x.Abs(), this.k+3)*/).Re, eps: eps, parallel: true, M: DefInteg.Method.GaussKronrod61, changestepcount: 0, a: 1, b: 10) / C).Re;
        }
    }
}