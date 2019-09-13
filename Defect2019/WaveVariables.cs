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
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Формы, хранящиеся в приложении
/// </summary>
public static class Forms
{
    public static UGrafic UG = new UGrafic();
    public static Uxt Uform = new Uxt();
}

/// <summary>
/// Основные константы программы
/// </summary>
public static class РабКонсоль
{
    #region Параметры DINN
    /// <summary>
    /// Воспроизведение музыки на форме
    /// </summary>
    public static bool DINNplay = false;

    public static double t1 { get; set; } = 0;
    public static double t4 { get; set; } = 15;
    public static double t2 = t1, t3 = t1, tm = 0.02, tp = 0, eps = 1e-6, pr = 1e-3, gr = 1e3;
    /// <summary>
    /// Число узлов в методе Гаусса-Кронрода
    /// </summary>
    public static GaussKronrod.NodesCount NodesCount = GaussKronrod.NodesCount.GK15;
    #endregion

    #region Параметры для дисперсионных кривых
    public static double steproot = 1e-3, polesBeg = 0.0, polesEnd = 15;
    public static double epsjump = 1e-1, epsroot = 1e-3;
    public static int countroot = 50;
    #endregion

    #region Параметры по частотам
    public static double ThU = 1e-3, SpU = 1e3, wc = 0.6283185/*0.1 * 2 * Math.PI*/, _T;
    public static double T => 2 * Math.PI / wc * ThU / SpU;

    public static double wbeg = 0.01256637061435917295385057353312, wend = 1.2566370614359172953850573533118;
    public static int wcount = 400;

    /// <summary>
    /// Текущий массив частот
    /// </summary>
    public static double[] wmas;
    #endregion

    #region Параметры анимации
    /// <summary>
    /// Скорость смены картинок для анимации (в ммсек)
    /// </summary>
    public static int animatime = 70;
    /// <summary>
    /// Число кластеров при создании анимации
    /// </summary>
    public static int clastersCount = 3;
    /// <summary>
    /// Максимальное число циклов анимации
    /// </summary>
    public static int animacycles = 15;
    #endregion
}

/// <summary>
/// Класс с методами самой модели
/// </summary>
public static class Functions
{
    #region Конструктор и константы
    static Functions()
    {
        ReadParams();
        AfterChaigeData();
    }
    public static readonly
        Complex I = Complex.I,
        I2 = new Complex(0, 0.5);
    public static readonly double
        sqrtfrac2pi = Math.Sqrt(2.0 / Math.PI),
    fracpi4 = Math.PI / 4;

    public static void AfterChaigeData()
    {
        mu2 = 2 * mu;
        ml2 = mu2 + lamda;
        im = new Complex(0, mu);

        k2coef = ro / mu;
        k1coef = ro / ml2;
        WriteParams();
        Expendator.WriteStringInFile("ClastersCount.txt", clastersCount.ToString());

        Bessel = new Func<Complex, double, double, Complex[]>(_Bessel);

        prmsnmem = new Memoize<Tuple<Complex, double>, Complex[]>((Tuple<Complex, double> t) => PRMSN(t.Item1, t.Item2));
        var prmsn = prmsnmem.Value;
        PRMSN_Memoized = (Complex a, double w) => prmsn(new Tuple<Complex, double>(a, w));

        var pol = new Memoize<double, Vectors>((double t) => PolesMas(t)).Value;
        PolesMasMemoized = (double x) => pol(x);

        var seq = new Memoize<Tuple<double, double, int>, double[]>((Tuple<double, double, int> t) => SeqW(t.Item1, t.Item2, t.Item3));
        SeqWMemoized = (double a, double b, int c) => seq.Value(new Tuple<double, double, int>(a, b, c));

        ur = new Memoize<Tuple<double, double, double, Source>, Tuple<Complex, Complex>>((Tuple<double, double, double, Source> t) => uxw(t.Item1, t.Item2, t.Item3, t.Item4));
        uxwMemoized = (double x, double y, double w, Source n) => ur.Value(new Tuple<double, double, double, Source>(x, y, w, n));

        cmas = new Memoize<Tuple<double, double, Source>, Tuple<Complex, Complex>[]>((Tuple<double, double, Source> t) => CMAS(t.Item1, t.Item2, t.Item3));
        CMAS_Memoized = (double x, double y, Source s) => cmas.Value(new Tuple<double, double, Source>(x, y, s));

        wmas = SeqWMemoized(РабКонсоль.wbeg, РабКонсоль.wend, РабКонсоль.wcount);
        var fs = new Memoize<double, CVectors>((double t) => Fi(wmas, t));
        FiMemoized = (double t) => fs.Value(t);

        var cs = new Memoize<Tuple<Complex[], double>, CVectors>((Tuple<Complex[], double> t) => t.Item1 * FiMemoized(t.Item2));
        Phif = (Complex[] fw, double t) => cs.Value(new Tuple<Complex[], double>(fw, t));       
    }
    public static Memoize<Tuple<double, double, double, Source>, Tuple<Complex, Complex>> ur;
    public static Memoize<Tuple<double, double, Source>, Tuple<Complex, Complex>[]> cmas;
    public static Memoize<Tuple<Complex, double>, Complex[]> prmsnmem;

    public static Func<double, double, int, double[]> SeqW = (double wbeg, double wend, int count) => Expendator.Seq(wbeg, wend, count);
    public static Func<double, double, int, double[]> SeqWMemoized;


    public static double lamda = 51.0835913312694, mu = 26.3157894736842, ro = 2.7, h = 2.0, crosscoef = 0.2;
    public static double ml2, mu2, k1coef, k2coef;
    private static Complex im;
    private static void WriteParams()
    {
        using(StreamWriter f=new StreamWriter("LastParamsConfig.txt"))
        {
            f.WriteLine($"lambda= {lamda.ToRString()}");
            f.WriteLine($"mu= {mu.ToRString()}");
            f.WriteLine($"ro= {ro.ToRString()}");
            f.WriteLine($"h= {h.ToRString()}");
            f.WriteLine($"steproot= {steproot.ToRString()}");
            f.WriteLine($"polesBeg= {polesBeg.ToRString()}");
            f.WriteLine($"polesEnd= {polesEnd.ToRString()}");
            f.WriteLine($"epsjump= {epsjump.ToRString()}");
            f.WriteLine($"epsroot= {epsroot.ToRString()}");
            f.WriteLine($"countroot= {countroot}");
            f.WriteLine($"wbeg= {wbeg.ToRString()}");
            f.WriteLine($"wend= {wend.ToRString()}");
            f.WriteLine($"wcount= {wcount}");
            f.WriteLine($"wc= {wc}");
            f.WriteLine($"animatime= {animatime}");
            f.WriteLine($"clastersCount= {clastersCount}");
            f.WriteLine($"animacycles= {animacycles}");
        }
    }
    private static void ReadParams()
    {
        string filename = "LastParamsConfig.txt";
        
        if (File.Exists(filename))
        using(StreamReader f=new StreamReader(filename))
            {
                int ToInt() => f.ReadLine().Split(' ')[1].ToInt32();
                double ToDouble()=> f.ReadLine().Split(' ')[1].ToDouble();

                lamda = ToDouble();
                mu = ToDouble();
                ro = ToDouble();
                h = ToDouble();
                steproot = ToDouble();
                polesBeg = ToDouble();
                polesEnd = ToDouble();
                epsjump = ToDouble();
                epsroot = ToDouble();
                countroot = ToInt();
                wbeg = ToDouble();
                wend = ToDouble();
                wcount = ToInt();
                wc = ToDouble();
                animatime = ToInt();
                clastersCount = ToInt();
                animacycles = ToInt();
            }             
    }

    #endregion

    #region Простейшие функции
    public static RealFunc k1 = (double w) => w * w * k1coef;
    public static RealFunc k2 = (double w) => w * w * k2coef;
    public static Func<Complex, double, Complex> sigma = (Complex a, double kw) => Complex.Sqrt((a - kw)) * Math.Sign(a.Abs - kw);

    public static Func<double, Complex> F1 = (double w) =>
    {
        Complex pi = Complex.I * Math.PI / wc;

        double w1 = 5 * wc + 4 * w, w2 = 3 * wc + 4 * w, w3 = -3 * wc + 4 * w, w4 = -5 * wc + 4 * w;
        Complex e(double c) => (Complex.Exp(pi * c) - 1) / c;

        return Complex.I * (e(w1) - e(w2) - e(w3) + e(w4));
    };
    public static Func<double, Complex> F2 = (double w) =>
    {
        int N = 7;

        double w1 = wc * (N + 1) / N + w, w2 = wc * (N - 1) / N + w, w3 = wc * (1 - N) / N + w, w4 = 2 - wc * (N + 1) / N, w5 = wc + w, w6 = w - wc;
        Complex ew(double t, double ww) => Complex.Exp(Complex.I * t * ww) / ww;
        Complex perv(double t) => ew(t, w1) + ew(t, w2) - ew(t, w3) - ew(t, w4) - 2 * (ew(t, w5) - ew(t, w6));

        return (perv(2 * Math.PI * N / wc) - perv(0)) / 8;
    };

    /// <summary>
    /// Получить тестовый массив f(w) по глобальным данным
    /// </summary>
    public static Func<Complex[]> GetFmas=()=> SeqWMemoized(wbeg, wend, wcount).Map((double d) => Functions.F1(d) + new Number.Complex(RandomNumbers.NextDouble2(0, 1e-7), RandomNumbers.NextDouble2(0, 1e-7)));

    #endregion

    #region Функции знаменателя, его производных и корней

    public static double epsforder => РабКонсоль.eps;

    /// <summary>
    /// Функция знаменателя, выраженная явно
    /// </summary>
    public static Func<Complex, double, Complex> Deltass = (Complex alp, double w) =>
        {

            double kt1 = k1(w), kt2 = k2(w);
            Complex al = alp * alp, ai = Complex.I;
            Complex s1 = sigma(al, kt1), s2 = sigma(al, kt2);
            Complex a = (al - 0.5 * kt2);
            Complex b = s1 * ai;
            Complex c = al * s2;
            Complex d = -a * ai;
            Complex q = Complex.Exp(-s1 * h), ww = Complex.Exp(-s2 * h);

            Complex ad = a * d, bc = b * c;
            return -((q * ww).Sqr() + 1.0) * (ad + bc).Sqr() + (ad * q + bc * ww).Sqr() + (ad * ww + bc * q).Sqr() - 2 * ad * bc * (q - ww).Sqr();
        };
    /// <summary>
    /// Дополнительный знаменатель, полученный от N
    /// </summary>
    public static Func<Complex, double, Complex> DeltassN = (Complex alp, double w) =>
    {
        double kt2 = k2(w);
        Complex al = alp * alp;
        Complex s2 = sigma(al, kt2);
        return s2 * Complex.Sh(s2 * h);
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
    /// Возвращает массив полюсов при такой-то частоте
    /// </summary>
    private static Func<double, Vectors> PolesMas = (double w) =>
       {
           ComplexFunc del = (Complex a) => Deltass(a, w);
           Vectors v1 = w < 0.1 ? Roots.OtherMethod(del, РабКонсоль.polesBeg, РабКонсоль.polesEnd, РабКонсоль.steproot / 200, 1e-12, Roots.MethodRoot.Brent, false) : Roots.OtherMethod(del, РабКонсоль.polesBeg, РабКонсоль.polesEnd, РабКонсоль.steproot, 1e-10, Roots.MethodRoot.Brent, false);
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
    /// Мемоизированная PolesMas
    /// </summary>
    public static Func<double, Vectors> PolesMasMemoized;
    #endregion
    //-----
    #region Матрица K

    /// <summary>
    /// Матрица Грина как функция альфы, частоты и координат
    /// </summary>
    public static Func<Complex, double, double, double, CSqMatrix> K = (Complex a, double x, double y, double w) =>
   {
       var c = PRMSN_Memoized(a, w);
       Complex ar = a * Math.Sqrt(x * x + y * y);
       Tuple<Complex, Complex> tup = new Tuple<Complex, Complex>(МатКлассы.SpecialFunctions.MyBessel(1, ar), МатКлассы.SpecialFunctions.MyBessel(0, ar));
       return InK(a, c, tup, x, y);
   };
    /// <summary>
    /// Матрица Грина при наборе нормалей
    /// </summary>
    public static Func<Complex, double, double, double, Normal2D[], Func<Point, CVectors>, CVectors> Ksum = (Complex a, double x, double y, double w, Normal2D[] nd, Func<Point, CVectors> Q) =>
       {
           var c = PRMSN_Memoized(a, w);
           Complex ar;
           Tuple<Complex, Complex> tup;
           CVectors mat = new CVectors(3);
           Point xy = new Point(x, y);

           for (int i = 0; i < nd.Length; i++)
           {
               ar = a * Point.Eudistance(nd[i].Position, xy);
               tup = new Tuple<Complex, Complex>(МатКлассы.SpecialFunctions.MyBessel(1, ar), МатКлассы.SpecialFunctions.MyBessel(0, ar));
               mat.FastAdd(InK(a, c, tup, x - nd[i].Position.x, y - nd[i].Position.y) * Q(nd[i].n));
           }
           return mat;
       };

    /// <summary>
    /// Матрица Грина при наборе нормалей
    /// </summary>
    public static Func<double, double, double, Normal2D[], Func<Point, Vectors>, CVectors> KsumRes = (double x, double y, double w, Normal2D[] nd, Func<Point, Vectors> Q) =>
   {
       var poles = PolesMasMemoized(w);
       Complex[][] c1 = new Complex[poles.Deg][], c2 = new Complex[poles.Deg][];
       CVectors sum = new CVectors(3);
       Complex[] res = new Complex[9];
       Point xy = new Point(x, y);
       Vectors QQ;

       double dist = Vectors.Union2(new Vectors(0.0), poles).MinDist, xp, yp;

       double eps = dist * crosscoef, eps2 = 0.5 * eps;
       double[] plus = new double[poles.Deg], pminus = new double[poles.Deg];

       for (int k = 0; k < poles.Deg; k++)
       {
           plus[k] = poles[k] + eps;
           pminus[k] = poles[k] - eps;
           c1[k] = PRMSN_Memoized(plus[k], w);
           c2[k] = PRMSN_Memoized(pminus[k], w);
       }

       for (int i = 0; i < nd.Length; i++)
       {
           QQ = Q(nd[i].n) * eps2;

           xp = x - nd[i].Position.x;
           yp = y - nd[i].Position.y;

           for (int k = 0; k < poles.Deg; k++)
           {
               InKtwiceFast(plus[k], pminus[k], c1[k], c2[k], HankelTuple(poles[k] * Point.Eudistance(nd[i].Position, xy)), xp, yp, ref res);
               sum.FastAdd(KQmult(res, QQ));
           }
       }
       return sum * I2;
   };

    public static Func<Complex, Complex[], Tuple<Complex, Complex>, double, double, CSqMatrix> InK = (Complex a, Complex[] PRMSN_Memoized, Tuple<Complex, Complex> beshank, double x, double y) =>
         {
             double x2 = x * x, y2 = y * y, r2 = x2 + y2, r = Math.Sqrt(r2);
             Complex ar = a * r, a2 = a * a;
             Complex j1ar = beshank.Item1, j0ar = beshank.Item2;

             Complex P = PRMSN_Memoized[0], R = PRMSN_Memoized[1], Mi = PRMSN_Memoized[2] * I, Si = PRMSN_Memoized[3] * I, Ni = PRMSN_Memoized[4] * I;
             Complex
               j1arr = j1ar / r,
               jx = -x * j1arr,
               jy = -y * j1arr,
               j0ara = j0ar * a,
               jtmp = j1arr * (x2 - y2),
               jxx = -(j0ara * x2 - jtmp) / r2,
               jxy = -x * y / r2 * (j0ara - 2 * j1arr),
               jyy = -(j0ara * y2 + jtmp) / r2;

             Complex
             K11 = (Mi * jxx + Ni * jyy),
             K12 = (Mi - Ni) * jxy,
             K13 = P * jx * a2,
             K22 = (Mi * jyy + Ni * jxx),
             K23 = P * jy * a2,
             K31 = Si * jx,
             K32 = Si * jy,
             K33 = R * j0ara;
             return new CSqMatrix(new Complex[3, 3] {
                 {K11,K12, K13},
             { K12,K22,K23},
             { K31,K32,K33}
                 });
         };

    public static Func<Complex, Complex, Complex[], Complex[], Tuple<Complex, Complex>, double, double, Complex[]> InKtwice = (Complex a1, Complex a2, Complex[] PRMSN1, Complex[] PRMSN2, Tuple<Complex, Complex> beshank, double x, double y) =>
      {

          double x2 = x * x, y2 = y * y, r2 = x2 + y2, r = Math.Sqrt(r2), xy = x * y;
          Complex ar1 = a1 * r, a21 = a1 * a1;
          Complex ar2 = a2 * r, a22 = a2 * a2;

          Complex j1ar = beshank.Item1, j0ar = beshank.Item2;

          Complex P = PRMSN1[0] * a21 - PRMSN2[0] * a22, R1 = PRMSN1[1], 
          Mi1 = PRMSN1[2] * I, Si = (PRMSN1[3] - PRMSN2[3]) * I, Ni1 = PRMSN1[4] * I;
          Complex R2 = PRMSN2[1], Mi2 = PRMSN2[2] * I, Ni2 = PRMSN2[4] * I;

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

          return new Complex[] {
                 ((Mi1 * jxx1 + Ni1 * jyy1)- (Mi2 * jxx2 + Ni2 * jyy2)) / r2,K12, P*jx,
              K12, ((Mi1 * jyy1 + Ni1 * jxx1)- (Mi2 * jyy2 + Ni2 * jxx2)) / r2,
              P * jy, Si * jx,Si * jy,R1 * j0ara1- R2 * j0ara2
  };
      };
    public static void InKtwiceFast(Complex a1, Complex a2, Complex[] PRMSN1, Complex[] PRMSN2, Tuple<Complex, Complex> beshank, double x, double y, ref Complex[] res)
    {

        double x2 = x * x, y2 = y * y, r2 = x2 + y2, r = Math.Sqrt(r2), xy = x * y;
        Complex ar1 = a1 * r, a21 = a1 * a1;
        Complex ar2 = a2 * r, a22 = a2 * a2;

        Complex j1ar = beshank.Item1, j0ar = beshank.Item2;

        Complex P = PRMSN1[0] * a21 - PRMSN2[0] * a22, R1 = PRMSN1[1], Mi1 = PRMSN1[2] * I, Si = (PRMSN1[3] - PRMSN2[3]) * I, Ni1 = PRMSN1[4] * I;
        Complex R2 = PRMSN2[1], Mi2 = PRMSN2[2] * I, Ni2 = PRMSN2[4] * I;

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

        res[0] = ((Mi1 * jxx1 + Ni1 * jyy1) - (Mi2 * jxx2 + Ni2 * jyy2)) / r2;
        res[1] = K12;
        res[2] = P * jx;
        res[3] = K12;
        res[4] = ((Mi1 * jyy1 + Ni1 * jxx1) - (Mi2 * jyy2 + Ni2 * jxx2)) / r2;
        res[5] = P * jy;
        res[6] = Si * jx;
        res[7] = Si * jy;
        res[8] = R1 * j0ara1 - R2 * j0ara2;
    }

    /// <summary>
    /// Быстрое произведение нужных матриц и векторов с учётом их структуры
    /// </summary>
    /// <param name="M"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static CVectors KQmult(CSqMatrix M, Vectors v)
    {
        CVectors r = new CVectors(3);
        for (int k = 0; k < 3; k++)
            r[k] = M[k, 0] * v[0] + M[k, 1] * v[1];
        return r;
    }
    public static CVectors KQmult(Complex[] M, Vectors v)
    {
        CVectors r = new CVectors(3);
        r[0] = M[0] * v[0] + M[1] * v[1];
        r[1] = M[3] * v[0] + M[4] * v[1];
        r[2] = M[6] * v[0] + M[7] * v[1];
        return r;
    }

    /// <summary>
    /// Быстрое произведение нужных матриц и векторов с учётом их структуры
    /// </summary>
    /// <param name="M"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static CVectors KQmult(Complex[,] M, Vectors v) => new CVectors(new Complex[3] { M[0, 0] * v[0] + M[0, 1] * v[1], M[1, 0] * v[0] + M[1, 1] * v[1], M[2, 0] * v[0] + M[2, 1] * v[1] });

    #endregion

    #region Функции для определения коэффициентов матрицы K
    private static Complex[] _Bessel(Complex a, double x, double y)
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
        double x2 = x * x, y2 = y * y, r = Math.Sqrt(x2 + y2), r2 = x2 + y2;
        Complex ar = a * r, a_r = a / r, a_r2 = a_r * a_r, a_rr = a_r / r;
        Complex j1ar = МатКлассы.SpecialFunctions.MyBessel(1, ar), j0ar = МатКлассы.SpecialFunctions.MyBessel(0, ar);

        //(a).Show();

        jx = -j1ar * x * a_r;
        jy = -j1ar * y * a_r;
        jxx = -a_rr * (j0ar * a * x2 + r * j1ar);
        jyy = -a_rr * (j0ar * a * y2 + r * j1ar);
        jxy = -a_r2 * x * y * j0ar;

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
    public static Func<Complex, double, double, Complex[]> Bessel;
    public static Func<double, double, Complex> Hankel;

    /// <summary>
    /// Возвращает первые два столбца из обратной матрицы
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Tuple<CVectors, CVectors> Arev(Complex al, double w)
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
        CVectors t1 = new CVectors(new Complex[] { Mat.GetMinMat(1, 1).Det, -Mat.GetMinMat(1, 2).Det, Mat.GetMinMat(1, 3).Det, -Mat.GetMinMat(1, 4).Det });
        CVectors t2 = new CVectors(new Complex[] { -Mat.GetMinMat(2, 1).Det, Mat.GetMinMat(2, 2).Det, -Mat.GetMinMat(2, 3).Det, Mat.GetMinMat(2, 4).Det });
        return new Tuple<CVectors, CVectors>(t1, t2);
    }

    /// <summary>
    /// Возвращает компоненты PRMSN_Memoized, нужные для матрицы Грина
    /// </summary>
    /// <param name="al"></param>
    /// <param name="z"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Complex[] PRMSN(Complex al, double w)
    {
        var c = Arev(al, w);
        CVectors v1 = c.Item1, v2 = c.Item2;
        Complex alp = al * al, s1 = sigma(alp, k1(w)), s2 = sigma(alp, k2(w)), ai = (alp * Complex.I);
        Complex e1 = 1, e3 = 1, e2 = Complex.Exp(-s1 * (h)), e4 = Complex.Exp(-s2 * (h));

        CVectors c1 = new CVectors(new Complex[] { e1, /*ai**/e2, s2 * e3, -s2 * e4/**ai*/}) / mu2, c2 = new CVectors(new Complex[] { s1 * e1, -s1 * e2/**ai*/, alp * e3, alp * e4/**ai*/}) / mu2;
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
        Complex N = Complex.I * Complex.Cth(s2 * h) / (mu * /*alp **/ s2);


        //$"{P} {R} {M} {S} \t{N} \t{del} \t{al}".Show();
        return new Complex[] { P/*/del*/, R/*/del*/, M/*/del*/, S/*/del*/, N/*/DeltassN(al,w) */};
    }

    /// <summary>
    /// Возвращает компоненты PRMSN_Memoized, нужные для матрицы Грина (мемоизированная)
    /// </summary>
    /// <param name="al"></param>
    /// <param name="z"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static Func<Complex, double, Complex[]> PRMSN_Memoized;
    public static Func<Complex, double, CSqMatrix> A = (Complex alp, double w) =>
        {
            double kt1 = k1(w), kt2 = k2(w);
            Complex al = alp * alp, ai =/*al**/Complex.I;
            Complex s1 = sigma(al, kt1), s2 = sigma(al, kt2);
            Complex a = (al - 0.5 * kt2);//*mu2;
            Complex b = s1 * ai;// * mu2;// *();
            Complex c = al * s2;// * mu2;
            Complex d = -a * ai;// * (al * Complex.I);
            //Complex q = Complex.Exp(s1 * h), ww = Complex.Exp(s2 * h);
            Complex q = Complex.Exp(-s1 * h), ww = Complex.Exp(-s2 * h);
            //$"{a} {b} {c} {d} {q} {ww}".Show();

            //$"sigm {s1} {s2}".Show();
            //$"exp {q} {ww}".Show();
            //$"elem a= {a*mu2} b= {b * mu2} c= {c * mu2} d= {d * mu2}".Show();

            CSqMatrix res = new CSqMatrix(new Complex[,] {
                {a,a*q,c,-c*ww },
                {-b,b*q,d,d*ww },
                {a*q ,a,c*ww,-c },
                {-b*q,b,d*ww, d}
            });
            //res.ReversColumns(2, 3);
            return res;
        };

    #endregion

    #region Функции Ханкеля
    /// <summary>
    /// Функция Ханкеля с умножением на корень (этот корень сокращается со знаменателем)
    /// </summary>
    public static Func<double, Tuple<Complex, Complex>> HankelTuple/*Old*/ = (double ar) =>
    {
        return new Tuple<Complex, Complex>(sqrtfrac2pi * Complex.Expi(ar + fracpi4), sqrtfrac2pi * Complex.Expi(ar - fracpi4));
    };

    /// <summary>
    /// Функция Ханкеля без умножения на корень
    /// </summary>
    public static Func<double, Tuple<Complex, Complex>> HankelTupleClear = (double ar) =>
    {
        double arsqrt = Math.Sqrt(ar);
        return new Tuple<Complex, Complex>(sqrtfrac2pi / arsqrt * Complex.Expi(ar + fracpi4), sqrtfrac2pi / arsqrt * Complex.Expi(ar - fracpi4));
    };

    /// <summary>
    /// Функция Ханкеля с умножением на корень и срезом для ИЛЮШИ
    /// </summary>
    public static Func<double, Tuple<Complex, Complex>> HankelTupleИлюшаСюдаСмотри = (double ar) =>
    {
        var sd = SheringFunction.GetSheredFunction((double t) => 1.0, 0, 10, 0.4);
        double tmp = sd(ar);
        return new Tuple<Complex, Complex>(sqrtfrac2pi * tmp * Complex.Expi(ar + fracpi4), sqrtfrac2pi * tmp * Complex.Expi(ar - fracpi4));
    };

    #endregion

    #region Функции для uxt
    /// <summary>
    /// Возвращает вектор преобразований Фурье от шапочек
    /// </summary>
    public static Func<double[], double, CVectors> Fi = (double[] w, double t) =>
    {
        double dw = w[1] - w[0];
        Complex it = Complex.I / t;

        Complex left(int i) => it * Complex.Expi(-w[i] * t) * (1 - it * (1 - Complex.Expi(dw * t)) / dw);
        Complex right(int i) => it * Complex.Expi(-w[i] * t) * (-1 - it * (1 - Complex.Expi(-dw * t)) / dw);
        Complex sum(int i) => Complex.Expi(-w[i] * t) / t / t / dw * (2.0 - 2.0 * Math.Cos(t * dw));

        CVectors r = new CVectors(w.Length);
        r[0] = right(0);
        r[w.Length - 1] = left(w.Length - 1);
        for (int i = 1; i < w.Length - 1; i++)
            r[i] = sum(i);
        return r;

    };
    public static Func<double, CVectors> FiMemoized;
    public static Func<Complex[], double, CVectors> Phif;

    private static Tuple<Complex, Complex> ToURUZ(CVectors v, Source s, double x, double y)
    {
        double cor = new Number.Complex(x - s.Center.x, y - s.Center.y).Arg;
        return new Tuple<Complex, Complex>(v[0] * Math.Cos(cor) + v[1] * Math.Sin(cor), v[2]);
    }

    public static Func<double, double, double, Source, Tuple<Complex, Complex>> uxw = (double x, double y, double w, Source s) => ToURUZ(KsumRes(x, y, w, s.Norms, (Point t) => new Vectors(t.x, t.y, 0.0)), s, x, y);

    public static Func<double, double, double, Source, Tuple<Complex, Complex>> uxwMemoized;

    /// <summary>
    /// tex: ${\bar c}= f({\bar w}) \cdot u(x,y,z,{\bar w}) $ покомпонентно
    /// </summary>
    public static Func<double, double, Source, Tuple<Complex, Complex>[]> CMAS = (double x, double y, Source s) =>
   {
       Tuple<Complex, Complex>[] c = new Tuple<Complex, Complex>[wcount];
      //tex: ${\bar c}= f({\bar w}) \cdot u(x,y,z,{\bar w}) $ покомпонентно

      //Parallel.For(0, wcount, (int i) =>
      for (int i = 0; i < wcount; i++)
       {
           c[i] = Expendator.Mult(uxwMemoized(x, y, wmas[i], s), s.Fmas[i]);
       }
      //);

      return c;
   };

    /// <summary>
    /// Мемоизированная функция вычисления коэффициентов
    /// </summary>
    public static Func<double, double, Source, Tuple<Complex, Complex>[]> CMAS_Memoized;

    public static Func<double, double, double, Source, Tuple<Complex, Complex>> TripleMult = (double x, double y, double t, Source s) =>
         {
             Tuple<Complex, Complex> v;
             Complex tmp1 = 0, tmp2 = 0;
             double[] w = wmas;
             Complex[] fw = s.Fmas;
             CVectors ft = Fi(w, t);

             for (int i = 0; i < wcount; i++)
             {
                 v = Expendator.Mult(uxwMemoized(x, y, w[i], s), fw[i] * ft[i]);
                 tmp1 += v.Item1;
                 tmp2 += v.Item2;
             }

             return new Tuple<Complex, Complex>(tmp1, tmp2);
         };

    /// <summary>
    /// tex: ${\bar c}= f({\bar w}) \cdot u(x,y,z,{\bar w}) $ покомпонентно
    /// </summary>
    private static Func<double, double, double, Source, Tuple<Complex, Complex>> Integraluxt = (double x, double y, double t, Source s) =>
      {
          var p = Phif(s.Fmas, t);
          Complex t1 = 0, t2 = 0;
          Tuple<Complex, Complex> tmp;

          for (int i = 0; i < wcount; i++)
          {
              tmp = Expendator.Mult(uxwMemoized(x, y, wmas[i], s), p[i]);
              t1 += tmp.Item1;
              t2 += tmp.Item2;
          }

          return new Tuple<Complex, Complex>(t1, t2);
      };


    /// <summary>
    /// Итоговая функция (через вычисленные массивы w и f(w)) для одного источника
    /// </summary>
    public static Func<double, double, double, Source, Tuple<Complex, Complex>> UxtOne = (double x, double y, double t, Source s) =>
    {
       //return ((Integraluxt(x,y,t,tuple,normal)).Re / Math.PI).DoubleMas;

       return ((CMAS_Memoized(x, y, s) * FiMemoized(t)));//этот вариант почему-то самый быстрый

       //double[] w = tuple.Item1;
       //return ((CMAS_Memoized(x, y, tuple, normal) * Fi(w, t)).Re / Math.PI).DoubleMas;

       // return ((TripleMult(x,y,t,tuple,normal)).Re / Math.PI).DoubleMas;
   };

    /// <summary>
    /// Функция u(x,t) по массиву источников
    /// </summary>
    public static Func<double, double, double, Source[], Tuple<double, double>> Uxt3 = (double x, double y, double t, Source[] smas) =>
    {
        Tuple<Complex, Complex> tmp;
        double d1 = 0, d2 = 0;

        for (int i = 0; i < smas.Length; i++)
        {
            tmp = UxtOne(x, y, t, smas[i]);
            d1 += tmp.Item1.Re;
            d2 += tmp.Item2.Re;
        }
        return new Tuple<double, double>(d1 / Math.PI, d2 / Math.PI);
    };
    /// <summary>
    /// Функция u(x,t) по массиву источников
    /// </summary>
    public static Func<double, double, double, Source[], Tuple<double, double>> Uxt2 = (double x, double y, double t, Source[] smas) =>
    {
        Tuple<Complex, Complex> tmp;
        Complex c1 = 0, c2 = 0;

        for (int i = 0; i < smas.Length; i++)
        {
            tmp = UxtOne(x, y, t, smas[i]);

            c1 += tmp.Item1;
            c2 += tmp.Item2;
        }
        return new Tuple<double, double>(c1.Abs, c2.Abs);
    };
    /// <summary>
    /// Функция u(x,t) по массиву источников
    /// </summary>
    public static Func<double, double, double, Source[], Tuple<double, double>> Uxt1 = (double x, double y, double t, Source[] smas) =>
    {
        double d1 = 0, d2 = 0;
        Tuple<Complex, Complex> tmp;

        for (int i = 0; i < smas.Length; i++)
        {
            tmp = UxtOne(x, y, t, smas[i]);
            d1 += tmp.Item1.Re;
            d2 += tmp.Item2.Re;
        }
        return new Tuple<double, double>((d1 / Math.PI).Sqr(), (d2 / Math.PI).Sqr());
    };


    #endregion
}

public static class OtherMethods
{
    /// <summary>
    /// Число сохранённых значений
    /// </summary>
    public static int Saved = 0;
    /// <summary>
    /// Число значений, которое надо сохранить вообще
    /// </summary>
    public static int SaveCount = 1;
    /// <summary>
    /// Информация о том, что сейчас происходит
    /// </summary>
    public static string info = null;

    /// <summary>
    /// Просто сохранять значения u(x,w), чтобы не тратить время зря (примитивная медленная версия)
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
        Saved = 0;
        SaveCount = xcount * ycount * smas.Length;
        var w = Expendator.Seq(wbeg, wend, wcount);
        var x = Expendator.Seq(x0, X, xcount);
        var y = Expendator.Seq(y0, Y, ycount);

        for (int i = 0; i < xcount; i++)
            for (int j = 0; j < ycount; j++)
                for (int s = 0; s < smas.Length; s++)
                {
                    //Parallel.For(0, wcount, (int k) => ur.OnlyAdd(new Tuple<double, double, double, Normal2D[]>(x[i],y[j],w[k],smas[s].Norms) ,uxw(x[i], y[j], w[k], smas[s].Norms)));
                    Parallel.For(0, wcount, (int k) => uxwMemoized(x[i], y[j], w[k], smas[s]));
                    Saved++;
                }
        Saved = 0;
    }

    /// <summary>
    /// Сохранить значения по сетке от массива источников (самая быстрая версия)
    /// </summary>
    /// <param name="x0"></param>
    /// <param name="X"></param>
    /// <param name="xcount"></param>
    /// <param name="y0"></param>
    /// <param name="Y"></param>
    /// <param name="ycount"></param>
    /// <param name="smas"></param>
    public static void Saveuxw3(double x0, double X, int xcount, double y0, double Y, Source[] smas)
    {
        bool rs = !EqualConfigs(x0, X, xcount, y0, Y, smas, out Source[] arr);

        if (rs)
        {
            var w = wmas;

            info = "Происходит запись вспомогательных файлов";
            Debug.WriteLine(info);

            Centers(smas);
            //Parallel.Invoke(
            //() => 
            Space(x0, X, xcount, y0, Y, smas);
            //,
            //() => Poles(w),
            //() => Normals(smas)
            // );

            info = "Файлы записаны";
            Debug.WriteLine(info);
            info = null;
            CalcD(x0, X, xcount, y0, Y, smas);
            info = "Происходит сохранение результата, чтобы в другой раз избежать повторных вычислений";
            Debug.WriteLine(info);
            WriteData(smas);
            info = "Результат записан";
            Debug.WriteLine(info);
        }
        else
        {
            if (arr.Length > 0)
            {
                info = "Создание недостающих файлов";
                CalcD(x0, X, xcount, y0, Y, arr);
                info = "Происходит сохранение результата, чтобы в другой раз избежать повторных вычислений";
                Debug.WriteLine(info);
                WriteData(arr);
                info = "Результат записан";
                Debug.WriteLine(info);
            }

            info = "Считываются данные с сохранённых источников";
            Debug.WriteLine(info);
            ReadData(x0, X, xcount, y0, Y, smas.Where(p => !arr.Contains(p)).ToArray());
            info = "Данные считаны";
            Debug.WriteLine(info);
        }
        info = null;
    }
    /// <summary>
    /// Вычислить значения по сетке от массива источников
    /// </summary>
    /// <param name="x0"></param>
    /// <param name="X"></param>
    /// <param name="xcount"></param>
    /// <param name="y0"></param>
    /// <param name="Y"></param>
    /// <param name="ycount"></param>
    /// <param name="smas"></param>
    public static void CalcD(double x0, double X, int xcount, double y0, double Y, Source[] smas)
    {
        Saved = 0;
        SaveCount = xcount * xcount * smas.Length;
        var w = wmas;

        double x, y;
        double xh = (X - x0) / (xcount - 1), yh = (Y - y0) / (xcount - 1);
        sbyte scount = (sbyte)(smas.Length);

        bool b;
        Point xy;
        Point[][][] QQ;

        double[] xp, yp, ros;

        Complex[][][] c1, c2;
        double[] eps2;
        double[][] plus, pminus;
        Vectors[] poles = new Vectors[wcount];
        double cor, sin, cos;

        void firstpolescalc()
        {
            c1 = new Complex[wcount][][];
            c2 = new Complex[wcount][][];
            eps2 = new double[wcount];
            plus = new double[wcount][];
            pminus = new double[wcount][];
            QQ = new Point[smas.Length][][];

            Parallel.For(0, wcount, (int i) =>
            {
                poles[i] = PolesMasMemoized(w[i]);

                c1[i] = new Complex[poles[i].Deg][];
                c2[i] = new Complex[poles[i].Deg][];

                double eps = Vectors.Union2(new Vectors(0.0), poles[i]).MinDist * crosscoef;
                eps2[i] = 0.5 * eps;
                plus[i] = new double[poles[i].Deg];
                pminus[i] = new double[poles[i].Deg];

                for (int k = 0; k < poles[i].Deg; k++)
                {
                    plus[i][k] = poles[i][k] + eps;
                    pminus[i][k] = poles[i][k] - eps;
                    c1[i][k] = PRMSN_Memoized(plus[i][k], w[i]);
                    c2[i][k] = PRMSN_Memoized(pminus[i][k], w[i]);
                }
            });

            Parallel.For(0, smas.Length, (int i) =>
            {
                QQ[i] = new Point[wcount][];
                for (int j = 0; j < wcount; j++)
                {
                    QQ[i][j] = new Point[smas[i].Norms.Length];
                    for (int k = 0; k < smas[i].Norms.Length; k++)
                        QQ[i][j][k] = new Point(eps2[j] * smas[i].Norms[k].n.x, eps2[j] * smas[i].Norms[k].n.y);
                }
            });
        }

        Tuple<Complex, Complex> CALC(Source s, int snumber, int ii)
        {
            Complex[] res = new Complex[9];
            Complex s1 = 0, s2 = 0, s3 = 0;
            Point QQs;
            for (int k = 0; k < poles[ii].Deg; k++)
            {
                for (int i = 0; i < s.Norms.Length; i++)
                {
                    QQs = QQ[snumber][ii][i];

                    //res = InKtwice(plus[ii][k], pminus[ii][k], c1[ii][k], c2[ii][k], HankelTuple(poles[ii][k] * ros[i]), xp[i], yp[i]);
                    InKtwiceFast(plus[ii][k], pminus[ii][k], c1[ii][k], c2[ii][k], HankelTuple(poles[ii][k] * ros[i]), xp[i], yp[i], ref res);
                    s1 += res[0] * QQs.x + res[1] * QQs.y;
                    s2 += res[3] * QQs.x + res[4] * QQs.y;
                    s3 += res[6] * QQs.x + res[7] * QQs.y;
                }
            }
            return new Tuple<Complex, Complex>((s1 * cos + s2 * sin) * I2, s3 * I2);
        }

        firstpolescalc();
        for (int i = 0; i < xcount; i++)
        {
            x = x0 + i * xh;
            for (int j = 0; j < xcount; j++)
            {
                y = y0 + j * yh;
                xy = new Point(x, y);

                //проверить принадлежность точки к какому-либо из источников
                b = false;
                for (int c = 0; c < scount; c++)
                    if (Point.Eudistance(xy, smas[c].Center) <= smas[c].radius * 2.0)
                    {
                        b = true;
                        break;
                    }

                if (!b)
                    for (sbyte s = 0; s < scount; s++)
                    {
                        cor = new Number.Complex(x - smas[s].Center.x, y - smas[s].Center.y).Arg;
                        sin = Math.Sin(cor);
                        cos = Math.Cos(cor);

                        //заполнить массив расстояний, уникальный для источника и точки, но одинаковый при любых нормалях
                        xp = new double[smas[s].Norms.Length];
                        yp = new double[smas[s].Norms.Length];
                        ros = new double[smas[s].Norms.Length];
                        for (int ii = 0; ii < smas[s].Norms.Length; ii++)
                        {
                            xp[ii] = x - smas[s].Norms[ii].Position.x;
                            yp[ii] = y - smas[s].Norms[ii].Position.y;
                            ros[ii] = Point.Eudistance(smas[s].Norms[ii].Position, xy);
                        }

                        Parallel.For(0, wcount, (int k) => ur.OnlyAdd(new Tuple<double, double, double, Source>(x, y, w[k], smas[s]), CALC(smas[s], s, k)));
                        Saved++;
                    }
                else
                    Saved += scount;

            }
        }
    }

    /// <summary>
    /// Просчитать CMAS_Memoized для последующего ускорения 
    /// </summary>
    /// <param name="xmas"></param>
    /// <param name="ymas"></param>
    /// <param name="sources"></param>
    public static void CalcUXT(double[] xmas, double[] ymas, Source[] sources)
    {
        int count = xmas.Length;
        Tuple<Complex, Complex>[] tmp;
        for (int ss = 0; ss < sources.Length; ss++)
            Parallel.For(0, count, (int xi) =>
            {
                for (int yi = 0; yi < count; yi++)
                    tmp = Functions.CMAS_Memoized(xmas[xi], ymas[yi], sources[ss]);
            }
            );
    }

    public enum ModeSourcesWriting { ReWrite, Add };
    private static void Centers(Source[] smas, ModeSourcesWriting mode = ModeSourcesWriting.ReWrite)
    {
        if (mode == ModeSourcesWriting.ReWrite)
            using (StreamWriter f = new StreamWriter("Centers.txt"))
            {
                f.WriteLine("Short sources defenition");
                for (int i = 0; i < smas.Length; i++)
                    f.WriteLine(smas[i].ToShortString());
            }
        else
        {
            string[] txt;
            using (StreamReader f = new StreamReader("Centers.txt"))
            {
                txt = f.ReadToEnd().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

            using (StreamWriter f = new StreamWriter("Centers.txt"))
            {
                for (int i = 0; i < txt.Length; i++)
                    f.WriteLine(txt[i]);

                for (int i = 0; i < smas.Length; i++)
                    f.WriteLine(smas[i].ToShortString());
            }

        }
    }
    private static void Space(double x0, double X, int xcount, double y0, double Y, Source[] smas)
    {
        using (StreamWriter f = new StreamWriter("Space.txt"))
        {
            f.WriteLine($"xmin= {x0.ToRString()}");
            f.WriteLine($"ymin= {y0.ToRString()}");
            f.WriteLine($"xmax= {X.ToRString()}");
            f.WriteLine($"ymax= {Y.ToRString()}");
            f.WriteLine($"countS= {xcount}");
            f.WriteLine($"countW= {wcount}");
            f.WriteLine($"wbeg= {wbeg.ToRString()}");
            f.WriteLine($"wend= {wend.ToRString()}");
            f.WriteLine($"lamda= {lamda.ToRString()}");
            f.WriteLine($"mu= {mu.ToRString()}");
            f.WriteLine($"ro= {ro.ToRString()}");
            f.WriteLine($"h= {h.ToRString()}");
        }
    }
    private static void Poles(double[] w)
    {
        using (StreamWriter f = new StreamWriter("Poles.txt"))
        {
            string[] s = new string[w.Length];
            Parallel.For(0, w.Length, (int i) => s[i] = PolesMasMemoized(w[i]).ToString().Replace(',', '.'));

            foreach (string c in s)
            {
                f.WriteLine(c.Substring(3, c.Length - 6).Replace('\t', ' ').Replace("  ", " "));
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
                    f.WriteLine($"{i + 1} {mas[i].Norms[j].Position.x.ToRString()} {mas[i].Norms[j].Position.y.ToRString()} {mas[i].Norms[j].n.x.ToRString()} {mas[i].Norms[j].n.y.ToRString()}".Replace(',', '.'));
        }
    }
    private static void ReadData(double x0, double X, int xcount, double y0, double Y, Source[] mas)
    {
        if (mas.Length == 0) return;
        SaveCount = xcount * xcount * wcount * mas.Length;
        Saved = 0;
        int[] saves = new int[mas.Length];
        Vectors xx = new Vectors(SeqWMemoized(x0, X, xcount));
        Vectors yy = new Vectors(SeqWMemoized(y0, Y, xcount));
        Vectors w = new Vectors(SeqWMemoized(wbeg, wend, wcount));

        Parallel.For(0, mas.Length, (int i) =>
        {
            using (StreamReader f = new StreamReader($"uxw {mas[i].ToShortString()}.txt"))
            {
                string s = f.ReadLine();
                s = f.ReadLine();
                double[] st;

                while (s != null && s.Length > 0)
                {
                    st = s.ToDoubleMas();

                    Functions.ur.OnlyAdd(
                        new Tuple<double, double, double, Source>(st[0], st[1], st[2], mas[i]),
                        new Tuple<Complex, Complex>(new Complex(st[3], st[4]), new Complex(st[5], st[6])));

                    saves[i]++;
                    s = f.ReadLine();
                    if (saves[i] % 12 == 0)
                        Saved = saves.Sum();
                }
            }
        });
        Saved = 0;
    }
    /// <summary>
    /// Записать сохранённые значения u(x,y,w,s) в файлы
    /// </summary>
    /// <param name="mas"></param>
    private static void WriteData(Source[] mas)
    {
        SaveCount = ur.dic.Count;
        Saved = 0;
        int which(Source f)
        {
            for (int i = 1; i < mas.Length; i++)
                if (f == mas[i])
                {
                    return i;
                }
            return 0;
        }

        StreamWriter[] writers = new StreamWriter[mas.Length];
        for (int i = 0; i < mas.Length; i++)
        {
            writers[i] = new StreamWriter($"uxw {mas[i].ToShortString()}.txt");
            writers[i].WriteLine("x y w Reur Imur Reuz Imuz");
        }

        foreach (var p in ur.dic)
        {
            writers[which(p.Key.Item4)].WriteLine($"{p.Key.Item1.ToRString()} {p.Key.Item2.ToRString()} {p.Key.Item3.ToRString()} {p.Value.Item1.Re.ToRString()} {p.Value.Item1.Im.ToRString()} {p.Value.Item2.Re.ToRString()} {p.Value.Item2.Im.ToRString()}");
            Saved++;
        }


        for (int i = 0; i < mas.Length; i++)
            writers[i].Close();
        Saved = 0;
    }

    private static bool EqualConfigs(double x0, double X, int xcount, double y0, double Y, Source[] smas, out Source[] emptymas)
    {
        List<Source> list = new List<Source>();
        List<string> names = new List<string>();
        for (int i = 0; i < smas.Length; i++)
        {
            list.Add(smas[i].dup);
            names.Add($"uxw {smas[i].ToShortString()}.txt");
        }
        emptymas = list.ToArray();

        if (!File.Exists("Space.txt"))
            return false;

        string s;

        using (StreamReader f = new StreamReader("Space.txt"))
        {
            if (x0 != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (y0 != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (X != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (Y != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (xcount != f.ReadLine().Split(' ')[1].ToInt32()) return false;
            if (wcount != f.ReadLine().Split(' ')[1].ToInt32()) return false;
            if (wbeg != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (wend != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (lamda != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (mu != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (ro != f.ReadLine().Split(' ')[1].ToDouble()) return false;
            if (h != f.ReadLine().Split(' ')[1].ToDouble()) return false;
        }


        System.IO.FileInfo file2 = new System.IO.FileInfo("Space.txt");
        System.IO.FileInfo file1;// = new System.IO.FileInfo("uxw.txt");
        for (int i = 0; i < names.Count; i++)
        {
            file1 = new System.IO.FileInfo(names[i]);
            if (File.Exists(names[i]) && file1.LastWriteTime > file2.LastWriteTime)
            {
                names.RemoveAt(i);
                list.RemoveAt(i);
                i--;
            }
        }

        emptymas = list.ToArray();
        return true;
    }

    public static void StartProcessOnly(string fileName, bool global = false, string path = null)
    {
        path = path ?? Environment.CurrentDirectory;

        Process process = new Process();
        process.StartInfo.FileName = Path.Combine(path, fileName);
        process.StartInfo.WorkingDirectory = path;

        if (!global)
        {
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        }

        process.Start();

        process.WaitForExit();

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

    public static void CopyFilesR()
    {
        string[] dir = Expendator.GetStringArrayFromFile("WhereData.txt", true);
        //  for (int i = 0; i < dir.Length; i++)
        Parallel.For(0, dir.Length, (int i) =>
        {
            Expendator.CopyFiles(Environment.CurrentDirectory, dir[i], "3Duxt(better).r", "3Duxt.r", "ReDraw3Duxt2.r", "Truezlims.r", "WavesSurface.r");
        });
    }
    public static void CopyFilesTxt()
    {
        string[] dir = Expendator.GetStringArrayFromFile("WhereData.txt", true);
        //  for (int i = 0; i < dir.Length; i++)
        Parallel.For(0, dir.Length, (int i) =>
        {
            Expendator.CopyFiles(Environment.CurrentDirectory, dir[i], "zlims(real).txt", "zlims.txt", "MetrixSumOrMax.txt", "MakeUxtByEvery.txt", "MakePDFs.txt", "MakeDistanceToDefect.txt", "ClastersCount.txt", "AutoLims.txt", "SurfaceMain.txt", "RMdator.exe", "LastTimeConfig.txt");
        });
    }

    public static string GetResource(string name) => Expendator.GetResource(name, "Defect2019");

    public static void PlaySound(string NameInResources) => new System.Media.SoundPlayer(OtherMethods.GetResource(NameInResources + ".wav")).Play();

    /// <summary>
    /// Определяет существование всех путей, указанных в файле
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static bool ExistAllDirectoriesFromFile(string filename)
    {
        var st = Expendator.GetStringArrayFromFile(filename, true);
        foreach (var p in st)
            if (!Directory.Exists(p))
                return false;
        return true;
    }

    public static void CorrectWhereDataFile()
    {
        if (!File.Exists("WhereData.txt") || !OtherMethods.ExistAllDirectoriesFromFile("WhereData.txt"))
        {
            MessageBox.Show("В рабочем каталоге отсутствует файл \"WhereData.txt\", либо среди указанных в нём директорий есть несуществующие. Требуется выбрать корректный файл", "Нет пути или папки",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            string s;
            var openFileDialog1 = new OpenFileDialog();
            while (true)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    s = openFileDialog1.FileName;
                else continue;
                if (OtherMethods.ExistAllDirectoriesFromFile(s))
                {
                    File.Copy(s, "WhereData.txt", true);
                    break;
                }
            }
        }
    }
}
