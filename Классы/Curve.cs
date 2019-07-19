using System;

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
    /// Класс плоских кривых 
    /// </summary>
    public class Curve
    {
        /// <summary>
        /// Параметризации координат в зависимости от параметра и радиуса
        /// </summary>
        public DRealFunc U = (double t, double r) => 0, V = (double t, double r) => 0;

        /// <summary>
        /// Поля, рассчитанные под параметризацию границы области (на случай, если границу области нужно задать немного иначе, потому что функция имеет особенности)
        /// </summary>
        private RealFunc uuu = null, vvv = null;

        /// <summary>
        /// Свойства, выдающие параметризацию границы области
        /// </summary>
        public RealFunc u { get { if (uuu == null) return (double t) => U(t, this.radius); return (double t) => uuu(t); } set { uuu = value; } }
        public RealFunc v { get { if (vvv == null) return (double t) => V(t, this.radius); return (double t) => vvv(t); } set { vvv = value; } }

        /// <summary>
        /// Площадь сегмента
        /// </summary>
        public TripleFunc S;
        /// <summary>
        /// Начальное значение параметра
        /// </summary>
        public double a;
        /// <summary>
        /// Конечное значение параметра
        /// </summary>
        public double b;

        /// <summary>
        /// Значение шага интегрирования для этой кривой
        /// </summary>
        protected double _h = FuncMethods.DefInteg.STEP;
        /// <summary>
        /// Число шагов при интегрировании
        /// </summary>
        protected int M = ITER_INTEG; //число шагов
                                      /// <summary>
                                      /// Базовый радиус кривой
                                      /// </summary>
        protected double radius = 3;
        /// <summary>
        /// Базовый радиус кривой
        /// </summary>
        public double BaseRadius
        {
            get { return radius; }
            set { radius = value; }
        }
        /// <summary>
        /// Функция, выдающая нужную длину отрезка параметризации в зависимости от радиуса, поскольку иногда отрезок изменения параметра t зависит от r
        /// </summary>
        public RealFunc End;

        /// <summary>
        /// Количество шагов при интегрировании по умолчанию
        /// </summary>
        public static int ITER_INTEG = 5000;

        [Obsolete]
        /// <summary>
        /// Задать число шагов
        /// </summary>
        /// <param name="MM"></param>
        protected void Get_h(int MM)
        {
            M = MM;
            _h = (b - a) / M; //присвоение шагу конкретного значения
        }

        /// <summary>
        /// Возврат точки на кривой по значению параметра
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public BasisPoint Transfer(double t)
        {
            if (!GetTRANSFER)
            {
                //"Begin".Show();
                BasisPoint point = new BasisPoint();
                point.x = this.u(t);
                point.y = this.v(t);//point.Show();//"End".Show();
                return point;
            }
            return (BasisPoint)Trans(t, radius);
        }

        private bool GetTRANSFER = false;
        private DPointFunc Trans;
        /// <summary>
        /// Свойство, выдающее и принимающее делегат, отображающий параметры в точку на кривой
        /// </summary>
        public DPointFunc TRANSFER
        {
            get
            {
                if (GetTRANSFER) return Trans;
                else return (double t, double r) => Transfer(t);
            }
            set
            {
                Trans = (double t, double r) => value(t, r);
                GetTRANSFER = true;
            }
        }


        /// <summary>
        /// Кривая по своей параметризации
        /// </summary>
        /// <param name="a0">Начальное значение параметра</param>
        /// <param name="b0">Конечное значение параметра</param>
        /// <param name="uu">Отображение в первую координату</param>
        /// <param name="vv">Отображение во вторую координату</param>
        public Curve(double a0, double b0, RealFunc uu, RealFunc vv)
        {
            a = a0;
            b = b0;
            u = uu;
            v = vv;
        }
        /// <summary>
        /// Кривая по своей параметризации и базовому радиусу
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="b0"></param>
        /// <param name="uu"></param>
        /// <param name="vv"></param>
        /// <param name="BASEradius"></param>
        public Curve(double a0, double b0, RealFunc uu, RealFunc vv, double BASEradius) : this(a0, b0, uu, vv)
        {
            radius = BASEradius;
        }
        /// <summary>
        /// Кривая по всем своим параметрам
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="b0"></param>
        /// <param name="uu"></param>
        /// <param name="vv"></param>
        /// <param name="BASEradius"></param>
        /// <param name="uuu"></param>
        /// <param name="vvv"></param>
        public Curve(double a0, double b0, RealFunc uu, RealFunc vv, double BASEradius, DRealFunc uuu, DRealFunc vvv, TripleFunc T, RealFunc end) : this(a0, b0, uu, vv, BASEradius)
        {
            this.U = uuu;
            this.V = vvv;
            this.S = T;
            this.End = end;
        }
        /// <summary>
        /// Кривая по своей параметризации и базовому радиусу
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="b0"></param>
        /// <param name="uu"></param>
        /// <param name="vv"></param>
        /// <param name="BaseRadius"></param>
        public Curve(double a0, double b0, DRealFunc uu, DRealFunc vv, double BaseRadius)
        {
            a = a0; b = b0;
            U = uu; V = vv;
            radius = BaseRadius;
        }
        /// <summary>
        /// Кривая, вырожденная в начало координат (все поля - нули)
        /// </summary>
        public Curve()
        {
            a = 0;
            b = 0;
        }
        /// <summary>
        /// Конструктор кривой, которая задаётся не через пары параметрических функций 
        /// </summary>
        /// <param name="a0">Начало отрезка параметризации</param>
        /// <param name="b0">Конец отрезка параметризации</param>
        /// <param name="Tr">Функция, выдающая точку на кривой в зависимости от параметра и радиуса кривой</param>
        /// <param name="BaseRad">Базовый радиус</param>
        /// <param name="T">Площадь сегмента</param>
        /// <param name="end">Возврат конца отрезка параметризации в зависимости от радиуса</param>
        public Curve(double a0, double b0, DPointFunc Tr, double BaseRad, TripleFunc T, RealFunc end)
        {
            a = a0; b = b0;
            S = T;
            Trans = Tr;
            radius = BaseRad;
            End = new RealFunc(end);
        }

        [Obsolete]
        /// <summary>
        /// Вычисление криволинейного интеграла первого рода по этой кривой от функции f методом трапеции 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public double FirstkindTr(Functional f)//(в программе не используется)
        {
            Get_h(ITER_INTEG);
            double value = 0;
            for (int k = 0; k <= M - 1; k++)
            {
                value += f(Transfer((a + (k + 1) * _h + a + (k) * _h) / 2)) * BasisPoint.Eudistance(Transfer(a + (k + 1) * _h), Transfer(a + (k) * _h));
            }
            return value;
        }

        /// <summary>
        /// Вычисление криволинейного интеграла первого рода по этой кривой от функции f методом Гаусса 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public double Firstkind(Functional f)
        {
            RealFunc h = (double t) => f(this.Transfer(t));
            return FuncMethods.DefInteg.GaussKronrod.Integral(h, this.a, this.b);
        }
    }
}

