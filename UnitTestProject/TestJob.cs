using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using МатКлассы;
using Defect2019;
using static Functions;
using Complex = МатКлассы.Number.Complex;


namespace UnitTestProject
{
    [TestClass]
    public class TestJob
    {
        [TestMethod]
        public void TestMethod1()
        {
            double w = 1.0;
            var c = PolesMasMemoized(w);
            //c.Show();

            double dist = Vectors.Union2(new Vectors(0.0), c).MinDist;
            double eps = dist/*poles.MinDist*/ * 0.4, eps2 = 0.5 * eps;

            // CVectors[] m = new CVectors[c.Deg];
            for (int i = 0; i < c.Deg; i++)
            {
                // m[i] =new CVectors( PRMSN(c[i], 1.0));
                double d = c[i] - eps;
                CVectors m = new CVectors(PRMSN(d, w));
                Console.WriteLine($"{d} {m}");

                d = c[i] + eps;
                m = new CVectors(PRMSN(d, w));
                Console.WriteLine($"{d} {m}");
            }

            using (StreamWriter f = new StreamWriter("vert.txt"))
            {
                f.WriteLine("w Reux Imux Reuy Imuy Reuz Imuz");

                var ws = Functions.SeqWMemoized(РабКонсоль.wbeg, РабКонсоль.wend, РабКонсоль.wcount);
                for (int i = 0; i < ws.Length; i++)
                {
                    var tmp = KsumRes2(200, 0, ws[i]);
                    f.WriteLine($"{ws[i]} {tmp[0].Re} {tmp[0].Im} {tmp[1].Re} {tmp[1].Im} {tmp[2].Re} {tmp[2].Im}".Replace(',', '.'));

                }

            }


        }
       
        /// <summary>
        /// Матрица Грина при наборе нормалей
        /// </summary>
        public static Func<double, double, double, CVectors> KsumRes2 = (double x, double y, double w) =>
        {
            var poles = PolesMasMemoized(w);
            Complex[][] c1 = new Complex[poles.Deg][], c2 = new Complex[poles.Deg][];
            double ar;
            Tuple<Complex, Complex> tup;
            CVectors sum = new CVectors(3);
            Point xy = new Point(x, y);
            Vectors QQ;

            double dist = Vectors.Union2(new Vectors(0.0), poles).MinDist, xp, yp;

            double eps = dist * 0.4, eps2 = 0.5 * eps;
            double[] plus = new double[poles.Deg], pminus = new double[poles.Deg];

            for (int k = 0; k < poles.Deg; k++)
            {
                plus[k] = poles[k] + eps;
                pminus[k] = poles[k] - eps;
                c1[k] = PRMSN(plus[k], w);
                c2[k] = PRMSN(pminus[k], w);

            }


            QQ = (new Vectors(0.0, 0.0, 1.0) * eps2);

            xp = x - 400;
            yp = y - 0;

            for (int k = 0; k < poles.Deg; k++)
            {
                ar = poles[k] * Point.Eudistance(new Point(400, 0), xy);
                tup = new Tuple<Complex, Complex>(МатКлассы.SpecialFunctions.Hankel(1.0, ar), МатКлассы.SpecialFunctions.Hankel(0.0, ar));



                //sum.FastAdd(KQmult3(InKtwice(plus[k], pminus[k], c1[k], c2[k], tup, xp, yp), QQ));
            }

            return sum * new Complex(0, 0.5);
        };

        /// <summary>
        /// Быстрое произведение нужных матриц и векторов с учётом их структуры
        /// </summary>
        /// <param name="M"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static CVectors KQmult3(Complex[,] M, Vectors v) => new CVectors(new Complex[3] { M[0,2]*v[2],M[1,2]*v[2],M[2,2]*v[2]});



        [TestMethod]
        public void tmp()
        {
            double[] w=new double[331], re=new double[331], im=new double[331];
            using(StreamReader f=new StreamReader("ws.dat"))
            {
                string s = f.ReadLine();
                s = f.ReadLine();
                int i = 0;
                while(s!=null && s.Length > 0)
                {
                    w[i++] = s.Replace('.', ',').Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0].ToDouble() * 2 * Math.PI / 1000;
                    s = f.ReadLine();
                }
            }

            using (StreamReader f = new StreamReader("0 200.dat"))
            {
                string s = f.ReadLine();
                s = f.ReadLine();
                int i = 0;
                while (s != null && s.Length > 0)
                {
                    var st = s.Replace('.',',').Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToDoubleMas();
                    re[i] = st[0];
                    im[i++] = st[1];
                    s = f.ReadLine();
                }
            }

            using(StreamWriter f=new StreamWriter("f(w) from (0 , 200).txt"))
            {
                f.WriteLine("w Refw Imfw");
                for (int i = 0; i < 331; i++)
                    f.WriteLine($"{w[i]} {re[i]} {-im[i]}");
            }

        }

        [TestMethod]
        public void DCtest()
        {
            double a = 1, w = 3, h = 0.02;
            for (int i = 0; i < 6; i++)
            {
                //new CSqMatrix( InKtwice(1, 0.5, PRMSN(a + i * h, w), PRMSN(a, w + i * h), HankelTuple(a * w * i + h), 3.2, 2.3)).Show();
                "".Show();
                //var p = HankelTuple(a * w + i*h);
                //Console.WriteLine($"{p.Item1} \t{p.Item2}");

                //PRMSN(a + i * h, w).ToCVector().Show();
                //var r = Arev(a + i * h, w);
                //Console.WriteLine(r.Item1);
                //Console.WriteLine(r.Item2);
                //Console.WriteLine();
            }
                
        }
    }
}
