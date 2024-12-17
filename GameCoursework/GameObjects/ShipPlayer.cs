using SharpDX;

namespace GameObjects
{
    /// <summary>
    /// Публичный класс ShipPlayer.
    /// </summary>
    public class ShipPlayer : Ship
    {
        /// <summary>
        /// Конструктор класса ShipPlayer.
        /// </summary>
        /// <param name="position"></param>
        public ShipPlayer(Vector2 position) : base(position)
        {
        }
        /// <summary>
        /// Получение скорости корабля.
        /// </summary>
        /// <returns></returns>
        public override int GetSpeed()
        {
            return speed;
        }
        /// <summary>
        /// Получение скорости снаряда.
        /// </summary>
        /// <returns></returns>
        public override int GetSpeedShot()
        {
            return speedBullet;
        }
        /// <summary>
        /// Получение времени перезарядки.
        /// </summary>
        /// <returns></returns>
        public override int GetTimeRecharge()
        {
            return weaponsReload;
        }
        /// <summary>
        /// Получение урона.
        /// </summary>
        /// <returns></returns>
        public override int GetDamage()
        {
            return damage;
        }
        /// <summary>
        /// Получение поглащения урона.
        /// </summary>
        /// <returns></returns>
        public override float GetDamageAbsorption()
        {
            return damageAbsorption;
        }
        /// <summary>
        /// Подсчет урона для игрока.
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }
}
