using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using OpenTK.Graphics;
using Politechnikon.game_elements;

namespace Politechnikon.engine
{
    public class Engine : GameWindow
    {
        public static int GRIDSIZE = 64, TILESIZE = 64;

        Texture2D texture, tileset;
        Level level;
        View view;


        private static Bitmap Icon;
        public Engine(int width, int height) : base(width,height,GraphicsMode.Default, "Politechnikon", GameWindowFlags.Default, DisplayDevice.Default, 2, 0, GraphicsContextFlags.ForwardCompatible)
        {
            initDisplay();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);

            view = new View(Vector2.Zero, 1.0, 0.0);

            Input.Initialize(this);
        }


        
        private void initDisplay()
        {
            Icon = (Bitmap)Image.FromFile(@"resources\\graphics\\misc\\Icon.png");
            Icon.SetResolution(64, 64);
            base.Icon = System.Drawing.Icon.FromHandle(Icon.GetHicon());
            base.VSync = VSyncMode.Adaptive;
        }



        protected override void OnLoad(EventArgs e)
        {
            //tutaj ładujemy obiekty
            base.OnLoad(e);
            texture = ContentPipe.LoadTexture("graphics\\misc\\Icon.png");
            tileset = ContentPipe.LoadTexture("graphics\\misc\\Icon.png");
            level = new Level(20, 20);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Input.MousePress(OpenTK.Input.MouseButton.Left))
            {
                Vector2 pos = new Vector2(Mouse.X, Mouse.Y) - new Vector2(this.Width,this.Height) / 2f;
                pos = view.ToWorld(pos);

                view.SetPosition(pos, TweenType.QuarticOut, 60);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Right))
            {
                view.SetPosition(view.PositionGoto + new Vector2(5, 0), TweenType.QuarticOut, 15);
            }
            if (Input.KeyDown(OpenTK.Input.Key.Left))
            {
                view.SetPosition(view.PositionGoto + new Vector2(-5, 0), TweenType.QuarticOut, 15);
            }
            if (Input.KeyDown(OpenTK.Input.Key.Up))
            {
                view.SetPosition(view.PositionGoto + new Vector2(0, -5), TweenType.QuarticOut, 15);
            }
            if (Input.KeyDown(OpenTK.Input.Key.Down))
            {
                view.SetPosition(view.PositionGoto + new Vector2(0, 5), TweenType.QuarticOut, 15);
            }

            view.Update();
            Input.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Black);

            Sprite.Begin(this.Width,this.Height);
            view.ApplyTransform();

            for (int x = 0; x < level.Width; x++)
            {
                for (int y = 0; y < level.Height; y++)
                {
                    RectangleF source = new RectangleF(0,0,0,0);

                    switch (level[x, y].Type)
                    {
                        case BlockType.Solid:
                            source = new RectangleF(0,0,64,64);
                            break;
                    }
                    Sprite.Draw(tileset, new Vector2(x * GRIDSIZE, y * GRIDSIZE), new Vector2((float)GRIDSIZE/TILESIZE), Color.White, Vector2.Zero, source);
                }
            }

            this.SwapBuffers();
        }
    }
}
