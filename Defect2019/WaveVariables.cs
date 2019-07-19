using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using МатКлассы;
using static МатКлассы.Number;
using static МатКлассы.FuncMethods.DefInteg;
using static МатКлассы.FuncMethods.Optimization;
using VectorNetFunc = System.Collections.Generic.List<System.Tuple<double, МатКлассы.Vectors>>;
using System.Collections;
using Computator.NET.Core.Functions;
using static Functions;
using System.Windows.Forms;
using Практика_с_фортрана;
using static МатКлассы.Waves;
using System.Media;
using Библиотека_графики;
using Defect2019;
using Point = МатКлассы.Point;
using static РабКонсоль;
using System.Diagnostics;


public static class Forms
{
    public static UGrafic UG=new UGrafic();
    public static Uxt Uform = new Uxt();
}

public static class РабКонсоль
{
    public static double wold = 2;

    #region Параметры DINN
    public static bool DINNplay = false;

    private static double t11 = 0, t44 = 15;
    public static double t1
    {
        get
        {
            //if (t11 < 0) t11 = Math.Min(k1(w), k2(w)) / 2;
            return t11;
        }
        set
        {
            t11 = value;
        }
    }
    public static double t4
    {
        get
        {
            //if (t44 < 0) t44 = Math.Max(k1(w), k2(w)) * 2;
            return t44;
        }
        set
        {
            t44 = value;
        }
    }

    public static double t2 = t1, t3 = t1, tm = 0.02, tp = 0, eps = 1e-6, pr = 1e-3, gr = 1e3;
    public static GaussKronrod.NodesCount NodesCount = GaussKronrod.NodesCount.GK15;
    #endregion

    #region Параметры для дисперсионок
    public static Complex[] Poles;
    public static double steproot = 1e-3, polesBeg = 0.0, polesEnd = 15;
    public static double epsjump = 1e-1, epsroot = 1e-3;
    public static int countroot = 50;
    public static void SetPoles(double beg = 0.005, double end = 15, double step = 1e-3, double eps = 1e-4, int count = 50)
    {
        РабКонсоль.Poles = ((Complex[])FuncMethods.Optimization.Halfc(Deltas, beg, end, step, eps, count))/*.Where(c=>c!=0).ToArray()*/;
        List<Complex> value = new List<Complex>(), newmas = new List<Complex>();

        double wtf = Deltas(РабКонсоль.Poles[0]).Abs;
        if (wtf < 1e-3)
            newmas.Add(РабКонсоль.Poles[0]);
        for (int j = 1; j < РабКонсоль.Poles.Length; j++)
            newmas.Add(РабКонсоль.Poles[j]);
        РабКонсоль.Poles = newmas.ToArray();
    }
    public static void SetPolesDef() => SetPoles(РабКонсоль.polesBeg, РабКонсоль.polesEnd, РабКонсоль.steproot, РабКонсоль.epsroot, РабКонсоль.countroot);
    #endregion

    public static double ThU = 1e-3, SpU = 1e3, wc = 0.1 * 2 * Math.PI, _T;
    public static double T => 2 * Math.PI / wc * ThU / SpU;
    public static double wbeg = 0.02, wend = 1.5;
    public static int wcount = 400;

    /// <summary>
    /// Скорость смены картинок для анимации (в ммсек)
    /// </summary>
    public static int animatime = 250;
}

public static class Functions
{
    #region Конструктор и константы
    static Functions()
    {
        AfterChaigeData();

        II = new Complex(0, 1);
        I2 = new Complex(0, 0.5);
    }
    private static Complex II,I2;
    public static void AfterChaigeData()
    {
        ml2 = 2 * mu + lamda;
        im = Complex.I * mu;
        mu2 = 2 * mu;
        h = z1 - z2;
        k2coef = ro / mu;
        k1coef = ro / ml2;

        //var KMatr = new Memoize<Tuple<Complex, Complex, double, double>, CSqMatrix>((Tuple<Complex, Complex, double, double> t) => K(t.Item1, t.Item2, t.Item3, t.Item4)).Value;
        //KMatrix = (Complex a1, Complex a2, double z, double w) => KMatr(new Tuple<Complex, Complex, double, double>(a1, a2, z, w));

        var del = new Memoize<Tuple<Complex, Complex, double>, CSqMatrix>((Tuple<Complex, Complex, double> t) => delta(t.Item1, t.Item2, t.Item3)).Value;
        Delta = (Complex a1, Complex a2, double w) => del(new Tuple<Complex, Complex, double>(a1, a2, w));
        var det = new Memoize<Tuple<Complex, Complex, double>, Complex>((Tuple<Complex, Complex, double> t) => delta(t.Item1, t.Item2, t.Item3).Det).Value;
        DeltaDet = (Complex a1, Complex a2, double w) => det(new Tuple<Complex, Complex, double>(a1, a2, w));

        //var bes = new Memoize<Tuple<Complex,double, double>, Complex[]>((Tuple<Complex, double, double> t) => _Bessel(t.Item1, t.Item2,t.Item3)).Value;
        //Bessel = (Complex a,double x,double y) => bes(new Tuple<Complex, double, double>(a,x,y));
        Bessel = new Func<Complex, double, double, Complex[]>(_Bessel);

        //var han = new Memoize<Tuple<double, double>, Complex>((Tuple<double, double> t) => Computator.NET.Core.Functions.SpecialFunctions.Hankel1(t.Item1, t.Item2)).Value;
        //Hankel = (double n, double x) => han(new Tuple<double, double>(n, x));

        prmsnmem = new Memoize<Tuple<Complex, double>, Complex[]>((Tuple<Complex,  double> t) => _PRMSN(t.Item1, t.Item2));
        var prmsn = prmsnmem.Value;
        PRMSN = (Complex a,  double w) => prmsn(new Tuple<Complex, double>(a, w));

        //var prmsnmemup = new Memoize<Tuple<Complex, double, double,bool>, Complex[]>((Tuple<Complex, double, double,bool> t) => _PRMSNUp(t.Item1, t.Item2, t.Item3,t.Item4));
        //var prmsnup = prmsnmemup.Value;
        //PRMSNUp = (Complex a, double z, double w,bool first) => prmsnup(new Tuple<Complex, double, double,bool>(a, z, w,first));
        ////PRMSN = new Func<Complex, double, double, Complex[]>(_PRMSN);

        var pol = new Memoize<double, Vectors>((double t) => _PolesPoles(t)).Value;
        PolesPoles = (double x) => pol(x);

        //var JJ = new Memoize<Complex, Tuple<Complex, Complex>>((Complex c) => new Tuple<Complex, Complex>(МатКлассы.SpecialFunctions.MyBessel(1.0, c), МатКлассы.SpecialFunctions.MyBessel(0.0, c))).Value;
        //J = (Complex c) => JJ(c);

        var seq = new Memoize<Tuple<double, double, int>, double[]>((Tuple<double, double, int> t) => _Seqw(t.Item1, t.Item2, t.Item3));
        Seqw = (double a, double b, int c) => seq.Value(new Tuple<double, double, int>(a, b, c));

        ur = new Memoize<Tuple<double, double,  double, Normal2D[]>, Complex[]>((Tuple<double, double, double, Normal2D[]> t) => _uRes(t.Item1, t.Item2, t.Item3, t.Item4));
        uRes = (double x, double y,  double w, Normal2D[] n) => ur.Value(new Tuple<double, double,  double, Normal2D[]>(x, y,  w, n));
    }
    public static Memoize<Tuple<double, double, double, Normal2D[]>, Complex[]> ur;


    public static Func<double, double, int, double[]> _Seqw = (double wbeg, double wend, int count) => Expendator.Seq(wbeg, wend, count);
    public static Func<double, double, int, double[]> Seqw;
    public static Memoize<Tuple<Complex, double>, Complex[]> prmsnmem;

    public static double lamda = 51.0835913312694, mu = 26.3157894736842, ro = 2.7, h;
    private static double ml2, z1 = 0, z2 = -2.0, mu2, k1coef, k2coef;
    private static Complex im;

    private static Func<Complex, Tuple<Complex, Complex>> J;
    #endregion

    #region Простейшие функции
    public static RealFunc k1 = (double w) => w * w * k1coef;
    public static RealFunc k2 = (double w) => w * w * k2coef;
    public static Func<Complex, double, Complex> sigma = (Complex a, double kw) =>
      {
          return Complex.Sqrt((a - kw)) * Math.Sign(a.Abs - kw);

          if (a.Abs > kw) return Complex.Sqrt((a - kw));
          Complex tmp = Complex.I * Complex.Sqrt((kw - a));
          if (a.Im * a.Re > 0) return tmp;
          return -tmp;

          //Complex tmp = a - kw;
          //if (a.Abs >= kw) return Complex.I * Complex.Sqrt(tmp);
          //return -Complex.Sqrt(-tmp);

          //if (a.Abs > kw) return Complex.Sqrt((a - kw));
          //Complex tmp = -Complex.I * Complex.Sqrt((kw - a));
          //if (a.Im * a.Re > 0) return tmp;
          //return -tmp;
      };


    public static Func<double, Complex> F1 = (double w) =>
    {
        Complex pi = Complex.I * Math.PI / wc;

        double w1 = 5 * wc + 4 * w, w2 = 3 * wc + 4 * w, w3 = -3 * wc + 4 * w, w4 = -5 * wc + 4 * w;
        Complex e(double c) => (Complex.Exp(pi * c) - 1) / c;

        return Complex.I * (e(w1) - e(w2) - e(w3) + e(w4));

        ////return Complex.I / w * 130.0 / 63.0;
        //double t = 2 * T;
        //Complex tmp = Complex.I * w * t;
        ////Debug.WriteLine($"tmp={tmp} {Complex.Exp(tmp * 9.0 / 4.0) / 9} { -Complex.Exp(tmp * 7.0 / 4) / 7 - Complex.Exp(tmp / 4) - Complex.Exp(-tmp / 4) + 128.0 / 63.0}");
        //return Complex.I / w * (Complex.Exp(tmp * 9.0 / 4.0) / 9 - Complex.Exp(tmp * 7.0 / 4) / 7 - Complex.Exp(tmp / 4) - Complex.Exp(-tmp / 4) + 128.0 / 63.0);
    };
    public static Func<double, Complex> F2 = (double w) =>
    {
        int N = 7;

        double w1 = wc * (N + 1) / N + w, w2 = wc * (N - 1) / N + w, w3 = wc * (1 - N) / N + w, w4 = 2 - wc * (N + 1) / N, w5 = wc + w, w6 = w - wc;
        Complex ew(double t, double ww) => Complex.Exp(Complex.I * t * ww) / ww;
        Complex perv(double t) => ew(t, w1) + ew(t, w2) - ew(t, w3) - ew(t, w4) - 2 * (ew(t, w5) - ew(t, w6));

        return (perv(2 * Math.PI * N / wc) - perv(0)) / 8;

        ////return 1.0 / (1 - 4 * N * N) / Complex.I / w;
        //double t = N * T, n = 1.0 / N;
        //Complex tmp = Complex.I * w * t;
        //return -1.0 / 8 / Complex.I * (Complex.Exp(tmp * (2 + n)) / Complex.I / w / (2 + n) + Complex.Exp(tmp * (2 - n)) / Complex.I / w / (2 - n) - Complex.Exp(tmp * (2)) / Complex.I / w - N * Complex.Exp(tmp * n) / Complex.I / w + N * Complex.Exp(-tmp * n) / Complex.I / w + 1.0 / (1 - 4 * N * N) / w / Complex.I);
    };
    #endregion

    #region Устаревшие функции
    private static Func<Complex, Complex, Complex> als = (Complex al1, Complex al2) => al1.Sqr() + al2.Sqr();
    private static Func<Complex, Complex, double, CSqMatrix> delta = (Complex a1, Complex a2, double w) =>
        {
            double kt1 = k1(w), kt2 = k2(w);
            Complex al = als(a1, a2);
            Complex s1s = al - kt1, s2s = al - kt2, s1 = sigma(al, kt1), s2 = sigma(al, kt2);

            Complex a = -lamda * al + ml2 * s1s;
            Complex b = im * al * 2 * s1;
            Complex c = 2 * mu * al * s2;
            Complex d = -im * al * (s2s + al);
            Complex e11 = Complex.Exp(s1 * z1), e12 = Complex.Exp(s1 * z2), e21 = Complex.Exp(s2 * z1), e22 = Complex.Exp(s2 * z2);

            return new CSqMatrix(new Complex[,] {
                 { a*e11,a/e11,c*e21,-c/e21},
                 { -b*e11,b/e11,d*e21,d/e21},
                 {a*e12,a/e12,c*e22,-c/e22 },
                 {-b*e12,b/e12,d*e22,d/e22 }
             });
        };
    /// <summary>
    /// Функция, возвращающая матрицу, чей определитель есть знаменатель delta
    /// </summary>
    public static Func<Complex, Complex, double, CSqMatrix> Delta;
    /// <summary>
    /// Функция, возвращающая матрицу, чей определитель есть знаменатель delta
    /// </summary>
    public static Func<Complex, Complex, double, Complex> DeltaDet;

    /// <summary>
    /// Матрица Грина при alpha=0, умноженнная на alpha
    /// </summary>
    public static Func<double, double, double, double, CSqMatrix> K0a = (double x, double y, double z, double w) =>
    {
        var b = Bessel(0, x, y);

        Complex i = new Complex(0, 1);
        Complex s2 = i * k2(w);
        Complex N = Math.Tan(k2(w) * h).Reverse() / mu / k2(w);
        Complex r = Math.Sqrt(x * x + y * y);

        Complex jxx = x * x / r / r, jxy = x * y / r / r, jyy = y * y / r / r;//jxx.Show();

        Complex
        K11 = N * jyy,
        K12 = -N * jxy,
        K13 = 0,
        K21 = new Complex(K12),
        K22 = N * jxx,
        K23 = 0,
        K31 = 0,
        K32 = 0,
        K33 = 0;

        return new CSqMatrix(new Complex[,] {
                 {K11,K12, K13},
             { K21,K22,K23},
             { K31,K32,K33}
              });
    };

    #endregion


    #region Функции знаменателя, его производных и корней

    public static double epsforder =>РабКонсоль.eps;

    /// <summary>
    /// Функция знаменателя, выраженная явно
    /// </summary>
    public static Func<Complex, double, Complex> Deltass = (Complex alp, double w) =>
        {
            //double kt1 = k1(w), kt2 = k2(w);
            //Complex al = alp * alp;
            //Complex s1 = sigma(al, kt1), s2 = sigma(al, kt2);

            //Complex a = 2 * mu * al - ml2 * kt1;
            //Complex b = /*im * al **/ 2 * s1;
            //Complex c = 2 * mu * al * s2;
            //Complex d = -/*im * al **/ (2 * al - kt2);
            ////м б надо будет в числителе на - умножить, так как я тут i*i вынес
            //Complex ad = a * d, bc = b * c;
            ////Complex e1 = Complex.Exp(h * s1), e2 = Complex.Exp(h * s2),e12=e1/e2,e21=1.0/e12,e12n=e1*e2,e21n=1.0/e12n;
            //return /*mu*mu**/(4 * ad * bc - Complex.Ch(h * (s1 + s2)) * (ad + bc).Sqr() + Complex.Ch(h * (s1 - s2)) * (ad - bc).Sqr());

            //исходный вариант
            //Complex a = al - kt2 / 2;
            //Complex b = s1;
            //Complex c = al * s2;
            //Complex d = -a;
            //Complex ad = a * d, bc = b * c;
            //return (4 * ad * bc - Complex.Ch(h * (s1 + s2)) * (ad + bc).Sqr() + Complex.Ch(h * (s1 - s2)) * (ad - bc).Sqr());

            //return A(alp, w).Det;

            double kt1 = k1(w), kt2 = k2(w);
            Complex al = alp * alp, ai =/*al**/Complex.I;
            Complex s1 = sigma(al, kt1), s2 = sigma(al, kt2);
            Complex a = (al - 0.5 * kt2);//*mu2;
            Complex b = s1 * ai;// * mu2;// *();
            Complex c = al * s2;// * mu2;
            Complex d = -a * ai;// * (al * Complex.I);
            //Complex q = Complex.Exp(s1 * h), ww = Complex.Exp(s2 * h);
            Complex q = Complex.Exp(-s1 * h), ww = Complex.Exp(-s2 * h);

            Complex ad = a * d, bc = b * c;
            return -((q * ww).Sqr() + 1.0) * (ad + bc).Sqr() + (ad * q + bc * ww).Sqr() + (ad * ww + bc * q).Sqr() - 2 * ad * bc * (q - ww).Sqr();

            //Complex a = (al - 0.5 * kt2), a2 = a * a;
            //Complex g = s1 * s2 * al*Complex.I;
            //return (-4 * a2 * g - Complex.Ch(h * (s1 + s2)) * (a2 - g).Sqr() + Complex.Ch(h * (s1 - s2)) * (a2 - g).Sqr()) * 2 * mu;
        };
    /// <summary>
    /// Дополнительный знаменатель, полученный от N
    /// </summary>
    public static Func<Complex, double, Complex> DeltassN = (Complex alp, double w) =>
    {
        double kt2 = k2(w);
        Complex al = alp * alp;
        Complex s2 = sigma(al, kt2);
        return/* mu**/ /*al**/ s2 * Complex.Sh(s2 * h);
    };
    /// <summary>
    /// Общий знаменатель
    /// </summary>
    public static Func<Complex, double, Complex> BigDelta = (Complex alp, double w) => DeltassN(alp, w) * Deltass(alp, w);
    /// <summary>
    /// Производная общего знаменателя
    /// </summary>
    public static Func<Complex, double, Complex> BigDeltaDeriv = (Complex alp, double w) => (BigDelta(alp + epsforder, w) - BigDelta(alp - epsforder, w) / (2 * epsforder));

    /// <summary>
    /// Производная общего знаменателя
    /// </summary>
    public static Func<Complex, double, Complex> BigDeltaDeriv0 = (Complex alp, double w) => {

        Complex D = Deltass(alp, w), N = DeltassN(alp, w), dD = DeltaDeriv(alp, w), dN = DeltaNDeriv(alp, w);
        return D * dN + N * dD;

    };

    public static Func<Complex, double, Complex> DeltaDeriv = (Complex alp, double w) => (Deltass(alp + epsforder, w) - Deltass(alp - epsforder, w) / (2 * epsforder));
    /// <summary>
    /// Явная производная знаменателя N
    /// </summary>
    public static Func<Complex, double, Complex> DeltaNDeriv = (Complex a, double w) =>
        {
            double kt2 = k2(w);
            Complex al = a * a;
            Complex s2 = sigma(al, kt2);
            return a * (Complex.Sh(s2 * h) / s2 + h * Complex.Ch(s2 * h));
        };

    /// <summary>
    /// Явные корни N
    /// </summary>
    /// <param name="omega"></param>
    /// <param name="tmin"></param>
    /// <param name="tmax"></param>
    /// <returns></returns>
    public static Vectors DeltassNPosRoots(double omega, double tmin, double tmax)
    {
        List<double> list = new List<double>();
        double alp, pi2 = (Math.PI / h).Sqr(), kappa = k2(omega), s = kappa;//kappa.Show();
        int k = 1;
        do
        {
            alp = Math.Sqrt(s);
            list.Add(alp);
            s = kappa - pi2 * k * k;//if (omega > 13) s.Show();
            k++;
        }
        while (/*alp <= tmax && alp >= tmin &&*/ s >= 0);//new Vectors(list.ToArray()).Show();
        return new Vectors(list.Where(n => n >= tmin && n <= tmax).ToArray());
    }
    /// <summary>
    /// Функция знаменателя, выраженная явно (при глобальной частоте)
    /// </summary>
    public static ComplexFunc Deltas = (Complex a) => Deltass(a, РабКонсоль.wold);

    /// <summary>
    /// Возвращает массив полюсов при такой-то частоте
    /// </summary>
    private static Func<double, Vectors> _PolesPoles = (double w) =>
       {
           ComplexFunc del = (Complex a) => Deltass(a, w);
           Vectors v1 = w<0.1? Roots.OtherMethod(del, РабКонсоль.polesBeg, РабКонсоль.polesEnd, РабКонсоль.steproot/200, 1e-12, Roots.MethodRoot.Brent, false):Roots.OtherMethod(del, РабКонсоль.polesBeg, РабКонсоль.polesEnd,РабКонсоль.steproot, 1e-10, Roots.MethodRoot.Brent, false);
           Vectors v2 = DeltassNPosRoots(w, РабКонсоль.polesBeg, РабКонсоль.polesEnd);
           // return new Vectors(Expendator.Distinct(v1.DoubleMas, v2.DoubleMas));
           v1.UnionWith(v2);
           if (v1.Deg != 3) v1 = Getv(w);
           return v1;
       };

    private static Vectors Getv(double w)
    {
        int i = 0;
        while (kGrafic.Model[i].Item1 > w) i++;
        return new Vectors(kGrafic.Model[i].Item2);
    }

    /// <summary>
    /// Мемоизированная _PolesPoles
    /// </summary>
    public static Func<double, Vectors> PolesPoles;
    #endregion

    #region Матрица K
    ///// <summary>
    ///// Матрица Грина как функция альфы, частоты и координат
    ///// </summary>
    //public static Func<Complex, double, double, double, double, CSqMatrix> K = (Complex a,double x,double y, double z, double w) =>
    //      {
    //          var c = PRMSN(a, z, w);
    //          var b = Bessel(a, x, y);

    //          Complex P = c[0], R = c[1], M = c[2], S = c[3], N = c[4];
    //          Complex jx = b[0], jy = b[1], jxx = b[2], jxy = b[3], jyy = b[4];
    //          Complex i = new Complex(0,1);
    //          Complex
    //          K11 =i*(M*jxx+N*jyy),
    //          K12 =i*(M-N)*jxy,
    //          K13 =-P*jx,
    //          K21 = new Complex(K12),
    //          K22 =i*(M*jyy+N*jxx),
    //          K23 =-P*jy,
    //          K31 =-i*S*jx,
    //          K32 =-i*S*jy,
    //          K33 =R;
    //          //K22.Show();"".Show();
    //          return new CSqMatrix(new Complex[,] {
    //             {K11,K12, K13},
    //         { K21,K22,K23},
    //         { K31,K32,K33}
    //          });
    //      };

    /// <summary>
    /// Матрица Грина как функция альфы, частоты и координат
    /// </summary>
    public static Func<Complex, double, double,  double, CSqMatrix> K = (Complex a, double x, double y,  double w) =>
    {
        var c = PRMSN(a, w);
        Complex ar = a * Math.Sqrt(x * x + y * y);
        Tuple<Complex, Complex> tup = new Tuple<Complex, Complex>(МатКлассы.SpecialFunctions.MyBessel(1, ar), МатКлассы.SpecialFunctions.MyBessel(0, ar));
        //Tuple<Complex, Complex> tup = J(ar);
        return InK(a, c, tup, x, y);
    };
    /// <summary>
    /// Матрица Грина при наборе нормалей
    /// </summary>
    public static Func<Complex, double, double,  double, Normal2D[],Func<Point,CVectors>, CVectors> Ksum=(Complex a, double x, double y,  double w, Normal2D[] nd, Func<Point, CVectors> Q) =>
    {
        var c = PRMSN(a, w);
        Complex ar;
        Tuple<Complex, Complex> tup;
        CVectors mat = new CVectors(3);
        Point xy = new Point(x, y);

        for(int i = 0; i < nd.Length; i++)
        {
            ar = a * Point.Eudistance(nd[i].Position, xy);
            tup = new Tuple<Complex, Complex>(МатКлассы.SpecialFunctions.MyBessel(1, ar), МатКлассы.SpecialFunctions.MyBessel(0, ar));
            mat.FastAdd( InK(a, c, tup, x-nd[i].Position.x, y - nd[i].Position.y) *Q(nd[i].n));
        }
        return mat;
    };

    /// <summary>
    /// Матрица Грина как функция от функция альфы, частоты и координат (без знаменателя)
    /// </summary>
    public static Func<Complex, double, double,  double, bool,CSqMatrix> KRes = (Complex a, double x, double y,  double w, bool first) =>
    {
        var c = PRMSNUp(a, w,first);//c.Show();
        Complex ar = a * Math.Sqrt(x * x + y * y);
        Tuple<Complex, Complex> tup = new Tuple<Complex, Complex>(МатКлассы.SpecialFunctions.Hankel(1, ar.Re), МатКлассы.SpecialFunctions.Hankel(0, ar.Re));
        return InK(a, c,tup, x, y);
    };

    /// <summary>
    /// Матрица Грина при наборе нормалей
    /// </summary>
    public static Func<double, double, double,  Normal2D[], Func<Point, Vectors>, CVectors> KsumRes = (double x, double y,  double w, Normal2D[] nd, Func<Point, Vectors> Q) =>
    {
        var poles = PolesPoles(w);//w.Show();
       Complex[][] c1=new Complex[poles.Deg][], c2 = new Complex[poles.Deg][];
        double ar;
        Tuple<Complex, Complex> tup;
        CVectors sum = new CVectors(3);
        Point xy = new Point(x, y);
        Vectors QQ;

        double dist = Vectors.Union2(new Vectors(0.0), poles).MinDist,xp,yp;

        double eps = dist/*poles.MinDist*/ * 0.4, eps2 = 0.5 * eps;
        double[] plus = new double[poles.Deg], pminus = new double[poles.Deg];

        for (int k = 0; k < poles.Deg; k++)
        {
            plus[k] = poles[k] + eps;
            pminus[k] = poles[k] - eps;
            c1[k] = PRMSN(plus[k],  w);
                c2[k] = PRMSN(pminus[k],  w);
            
        }


        //CSqMatrix m1=new CSqMatrix(new Complex[3,3]);
        for (int i = 0; i < nd.Length; i++)
        {
            QQ = (Q(nd[i].n) * eps2);

            xp = x - nd[i].Position.x;
            yp = y - nd[i].Position.y;

            for (int k = 0; k < poles.Deg; k++)
            {
                ar = poles[k] * Point.Eudistance(nd[i].Position, xy);
                tup = new Tuple<Complex, Complex>(МатКлассы.SpecialFunctions.Hankel(1.0, ar), МатКлассы.SpecialFunctions.Hankel(0.0, ar));

                sum.FastAdd(KQmult(InKtwice(plus[k], pminus[k], c1[k], c2[k], tup, xp, yp), QQ));

               // sum.FastAdd(InKtwice(plus[k], pminus[k], c1[k], c2[k], tup, xp, yp) * QQ);

                //sum.FastAdd(InK(plus[k], c1[k], tup,xp , yp) * QQ);
                //sum.FastLessen(InK(pminus[k], c2[k], tup, xp, yp) * QQ);

               // m1.FastAdd(InK(poles[k] + eps, c1[k], tup, x - nd[i].Position.x, y - nd[i].Position.y));
               // m1.FastLessen(InK(poles[k] - eps, c2[k], tup, x - nd[i].Position.x, y - nd[i].Position.y));

            }
            //sum.FastAdd(m1 *(Q(nd[i].n) * 0.5 * eps));
            //m1 = new CSqMatrix(new Complex[3, 3]);
        }

        return sum * /*Math.PI **/ I2;
    };

    public static Func<Complex, Complex[],Tuple<Complex,Complex>,double, double, CSqMatrix> InK = (Complex a, Complex[] PRMSN,Tuple<Complex,Complex> beshank,double x, double y) =>
      {
   
        double x2 = x * x, y2 = y * y, r2 = x2 + y2, r = Math.Sqrt(r2);
        Complex ar = a * r,a2=a*a;
        Complex j1ar = beshank.Item1/*МатКлассы.SpecialFunctions.MyBessel(1, ar)*/, j0ar = beshank.Item2/*МатКлассы.SpecialFunctions.MyBessel(0, ar)*/;

        Complex P = PRMSN[0], R = PRMSN[1], Mi = PRMSN[2] * II, Si = PRMSN[3] * II, Ni = PRMSN[4]*II;
          Complex
            j1arr= j1ar / r,
            jx = -x * j1arr,
            jy = -y * j1arr,
            //jxx =-(j0ar*a*x2+r*j1ar)/r2 , 
            //jxy =-a/r2*x*y*j0ar , 
            //jyy = -(j0ar * a * y2 + r * j1ar) / r2;
            j0ara=j0ar*a,
            jtmp= j1arr * (x2 - y2),
            jxx = -(j0ara * x2 - jtmp) / r2,
            jxy = -x * y/ r2 *(j0ara-2*j1arr),
            jyy = -(j0ara * y2 + jtmp) / r2;
          //Complex i = new Complex(0, 1);

        Complex
        K11 = (Mi * jxx + Ni * jyy),
        K12 = (Mi - Ni) * jxy,
        K13 = P * jx*a2,
       // K21 = new Complex(K12),
        K22 = (Mi *jyy + Ni * jxx),
        K23 = P * jy*a2,
        K31 = Si * jx,
        K32 = Si * jy,
        K33 = R*j0ara;
        //K22.Show();"".Show();
        return new CSqMatrix(new Complex[3,3] {
                 {K11,K12, K13},
             { K12,K22,K23},
             { K31,K32,K33}
              });
      };

    public static Func<Complex,Complex,Complex[], Complex[], Tuple<Complex, Complex>, double, double, Complex[,]> InKtwice = (Complex a1, Complex a2, Complex[] PRMSN1, Complex[] PRMSN2, Tuple<Complex, Complex> beshank, double x, double y) =>
    {

        double x2 = x * x, y2 = y * y, r2 = x2 + y2, r = Math.Sqrt(r2),xy=x*y;
        Complex ar1 = a1 * r, a21 = a1 * a1;
        Complex ar2 = a2 * r, a22 = a2 * a2;

        Complex j1ar = beshank.Item1, j0ar = beshank.Item2;

        Complex P = PRMSN1[0]*a21-PRMSN2[0]*a22, R1 = PRMSN1[1], Mi1 = PRMSN1[2] * II, Si = (PRMSN1[3]-PRMSN2[3]) * II, Ni1 = PRMSN1[4] * II;
        Complex R2 = PRMSN2[1], Mi2 = PRMSN2[2] * II, Ni2 = PRMSN2[4] * II;

        Complex
          j1arr = j1ar / r,
          jx = -x * j1arr,
          jy = -y * j1arr,
          j0ara1 = j0ar * a1,
           j0ara2 = j0ar * a2,
          jtmp = j1arr * (x2 - y2),
          jxx1 = -(j0ara1 * x2 - jtmp),
          jxy1 = -xy * (j0ara1 - 2 * j1arr),
          jyy1 = -(j0ara1 * y2 + jtmp),
        jxx2 = -(j0ara2 * x2 - jtmp),
          jxy2 = -xy * (j0ara2 - 2 * j1arr),
          jyy2 = -(j0ara2 * y2 + jtmp);

        Complex
        K12 = ((Mi1 - Ni1) * jxy1 - (Mi2 - Ni2) * jxy2) / r2;

        return new Complex[3, 3] {
                 {((Mi1 * jxx1 + Ni1 * jyy1)- (Mi2 * jxx2 + Ni2 * jyy2)) / r2,K12, P*jx},
             { K12,((Mi1 * jyy1 + Ni1 * jxx1)- (Mi2 * jyy2 + Ni2 * jxx2)) / r2,P * jy},
             { Si * jx,Si * jy,R1 * j0ara1- R2 * j0ara2}
              };

        //return new CSqMatrix(new Complex[3, 3] {
        //         {K11,K12, K13},
        //     { K12,K22,K23},
        //     { K31,K32,K33}
        //      });
    };


    /// <summary>
    /// Матрица Грина в чистом виде (в программе не нужна)
    /// </summary>
    public static Func<Complex, Complex, double, CSqMatrix> ClearK = (Complex a1, Complex a2, double w) =>
      {
          Complex a12 = a1 * a1,a22=a2*a2,a=a12+a22,aa=a1*a2;
          var c = PRMSN(Complex.Sqrt(a), w);

          Complex P = c[0], R = c[1], M = c[2], S = c[3], N = c[4];
          Complex i = new Complex(0, 1);

          Complex
          K11 = -i * (M * a12 + N * a22)/a,
          K12 = -i * (M - N) * aa/a,
          K13 = -i*P * a1,
          K21 = new Complex(K12),
          K22 = -i * (M * a22 + N * a12)/a,
          K23 = -i*P *  a2,
          K31 = S * a1/a,
          K32 = S * a2/2,
          K33 = R;
          //K22.Show();"".Show();
          return new CSqMatrix(new Complex[,] {
                 {K11,K12, K13},
             { K21,K22,K23},
             { K31,K32,K33}
              });
      };

    /// <summary>
    /// Быстрое произведение нужных матриц и векторов с учётом их структуры
    /// </summary>
    /// <param name="M"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static CVectors KQmult(CSqMatrix M,Vectors v)
    {
        CVectors r = new CVectors(3);
        for (int k = 0; k < 3; k++)
            r[k] = M[k, 0] * v[0] + M[k, 1] * v[1];
        return r;
    }

    /// <summary>
    /// Быстрое произведение нужных матриц и векторов с учётом их структуры
    /// </summary>
    /// <param name="M"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static CVectors KQmult(Complex[,] M, Vectors v)=> new CVectors(new Complex[3] { M[0, 0] * v[0] + M[0, 1] * v[1], M[1, 0] * v[0] + M[1, 1] * v[1], M[2, 0] * v[0] + M[2, 1] * v[1] });
    

    #endregion

    #region Функции для определения коэффициентов матрицы K
    private static Complex littleBessel1(Complex a) => МатКлассы.SpecialFunctions.MyBessel(1, a);
    private static Complex littleBessel2( Complex a) => МатКлассы.SpecialFunctions.MyBessel(2, a);
    private static Func<Complex, Complex> LittleBessel1,LittleBessel2;

    private static Complex[] _Bessel(Complex a,double x,double y)
    {
        //Complex jx, jy, jxx, jxy, jyy;
        //double x2=x*x,y2=y*y, r = Math.Sqrt(x2 + y2);
        //Complex ar = a * r,a_r=a/r,a_r2=a_r*a_r;
        //Complex j1ar =/*LittleBessel1(ar) */МатКлассы.SpecialFunctions.MyBessel(1, ar),j2ar=/*LittleBessel2(ar)*/ МатКлассы.SpecialFunctions.MyBessel(2, ar);

        ////(a).Show();

        //jx = -j1ar * x *a_r;
        //jy = -j1ar * y *a_r;
        //jxx = j2ar * x2* a_r2 - j1ar * a_r;
        //jyy= j2ar * y2 *a_r2 - j1ar *a_r;
        //jxy = a_r*x*y/(x2+y2)*(ar*j2ar-j1ar*(1.0-a));
        //$"{jx} {jy} {jxx} {jxy} {jyy}".Show();

        Complex jx, jy, jxx, jxy, jyy;
        double x2 = x * x, y2 = y * y, r = Math.Sqrt(x2 + y2),r2=x2+y2;
        Complex ar = a * r, a_r = a / r, a_r2 = a_r * a_r,a_rr=a_r/r;
        Complex j1ar =МатКлассы.SpecialFunctions.MyBessel(1, ar), j0ar = МатКлассы.SpecialFunctions.MyBessel(0, ar);

        //(a).Show();

        jx = -j1ar * x * a_r;
        jy = -j1ar * y * a_r;
        jxx = -a_rr*(j0ar*a*x2+r*j1ar);
        jyy = -a_rr * (j0ar * a * y2 + r*j1ar);
        jxy = -a_r2 * x * y*j0ar;

        return new Complex[] { jx, jy, jxx, jxy, jyy };
    }
    private static Complex[] _Hankel(Complex a, double x, double y)
    {
        Complex jx, jy, jxx, jxy, jyy;
        double x2 = x * x, y2 = y * y, r = Math.Sqrt(x2 + y2);
        Complex ar = a * r, ar3 = ar * r * r;
        Complex j1ar =/*LittleBessel1(ar) */МатКлассы.SpecialFunctions.Hankel(1, ar.Re), j2ar =/*LittleBessel2(ar)*/ МатКлассы.SpecialFunctions.Hankel(2, ar.Re);

        //(a).Show();

        jx = -j1ar * x / r;
        jy = -j1ar * y / r;
        jxx = j2ar * x2 / r / r - j1ar * (x2 + a * y2) / ar3;
        jyy = j2ar * y2 / r / r - j1ar * (y2 + a * x2) / ar3;
        jxy = j2ar * x * y / r / r + j1ar / ar3 * (a - 1);
        //$"{jx} {jy} {jxx} {jxy} {jyy}".Show();
        return new Complex[] { jx, jy, jxx, jxy, jyy };
    }

    /// <summary>
    /// Возвращает первые и вторые производные функции J0(alpha,x,y), мемоизируется
    /// </summary>
    public static Func<Complex, double,double, Complex[]> Bessel;
    public static Func<double, double, Complex> Hankel;

    /// <summary>
    /// Возвращает первые два столбца из обратной матрицы
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Tuple<CVectors,CVectors> Arev(Complex al,double w)
    {
        //Complex alp = al * al,ima=Complex.I*mu*alp;
        //Complex s1=sigma(alp,k1(w)),s2=sigma(alp,k2(w));
        //Complex q = Complex.Exp(s1 * h),qv= Complex.Exp(-s1 * h), ww=Complex.Exp(s2*h) ,qp= Complex.Exp(s2 * h), qm=Complex.Exp(-s2*h)/*1.0/qp*/;
        //Complex sh = (qp - qm) * 0.5, ch = (qp + qm) * 0.5;

        //Complex a = (al - 0.5 * k2(w));
        //Complex b = s1;
        //Complex c = al * s2;
        //Complex d = -a;
        //Complex ad = a * d;
        //Complex bc = b * c;
        //Complex cc = -Complex.I * s2;
        //Complex cq = cc *qv, dq = d *qv;

        ////тут большие сомнения насчёт умножения на ima
        //Complex[] m1 = new Complex[4], m2 = new Complex[4];
        //m1[0] = d * (bc - q * (ad * sh + bc * ch));
        //m2[0] = dq * (-bc * q + (bc * ch - ad * sh));
        //m1[1] = cc * (ad - q * (ad * ch + bc * sh));//;*ima
        //m2[1] = cq * (ad * q + (bc * sh - ad * ch));//; * ima
        //m1[2] = d * (bc * q + ad * sh - bc * ch);
        //m2[2] = dq * (-bc + q * (ad * sh + bc * ch));
        //m1[3] = cc * (ad * q - ad * ch + bc * sh);//; * ima
        //m2[3] = cq * (ad - q * (ad * ch + bc * sh));// ;* ima

        //Complex a = (al - 0.5 * k2(w)), a2 = a * a;
        //Complex g = s1 * s2 * al * Complex.I;
        //Complex a2q = a2 * q, gq = g * q, c = -Complex.I * s2, cq = c / q, aq = a / q;
        //m1[0] = -a * (a2q * sh - gq * ch + g);
        //m2[0] = -aq * (a2 * sh + g * ch - gq);//*(-1);
        //m1[1] = c * (-a2 + a2q * ch - gq * sh);//* (-1) ;
        //m2[1] = cq * (a2 * ch + g * sh - a2q);
        //m1[2] = -a * (-a2 * sh - g * ch + gq);
        //m2[2] = -aq * (-a2q * sh + gq * ch - g);// * (-1);
        //m1[3] = c * (a2 * ch + g * sh - a2q);//* (-1) ;
        //m2[3] = cq * (a2q * ch - gq * sh - a2);


        //---------------------------------------------------------------------------------------------------------
        //Complex ia = Complex.I * al;
        //Complex a = (al - 0.5 * k2(w));
        //Complex b = s1 * ia;
        //Complex c = al * s2;
        //Complex d = -a * ia;
        //Complex ad = a * d;
        //Complex bc = b * c;
        //Complex w2 = ww * ww, q2 = q * q;
        //Complex pl = ad + bc, ml = ad - bc;

        // Complex coef = -Complex.Exp(-h * (s1 + s2))/alp/alp;coef.Show();

        //Complex[] m1 = new Complex[4], m2 = new Complex[4];
        //m1[0] = d * q * (pl * w2 * q - q * ml - 2 * bc * ww);
        //m2[0] = -c * q * (q * w2 * pl + q * ml - 2 * ad * ww);
        //m1[1] = -d * (w2 * ml + 2 * bc * ww * q - pl);
        //m2[1] = c * (w2 * ml + pl - 2 * ad * q * ww);
        //m1[2] = b * ww * (pl * q2 * ww + ww * ml - 2 * ad * q);
        //m2[2] = a * ww * (pl * q2 * ww - ww * ml - 2 * bc * q);
        //m1[3] = -b * (ml * q2 - 2 * ad * ww * q + pl);
        //m2[3] = -a * (q2 * ml + pl + 2 * bc * q * ww);

        //return new Tuple<CVectors, CVectors>(new CVectors(m1), new CVectors(m2));

       // CSqMatrix Mat = A(al, w).Invert();
        CSqMatrix Mat = A(al, w).InvertByMathNet();
        //CSqMatrix S = A(al, w).InvertByMathNet();  $"{(A(al, w) * Mat - SqMatrix.E(4)).CubeNorm} {(A(al, w) * S - SqMatrix.E(4)).CubeNorm}".Show();
        //CSqMatrix Mat2 = A(al, w).InvertSum;$"{(A(al, w) * Mat-SqMatrix.E(4)).CubeNorm} \t{(A(al, w) * Mat2 - SqMatrix.E(4)).CubeNorm}".Show();

        return new Tuple<CVectors, CVectors>(Mat.GetColumn(0), Mat.GetColumn(1));
        //CSqMatrix s = A(al, w);
        //return new Tuple<CVectors, CVectors>(new CVectors(new Complex[] {s.GetMinMat(1,1).DetSarius, s.GetMinMat(1, 2).DetSarius , s.GetMinMat(1, 2).DetSarius , s.GetMinMat(1, 2).DetSarius }), new CVectors(new Complex[] { s.GetMinMat(2, 1).DetSarius, s.GetMinMat(2, 2).DetSarius, s.GetMinMat(2, 2).DetSarius, s.GetMinMat(2, 2).DetSarius }));

    }
    /// <summary>
    /// Возвращает первые два столбца обратной матрицы без деления на определитель
    /// </summary>
    /// <param name="al"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Tuple<CVectors, CVectors> ArevUp(Complex al, double w)
    {
        CSqMatrix Mat = A(al, w);
        CVectors t1 = new CVectors(new Complex[] {Mat.GetMinMat(1,1).Det, -Mat.GetMinMat(1, 2).Det, Mat.GetMinMat(1, 3).Det, -Mat.GetMinMat(1, 4).Det });
        CVectors t2 = new CVectors(new Complex[] { -Mat.GetMinMat(2, 1).Det, Mat.GetMinMat(2, 2).Det, -Mat.GetMinMat(2, 3).Det, Mat.GetMinMat(2, 4).Det });
        return new Tuple<CVectors, CVectors>(t1, t2);
    }

    /// <summary>
    /// Возвращает компоненты PRMSN, нужные для матрицы Грина
    /// </summary>
    /// <param name="al"></param>
    /// <param name="z"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Complex[] _PRMSN(Complex al, double w)
    {
        var c = Arev(al, w);
        CVectors v1 = c.Item1, v2 = c.Item2;
        Complex alp = al * al, s1 = sigma(alp, k1(w)), s2 = sigma(alp, k2(w)),ai=(alp*Complex.I);
        Complex e1 = 1, e3 = 1, e2 = Complex.Exp(-s1 * ( h)), e4 = Complex.Exp(-s2 * ( h));

        CVectors c1=new CVectors(new Complex[] { e1, /*ai**/e2,s2 * e3 , -s2 *e4/**ai*/})/mu2,c2=new CVectors(new Complex[] { s1*e1, -s1 *e2/**ai*/, alp* e3, alp*e4/**ai*/})/mu2;
        //Complex del = Deltass(al, w);//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!я там случаной поставил 2 вместо w

        //CSqMatrix Mat = A(al, w);//Mat.Show(); "".Show();
        //var inv = Mat.Invert4();var inv2 = Mat.Invert(); var inv3=Mat.Invert(true);
        //$"{al} \t{(Mat * inv-SqMatrix.E(4)).CubeNorm} \t{(Mat * inv2 - SqMatrix.E(4)).CubeNorm} \t{(Mat * inv3 - SqMatrix.E(4)).CubeNorm}".Show();//(Mat * Mat.Transpose.Invert().Transpose - SqMatrix.E(4)).Show(); "----------------------------------------------------------------".Show();
        //v1 = Mat.GetColumn(0)*del;v2 = Mat.GetColumn(1)*del;
        //CVectors vv1 = Mat * (v1)/Deltass(al,w),vv2=Mat*(v2 )/ Deltass(al, w);
        ////al.Show();
        //v1.Show();
        //v2.Show();
        //"".Show();

        Complex P = v1 * c1;
        Complex R = v1 * c2;
        Complex M = v2 * c1;
        Complex S = v2 * c2;
        //Complex N = Complex.I*Complex.Ch(s2 * (z + h))/mu/*/s2/im/alp/Complex.Sh(s2*h)*/;
        Complex N = Complex.I * Complex.Cth(s2*h) / (mu * /*alp **/ s2);


        //$"{P} {R} {M} {S} \t{N} \t{del} \t{al}".Show();
        return new Complex[] { P/*/del*/, R/*/del*/, M/*/del*/, S/*/del*/, N/*/DeltassN(al,w) */};
    }
    public static Complex[] _PRMSNUp(Complex al,  double w,bool first)
    {
        var c = ArevUp(al, w);
        CVectors v1 = c.Item1, v2 = c.Item2;
        Complex alp = al * al, s1 = sigma(alp, k1(w)), s2 = sigma(alp, k2(w)), ai = (alp * Complex.I);
        Complex e1 = 1, e3 = 1, e2 = Complex.Exp(-s1 * ( h)), e4 = Complex.Exp(-s2 * ( h));

        CVectors c1 = new CVectors(new Complex[] { e1, /*ai**/e2, s2 * e3, -s2 * e4/**ai*/}) / mu2, c2 = new CVectors(new Complex[] { s1 * e1, -s1 * e2/**ai*/, alp * e3, alp * e4/**ai*/}) / mu2;

        Complex P = v1 * c1;
        Complex R = v1 * c2;
        Complex M = v2 * c1;
        Complex S = v2 * c2;
        Complex N = Complex.I * Complex.Ch(s2 * h) / (mu/*alp **/)/*/s2/im/alp/Complex.Sh(s2*h)*/;

        //Complex del = Deltass(al, w), nel = DeltassN(al, w);
        Complex del = DeltaDeriv(al, w),nel=DeltaNDeriv(al, w);//$"det = {del}  Ndet = {nel}".Show();

        //$"Pup = {P} Rup = {R} Mup= {M} Sup = {S} Nup = {N}".Show();

        //return new Complex[] { P*nel, R*nel, M*nel, S*nel, N*del };
        if (first)
            return new Complex[] { P / del, R / del, M / del, S / del, 0.0 };
        return new Complex[] { 0.0, 0.0, 0.0, 0.0, N / nel };
    }

    /// <summary>
    /// Возвращает компоненты PRMSN, нужные для матрицы Грина (мемоизированная)
    /// </summary>
    /// <param name="al"></param>
    /// <param name="z"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Func<Complex, double, Complex[]> PRMSN;
    /// <summary>
    /// Возвращает компоненты PRMSN, нужные для матрицы Грина (мемоизированная)
    /// </summary>
    /// <param name="al"></param>
    /// <param name="z"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Func<Complex, double, bool, Complex[]> PRMSNUp;
    public static Func<Complex, double, CSqMatrix> A = (Complex alp, double w) =>
        {
            double kt1 = k1(w), kt2 = k2(w);
            Complex al = alp * alp,ai=/*al**/Complex.I;
            Complex s1 = sigma(al, kt1), s2 = sigma(al, kt2);
            Complex a = (al - 0.5*kt2);//*mu2;
            Complex b = s1*ai;// * mu2;// *();
            Complex c = al *s2;// * mu2;
            Complex d = -a*ai;// * (al * Complex.I);
            //Complex q = Complex.Exp(s1 * h), ww = Complex.Exp(s2 * h);
            Complex q = Complex.Exp(-s1 * h), ww = Complex.Exp(-s2 * h);
            //$"{a} {b} {c} {d} {q} {ww}".Show();

            //$"sigm {s1} {s2}".Show();
            //$"exp {q} {ww}".Show();
            //$"elem a= {a*mu2} b= {b * mu2} c= {c * mu2} d= {d * mu2}".Show();

            CSqMatrix res =new CSqMatrix(new Complex[,] { 
                {a,a*q,c,-c*ww },
                {-b,b*q,d,d*ww },
                {a*q ,a,c*ww,-c },
                {-b*q,b,d*ww, d}
            });
            //res.ReversColumns(2, 3);
            return res;
        };

    #endregion

    #region Функции для uxt
    /// <summary>
    /// Возвращает вектор преобразований Фурье от шапочек
    /// </summary>
    public static Func<double[], double, CVectors> Fi = (double[] w, double t) => {
        double dw = w[1] - w[0];
        Complex it = Complex.I / t;

        //Debug.WriteLine($"{it * (1 - Complex.Exp(Complex.I * dw * t))/dw}");

        Complex left(int i) => it * Complex.Exp(-Complex.I * w[i] * t) * (1 - it * (1 - Complex.Exp(Complex.I * dw * t)) / dw);
        Complex right(int i) => it * Complex.Exp(-Complex.I * w[i] * t) * (-1 - it * (1 - Complex.Exp(-Complex.I * dw * t)) / dw);
        Complex sum(int i) => Complex.Exp(-Complex.I * w[i] * t) / t / t / dw * (1.0 - Complex.Ch(Complex.I * dw * t)) / 2;

        CVectors r = new CVectors(w.Length);
        r[0] = right(0);
        r[w.Length - 1] = left(w.Length - 1);
        for (int i = 1; i < w.Length - 1; i++)
            r[i] = sum(i);
        return r;

    };

    public static Func<double, double, double,  Normal2D[], Complex[]> _uRes = (double x, double y,  double w, Normal2D[] normal) => KsumRes(x, y, w, normal, (Point t) =>  new Vectors(t.x, t.y, 0.0)).ComplexMas;

    public static Func<double, double, double,  Normal2D[], Complex[]> uRes;
    /// <summary>
    /// Итоговая функция (через вычисленные массивы w и f(w)) для одного источника
    /// </summary>
    public static Func<double, double, double, Tuple<double[], Complex[]>, Normal2D[], double[]> UxtOne = (double x, double y,  double t, Tuple<double[], Complex[]> tuple, Normal2D[] normal) =>
    {
        double[] w = tuple.Item1;
        Complex[] fw = tuple.Item2;

        CVectors[] c = new CVectors[wcount];
        //tex: ${\bar c}= f({\bar w}) \cdot u(x,y,z,{\bar w}) $ покомпонентно
        Parallel.For(0, wcount, (int i) =>
        {
            c[i] = new CVectors(Expendator.Mult(uRes(x, y, w[i], normal), fw[i]));
        });
        return ((c * Fi(w, t)).Re / Math.PI).DoubleMas;
    };

    /// <summary>
    /// Функция u(x,t) по массиву источников
    /// </summary>
    public static Func<double, double,  double, Source[], Tuple<double, double>> Uxt = (double x, double y,  double t, Source[] smas) => {

        Tuple<double, double> res = new Tuple<double, double>(0,0);
        double[] tmp;
        double cor;

        for(int i = 0; i < smas.Length; i++)
        {
            tmp = UxtOne(x, y, t, smas[i].Fmas, smas[i].Norms);
             cor = new Number.Complex(x - smas[i].Center.x, y -smas[i].Center.y).Arg;
            res = new Tuple<double, double>(res.Item1+tmp[0]*Math.Cos(cor)+tmp[1]*Math.Sin(cor), res.Item2+tmp[2]);
        }

        return res;
    };

    #endregion
}

public static class OtherMethods
{
    /// <summary>
    /// Число сохранённых значений
    /// </summary>
    public static int Saved=0;
    /// <summary>
    /// Число значений, которое надо сохранить вообще
    /// </summary>
    public static int SaveCount=1;

    /// <summary>
    /// Просто сохранять значения u(x,w), чтобы не тратить время зря
    /// </summary>
    /// <param name="x0"></param>
    /// <param name="X"></param>
    /// <param name="xcount"></param>
    /// <param name="y0"></param>
    /// <param name="Y"></param>
    /// <param name="ycount"></param>
    /// <param name="smas"></param>
    public static void Saveuxw(double x0, double X, int xcount, double y0, double Y, int ycount, Source[] smas)
    {
        SaveCount = xcount * ycount * smas.Length;
        var w = Expendator.Seq(wbeg, wend, wcount);
        var x = Expendator.Seq(x0, X, xcount);
        var y = Expendator.Seq(y0, Y, ycount);

        for (int i = 0; i < xcount; i++)
            for (int j = 0; j < ycount; j++)
                for (int s = 0; s < smas.Length; s++)
                {
                //Parallel.For(0, wcount, (int k) => ur.OnlyAdd(new Tuple<double, double, double, Normal2D[]>(x[i],y[j],w[k],smas[s].Norms) ,_uRes(x[i], y[j], w[k], smas[s].Norms)));
                    Parallel.For(0, wcount, (int k) => uRes(x[i], y[j], w[k], smas[s].Norms));
                    Saved++;
                }
        Saved = 0;
    }
    public static void Saveuxw2(double x0, double X, int xcount, double y0, double Y, int ycount, Source[] smas)
    {
       DataToFiles(x0, X, xcount, y0, Y, ycount, smas);
    }

    /// <summary>
    /// Записать основные данные в файл для последующего вызова скрипта
    /// </summary>
    /// <param name="x0"></param>
    /// <param name="X"></param>
    /// <param name="xcount"></param>
    /// <param name="y0"></param>
    /// <param name="Y"></param>
    /// <param name="ycount"></param>
    /// <param name="smas"></param>
    public static void DataToFiles(double x0, double X, int xcount, double y0, double Y, int ycount, Source[] smas)
    {
        var w = Expendator.Seq(wbeg, wend, wcount);

        Debug.WriteLine("Происходит запись файлов");

        Parallel.Invoke(
            ()=>Space(x0,X,xcount,y0,Y,smas),
            ()=>Poles(w),
            ()=>Normals(smas)
            );

        Debug.WriteLine("Файлы записаны. Выпзывается скрипт на D");

        Process process = new Process();
        process.StartInfo.FileName = "dscript.exe";//исправить
        process.Start();
        process.WaitForExit();

        Debug.WriteLine("Данные считываются");
        ReadData(x0,X,xcount,y0,Y,smas);
        Debug.WriteLine("Данные считаны");
    }

    private static void Space(double x0, double X, int xcount, double y0, double Y,Source[] smas)
    {
        int sourceCount = smas.Length;
        using (StreamWriter f=new StreamWriter("Space.txt"))
        {
            f.WriteLine($"xmin= {x0}".Replace(',','.'));
            f.WriteLine($"ymin= {y0}".Replace(',', '.'));
            f.WriteLine($"xmax= {X}".Replace(',', '.'));
            f.WriteLine($"ymax= {Y}".Replace(',', '.'));
            f.WriteLine($"countS= {xcount}".Replace(',', '.'));
            f.WriteLine($"countW= {wcount}".Replace(',', '.'));
            f.Write($"sourceCount= {sourceCount}");
            for (int i = 0; i < sourceCount; i++)
                f.Write($" {smas[i].Norms.Length}");
            f.WriteLine();

            f.WriteLine($"wbeg= {wbeg}".Replace(',', '.'));
            f.WriteLine($"wend= {wend}".Replace(',', '.'));
            f.WriteLine($"lamda= {lamda}".Replace(',', '.'));
            f.WriteLine($"mu= {mu}".Replace(',', '.'));
            f.WriteLine($"ro= {ro}".Replace(',', '.'));
            f.WriteLine($"h= {h}".Replace(',', '.'));
        }
    }
    private static void Poles(double[] w)
    {
        using (StreamWriter f = new StreamWriter("Poles.txt"))
        {
            string[] s=new string[w.Length];
            Parallel.For(0, w.Length, (int i) => s[i]=PolesPoles(w[i]).ToString().Replace(',', '.'));

            foreach (string c in s)
            {
                f.WriteLine(c.Substring(3,c.Length-6).Replace('\t',' ').Replace("  "," "));
            }
        }
    }
    private static void Normals(Source[] mas)
    {
        using (StreamWriter f = new StreamWriter("Normals.txt"))
        {
            f.WriteLine("number \tposition.x \tposition.y \tn.x \tn.y");
            for (int i = 0; i < mas.Length; i++)
                for (int j = 0; j < mas[i].Norms.Length; j++)
                    f.WriteLine($"{i+1} {mas[i].Norms[j].Position.x} {mas[i].Norms[j].Position.y} {mas[i].Norms[j].n.x} {mas[i].Norms[j].n.y}".Replace(',','.'));
        }
    }
    private static void ReadData(double x0, double X, int xcount, double y0, double Y, Source[] mas)
    {
        Vectors xx = new Vectors(Seqw(x0, X, xcount));
        Vectors yy = new Vectors(Seqw(y0, Y, xcount));
        Vectors w = new Vectors(Seqw(wbeg, wend, wcount));

        using (StreamReader f=new StreamReader("uxw.txt"))
        {
            string s = f.ReadLine();
            s = f.ReadLine();
            double[] st;
            
            while(s!=null && s.Length > 0)
            {
                st=s.Replace('.', ',').ToDoubleMas();

                int ind = (int)st[3] - 1;

                Functions.ur.OnlyAdd(
                    new Tuple<double, double, double, Normal2D[]>(
                       xx.BinaryApproxSearch( st[0]),
                       yy.BinaryApproxSearch( st[1]),
                       w.BinaryApproxSearch( st[2]),
                        mas[ind].Norms
                        ),
                    new Complex[] { new Complex(st[4],st[5]),new Complex(st[6],st[7]),new Complex(st[8],st[9])});

                s = f.ReadLine();
            }
           
            
        }
    }

    public static void StartProcess2(string fileName)
    {
        Process process = new Process();
        process.StartInfo.FileName = fileName;

        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;


        process.EnableRaisingEvents = true;


        process.Start();

        process.WaitForExit();
    }
}
