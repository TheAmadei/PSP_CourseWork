using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace GameObjects
{
    /// <summary>
    /// Публичный класс ShipUtilities.
    /// </summary>
    public static class ShipUtilities
    {
        /// <summary>
        /// Изменение корабля.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap(ref int a, ref int b)
        {
            int c = a;
            a = b;
            b = c;
        }
        /// <summary>
        /// Получение параметров для декоратора.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int[] GetShipParameters(int index)
        {
            return File.ReadAllLines("parameters")[index].Split(' ').Select(n => Convert.ToInt32(n)).ToArray();
        }
        /// <summary>
        /// Смена направления корабля.
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="direction"></param>
        public static void SwapDirection(Ship pl, int direction)
        {
            if ((direction == 1 || direction == 3) && pl.height > pl.width)
            {
                ShipUtilities.Swap(ref pl.width, ref pl.height);
            }
            else if ((direction == 2 || direction == 4) && pl.height < pl.width)
            {
                ShipUtilities.Swap(ref pl.width, ref pl.height);
            }
            pl.direction = direction;
        }
    }
}
