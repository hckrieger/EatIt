using EatIt.GameObjects;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace EatIt.Scenes
{
    class MainScene : GameState
    {
        Player player = new Player();
        double dropInterval;
        //float startDropInterval;

        List<FallingObject> objectPool;
        Enemy enemy = new Enemy(); 
        int objectQty = 7;
        int catchSet;
        int maxCatchSet;
        int totalCatches;
        int level;
        bool startFlicker;
        double maxDouble;
        float flickerTimer = .15f;
        float durationofFlickering;
        TextGameObject intervalText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        TextGameObject levelText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        TextGameObject maxCatchSetText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        TextGameObject directionTimerText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        TextGameObject speedText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        TextGameObject scoreFont = new TextGameObject("Fonts/ScoreFont", .4f, Color.DarkSlateBlue, TextGameObject.Alignment.Right);
        TextGameObject flickerText = new TextGameObject("Fonts/TestFont", 1f, Color.White);

        SpriteGameObject gameOverWindow = new SpriteGameObject("Sprites/GameOverWindow", 1f);

        public MainScene()
        {
            

            objectPool = new List<FallingObject>();
            gameObjects.AddChild(player);

            intervalText.LocalPosition = new Vector2(10, 10);
            gameObjects.AddChild(intervalText); 

            levelText.LocalPosition = new Vector2(10, 25);
            gameObjects.AddChild(levelText);

            maxCatchSetText.LocalPosition = new Vector2(10, 50);
            gameObjects.AddChild(maxCatchSetText);

            directionTimerText.LocalPosition = new Vector2(10, 75);
            gameObjects.AddChild(directionTimerText);

            speedText.LocalPosition = new Vector2(10, 100);
            gameObjects.AddChild(speedText);

            scoreFont.LocalPosition = new Vector2(545, 425);
            gameObjects.AddChild(scoreFont);

            flickerText.LocalPosition = new Vector2(100, 100);
            gameObjects.AddChild(flickerText);

            gameOverWindow.SetOriginToCenter();
            gameOverWindow.Visible = false;
            gameOverWindow.LocalPosition = new Vector2(275, 225);
            gameObjects.AddChild(gameOverWindow);

            for (int i = 0; i < objectQty; i++)
            {
                objectPool.Add(new FallingObject());
                gameObjects.AddChild(objectPool[i]);
            }

            gameObjects.AddChild(enemy);

            Reset();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            dropInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dropInterval <= 0)
            {
                foreach(FallingObject obj in objectPool)
                {
                    if (!obj.Active)
                    {
                        obj.Activate(); 
                        break;
                    }
                }

                dropInterval = ExtendedGame.Random.NextDouble();
                if (dropInterval > maxDouble)
                    dropInterval = maxDouble;
            }

            foreach (FallingObject obj in objectPool)
            {
                if (obj.BoundingBox.Intersects(player.BoundingBox))
                {
                    ExtendedGame.AssetManager.PlaySoundEffect($"Sound/contact_{obj.Index}");
                    if (totalCatches % 100 == 0)
                        ExtendedGame.AssetManager.PlaySoundEffect("Sound/score_hundred");
                    
                    catchSet++;
                    totalCatches++;
                    //obj.Deactivate();
                }

                if (obj.BoundingBox.Intersects(player.BoundingBox)/* || obj.LocalPosition.Y >= 550*/)
                {
                    obj.Deactivate();
                }

                if (obj.LocalPosition.Y >= 550)
                {

                    GameOver(gameTime);

                }

            }


            //flickerText.Text = "Flicker Timer: " + flickerTimer.ToString();
            //intervalText.Text = "drop interval: " + maxDouble.ToString();
            //levelText.Text = "level: " + level.ToString();
            //maxCatchSetText.Text = "max catch set: " + maxCatchSet.ToString();
            //directionTimerText.Text = "max switch time:" + enemy.MaxSwitchTime.ToString();
            // speedText.Text = "speed: " + player.Speed.ToString();
            scoreFont.Text = "Score: " + totalCatches.ToString();
            if (catchSet >= maxCatchSet)
            {
                level++;



                if (level < 35)
                {
                    
                    foreach (FallingObject obj in objectPool)
                    {
                        obj.FallingSpeed += 9.25f;
                    }
                    player.Speed = Math.Abs(player.Speed) + 9.5f;
                    enemy.Speed = Math.Abs(enemy.Speed) + 9.5f;


                    if (level % 5 == 0)
                        maxCatchSet++;
                    maxDouble -= .005d;

                    enemy.MaxSwitchTime -= .045;
                }

                catchSet = 0;
                    
            }

            if (durationofFlickering <= 0)
            {
                gameOverWindow.Visible = true;
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (durationofFlickering <= 0)
            {
                if (inputHelper.KeyPressed(Keys.Space))
                {
                    Reset();
                }
            }
        }

        public Enemy Enemy
        {
            get { return enemy; }
        }

        public Player Player
        {
            get { return player; }
        }

        public void GameOver(GameTime gameTime)
        {
            

            enemy.Speed = 0;
            player.Speed = 0;

            if (startFlicker)
            {
                ExtendedGame.AssetManager.PlaySoundEffect("Sound/flicker_duration");
                startFlicker = false;
            }
                
            foreach (FallingObject obj in objectPool)
            {
                obj.Pause();
            }

            durationofFlickering -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (durationofFlickering <= 0)
            {
                flickerTimer -= 0;
                ExtendedGame.BackgroundColor = Color.Black;
            } else
            {
                flickerTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

                if (flickerTimer <= 0)
                {
                    if (ExtendedGame.BackgroundColor == Color.Black)
                        ExtendedGame.BackgroundColor = Color.Red;
                    else
                        ExtendedGame.BackgroundColor = Color.Black;

                    flickerTimer = .15f;
                   
                }

            
        }

        public override void Reset()
        {
            base.Reset();
            player.Reset();
            enemy.Reset();
            foreach (FallingObject obj in objectPool)
                obj.Reset();

            startFlicker = true;
            durationofFlickering = 2.75f;
            maxDouble = .375d;
            level = 1;
            totalCatches = 0;
            maxCatchSet = 10;
            catchSet = 0;
            gameOverWindow.Visible = false;
        }
    }
}
