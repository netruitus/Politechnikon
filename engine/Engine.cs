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

namespace Politechnikon.engine
{
    public class Engine : GameWindow
    {
        Texture2D texture;
        View view;


        private static Bitmap Icon;
        public Engine(int width, int height) : base(width,height,GraphicsMode.Default, "Politechnikon", GameWindowFlags.Default, DisplayDevice.Default, 2, 0, GraphicsContextFlags.ForwardCompatible)
        {
            initDisplay();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);

            view = new View(Vector2.Zero, 1.0, 0.0);

            Mouse.ButtonDown += Mouse_ButtonDown;
        }

        void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            Vector2 pos = new Vector2(e.Position.X, e.Position.Y);
            pos -= new Vector2(this.Width, this.Height) /2f;
            pos = view.ToWorld(pos);
            view.position = pos;
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
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            view.position.Y -= 0.01f;
            view.position.X -= 0.01f;

            view.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            Sprite.Begin(this.Width,this.Height);
            view.ApplyTransform();

            Sprite.Draw(texture,Vector2.Zero, new Vector2(2f,2f),Color.Green, new Vector2(0,0));
            Sprite.Draw(texture, Vector2.Zero, new Vector2(2f, 2f), Color.Red, new Vector2(100, 50));
            Sprite.Draw(texture, Vector2.Zero, new Vector2(2f, 2f), Color.Yellow, new Vector2(100, 100));
            Sprite.Draw(texture, Vector2.Zero, new Vector2(2f, 2f), Color.Blue, new Vector2(10, 100));

            this.SwapBuffers();
        }
    }
}
