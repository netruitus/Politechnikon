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
        private static Bitmap Icon;
        public Engine(int width, int height) : base(width,height,GraphicsMode.Default, "Politechnikon", GameWindowFlags.Default, DisplayDevice.Default, 2, 0, GraphicsContextFlags.ForwardCompatible)
        {
            initDisplay();



            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
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
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            GL.BindTexture(TextureTarget.Texture2D, texture.ID);
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(Color.Red);
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);

            GL.Color3(Color.Blue);
            GL.TexCoord2(1, 0);
            GL.Vertex2(0.9f, 0);

            GL.Color3(Color.Orange);
            GL.TexCoord2(1, 1);
            GL.Vertex2(1,-0.9f);

            GL.Color3(Color.Green);
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, -1);

            GL.End();

            this.SwapBuffers();
        }
    }
}
