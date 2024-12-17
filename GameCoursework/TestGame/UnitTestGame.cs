using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameObjects;
using SharpDX;
using SeaBattleGame;
using System.Collections.Generic;
using Collision = GameObjects.Collision;

namespace TestGame
{
    [TestClass]
    public class UnitTest
    {
        ShipFactory factory = new ShipFactory();

        [TestMethod]
        public void TestMethodShips()
        {
            Ship player1 = new ShipPlayer(new Vector2(1, 1));
            Ship player2 = new ShipPlayer(new Vector2(2, 2));

            Assert.IsTrue(Collision.CollisionCheck(player1, player2));
        }
        [TestMethod]
        public void TestMethodBulletsShip()
        {
            Ship player1 = new ShipPlayer(new Vector2(1, 1));

            Bullet bullet = new Bullet(new Vector2(1, 1), 1);

            Assert.IsTrue(Collision.CollisionBullet(player1, bullet));
        }

        [TestMethod]
        public void TestMethodPlayerRock()
        {
            Rock rock = new Rock(new Vector2(1, 1));

            List<Rock> rocks = new List<Rock>();
            rocks.Add(rock);

            Ship player1 = new ShipPlayer(new Vector2(1, 1));

            Assert.IsTrue(Collision.CollisionRockPl(player1, rocks));
        }

        [TestMethod]
        public void TestMethodFactory()
        {
            Ship player = factory.GetShip(ShipType.Pharaoh);

            Assert.IsTrue(player is PharaohShip);
        }
    }
}
