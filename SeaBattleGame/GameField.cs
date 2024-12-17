using SharpDX;
using System.Windows.Input;
using System.Collections.Generic;
using GameObjects;
using System;
using UDPLib;
using Collision = GameObjects.Collision;

namespace SeaBattleGame
{
    /// <summary>
    /// Публичный класс GameField.
    /// </summary>
    public class GameField
    {
        /// <summary>Time helper object for current time and delta time measurements.</summary>
        private TimeHelper _timeHelper;
        /// <summary>
        /// Первый игрок.
        /// </summary>
        public Ship player1;
        /// <summary>
        /// Второй игрок.
        /// </summary>
        public Ship player2;
        /// <summary>
        /// Скалы.
        /// </summary>
        public List<Rock> rocks;
        /// <summary>
        /// Создание типа корябля.
        /// </summary>
        ShipFactory shipFactory = new ShipFactory();
        /// <summary>
        /// Границы отрисовки.
        /// </summary>
        Vector4 borders;
        /// <summary>
        /// Конструктор класса GameField.
        /// </summary>
        public Client client;
        public GameField()
        {

        }
        /// <summary>
        /// Инициализирование объектов.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="idShip"></param>
        /// <param name="client"></param>
        /// <param name="_timeHelper"></param>
        public void Initialize(Dictionary<string, ShipType> dc, int idShip, Client client, TimeHelper _timeHelper)
        {
            GeneratePlayer(dc, idShip);
            GenerateRocks();
            borders = new Vector4(50, 0, 1800, 800);
            this.client = client;
            this._timeHelper = _timeHelper;
        }
        /// <summary>
        /// Создание игроков.
        /// </summary>
        /// <param name="dc"></param>
        private void GeneratePlayer(Dictionary<string, ShipType> dc, int idShip)
        {
            Vector2 position1, position2;
            if (idShip == 1)
            {
                position1 = new Vector2(150, 450);
                position2 = new Vector2(1700, 450);
            }
            else
            {
                position2 = new Vector2(150, 450);
                position1 = new Vector2(1700, 450);
            }
            ShipType ship1, ship2;
            ship1 = dc["Ship1"];
            ship2 = dc["Ship2"];
            player1 = shipFactory.GetShip(ship1);
            player2 = shipFactory.GetShip(ship2);
            player1.position = position1;
            player2.position = position2;
        }
        /// <summary>
        /// Генерация скал.
        /// </summary>
        private void GenerateRocks()
        {
            rocks = new List<Rock>();
            rocks.Add(new Rock(new Vector2(100, 100)));
            rocks.Add(new Rock(new Vector2(720, 460)));
            rocks.Add(new Rock(new Vector2(1300, 700)));
            rocks.Add(new Rock(new Vector2(1350, 200)));
        }
        /// <summary>
        /// Движение игрока.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pl"></param>
        /// <param name="vec"></param>
        /// <param name="direction"></param>
        private void MovePlayer(Key key, Ship pl, Vector2 vec, int direction)
        {
            if (Keyboard.IsKeyDown(key))
            {
                pl.position += vec;
                ShipUtilities.SwapDirection(pl, direction);
                if (Collision.CollisionCheck(player1, player2) || Collision.CollisionRockPl(pl, rocks) || !BorderCheck(pl.position))
                    pl.position -= vec;
            }
        }
        /// <summary>
        /// Обновление позиции игрока.
        /// </summary>
        public void UpdatePosition()
        {
            if (player1.isAlive && player2.isAlive)
            {
                //Движение первого игрока.
                MovePlayer(Key.W, player1, new Vector2(0, -player1.GetSpeed()), 2); //Вектор смещения, 2 - направление
                MovePlayer(Key.S, player1, new Vector2(0, player1.GetSpeed()), 4);
                MovePlayer(Key.A, player1, new Vector2(-player1.GetSpeed(), 0), 1);
                MovePlayer(Key.D, player1, new Vector2(player1.GetSpeed(), 0), 3);

                client.MyShip.x = (int)player1.position.X;
                client.MyShip.y = (int)player1.position.Y;
                client.MyShip.dircetion = player1.direction;

                //Движение второго игрока.
                player2.position = new Vector2(client.EnemyShip.x, client.EnemyShip.y);
                ShipUtilities.SwapDirection(player2, client.EnemyShip.dircetion);

                //Стрельба первого игрока.
                if (Keyboard.IsKeyDown(Key.F) && player1.IsReload <= 0)
                {
                    Shooting(player1);
                    client.MyShip.bullet = 1;
                }

                if (player1.IsReload > 0)
                    player1.IsReload -= (int)(_timeHelper.DeltaT * 70);

                //Стрельба второго игрока.
                if (client.EnemyShip.bullet == 1)
                {
                    Shooting(player2);
                    client.EnemyShip.bullet = 0;
                }

                if (player2.IsReload > 0)
                    player2.IsReload -= (int)(_timeHelper.DeltaT * 70);

                //Смена режима стрельбы
                if (Keyboard.IsKeyDown(Key.E) && player1.IsReload <= 0)
                {
                    player1.weaponsMode = !player1.weaponsMode;
                    player1.IsReload = player1.GetTimeRecharge();
                    client.MyShip.mode = 1;
                }
                if (client.EnemyShip.mode == 1)
                {
                    player2.weaponsMode = !player2.weaponsMode;
                    player2.IsReload = player2.GetTimeRecharge();
                    client.EnemyShip.mode = 0;
                }

                //Жив ли первый игрок.
                if (player1.isAlive && player1.Health == 0)
                {
                    player1.isAlive = false;
                }

                //Жив ли второй игрок.
                if (player2.isAlive && player2.Health == 0)
                {
                    player2.isAlive = false;
                }
            }
        }
        /// <summary>
        /// Стрельба, создание снарядов.
        /// </summary>
        /// <param name="pl"></param>
        private void Shooting(Ship pl)
        {
            if (pl.weaponsMode)
            {
                int direction = pl.direction == 2 || pl.direction == 4 ? 1 : 2;
                //Если корабль вертикально
                if (direction == 1)
                {
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X, pl.position.Y - 5), 1)); // 1- направление
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X, pl.position.Y - 25), 1));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X, pl.position.Y + 15), 1));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X, pl.position.Y - 5), 3));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X, pl.position.Y - 25), 3));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X, pl.position.Y + 15), 3));
                }
                //Если корабль горизантально
                else
                {
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X - 5, pl.position.Y), 2));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X - 25, pl.position.Y), 2));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X + 15, pl.position.Y), 2));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X - 5, pl.position.Y), 4));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X - 25, pl.position.Y), 4));
                    pl.bullets.Add(new Bullet(new Vector2(pl.position.X + 15, pl.position.Y), 4));
                }
            }
            else
                pl.bullets.Add(new Bullet(new Vector2(pl.position.X, pl.position.Y), pl.direction));
            pl.IsReload = pl.GetTimeRecharge();
        }
        /// <summary>
        /// Проверка границ отрисовки.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool BorderCheck(Vector2 position)
        {
            return (borders.X < position.X && borders.Y < position.Y && borders.Z > position.X && borders.W > position.Y);
        }
        /// <summary>
        /// Движение снаряда.
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="plEnemy"></param>
        public void BulletsMove(Ship pl, Ship plEnemy)
        {
            for (int i = 0; i < pl.bullets.Count; i++)
            {
                Bullet bullet = pl.bullets[i];
                switch (bullet.direction)
                {
                    case 1:
                        bullet.position.X -= _timeHelper.DeltaT * pl.GetSpeedShot() * 100;
                        break;
                    case 2:
                        bullet.position.Y -= _timeHelper.DeltaT * pl.GetSpeedShot() * 100;
                        break;
                    case 3:
                        bullet.position.X += _timeHelper.DeltaT * pl.GetSpeedShot() * 100;
                        break;
                    case 4:
                        bullet.position.Y += _timeHelper.DeltaT * pl.GetSpeedShot() * 100;
                        break;
                }

                if (!BorderCheck(bullet.position))
                {
                    pl.bullets.RemoveAt(i);
                    i--;
                }
                
                //Проверка на колиизию снаряда и скалы.
                foreach (Rock rock in rocks)
                {
                    if (Collision.CollisionRect((int)bullet.position.X, (int)bullet.position.Y, (int)rock.position.X, (int)rock.position.Y, 10, 10, rock.scale, rock.scale))
                    {
                        pl.bullets.RemoveAt(i);
                        i--;
                    }
                }
                //Получение урона игроков.
                if (Collision.CollisionBullet(plEnemy, bullet))
                {
                    if (plEnemy.isAlive)
                        if (pl.weaponsMode)
                            plEnemy.TakeDamage((int)(pl.GetDamage() - plEnemy.GetDamageAbsorption() * pl.GetDamage()));
                        else
                            plEnemy.TakeDamage((int)((pl.GetDamage() - plEnemy.GetDamageAbsorption() * pl.GetDamage()) * 3f));
                    pl.bullets.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
