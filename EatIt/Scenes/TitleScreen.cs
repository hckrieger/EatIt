using EatIt.GameObjects;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace EatIt.Scenes
{
    class TitleScreen : GameState
    {
        TextGameObject test = new TextGameObject("Fonts/TestFont", 1f, Color.White);

        public TitleScreen()
        {
            test.LocalPosition = new Vector2(100, 100);

            test.Text = "Press Space";
            gameObjects.AddChild(test);

            //gameObjects.AddChild(player);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (inputHelper.KeyPressed(Keys.Space))
            {
                ExtendedGame.GameStateManager.SwitchTo(Game1.MAIN);
            }
        }
    }
}
