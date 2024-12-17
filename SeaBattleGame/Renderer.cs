using Controller;
using GameObjects;
using UDPLib;
using SharpDX;
using SharpDX.Direct2D1;
using System.Collections.Generic;

namespace SeaBattleGame
{
    /// <summary>
    /// Класс Renderer.
    /// </summary>
    class Renderer : Direct2DComponent
    {
        GameField field = new GameField();
        private TimeHelper _timeHelper;
        private SolidColorBrush color1;
        private SolidColorBrush colorHealth;
        private SolidColorBrush colorHealthBg;
        private SolidColorBrush colorReload;
        private SolidColorBrush colorReloadBg;
        SharpDX.DirectWrite.Factory factory;
        Dictionary<string, ShipType> dc;

        Bitmap ship1Left, ship1Right, ship1Up, ship1Down,
        ship2Left, ship2Right, ship2Up, ship2Down,
        bulletBmp, seaBmp, rockBmp, gameOver;

        /// <summary>
        /// Конструктор класса Renderer.
        /// </summary>
        /// <param name="dc"></param>
        public Renderer(Dictionary<string, ShipType> dc, int idShip, Client client)
        {

            _timeHelper = new TimeHelper();
            field.Initialize(dc, idShip, client, _timeHelper);
            this.dc = dc;
        }
        /// <summary>
        /// Инициализирование Bitmap кораблей.
        /// </summary>
        /// <param name="dc"></param>
        private void InitializeBmp(Dictionary<string, ShipType> dc)
        {
            ship1Left = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\" + dc["Ship1"].ToString() + "Left.png");
            ship1Right = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\" + dc["Ship1"].ToString() + "Right.png");
            ship1Up = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\" + dc["Ship1"].ToString() + "Up.png");
            ship1Down = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\" + dc["Ship1"].ToString() + "Down.png");

            ship2Left = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\" + dc["Ship2"].ToString() + "Left.png");
            ship2Right = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\" + dc["Ship2"].ToString() + "Right.png");
            ship2Up = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\" + dc["Ship2"].ToString() + "Up.png");
            ship2Down = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\" + dc["Ship2"].ToString() + "Down.png");
        }
        /// <summary>
        /// Инициализация рендера.
        /// </summary>
        protected override void InternalInitialize()
        {
            base.InternalInitialize();

            InitializeBmp(dc);
            bulletBmp = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\bullet.png");
            seaBmp = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\sea.png");
            rockBmp = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\rock.png");
            gameOver = BitmapWorker.LoadFromFile(RenderTarget2D, @"resources\gameOver.png");

            color1 = new SolidColorBrush(RenderTarget2D, new Color(0, 0, 0));
            colorHealth = new SolidColorBrush(RenderTarget2D, new Color(195, 0, 0));
            colorHealthBg = new SolidColorBrush(RenderTarget2D, new Color(100, 0, 0));
            colorReload = new SolidColorBrush(RenderTarget2D, new Color(0, 55, 195));
            colorReloadBg = new SolidColorBrush(RenderTarget2D, new Color(0, 30, 100));
            factory = new SharpDX.DirectWrite.Factory();

        }
        /// <summary>
        /// Внутряннее деинициализирование.
        /// </summary>
        protected override void InternalUninitialize()
        {
            Utilities.Dispose(ref color1);

            base.InternalUninitialize();
        }
        /// <summary>
        /// Отрисовка игрового поля.
        /// </summary>
        protected override void Render()
        {

            _timeHelper.Update();
            //Обновление позиции.
            field.UpdatePosition();
            //Фон.
            RenderTarget2D.Clear(new Color(255, 255, 255));
            RenderTarget2D.DrawBitmap(seaBmp, new RectangleF(0, 0, 1900, 1000), 1.0f, BitmapInterpolationMode.Linear);

            //Полет пули и проверка на удар.
            field.BulletsMove(field.player1, field.player2);
            field.BulletsMove(field.player2, field.player1);

            //Отрисовка скал.
            DrawRocks();

            //Отрисовка игроков.
            Bitmap ship = ChooseDirectionShip1(field.player1.direction);
            RenderTarget2D.DrawBitmap(ship, new RectangleF(field.player1.position.X - field.player1.width / 2, field.player1.position.Y - field.player1.height / 2, field.player1.width, field.player1.height), 1.0f, BitmapInterpolationMode.Linear);
            
            ship = ChooseDirectionShip2(field.player2.direction);
            RenderTarget2D.DrawBitmap(ship, new RectangleF(field.player2.position.X - field.player2.width / 2, field.player2.position.Y - field.player2.height / 2, field.player2.width, field.player2.height), 1.0f, BitmapInterpolationMode.Linear);

            var textFormat_pl = new SharpDX.DirectWrite.TextFormat(factory, "Haettenschweiler", 30);
            RenderTarget2D.DrawText("Игрок 1", textFormat_pl, new RectangleF(50, 20, 85, 46), color1);
            RenderTarget2D.DrawText("Игрок 2", textFormat_pl, new RectangleF(1720, 20, 85, 46), color1);
            //Полоса здоровья первого игрока.
            RenderTarget2D.FillRectangle(new RectangleF(50, 60, 400, 20), colorHealthBg);
            RenderTarget2D.FillRectangle(new RectangleF(50, 60, (int)(field.player1.Health * (400.0 / 1000)), 20), colorHealth);
            //Полоса перезарядки первого игрока.
            RenderTarget2D.FillRectangle(new RectangleF(50, 100, 400, 20), colorReloadBg);
            RenderTarget2D.FillRectangle(new RectangleF(50, 100, (int)((field.player1.GetTimeRecharge() - field.player1.IsReload) * (400.0 / field.player1.GetTimeRecharge())), 20), colorReload);

            //Полоса здоровья второго игрока.
            RenderTarget2D.FillRectangle(new RectangleF(1400, 60, 400, 20), colorHealthBg);
            RenderTarget2D.FillRectangle(new RectangleF(1400, 60, (int)(field.player2.Health * (400.0 / 1000)), 20), colorHealth);
            //Полоса перезарядки второго игрока.
            RenderTarget2D.FillRectangle(new RectangleF(1400, 100, 400, 20), colorReloadBg);
            RenderTarget2D.FillRectangle(new RectangleF(1400, 100, (int)((field.player2.GetTimeRecharge() - field.player2.IsReload) * (400.0 / field.player2.GetTimeRecharge())), 20), colorReload);

            //Отрисовка снарядов.
            DrawBullets(field.player1);
            DrawBullets(field.player2);

            if (!field.player1.isAlive || !field.player2.isAlive || field.client.isEnd != 0)
            {
                var textFormat = new SharpDX.DirectWrite.TextFormat(factory, "Ascent 2 Stardom(RUS BY LYAJKA)", 110);
                RenderTarget2D.DrawBitmap(gameOver, new RectangleF(0, 0, 1900, 1000), 1.0f, BitmapInterpolationMode.Linear);
                if (field.client.isEnd == 0)
                {
                    if (!field.player1.isAlive)
                    {
                        field.client.isEnd = 2;
                    }
                    else
                    {
                        field.client.isEnd = 1;
                    }
                }

                RenderTarget2D.DrawText($"Игрок {field.client.isEnd} Выиграл!", textFormat, new RectangleF(450, 500, 1000, 1000), color1);
            }

        }
        /// <summary>
        /// Отрисовка снарядов.
        /// </summary>
        /// <param name="pl"></param>
        private void DrawBullets(Ship pl)
        {
            foreach (Bullet bullet in pl.bullets)
            {
                RenderTarget2D.DrawBitmap(bulletBmp, new RectangleF(bullet.position.X, bullet.position.Y, 10, 10), 1.0f, BitmapInterpolationMode.Linear);
            }
        }
        /// <summary>
        /// Отрисовка скал.
        /// </summary>
        private void DrawRocks()
        {
            foreach (Rock rock in field.rocks)
            {
                RenderTarget2D.DrawBitmap(rockBmp, new RectangleF(rock.position.X, rock.position.Y, rock.scale, rock.scale), 1.0f, BitmapInterpolationMode.Linear);
            }
        }
        /// <summary>
        /// Выбор позиции первого корабля.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Bitmap ChooseDirectionShip1(int direction)
        {
            switch (direction)
            {
                case 1: return ship1Left;
                case 2: return ship1Up;
                case 3: return ship1Right;
                case 4: return ship1Down;
            }
            return ship1Left;
        }
        /// <summary>
        /// Выбор позиции второго корябля.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Bitmap ChooseDirectionShip2(int direction)
        {
            switch (direction)
            {
                case 1: return ship2Left;
                case 2: return ship2Up;
                case 3: return ship2Right;
                case 4: return ship2Down;
            }
            return ship2Left;
        }
    }
}