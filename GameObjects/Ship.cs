using SharpDX;
using System.Collections.Generic;

namespace GameObjects
{
    /// <summary>
    /// Публичный абстрактный класс Ship.
    /// </summary>
    public abstract class Ship
    {
        /// <summary>
        /// Ширина коллайдера.
        /// </summary>
        public int colliderWidth;
        /// <summary>
        /// Высота коллайдера.
        /// </summary>
        public int colliderHeight;
        /// <summary>
        /// Показатель жизни игрока.
        /// </summary>
        public bool isAlive;
        /// <summary>
        /// Здоровье игрока.
        /// </summary>
        public int Health
        {
            get { return health; }
            set
            {
                if (value < 0) health = 0;
                else health = value;
            }
        }
        private int health;
        /// <summary>
        /// Направление корабля.
        /// </summary>
        public int direction;
        /// <summary>
        /// Перезарядка оружия.
        /// </summary>
        public int IsReload;
        /// <summary>
        /// Позиция корабля.
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// Длина корабля.
        /// </summary>
        public int height;
        /// <summary>
        /// Ширина корабля.
        /// </summary>
        public int width;
        /// <summary>
        /// Снаряды.
        /// </summary>
        public List<Bullet> bullets = new List<Bullet>();
        /// <summary>
        /// Скорость снаряда.
        /// </summary>
        public int speedBullet;
        /// <summary>
        /// Режим оружия.
        /// </summary>
        public bool weaponsMode;
        /// <summary>
        /// Скорость корабля.
        /// </summary>
        public int speed;
        /// <summary>
        /// Режим перезагрузки.
        /// </summary>
        public int weaponsReload;
        /// <summary>
        /// Урон.
        /// </summary>
        public int damage;
        /// <summary>
        /// Поглащение урона.
        /// </summary>
        public float damageAbsorption;

        /// <summary>
        /// Конструктор класс Ship.
        /// </summary>
        /// <param name="position"></param>
        public Ship(Vector2 position)
        {
            weaponsMode = true;
            isAlive = true;
            Health = 1000;
            IsReload = 0;
            this.position = position;
            height = 90;
            width = 180;
            colliderWidth = 90;
            colliderHeight = 90;
            direction = 1;
            damageAbsorption = 0;
        }
        /// <summary>
        /// Скорость корабля.
        /// </summary>
        /// <returns></returns>
        public abstract int GetSpeed();
        /// <summary>
        /// Скорость снаряда.
        /// </summary>
        /// <returns></returns>
        public abstract int GetSpeedShot();
        /// <summary>
        /// Перезарядка оружия.
        /// </summary>
        /// <returns></returns>
        public abstract int GetTimeRecharge();
        /// <summary>
        /// Получение урона.
        /// </summary>
        /// <returns></returns>
        public abstract int GetDamage();
        /// <summary>
        /// Поглащение урона, в процентах.
        /// </summary>
        /// <returns></returns>
        public abstract float GetDamageAbsorption();
        /// <summary>
        /// Получение урона для игрока.
        /// </summary>
        /// <param name="damage"></param>
        public abstract void TakeDamage(int damage);
    }
}
