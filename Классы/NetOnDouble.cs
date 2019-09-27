using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace МатКлассы
{
    /// <summary>
    /// Сетка на отрезке действительной оси
    /// </summary>
    public class NetOnDouble
    {
        /// <summary>
        /// Начало отрезка
        /// </summary>
        public readonly double Begin;
        /// <summary>
        /// Конец отрезка
        /// </summary>
        public readonly double End;
        /// <summary>
        /// Число точек в сетке (включая начало отрезка и конец, второе если WithEnd=true)
        /// </summary>
        public readonly int Count;
        /// <summary>
        /// Шаг по отрезку, расстояние между соседними точками
        /// </summary>
        public readonly double Step;
        /// <summary>
        /// Флаг, указывающий, содержит ли сетка конец отрезка
        /// </summary>
        public readonly bool WithEnd;

        /// <summary>
        /// Создать сетку по параметрам
        /// </summary>
        /// <param name="begin">Начало отрезка</param>
        /// <param name="end">Конец отрезка</param>
        /// <param name="count">Число точек в сетке</param>
        /// <param name="withend">Включать ли конец отрезка в сетку</param>
        public NetOnDouble(double begin, double end, int count, bool withend = true)
        {
            Begin = begin;
            End = end;
            Count = count;

            WithEnd = withend;
            if (WithEnd)
                Step = (End - Begin) / (Count - 1);
            else
                Step = (End - Begin) / Count;
        }
        /// <summary>
        /// Создать отрезок по концам и шагу
        /// </summary>
        /// <param name="begin">Начало отрезка</param>
        /// <param name="end">Конец отрезка</param>
        /// <param name="step">Шаг</param>
        public NetOnDouble(double begin, double end, double step)
        {
            Begin = begin;
            Count = (int)Math.Floor(Math.Abs(end - begin) / step);
            Step = step;

            End = end;
            WithEnd = begin + step * Count == end;
        }

        private double[] arr = null;
        /// <summary>
        /// Сам массив сетки
        /// </summary>
        public double[] Array
        {
            get
            {
                if (arr == null)
                    arr = Enumerable.Range(0, Count).Select(i => Begin + i * Step).ToArray();
                return arr;
            }
        }
    }
}
