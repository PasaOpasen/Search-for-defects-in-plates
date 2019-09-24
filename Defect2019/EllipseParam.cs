using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using МатКлассы;

namespace Работа2019
{
    /// <summary>
    /// Параметры эллипса
    /// </summary>
    public struct EllipseParam
    {
        internal Point focSource, focSensor;
        internal readonly Point xM;
        internal double L, a, b, tau;
        public System.Drawing.Color Color;
        public EllipseParam(Point SourceCenter, Point SensorCenter, double s, System.Drawing.Color color)
        {
            L = Point.Eudistance(SourceCenter, SensorCenter);
            if (s < L)
                throw new ArgumentException($"s < L !!! ({s} < {L})");

            focSource = SourceCenter;
            focSensor = SensorCenter;
            a = s / 2;
            b = Math.Sqrt(a * a - (L / 2).Sqr());
            Color = color;

            xM = new Point((focSensor.x + focSource.x) / 2, (focSensor.y + focSource.y) / 2);

            tau = Math.Atan((focSensor.y - focSource.y) / (focSensor.x - focSource.x));
            if (focSensor.x < focSource.x)
                tau += Math.PI;
        }

        public Point[] GetPointArray(int count)
        {
            double cost = Math.Cos(tau), sint = Math.Sin(tau);
            var angles = Expendator.Seq(0, 2 * Math.PI, count, false);
            Point[] res = new Point[count];
            for (int i = 0; i < angles.Length; i++)
            {
                double sinf = Math.Sin(angles[i]), cosf = Math.Cos(angles[i]);
                res[i] = new Point(xM.x + a * cost * cosf - b * sint * sinf, xM.y + a * sint * cosf + b * cost * sinf);
            }
            return res;
        }
        public override string ToString()
        {
            return $"source = {focSource} sensor = {focSensor} s = {2*a} color = {Color}";
        }

        /// <summary>
        /// Записать коллекцию параметров в файл
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="list"></param>
        public static void WriteInFile(string filename, IEnumerable<EllipseParam> list) => Expendator.WriteInFile(filename, list.Select(l => l.ToString()).ToArray());
    }
}
