using System.Collections.Generic;

namespace GameObjects
{
    /// <summary>
    /// Статический класс Collision.
    /// </summary>
    public static class Collision
    {
        /// <summary>
        /// Общая проверка на коллизию.
        /// </summary>
        /// <param name="r1x"></param>
        /// <param name="r1y"></param>
        /// <param name="r2x"></param>
        /// <param name="r2y"></param>
        /// <param name="r1w"></param>
        /// <param name="r1h"></param>
        /// <param name="r2w"></param>
        /// <param name="r2h"></param>
        /// <returns></returns>
        public static bool CollisionRect(int r1x, int r1y, int r2x, int r2y, int r1w, int r1h, int r2w, int r2h)
        {
            if (r1x + r1w >= r2x &&
                r1x <= r2x + r2w &&
                r1y + r1h >= r2y &&
                r1y <= r2y + r2h)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Проверка на коллизию между игроками.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        public static bool CollisionCheck(Ship player1, Ship player2)
        {
            int r1x, r1y, r2x, r2y, r1w, r1h, r2w, r2h;
            r1x = (int)(player1.position.X - player1.width / 2);
            r1y = (int)(player1.position.Y - player1.height / 2);
            r2x = (int)(player2.position.X - player2.width / 2);
            r2y = (int)(player2.position.Y - player2.height / 2);
            r1w = player1.width;
            r1h = player1.height;
            r2w = player2.width;
            r2h = player2.height;
            return Collision.CollisionRect(r1x, r1y, r2x, r2y, r1w, r1h, r2w, r2h);
        }

        /// <summary>
        /// Проверка на коллизию игрока с пулей.
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="bl"></param>
        /// <returns></returns>
        public static bool CollisionBullet(Ship pl, Bullet bl)
        {
            int r1x, r1y, r2x, r2y, r1w, r1h, r2w, r2h;
            r1x = (int)(pl.position.X - pl.width / 2);
            r1y = (int)(pl.position.Y - pl.height / 2);
            r2x = (int)bl.position.X;
            r2y = (int)bl.position.Y;
            r1w = pl.width;
            r1h = pl.height;
            r2w = 10;
            r2h = 10;
            return Collision.CollisionRect(r1x, r1y, r2x, r2y, r1w, r1h, r2w, r2h);
        }

        /// <summary>
        /// Проверка на коллизию игрока со скалой.
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="rocks"></param>
        /// <returns></returns>
        public static bool CollisionRockPl(Ship pl, List<Rock> rocks)
        {
            int r1x, r1y, r1w, r1h;
            r1x = (int)(pl.position.X - pl.colliderWidth / 2);
            r1y = (int)(pl.position.Y - pl.colliderHeight / 2);
            r1w = pl.colliderWidth;
            r1h = pl.colliderHeight;
            foreach (Rock rock in rocks)
            {
                if (Collision.CollisionRect(r1x, r1y, (int)rock.position.X, (int)rock.position.Y, r1w, r1h, rock.scale, rock.scale))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
