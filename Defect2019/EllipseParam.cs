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
        private Point foc1, foc2;
        double L, a, b;
        System.Drawing.Color Color;
        public EllipseParam(Point p1,Point p2, double s, System.Drawing.Color color)
        {
            L = Point.Eudistance(p1, p2);
            foc1 = p1;
            foc2 = p2;
            a = s / 2;
            b = Math.Sqrt(a * a - (L / 2).Sqr());
            Color = color;
        }
    }
}
