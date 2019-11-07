using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Работа2019
{
   public static class SoundMethods
    {
        public static void Back() => OtherMethods.PlaySound("circleback");
        public static void Clear() => OtherMethods.PlaySound("clear");
        public static void OK() => OtherMethods.PlaySound("roger");
        public static void SetPositions() => OtherMethods.PlaySound("ct_point");
        public static void TukTuk() => OtherMethods.PlaySound("achievement_earned");
        public static void CS() => OtherMethods.PlaySound("gamestartup");
        public static void Egg() => OtherMethods.PlaySound("Пасхалка");
    }
}
