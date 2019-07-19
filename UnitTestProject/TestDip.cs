using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using МатКлассы;
using static МатКлассы.Number;
using System.Threading.Tasks;
using static МатКлассы.FuncMethods.DefInteg;
using Point = МатКлассы.Point;
using Курсач;
using static Курсач.KursMethods;


namespace UnitTestProject
{
    [TestClass]
    public class TestDip
    {
        Func<double, double, double> ys = (double t, double a) =>
        {
            return a * Math.Sin(t);
        };
        Func<double, double, double> xs = (double t, double a) =>
        {
            return a * Math.Cos(t);
        };

        Func<double, double, double> yk = (double t, double a) =>
            {
                if (t < a) return 0;
                if (t < 2 * a) return t - a;
                if (t < 3 * a) return a;
                return 4 * a - t;
            };
        Func<double, double, double> xk = (double t, double a) =>
        {
            if (t < a) return t;
            if (t < 2 * a) return a;
            if (t < 3 * a) return 3 * a - t;
            return 0;
        };

        Func<double, double, double> yt = (double t, double a) =>
        {
            if (t < a / 2) return t * Math.Sqrt(3);
            if (t < a) return -t * Math.Sqrt(3) + a * Math.Sqrt(3);
            return 0;
        };
        Func<double, double, double> xt = (double t, double a) =>
        {
            if (t < a) return t;
            return 3 * a - 2 * t;
        };

        Func<double, double, double> yo = (double t, double a) =>
        {
            if (t < a / 2) return Math.Sqrt(a * a - (t - a).Sqr());
            if (t < a) return Math.Sqrt(a * a - t * t);
            return 0;
        };
        Func<double, double, double> xo = (double t, double a) =>
        {
            if (t < a) return t;
            return 2 * a - t;
        };


        [TestMethod]
        public void DrawFig()
        {
            double a = 4;
            int count = 1000;
            double h = 4 * a / (count - 1);

            StreamWriter f = new StreamWriter("Rect.txt");
            StreamWriter circ = new StreamWriter("Circle.txt");
            StreamWriter triag = new StreamWriter("Tria.txt");
            StreamWriter os = new StreamWriter("Os.txt");

            for (int i = 0; i < count; i++)
            {
                double arg = i * h;
                f.WriteLine($"{xk(arg, a)} {yk(arg, a)}");
                circ.WriteLine($"{xs(arg, 2)} {ys(arg, 2)}");
            }

            a = 2;
            h = 1.5 * a / (count - 1);
            for (int i = 0; i < count; i++)
            {
                double arg = i * h;
                triag.WriteLine($"{xt(arg, a)} {yt(arg, a)}");

            }

            h = 2 * a / (count - 1);
            for (int i = 0; i < count; i++)
            {
                double arg = i * h;
                os.WriteLine($"{xo(arg, a)} {yo(arg, a)}");

            }

            f.Close();
            circ.Close();
            triag.Close();
            os.Close();
        }

        [TestMethod]
        public void DrawCurves()
        {
            SequenceFunc t = (double x, int kk) => Math.Log(0.01 + (x + 1 - 0.1 * kk).Sqr());//1.0/(1.0+kk*x*x);//FuncMethods.Monoms;


            RealFunc[] f = new RealFunc[] { (double x) => Math.Exp(x / (x.Abs() + 1)), (double x) => Math.Sin(x + 2) * Math.Cos(2 * x), (double x) => x.Abs() * Math.Exp(Math.Cos(x)), (double x) => Math.Log(1 + x.Abs()) * Math.Sinh(x / 2), (double x) => Math.Sin(x) - 8.0 / (2 * x + 6), (double x) => x.Abs() + Math.Log(0.01 + x * x) };
            Point[] b = new Point[] { new Point(0, 5), new Point(-4, 4), new Point(-2, 4), new Point(-2, 2), new Point(-1, 1), new Point(-1, 1) };

            int k = 64;
            int[] mas = new int[k];
            for (int i = 0; i < k; i++)
                mas[i] = i + 1;

            Parallel.For(0, f.Length, (int i) =>
            {
                var c = FuncMethods.UltraVsNormal(f[i], t, SequenceFuncKind.Other, k, b[i].x, b[i].y);
                StreamWriter fs = new StreamWriter($"Monoms {i + 1}.txt");
                fs.WriteLine($"count normal ultra");
                for (int j = 0; j < k; j++)
                    fs.WriteLine($"{mas[j]} {c.Item1[j]} {c.Item2[j]}");
                fs.Close();
            });


            //k = 75;
            //mas = new int[k];
            //for (int i = 0; i < k; i++)
            //    mas[i] = i + 1;
            //Parallel.For(0, f.Length, (int i) =>
            //{
            //    t = FuncMethods.TrigSystem(b[i].x, b[i].y);
            //    var c = FuncMethods.UltraVsNormal(f[i], t, SequenceFuncKind.Orthogonal, k, b[i].x, b[i].y);
            //    StreamWriter fs = new StreamWriter($"Trig {i + 1}.txt");
            //    fs.WriteLine($"count normal ultra");
            //    for (int j = 0; j < k; j++)
            //        fs.WriteLine($"{mas[j]} {c.Item1[j]} {c.Item2[j]}");
            //    fs.Close();
            //});

        }

        [TestMethod]
        public void DrawCurves2()
        {
            SequenceFunc t = (double x, int k) => Math.Exp(k * x);//FuncMethods.Monoms;

            int[] count = new int[] { 9, 16, 34 };
            RealFunc[] f = new RealFunc[count.Length];
            Point[] b = new Point[] { new Point(-1, 5), new Point(-4, 4), new Point(-2, 4), new Point(-2, 2), new Point(-1, 1), new Point(-1, 1) };

            RealFunc f1 = (double x) => Math.Cos(2 * x) * (Math.Exp((x / 3).Abs()) - 4 * Math.Log(1 + x.Abs()));//Math.Sin(5 * x) / Math.Exp(x.Abs() / 3);
            RealFunc fl;


            Parallel.For(0, f.Length, (int i) =>
            {
                f[i] = FuncMethods.Approx(f1, t, SequenceFuncKind.Other, count[i], b[0].x, b[0].y, true);
            });
            fl = FuncMethods.Approx(f1, t, SequenceFuncKind.Other, count[count.Length - 1], b[0].x, b[0].y);

            int c = 280;
            double h = (b[0].y - b[0].x) / (c - 1);


            StreamWriter fs = new StreamWriter($"Monoms vs.txt");
            //fs.WriteLine($"count normal ultra");
            for (int j = 0; j < c; j++)
            {
                double arg = b[0].x + j * h;
                fs.Write($"{arg} {f1(arg)} {fl(arg)}");
                for (int i = 0; i < count.Length; i++)
                    fs.Write($" {f[i](arg)}");
                fs.WriteLine();
            }

            fs.Close();
        }

        [TestMethod]
        public void Dinteg()
        {
            double rr = 5;
            Curve c = new Curve(0, 2 * Math.PI, (double t, double r) => r * Math.Cos(t), (double t, double r) => r * Math.Sin(t), rr);
            c.S = (double tx, double ty, double r) => tx * ty * r; c.End = (double r) => 2 * Math.PI;
            Functional[] f = new Functional[5];
            f[1] = (Point t) => Math.Sqrt(t.x * t.x + t.y * t.y);
            f[2] = (Point t) => /*Math.Sqrt(t.x * t.x + t.y * t.y)*/t.x * t.y/*Math.Exp(-t.x*t.x-t.y*t.y)*/;
            f[3] = (Point t) => /*Math.Sqrt(t.x * t.x + t.y * t.y)*//*t.x*t.y*/Math.Exp(-t.x * t.x - t.y * t.y);
            f[0] = (Point t) => Math.Log(1 + t.x * t.x + t.y * t.y);
            f[4] = (Point t) => (3 * t.x - 2 * t.y) / (t.x * t.x + t.y * t.y);

            double[] integ = new double[] { Math.PI * (-rr * rr + Math.Log(1 + rr * rr) * (1 + rr * rr)) /*Math.PI * rr * rr*/, Math.PI * rr * rr * rr * 2 / 3, 0, Math.PI * (1 - Math.Exp(-rr * rr)), 0 };
            int[] cy = new int[120];
            for (int i = 0; i < 120; i++)
                cy[i] = 10 + i * 3;
            FuncMethods.DefInteg.DemonstrationToFile("eps.txt", "epstime.txt", f, c, c.S, cy, integ);
        }

        [TestMethod]
        public void Dinteg2()
        {
            double rr = 2;
            Curve c = new Curve(0, (Math.PI + 2),
                (double t, double r) => { if (t < Math.PI) return r * Math.Cos(t); return -r + (t - Math.PI) * r; },
                (double t, double r) => { if (t < Math.PI) return r * Math.Sin(t) + 0.5 * (rr - r); return 0.5 * (rr - r); },
                rr);
            c.S = (double tx, double ty, double r) => r * ty * tx * Math.PI / (Math.PI + 2);
            c.End = (double r) => (Math.PI + 2);

            Functional[] f = new Functional[5];
            f[0] = (Point t) => 1;//t.x+t.y ;
            f[1] = (Point t) => /*Math.Sqrt(t.x * t.x + t.y * t.y)*/t.x * t.y * t.y + 1/*Math.Exp(-t.x*t.x-t.y*t.y)*/;
            f[2] = (Point t) => Math.Exp((t.x.Sqr() + t.y.Sqr()) - 10);
            f[3] = (Point t) => Math.Log(1 + Math.Sqrt(t.x * t.x + t.y * t.y));
            f[4] = (Point t) => t.x + t.y;

            double[] integ = new double[] { rr * rr / 2 * Math.PI, Math.PI * rr * rr / 2, Math.PI / 2 * Math.Exp(rr * rr - 10), Math.PI / 2 * (rr * rr * Math.Log(1 + rr) + rr - rr * rr / 2 - Math.Log(1 + rr)) /*0.5 * (Math.Log(1 + rr * rr) * (Math.PI * rr * rr + 1) - rr * rr)*/, rr * rr * rr * 2 / 3 };
            // double[] integ = new double[] { 0,0,0,0 };
            int[] cy = new int[120];
            for (int i = 0; i < 120; i++)
                cy[i] = 10 + i * 40;
            FuncMethods.DefInteg.DemonstrationToFile("eps.txt", "epstime.txt", f, c, c.S, cy, integ, Method.GaussKronrod61fromFortran);
        }

        [TestMethod]
        public void Dinteg3()
        {
            //var ar = new AreaForDoubleInteg(0, 2, (t) => -Math.Cos(t), t => Math.Cos(t));
            //ar.DInteg((Point p) => p.y, Method.GaussKronrod61, 100).Show();

            double rr = 5;
            var ar = new AreaForDoubleInteg(-rr, rr, (t) => -Math.Sqrt(rr*rr-(t).Sqr()), (t) => Math.Sqrt(rr * rr - (t).Sqr()));
            Functional[] f = new Functional[5];
            f[1] = (Point t) => Math.Sqrt(t.x * t.x + t.y * t.y);
            f[2] = (Point t) => t.x * t.y;
            f[3] = (Point t) => Math.Exp(-t.x * t.x - t.y * t.y);
            f[0] = (Point t) => Math.Log(1 + t.x * t.x + t.y * t.y);
            f[4] = (Point t) => (3 * t.x - 2 * t.y) / (t.x * t.x + t.y * t.y);
            double[] integ = new double[] { Math.PI * (-rr * rr + Math.Log(1 + rr * rr) * (1 + rr * rr)) /*Math.PI * rr * rr*/, Math.PI * rr * rr * rr * 2 / 3, 0, Math.PI * (1 - Math.Exp(-rr * rr)), 0 };
            for (int i = 0; i < f.Length; i++)
            {
                $"{ar.DInteg(f[i], Method.GaussKronrod61Sq, 400) - integ[i]}".Show();
            }
        }

        [TestMethod]
        public void UpVal()
        {
            double rr = 1;
            Curve c = new Curve(0, 2 * Math.PI, (double t, double r) => r * Math.Cos(t), (double t, double r) => r * Math.Sin(t), rr);
            c.S = (double tx, double ty, double r) => tx * ty * r; c.End = (double r) => 2 * Math.PI;

            //Functional mp = (Point g) => 1;
            //DoubleIntegral(mp, c, c.S, parallel: true, cy: 300, M: Method.GaussKronrod61).Show();

            Func<Point,int,double> ro = (Point p, int k)=> {
                var cc = new Complex(p.x, p.y);
                return Math.Pow(cc.Abs, k) * Math.Cos(k * cc.Arg);
            };
            Func<Point, Point, double> E = (Point x, Point y) => Math.Log(Point.Eudistance(x, y));


            int xc = 100, yc = 100;
            double x0 = -8, X = 8, y0 = -8, Y = 8;

            double hx = (X - x0) / (xc - 1), hy = (Y - y0) / (yc - 1);

            double[] xa=new double[xc], ya=new double[yc];
            
            for (int i = 0; i < xc; i++)
                xa[i] = x0 + hx * i;
            for (int i = 0; i < yc; i++)
                ya[i] = y0 + hy * i;


            int[] kk = new int[] {3,5,25,50,200,500};
            double[][,] val=new double[kk.Length][,];
            for(int i = 0; i < kk.Length; i++)
            {
                val[i] = new double[xc, yc];
                Functional fl = (Point x) =>
                  {
                      Functional tmp = (Point y) => ro(y, kk[i]) * E(x, y);
                      return DoubleIntegral(tmp, c, c.S, parallel: true,cy:200,M: Method.GaussKronrod61);
                  };
                Functional fl2 = (Point x) =>
                {
                    Complex z = new Complex(x.x, x.y);
                    
                    Functional tmp = (Point y) => {
                        Complex w = new Complex(y.x, y.y);
                        return Math.Pow(w.Abs, kk[i] ) * Math.Cos(kk[i] * (w.Arg)) * Math.Log(z.Abs.Sqr()+w.Abs.Sqr()-2*z.Abs*w.Abs*Math.Cos(w.Arg));
                    };
                    return DoubleIntegral(tmp, c, c.S, parallel: true, cy: 200, M: Method.GaussKronrod61)/2*Math.Cos(kk[i]*z.Arg);
                };

                Functional fr = (Point x) =>
                  {
                      Complex z = new Complex(x.x, x.y);
                      return Math.PI * Math.Cos(kk[i] * z.Arg) / (kk[i] + 2) * Math.Max(Math.Log((z.Abs - 1).Sqr()).Abs(), Math.Log((z.Abs + 1).Sqr()).Abs());
                  };

                for (int ix = 0; ix < xc; ix++)
                    for (int iy = 0; iy < yc; iy++)
                    {
                        Point t = new Point(xa[ix], ya[iy]),o=new Point(0);
                        double rad = Point.Eudistance(t, o);
                        // $"t = {t} fr = {fr(t)}  fl = {fl(t)}".Show();
                        //$"{fl(t)} {fl2(t)}".Show();
                        //Assert.IsTrue((fl(t) - fl2(t)).Abs() < 1e-7);

                        if(rad>1.0)
                        val[i][ix, iy] = fr(t).Abs() - fl(t).Abs();
                        else val[i][ix, iy] = Double.NaN;
                    }
                        
            }

            StreamWriter args = new StreamWriter("arg.txt");
            StreamWriter vals = new StreamWriter("vals.txt");

            for (int i = 0; i < xc; i++)
                args.WriteLine($"{xa[i]} {ya[i]}");

            for(int i=0;i<xc;i++)
                for(int j=0;j<yc;j++)
                {
                    string st = "";
                    for (int s = 0; s < kk.Length; s++)
                        st+=(val[s][i, j] + " ");
                    vals.WriteLine(st.Replace("NaN","NA"));
                }


            args.Close();
            vals.Close();

        }

        [TestMethod]
        public void Test()
        {
            //for (CIRCLE = 1; CIRCLE <= CountCircle; CIRCLE++)
            //    for (GF = 1; GF <= KGF - 1; GF++)
            //    {
            //        // while (!ScriptEnded) { }
            //        var t = BiharmonicEquation.CreateAndSolve(50, GF, CIRCLE);
            //        string s = $"BiharmEpss curve={CircleName[CIRCLE - 1]} func={FuncName[GF - 1]}.txt";
            //        StreamWriter fs = new StreamWriter(s);
            //        for (int i = 0; i < masPoints.Length; i++)
            //            fs.WriteLine($"{i + 1} {t.Item1[i]} {t.Item2[i]}");

            //        fs.Close();
            //        using (StreamWriter names = new StreamWriter("name.txt"))
            //        {
            //            names.WriteLine(s);
            //        }
            //        //StartProcess();
            //    }
            var t = BiharmonicEquation.CreateAndSolve(50, 1, 1);
        }

        [TestMethod]
        public void Polynoms()
        {
            RealFunc f = (double x) =>1.0/(1.0+x*x);
            double a = -9, b = 5;

            using(StreamWriter fs=new StreamWriter("Runge.txt"))
            {
                for(int i = 1; i < 30; i++)
                {
                    Polynom p = Polynom.Lag(f, i-1, a, b);
                    fs.WriteLine($"{i} {FuncMethods.RealFuncMethods.NormDistanceС(f,p.Value,a,b)} {FuncMethods.RealFuncMethods.NormDistance(f, p.Value, a, b)}");
                }
            }
        }
    }
}
