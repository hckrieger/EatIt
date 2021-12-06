using EatIt.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EatIt.GameObjects
{
    class Enemy : SpriteGameObject
    {
    
        float xPos;
        float yPos;
        public double DirectionTimer { get; set; }
        public double MaxSwitchTime { get; set; }
        public float Speed { get; set; }

        public Enemy() : base("Sprites/Enemy", .5f)
        {
            Reset();
            SetOriginToCenter();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            LocalPosition = new Vector2(xPos, yPos);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            DirectionTimer -= dt;

            xPos += dt * Speed;

            if (LocalPosition.X >= 528 && Speed > 0 || LocalPosition.X <= 28 && Speed <= 0 || DirectionTimer <= 0)
            {
                Speed = -Speed;
                DirectionTimer = ExtendedGame.Random.NextDouble() * MaxSwitchTime;
            }
        }

        public override void Reset()
        {
            base.Reset();

            Speed = 250;
            xPos = 275;
            yPos = 16;
            MaxSwitchTime = 3d;
        }
    }
}
