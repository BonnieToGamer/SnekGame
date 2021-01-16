using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace newSnekGame
{
    class Snek
    {
        // Strings
        private string dedText;
        private string wonText;

        // Integers
        private int fontSize;
        private int debugSpacer;
        private int debugTimer;
        private int dx, dy;
        private int snekWidth, snekHeight;
        private int moveTimer;
        private int score;
        private int r1;
        private int r2;
        private int snekLength;
        private int maxSize;

        // Booleans
        private bool debug;
        private bool ded;
        private bool won;
        private bool keyboardLock;
        private bool AI;


        // Arrays and Lists
        private string[] debugStrings;
        private List<Vector2> snek;
        private int[,] grid;
        private int[,] hamCycle;

        // Vectors
        private Vector2 applePos;

        // Texture2D
        private Texture2D pixel;

        // Rectangles
        private Rectangle snekHeadBBox;
        private Rectangle appleBBox;
        private Rectangle borderBBox;

        // Weird
        private KeyboardState kstate;
        private Random r;

        public void SnekINIT(GraphicsDevice graphicsDevice)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            //pixel.CreateBorder(1, Color.Black);

            borderBBox = new Rectangle(new Point(25, 50), new Point(775, 425));

            grid = new int[30, 18];
            hamCycle = new int[30, 18];
            AI = false;
            fontSize = 10;
            snekLength = 0;
            debug = false;
            debugSpacer = fontSize;
            snek = new List<Vector2>
            {
                new Vector2(225, 225)
            };
            applePos = new Vector2(400, 225);
            snekWidth = 25;
            snekHeight = 25;
            ded = false;
            won = false;
            keyboardLock = false;
            r = new Random();
            maxSize = 510;
        }

        public void SnekUPDATE()
        {
            kstate = Keyboard.GetState();

            if (!ded && !won)
            {
                SnekMOVE();
                snekHeadBBox = new Rectangle(snek[0].ToPoint(), new Point(snekWidth, snekHeight));
                appleBBox = new Rectangle(applePos.ToPoint(), new Point(snekWidth, snekHeight));

                if (!EasyCollision(snekHeadBBox, borderBBox))
                    ded = true;

                if (EasyCollision(snekHeadBBox, appleBBox))
                {
                    if (snek.Count < maxSize)
                    {
                        GenerateApple();
                        grid[(int)applePos.X / 25, (int)applePos.Y / 25] = 3;
                        snek.Add(new Vector2(-25, -25));
                        snekLength++;
                    }

                    else
                    {
                        won = true;
                    }
                    score++;
                }

                for (int i = 1; i < snekLength; i++)
                {
                    if (snek[0] == snek[i])
                    {
                        ded = true;
                    }
                }
            }

            else
            {
                if (kstate.IsKeyDown(Keys.Space))
                {
                    ded = false;
                    won = false;
                    snek.RemoveRange(0, snek.Count);
                    snek.Add(new Vector2(225, 225));
                    applePos = new Vector2(400, 225);
                    snekLength = 0;
                    score = 0;
                    dx = 0;
                    dy = 0;
                }
            }

            // Debug
            if (kstate.IsKeyDown(Keys.F3) && debugTimer > 10)
            {
                if (debug)
                    debug = false;

                else
                    debug = true;
                debugTimer = 0;
            }

            if (debugTimer > 20)
                debugTimer = 0;
            else
                debugTimer++;
        }

        private void SnekMOVE()
        {
            for (int y = 0; y < 18; y++)
            {
                for (int x = 0; x < 30; x++)
                {
                    grid[x, y] = 0;
                }
            }
            if (!AI)
            {
                // Check for keypresses every frame
                if (kstate.IsKeyDown(Keys.Up) && dy != 25 && !keyboardLock)
                {
                    dy = -25;
                    dx = 0;
                    keyboardLock = true;
                }
                else if (kstate.IsKeyDown(Keys.Down) && dy != -25 && !keyboardLock)
                {
                    dy = 25;
                    dx = 0;
                    keyboardLock = true;
                }
                else if (kstate.IsKeyDown(Keys.Left) && dx != 25 && !keyboardLock)
                {
                    dx = -25;
                    dy = 0;
                    keyboardLock = true;
                }
                else if (kstate.IsKeyDown(Keys.Right) && dx != -25 && !keyboardLock)
                {
                    dx = 25;
                    dy = 0;
                    keyboardLock = true;
                }
            }

            // Only moves after a certain delay
            if (moveTimer >= 10)
            {
                moveTimer = 0;
                for (int i = snekLength; i >= 1; i--)
                {
                    snek[i] = snek[i - 1];
                    grid[(int)snek[i].X / 25, (int)snek[i].Y / 25] = 8;
                }
                
                snek[0] = new Vector2(snek[0].X + dx, snek[0].Y + dy);
                grid[(int)snek[0].X / 25, (int)snek[0].Y / 25] = 7;
            }
            moveTimer++;
            keyboardLock = false;
        }

        public void SnekDRAW(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, SpriteFont font)
        {
            if(!ded && !won)
            {
                DrawBorder(spriteBatch);
                spriteBatch.Draw(pixel, new Rectangle(snek[0].ToPoint(), new Point(snekWidth, snekHeight)), Color.White);
                for (int i = snekLength; i >= 1; i--)
                {
                    spriteBatch.Draw(pixel, new Rectangle(snek[i].ToPoint(), new Point(snekWidth, snekHeight)), Color.White);
                }
                spriteBatch.Draw(pixel, new Rectangle(applePos.ToPoint(), new Point(snekWidth, snekHeight)), Color.Red);
                spriteBatch.DrawString(font, "Score: " + score.ToString(), new Vector2(25, 15), Color.White);
            }

            else if (ded)
            {
                dedText = "YOU DIED! YOUR SCORE WAS " + score.ToString() + "\n  PRESS SPACE TO CONTINUE";
                spriteBatch.DrawString(font, dedText, new Vector2((graphics.PreferredBackBufferWidth / 2) - 75, (graphics.PreferredBackBufferHeight / 2) - 12), Color.White);
            }

            else
            {
                wonText = "YOU WON! YOUR SCORE WAS " + score.ToString() + "\n  PRESS SPACE TO CONTINUE";
                spriteBatch.DrawString(font, wonText, new Vector2((graphics.PreferredBackBufferWidth / 2) - 75, (graphics.PreferredBackBufferHeight / 2) - 12), Color.White);
            }

            // Draws the debug sceen
            if (debug)
            {
                debugStrings = new string[3] { "Snek head Location: " + snek[0].ToString(), "Snek count: " + snek.Count.ToString(), 
                    "Body: " + snek[0].ToString()};
                foreach (string strings in debugStrings)
                {
                    spriteBatch.DrawString(font, strings, new Vector2(0, debugSpacer), Color.White);
                    debugSpacer += fontSize;
                }
                debugSpacer = fontSize;
            }
        }
        
        private void GenerateApple()
        {
            Apple:
            r1 = r.Next(1, 30);
            r2 = r.Next(2, 18);

            applePos = new Vector2(r1 * 25, r2 * 25);
            if (snek[0] == applePos)
                goto Apple;
            for (int i = -snekLength; i <= -1; i++)
            {
                if (snek[i * -1] == applePos)
                    goto Apple;
            }
        }

        private void DrawBorder(SpriteBatch spriteBatch)
        {
            // Left and Right
            spriteBatch.Draw(pixel, new Rectangle(23, 50, 1, 425), Color.White); // Left
            spriteBatch.Draw(pixel, new Rectangle(801, 50, 1, 425), Color.White); // Right

            // Top and Bottom
            spriteBatch.Draw(pixel, new Rectangle(25, 48, 775, 1), Color.White); // Top
            spriteBatch.Draw(pixel, new Rectangle(25, 476, 776, 1), Color.White); // Bottom
        }

        private bool EasyCollision(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Intersects(rect2);
        }

        private void smooth_brain()
        {

        }
    }

    static class Utilities
    {
        public static void CreateBorder(this Texture2D texture, int borderWidth, Color borderColor)
        {
            Color[] colors = new Color[texture.Width * texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    bool colored = false;
                    for (int i = 0; i <= borderWidth; i++)
                    {
                        if (x == i || y == i || x == texture.Width - 1 - i || y == texture.Height - 1 - i)
                        {
                            colors[x + y * texture.Width] = borderColor;
                            colored = true;
                            break;
                        }
                    }

                    if (colored == false)
                        colors[x + y * texture.Width] = Color.Transparent;
                }
            }

            texture.SetData(colors);
        }
    }
}
