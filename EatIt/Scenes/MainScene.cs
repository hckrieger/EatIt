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

        public enum GameState 
        {
            Playing,
            GameStart,
            GameOver
        };

        public GameState gameState;

        List<FallingObject> objectPool;
        Enemy enemy = new Enemy(); 
        int objectQty = 7;
        int catchSet;
        int maxCatchSet;
        int totalCatches, runningTotal;
        int level;
        int games = 0;
        int highScore = 0;
        bool startFlicker;
        double maxDouble;
        float flickerTimer = .15f;
        float durationofFlickering;
        bool initiateTie;

        //TextGameObject intervalText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        //TextGameObject levelText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        //TextGameObject maxCatchSetText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        //TextGameObject directionTimerText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        //TextGameObject speedText = new TextGameObject("Fonts/TestFont", 1f, Color.White);
        TextGameObject scoreFont = new TextGameObject("Fonts/ScoreFont", .4f, Color.DarkSlateBlue, TextGameObject.Alignment.Right);
        TextGameObject flickerText = new TextGameObject("Fonts/TestFont", 1f, Color.White);

        SpriteGameObject displayWindow = new SpriteGameObject("Sprites/GameOverWindow", .9f);
        TextGameObject displayHeader = new TextGameObject("Fonts/DisplayHeader", 1f, Color.Black, TextGameObject.Alignment.Center);
        TextGameObject displayMessage = new TextGameObject("Fonts/DisplayMessage", 1f, Color.Black, TextGameObject.Alignment.Center);
        public MainScene()
        {
            gameState = GameState.GameStart;

            objectPool = new List<FallingObject>();
            gameObjects.AddChild(player);

            //intervalText.LocalPosition = new Vector2(10, 10);
            //gameObjects.AddChild(intervalText);

            //levelText.LocalPosition = new Vector2(10, 25);
            //gameObjects.AddChild(levelText);

            //maxCatchSetText.LocalPosition = new Vector2(10, 50);
            //gameObjects.AddChild(maxCatchSetText);

            //directionTimerText.LocalPosition = new Vector2(10, 75);
            //gameObjects.AddChild(directionTimerText);

            //speedText.LocalPosition = new Vector2(10, 100);
            //gameObjects.AddChild(speedText);

            scoreFont.LocalPosition = new Vector2(545, 425);
            gameObjects.AddChild(scoreFont);

            flickerText.LocalPosition = new Vector2(100, 100);
            gameObjects.AddChild(flickerText);

            displayWindow.SetOriginToCenter();
            //gameOverWindow.Visible = false;
            displayWindow.LocalPosition = new Vector2(275, 225);
            gameObjects.AddChild(displayWindow);


            displayHeader.LocalPosition = new Vector2(275, 140);
            gameObjects.AddChild(displayHeader);

            
            gameObjects.AddChild(displayMessage);

            //Initiate the pool of falling objects so you can reuse them without having to re-allocate memory for them
            for (int i = 0; i < objectQty; i++)
            {
                objectPool.Add(new FallingObject());
                gameObjects.AddChild(objectPool[i]);
            }

            gameObjects.AddChild(enemy);

            enemy.Speed = 0;
            player.Speed = 0;
            //Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            dropInterval -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //If the drop interval timer runs out......
            if (dropInterval <= 0 && gameState == GameState.Playing)
            {
                //Drop an object
                foreach(FallingObject obj in objectPool)
                {
                    if (!obj.Active)
                    {
                        obj.Activate(); 
                        break;
                    }
                }

                //Reset timer and cap it's maximum random time
                dropInterval = ExtendedGame.Random.NextDouble();
                if (dropInterval > maxDouble)
                    dropInterval = maxDouble;
            }

            foreach (FallingObject obj in objectPool)
            {
                //If the the object collides with the player
                if (obj.BoundingBox.Intersects(player.BoundingBox))
                {
                    //Then play a specific pitch that matches with the specific color of that object
                    ExtendedGame.AssetManager.PlaySoundEffect($"Sound/contact_{obj.Index}");

                    //Play jingle if player beats the high score in the middle of playing
                    if (games > 1 && totalCatches == highScore)
                        ExtendedGame.AssetManager.PlaySoundEffect("Sound/score_hundred");

                    //Increase the catch set, total catches and running total of catches after every collision
                    catchSet++;
                    totalCatches++;
                    runningTotal++;
                    //obj.Deactivate();


                    //Deactivate the game object so it's ready to be reactivated when needed
                    obj.Deactivate();
                }

                //If the player misses the falling object and it touches the bottom of the screen......
                if (obj.LocalPosition.Y >= 450)
                {
                    //initiate gameover method (see below)
                    GameOver(gameTime);
                }

            }



            //flickerText.Text = "Flicker Timer: " + flickerTimer.ToString();
            //intervalText.Text = "drop interval: " + maxDouble.ToString();
            //levelText.Text = "level: " + level.ToString();
            //maxCatchSetText.Text = "max catch set: " + maxCatchSet.ToString();
            //directionTimerText.Text = "max switch time:" + enemy.MaxSwitchTime.ToString();
            //speedText.Text = "speed: " + player.Speed.ToString();
            scoreFont.Text = "Score: " + totalCatches.ToString();

            //After a given number of consecutive collisions
            if (catchSet >= maxCatchSet && gameState == GameState.Playing && level <= 35)
            {
                //increase the level so it increases the difficulty gradually
                level++;


               
                    //increase the speed of the enemy, the player and the falling object
                    foreach (FallingObject obj in objectPool)
                    {
                        obj.FallingSpeed += 9f;
                    }
                    player.Speed = Math.Abs(player.Speed) + 9f;
                    enemy.Speed = Math.Abs(enemy.Speed) + 9f;

                    //if the level is divisible by 7 then increase the set number of collisions before it goes to the next level
                    if (level % 7 == 0)
                        maxCatchSet++;

                    //subtract the max random drop interval 
                    maxDouble -= .0045d;

                    //subtract the max random time for the enemy to switch directions
                    enemy.MaxSwitchTime -= .0333;
                

                catchSet = 0;
                    
            }
            

            //Make the window displaying the score visible
            if (gameState != GameState.Playing)
            {
                displayWindow.Visible = true;
                displayHeader.Visible = true;
            }


            //When the game is over and the score window is up
            if (gameState == GameState.GameOver && durationofFlickering <= 0)
            {
                

                //Show specific text depending on where the players score is relative to the high score. 
                if (games == 1)
                {
                    if (totalCatches < 10)
                        displayHeader.Text = "Try again";
                    else
                        displayHeader.Text = "Good start!";
                }

                if (totalCatches == highScore && initiateTie == true && games > 1)
                {
                    displayHeader.Text = "You tied the high score";
                }

                if (games > 1 && totalCatches > highScore)
                {
                    
                    if (totalCatches > highScore)
                    {
                        highScore = totalCatches;
                        initiateTie = false;
                    }

                    displayHeader.Text = "You got the high score!";

                    
                }


                //If the players score is greater than the high score than change the high score to that
                if (totalCatches > highScore)
                    highScore = totalCatches;

                if (games > 1 && totalCatches < highScore)
                {
                    var difference = highScore - totalCatches;
                    displayHeader.Text = difference.ToString() + " short of the highscore";
                }



                //displayHeader.Text = "Great Job!";
                displayMessage.Text = "\nScore: " + totalCatches +
                    "\nHigh Score: " + highScore.ToString() +
                    "\nAverage: " + Average.ToString() +
                    "\nGames Played: " + games.ToString() +
                    "\n\nPress space to play.";

            } else if (gameState == GameState.GameStart)
            {
                displayMessage.LocalPosition = new Vector2(275, 175);
                displayHeader.Text = "Eat It";
                displayMessage.Text = "Instructions: \n" +
                    "Use the left and right arrow key\n" +
                    "to move. Catch the falling objects\n" +
                    "so they don't hit the ground.\n" +
                    "Press Space to play\n\n" +
                    "Programmed by Hunter Krieger";
            } else
            {
                displayHeader.Text = "";
                displayMessage.Text = "";
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);
            if (gameState != GameState.Playing && durationofFlickering <= 0)
            {
                if (inputHelper.KeyPressed(Keys.Space) && gameState != GameState.Playing)
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
            
            //Stop the movement of everything
            enemy.Speed = 0;
            player.Speed = 0;
            foreach (FallingObject obj in objectPool)
            {
                obj.Pause();
            }

            
            if (startFlicker)
            {
                //Play the 'game over' sound
                ExtendedGame.AssetManager.PlaySoundEffect("Sound/flicker_duration");
                startFlicker = false;
            }
                


            durationofFlickering -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //flicker the background color between red and black really fast
            if (flickerTimer <= 0)
            {
                if (ExtendedGame.BackgroundColor == Color.Black)
                    ExtendedGame.BackgroundColor = Color.Red;
                else
                    ExtendedGame.BackgroundColor = Color.Black;

                flickerTimer = .15f;

            }

            //stop flicker after certain amount of time
            if (durationofFlickering <= 0)
            {
                flickerTimer -= 0;
                ExtendedGame.BackgroundColor = Color.Black;
                gameState = GameState.GameOver;
            } else
            {
                flickerTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }



            
        }


        //reset values to play game again
        public override void Reset()
        {
            base.Reset();

            player.Reset();
            enemy.Reset();
            foreach (FallingObject obj in objectPool)
                obj.Reset();

            displayWindow.Visible = false;
            displayHeader.Visible = false;
            gameState = GameState.Playing;
            startFlicker = true;
            durationofFlickering = 2.75f;
            dropInterval = .333f;
            maxDouble = .375d;
            level = 1;
            totalCatches = 0;
            maxCatchSet = 10;
            catchSet = 0;
            games++;
            initiateTie = true;
        }

        public decimal Average
        {
            get { 
                var average = runningTotal / games;
                return Math.Round((decimal)average);
            }
        }

    }
}
