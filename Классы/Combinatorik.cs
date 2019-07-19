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
    /// Методы комбинаторики
    /// </summary>
    public class Combinatorik
    {
        /// <summary>
        /// Число перестановок (факториал)
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int P(int k)
        {
            if (k < 2) return 1;
            int s = 1;
            for (int i = 2; i <= k; i++) s *= i;
            return s;
        }
        /// <summary>
        /// Перестановки с повторениями
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int P(params int[] k)
        {
            int sum = 0;
            for (int i = 0; i < k.Length; i++) sum += k[i];
            sum = P(sum);
            for (int i = 0; i < k.Length; i++) sum /= P(k[i]);
            return sum;
        }
        /// <summary>
        /// Число размещений из n по m 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int A(int m, int n)
        {
            int s = 1;
            for (int i = m + 1; i <= n; i++) s *= i;
            return s;
        }
        /// <summary>
        /// Размещения с повторениями
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int AA(int m, int n) { return (int)Math.Pow(n, m); }
        /// <summary>
        /// Число сочетаний из n по m
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int C(int m, int n) { return A(m, n) / P(m); }
        /// <summary>
        /// Сочетания с повторениями
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int CС(int m, int n) { return C(m, n + m - 1); }
    }
}

