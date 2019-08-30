using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using МатКлассы;
using static МатКлассы.Number;
using Point = МатКлассы.Point;
using System.Diagnostics;


namespace UnitTestProject
{
    [TestClass]
    public class TestBaseLib
    {
        [TestMethod]
        public void ComplexSqrt()
        {
            for(int i = 0; i < 100; i++)
            {
                Random random = new Random(i);
                double x = random.NextDouble()*100, y = random.NextDouble()*100;
                Complex z = new Complex(x, y);
                Complex s = Complex.Sqrt(z);
                $"{z-s*s}".Show();
                Assert.IsTrue((z-s*s).Abs<1e-12);
            }
        }

        [TestMethod]
        public void InvertMat()
        {
            CSqMatrix mat = new CSqMatrix(new Complex[,] {
                { 11,12,4},
                { 0,9,30},
                { 40,5,3}
            });
            double n1 = (mat.Invert() * mat -SqMatrix.E(3)).CubeNorm;
            n1.Show();
            double n2= (mat.InvertSum * mat - SqMatrix.E(3)).CubeNorm;
            n2.Show();
            Assert.IsTrue(n1 <= 1e-6 && n2<= 1e-6);
        }

        [TestMethod]
        public void Mat()
        {
            CSqMatrix mat = new CSqMatrix(new Complex[,] {
                { 11,12,4},
                { 0,9,30},
                { 40,5,3}
            });
            Complex a = new Complex(1, 2);
            (mat*a).Show();
        }

        [TestMethod]
        public void ReverseColAndOthersMet()
        {
            CSqMatrix mat = new CSqMatrix(new Complex[,] {
                { new Complex(1,4),12,4},
                { 0,9,30},
                { 40,5,3}
            });
            mat.ReversColumns(1, 2);mat.Show();"".Show();

            mat.MultplyRows(2, 1).Show();

            mat.GetColumn(0).Show();
            mat.GetColumn(1).Show();
            mat.GetColumn(2).Show();
        }

        [TestMethod]
        public void Dets()
        {
            CSqMatrix mat = new CSqMatrix(new Complex[,] {
                { new Complex(6,4),12,4},
                { 0,9,30},
                { 409,5,3}
            });

            DateTime t1=DateTime.Now, t2;

            mat.Det.Show();
            t2 = DateTime.Now;
            (t2 - t1).Show();

            mat.DetByMathNet.Show();
            t1 = DateTime.Now;
            (t1 - t2).Show();
        }

        [TestMethod]
        public void Minors()
        {
            CSqMatrix mat = new CSqMatrix(new Complex[,] {
                { new Complex(6,4),12,4},
                { 0,9,30},
                { 409,5,3}
            });
            mat.GetMinMat(1, 1).Show();"".Show();
            mat.GetMinMat(1, 2).Show(); "".Show();
            mat.GetMinMat(2, 2).Show(); "".Show();
            mat.GetMinMat(3, 2).Show(); "".Show();
        }

        [TestMethod]
        public void Bessels()
        {
            double r = 10;
            Complex[] a = new Complex[] { new Complex(0.5,-0.02),0.05,10};
            for(int i = 0; i < a.Length; i++)
            {
                a[i].Show();
                SpecialFunctions.MyBessel(0, a[i]*r).Show();
                SpecialFunctions.MyBessel(1, a[i]*r).Show();
                "".Show();
            }
        }

        [TestMethod]
        public void BesselsTime()
        {
            DateTime t1 = DateTime.Now;
            TimeSpan t2;
            Complex[] mas = new Complex[] { 1, 2, 3, 45, new Complex(1, 2), new Complex(2, 0.2), new Complex(0.05, -0.1) };
            for(int i = 0; i < mas.Length; i++)
            {
                Complex a = SpecialFunctions.MyBessel(0, mas[i]),b=SpecialFunctions.MyBessel(1,mas[i]);
                t2 = (DateTime.Now - t1);
                $"a = {mas[i]} bes = {a} {b} time = {t2}".Show();
                t1 = DateTime.Now;
            }
        }

        [TestMethod]
        public void DupTest()
        {
            CSqMatrix mas = new CSqMatrix(new Complex[,] { { 1, 2 }, { 3, 2 } });
            var s = mas.dup;
            s[1, 1] = 4;
            //mas.Show();
            //s.Show();

            double[] r = new double[] { 1, 2, 3, 4 };
            var t = r.Dup();
            t[0] = 0;
            r.Show();t.Show();
        }

        [TestMethod]
        public void BiserchTest()
        {
            double[] a = new double[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = Expendator.BinarySearch(a, 3);
            Assert.IsTrue(t.Item1 == 2 && t.Item2 == 2);
            t = Expendator.BinarySearch(a, 4.5);
            Assert.IsTrue(t.Item1 == 3 && t.Item2 == 4);
        }

        [TestMethod]
        public void HiveTest()
        {
            Func<Vectors, double> f = BeeHiveAlgorithm.Parabol;
            var s = BeeHiveAlgorithm.GetGlobalMin(f, 2,-100,100, eps: 1e-15, countpoints: 400, maxcountstep: 200);
            $"f от {s.Item1} = {s.Item2}".Show();

            f = BeeHiveAlgorithm.Rastr;
            s = BeeHiveAlgorithm.GetGlobalMin(f, 2, -100, 100, eps: 1e-15, countpoints: 400, maxcountstep: 200);
            $"f от {s.Item1} = {s.Item2}".Show();

            f = BeeHiveAlgorithm.Shvel;
            s = BeeHiveAlgorithm.GetGlobalMin(f, 2, -100, 100, eps: 1e-15, countpoints: 400, maxcountstep: 200);
            $"f от {s.Item1} = {s.Item2}".Show();
        }

        [TestMethod]
        public void Graf3Dtest()
        {
            Functional f = (Point a) => Math.Cos(a.Abs);
            ForScripts.MakeFilesForSurfaces(-2, 2, -4, 5, 100, "", new Functional[] { f },(Point p)=>true);
        }

        [TestMethod]
        public void FastAddTest()
        {
            CVectors a = new CVectors(new double[] { 1, 2, 3, 4 });
            CVectors b= new CVectors(new double[] { 4, 3, 2, 1 });
            CVectors c = new CVectors(new double[] { 0, 0, 0, 1 });
            CVectors d = new CVectors(new double[] { -1, -2, -3, -4 });

            a.Show();
            a.FastAdd(b);a.Show();
            a.FastAdd(c);a.Show();
            a.FastAdd(d);a.Show();
        }

        [TestMethod]
        public void CompMult()
        {
            Complex[] mas = new Complex[] { new Complex(1, 3), new Complex(4, -1) };
            var r = Expendator.Mult(mas, new Complex(2));
            new CVectors(r).Show();

        }

        [TestMethod]
        public void ToS()
        {
            double[] mas = new double[] { 9.73983183883746, 0.00655354706609585,123.083E-4 };
            for (int i = 0; i < mas.Length; i++)
                mas[i].ToString(4).Show();
        }

        [TestMethod]
        public void SpeedMatrixOp()
        {
            CSqMatrix s = new CSqMatrix(new Complex[,]
                {
                    {new Complex(1),new Complex(1e8),new Complex(1e17) },
                    { new Complex(1e-14),new Complex(2,1),new Complex(1e54)},
                    {new Complex(1),new Complex(1e13),new Complex(1e17) }
                }
                );

            CSqMatrix d = new CSqMatrix(s);
            d[1, 1] = 1e42;


            //                Stopwatch t = new Stopwatch();
            //                t.Restart();
            //            for(int i = 0; i < 10000; i++)
            //            {
            //                s.FastAdd(d);
            //}
            //                t.ElapsedMilliseconds.Show();


                Stopwatch t = new Stopwatch();
                t.Restart();
                s /= 100000;
                d /= 100000;

                         for (int i = 0; i < 10000; i++)
            {   s.FastAdd(d);}
                s *= 100000;
                t.ElapsedMilliseconds.Show();
            

        }

        [TestMethod]
        public void GPeriods()
        {
            double[] m1 = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            double[] m2 = new double[] { 1, 2, 3, 4, 5, 6 };
            double[] m3 = new double[] { 1, 2, 3, 1, 2, 3 };
            double[] m4 = new double[] { 1, 1, 1, 1, 2, 1, 1, 1, 1, 2 };
          Expendator.GetPeriod(m1).Show();
            Expendator.GetPeriod(m2).Show();
            Expendator.GetPeriod(m3).Show();
            Expendator.GetPeriod(m4).Show();
        }


        [TestMethod]
        public void DF()
        {
            double x = 0.0012345678909876543;
            x.ToString().Show();
            x.ToString("g17").Show();
            x.ToString("g").Show();
            x.ToString("r").Show();
        }

        [TestMethod]
        public void st()
        {
            string s = "e3\\\\yt";
            s.Show();
            s.Replace("\\", @"\").Show();
            s.Replace("\\\\", @"\").Show();
            Environment.CurrentDirectory.Show();
        }
    }
}
