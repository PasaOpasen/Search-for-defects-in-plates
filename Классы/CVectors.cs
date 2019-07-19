﻿using System.Collections;
using static МатКлассы.Number;

/// <summary>
/// Библиотека математических классов, написанная Опасным Пасей (Дмитрией Пасько/Деметрием Паскалём).
/// Начал писать примерно в конце февраля 2018-го, с класса полиномов.
/// С конца января 2019-го пишу продолжение библиотеки (МатКлассы),
/// в текущую библиотеку иногда добавляю новый функционал.
/// Сильные стороны библиотеки: классы комплексный чисел, векторов, полиномов,
/// матриц, методов интегрирования, графов (особое внимание), СЛАУ, методы расширения
/// Недостатки: мало где заботился об исключениях, содержимое методов почти не комментрируется,
/// в классе СЛАУ из-за диплома, вышедшего с С++, есть слишком сложные низкоуровневые методы
/// и путаница из-за тесной связи с классом кривых, 
/// класс вероятностей начал из эксперимента и почти ничего не написал,
/// очень много открытых полей и методов,
/// почти не проводил тестирование,
/// но большинство методов использовались в визуальных приложениях
/// и так были отлажены
/// Всё написал сам, кроме 3-5% кода, взятого из открытых источников
/// 
/// ------------Контакты:
/// Telegram: 8 961 519 36 46 (на звонки не отвечаю)
/// Mail:     qtckpuhdsa@gmail.com
/// Discord:  Пася Опасен#3065
/// VK:       https://vk.com/roman_disease
/// Steam:    https://vk.com/away.php?to=https%3A%2F%2Fsteamcommunity.com%2Fid%2FPasaOpasen&cc_key=
///      Активно пользуюсь всеми указанными сервисами
/// </summary>
namespace МатКлассы
{
    /// <summary>
    /// Комплексные векторы
    /// </summary>
    public class CVectors : IComparer,Idup<CVectors>
    {
       public CVectors dup => new CVectors(this);

        private Complex[] mas;
        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Complex this[int i]
        {
            get { return mas[i]; }
            set
            {
                mas[i] = value;
            }
        }
        /// <summary>
        /// Комплексный массив, отождествлённый с вектором
        /// </summary>
        public Complex[] ComplexMas
        {
            get
            {
                Complex[] res = new Complex[this.Degree];
                for (int i = 0; i < res.Length; i++)
                    res[i] = new Complex(mas[i]);
                return res;
            }
        }
        /// <summary>
        /// Действительная часть вектора
        /// </summary>
        public Vectors Re
        {
            get
            {
                Vectors v = new Vectors(this.mas.Length);
                for (int i = 0; i < v.n; i++)
                    v[i] = mas[i].Re;
                return v;
            }
        }
        /// <summary>
        /// Комплексная часть вектора
        /// </summary>
        public Vectors Im
        {
            get
            {
                Vectors v = new Vectors(this.mas.Length);
                for (int i = 0; i < v.n; i++)
                    v[i] = mas[i].Im;
                return v;
            }
        }
        /// <summary>
        /// Размерность вектора
        /// </summary>
        public int Degree => mas.Length;
        /// <summary>
        /// Модуль вектора как сумма евклидовых норм действительной и мнимой части
        /// </summary>
        public double Abs => this.Re.EuqlidNorm + this.Im.EuqlidNorm;

        /// <summary>
        /// Коплексный вектор по комплексному массиву 
        /// </summary>
        /// <param name="mas"></param>
        public CVectors(Complex[] mas)
        {
            this.mas = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                this.mas[i] = new Complex(mas[i]);
        }
        /// <summary>
        /// Комплексный вектор по действительному массиву
        /// </summary>
        /// <param name="mas"></param>
        public CVectors(double[] mas)
        {
            this.mas = new Complex[mas.Length];
            for (int i = 0; i < mas.Length; i++)
                this.mas[i] = new Complex(mas[i], 0);
        }
        /// <summary>
        /// Комплексный вектор по действительной и мнимой части
        /// </summary>
        /// <param name="R"></param>
        /// <param name="I"></param>
        public CVectors(Vectors R, Vectors I) : this(R.DoubleMas)
        {
            for (int i = 0; i < I.Deg; i++)
                mas[i] += Complex.I * I[i];
        }
        /// <summary>
        /// Пустой комплексный вектор указанной размерности
        /// </summary>
        /// <param name="k"></param>
        public CVectors(int k) : this(new double[k]) { }
        /// <summary>
        /// Копирование коплексного вектора
        /// </summary>
        /// <param name="v"></param>
        public CVectors(CVectors v) : this(v.ComplexMas) { }

        /// <summary>
        /// Комплексно сопряженный вектор
        /// </summary>
        public CVectors Conjugate
        {
            get
            {
                CVectors v = new CVectors(this.Degree);
                for (int i = 0; i < v.Degree; i++)
                    v[i] = this[i].Conjugate;
                return v;
            }
        }

        /// <summary>
        /// Перевод в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "(";
            for (int i = 0; i < this.Degree - 1; i++)
                s += this[i].ToString() + "   ";
            s += this[Degree - 1].ToString() + ")";
            return s;
        }

        int IComparer.Compare(object x, object y)
        {
            CVectors a = x as CVectors;
            CVectors b = y as CVectors;
            for (int i = 0; i < a.Degree; i++)
            {
                int val = a[i].CompareTo(b[i]);
                if (val != 0) return val;
            }
            return 0;
        }

        /// <summary>
        /// Преобразование комплексного вектора в действительный (по действительной части)
        /// </summary>
        /// <param name="v"></param>
        public static explicit operator Vectors(CVectors v) => v.Re;
        /// <summary>
        /// Преобразование действительной вектора в комплексный
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator CVectors(Vectors v) => new CVectors(v.DoubleMas);

        /// <summary>
        /// Пустой комплексный вектор
        /// </summary>
        public static CVectors EmptyVector => new CVectors(new Complex[] { });

        /// <summary>
        /// Быстрое добавление к вектору другого вектора
        /// </summary>
        /// <param name="v"></param>
        public void FastAdd(CVectors v)
        {
            for (int i = 0; i < this.Degree; i++)
                mas[i] += v[i];
        }
        public void FastLessen(CVectors v)
        {
            for (int i = 0; i < this.Degree; i++)
                mas[i] -= v[i];
        }

        /// <summary>
        /// Скалярное произведение векторов
        /// </summary>
        /// <param name="q"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static Complex operator *(CVectors q, CVectors w)
        {
            Complex sum = 0;
            for (int i = 0; i < q.Degree; i++)
                sum += q[i] * w[i];
            return sum;
        }

        public static CVectors operator +(CVectors q, CVectors w) => new CVectors(q.Re + w.Re, q.Im + w.Im);
        public static CVectors operator -(CVectors q, CVectors w) => new CVectors(q.Re - w.Re, q.Im - w.Im);
        public static CVectors operator -(CVectors q) => new CVectors(-q.Re, -q.Im);
        public static CVectors operator *(CVectors v, Complex c)
        {
            CVectors res = new CVectors(v);
            for (int i = 0; i < v.Degree; i++)
                res[i] *= c;
            return res;
        }
        public static CVectors operator *(Complex c, CVectors v) => v * c;
        public static CVectors operator /(CVectors v,Complex c)
        {
            CVectors res = new CVectors(v);
            for (int i = 0; i < res.Degree; i++)
                res[i] /= c;
            return res;
        }


        public static CVectors operator *(CVectors[] mas,CVectors vec)
        {
            CVectors res = new CVectors(mas[0].Degree);
            for (int i = 0; i < mas.Length; i++)
                res.FastAdd(mas[i] * vec[i]);
            return res;
        }

    }
}

