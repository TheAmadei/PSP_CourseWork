using SharpDX;

namespace GameObjects
{
    /// <summary>
    /// Абстрактный класс декоратор ShipDecorator.
    /// </summary>
    public abstract class ShipDecorator : Ship
    {
        /// <summary>
        /// Игрок.
        /// </summary>
        protected Ship player;
        /// <summary>
        /// Конструктор класса ShipDecorator.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="player"></param>
        public ShipDecorator(Vector2 position, Ship player) : base(position)
        {
            this.player = player;
        }
    }
}
