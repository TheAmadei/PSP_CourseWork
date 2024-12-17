namespace GameObjects
{
    /// <summary>
    /// Класс декоратор PharaohShip.
    /// </summary>
    public class PharaohShip : ShipDecorator
    {
        int[] parameters;
        /// <summary>
        /// Конструктор класса PharaohShip.
        /// </summary>
        /// <param name="player"></param>
        public PharaohShip(Ship player) : base(player.position, player)
        {
            parameters = ShipUtilities.GetShipParameters(4);
        }
        /// <summary>
        /// Получение параметров для скорости корябля.
        /// </summary>
        /// <returns></returns>
        public override int GetSpeed()
        {
            return player.GetSpeed() + parameters[0];
        }
        /// <summary>
        /// Получение параметров для скорости снаряда.
        /// </summary>
        /// <returns></returns>
        public override int GetSpeedShot()
        {
            return player.GetSpeedShot() + parameters[1];
        }
        /// <summary>
        /// Получение параметров для перезарядки оружия.
        /// </summary>
        /// <returns></returns>
        public override int GetTimeRecharge()
        {
            return player.GetTimeRecharge() + parameters[2];
        }
        /// <summary>
        /// Получение параметров для урона.
        /// </summary>
        /// <returns></returns>
        public override int GetDamage()
        {
            return player.GetDamage() + parameters[3];
        }
        /// <summary>
        /// Получение параметров для поглащения урона.
        /// </summary>
        /// <returns></returns>
        public override float GetDamageAbsorption()
        {
            return player.GetDamageAbsorption() + parameters[4] / 100.0f;
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
