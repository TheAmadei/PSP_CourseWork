using SharpDX;

namespace GameObjects
{
    /// <summary>
    /// Публичный класс Bullet.
    /// </summary>
    public class Bullet
    {
        /// <summary>
        /// Позиция.
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// Направление.
        /// </summary>
        public int direction;

        /// <summary>
        /// Конструктор класса Bullet.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public Bullet(Vector2 position, int direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }
}
