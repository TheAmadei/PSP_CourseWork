using SharpDX;

namespace GameObjects
{
    /// <summary>
    /// Публичный класс ShipFactory.
    /// </summary>
    public class ShipFactory
    {
        /// <summary>
        /// Фабричный метод создания корбаля.
        /// </summary>
        /// <param name="shipType"></param>
        /// <returns></returns>
        public Ship GetShip(ShipType shipType)
        {
            Ship player = new ShipPlayer(new Vector2());
            switch (shipType)
            {
                case ShipType.Alaska:
                    player = new AlaskaShip(player);
                    break;
                case ShipType.Arabella:
                    player = new ArabellaShip(player);
                    break;
                case ShipType.Emma:
                    player = new EmmaShip(player);
                    break;
                case ShipType.Ghost:
                    player = new GhostShip(player);
                    break;
                case ShipType.Pharaoh:
                    player = new PharaohShip(player);
                    break;
            }
            return player;
        }
    }
}
