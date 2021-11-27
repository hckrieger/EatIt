using EatIt.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EatIt
{
    public class Game1 : ExtendedGame
    {
        public const string MAIN = "MAIN_SCENE";
        public const string TITLE = "TITLE_SCREEN";

        public Game1()
        {
            IsMouseVisible = true;
            worldSize = new Point(550, 450);
            windowSize = new Point(550, 450);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            GameStateManager.AddGameState(MAIN, new MainScene());

            GameStateManager.AddGameState(TITLE, new TitleScreen());
            GameStateManager.SwitchTo(TITLE);

            
        }
    }
}
