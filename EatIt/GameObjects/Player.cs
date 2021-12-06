using EatIt.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace EatIt.GameObjects
{
    class Player : SpriteGameObject
    {
        public float Speed { get; set; } = 0;

        public Player() : base("Sprites/Eater", .5f)
        {
            SetOriginToCenter();
            Reset();

        }



        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            //Input controls for player movement
            if (inputHelper.KeyDown(Keys.Left) && LocalPosition.X >= 32)
                velocity.X = -Speed;
            else if (inputHelper.KeyDown(Keys.Right) && LocalPosition.X <= 518)
                velocity.X = Speed;
            else
                velocity.X = 0;
        }

        public override void Reset()
        {
            base.Reset();
            LocalPosition = new Vector2(275, 393);
            Speed = 250;
        }
    }
}
