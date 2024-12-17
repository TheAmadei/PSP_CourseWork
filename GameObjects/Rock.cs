using SharpDX;

namespace GameObjects
{
    /// <summary>
    /// Публичный класс Rock.
    /// </summary>
    public class Rock
    {
        /// <summary>
        /// Масштаб.
        /// </summary>
        public int scale;
        /// <summary>
        /// Позиция.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// Конструктор класса Rock.
        /// </summary>
        /// <param name="position"></param>
        public Rock(Vector2 position)
        {
            this.position = position;
            scale = 120;
        }
    }
}
