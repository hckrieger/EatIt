using EatIt.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EatIt.GameObjects
{
    class FallingObject : SpriteGameObject
    {
        Vector2 spawnPosition;
        public float FallingSpeed { get; set; }

        public FallingObject() : base("Sprites/FallingObjects@2x1", .4f)
        {
            SetOriginToCenter();
            Reset();
            spawnPosition = LocalPosition;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Active)
                Visible = false;
            else
                Visible = true;
        }

        

        public MainScene MainScene
        {
            get
            {
                return (MainScene)ExtendedGame.GameStateManager.GetGameState(Game1.MAIN);
            }
        }

        public void Activate()
        {
            Active = true;
            SheetIndex = ExtendedGame.Random.Next(2);
            if (MainScene != null)
                LocalPosition = MainScene.Enemy.LocalPosition;
            
            velocity.Y = FallingSpeed;
        }

        public void Deactivate()
        {
            
            Active = false;
            velocity.Y = 0;
            LocalPosition = new Vector2(-100, -100);
        }

        public void Pause()
        {
            velocity.Y = 0;
           
        }

        public override void Reset()
        {
            Deactivate();
            FallingSpeed = 250;
        }
    }
}
