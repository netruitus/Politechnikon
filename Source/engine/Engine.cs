﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using Politechnikon.game_elements;
using Politechnikon.game_logic;

namespace Politechnikon.engine
{

    public class Engine : GameWindow
    {
        private Mechanic GameMechanic;
        private List<InitializedObjectTexture> ObjectTextureList;
        private List<InitializedObjectTexture> TextTextureList;
        private View view;
        private float sc = 1;
        private static Bitmap Icon;

        public Engine(int width, int height) : base(width,height,GraphicsMode.Default, "Politechnikon", GameWindowFlags.Default, DisplayDevice.Default, 2, 0, GraphicsContextFlags.ForwardCompatible)
        {
            initDisplay();
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
            view = new View(Vector2.Zero, 1.0, 0.0);
            Input.Initialize(this);
            ObjectTextureList = new List<InitializedObjectTexture>();
            TextTextureList = new List<InitializedObjectTexture>();
            GameMechanic = new Mechanic(ObjectTextureList, TextTextureList, this);
        }

  
        private void initDisplay()
        {
            //inicjalizacja ikonki
            Icon = (Bitmap)Image.FromFile(@"Resources\\graphics\\misc\\Icon.png");
            Icon.SetResolution(64, 64);
            base.Icon = System.Drawing.Icon.FromHandle(Icon.GetHicon());
            //ustawienie vsynca na adaptywny (średnio w granicach limit do 60 fps)
            base.VSync = VSyncMode.Adaptive;
            //Zablokowanie okna gry
            WindowBorder = WindowBorder.Fixed;    
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            //ładowanie obiektów oraz ustawienie pierwotnych ustawień
            base.OnLoad(e);
            view.SetPosition(new Vector2(0 + this.Width / 2, 0 + this.Height / 2));

            GameMechanic.InitObjects();

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //aktualizacja widoku, inputów oraz obiektów, jeśli takowe są zainicjowane
            base.OnUpdateFrame(e);

            GameMechanic.GetInput();
            GameMechanic.update();

            view.Update();
            Input.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //renderowanie tekstur układanych na stosie listy
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Black);
            Sprite.Begin((int)(this.Width / sc), (int)(this.Height / sc));
            view.ApplyTransform();

            for (int i = 0; i < ObjectTextureList.Count; i++)
            {
                RectangleF TextureRect = new RectangleF(0, 0, ObjectTextureList[i].Width, ObjectTextureList[i].Height);
                Sprite.Draw(ObjectTextureList[i].Texture, ObjectTextureList[i].Position, new Vector2(1,1), ObjectTextureList[i].Color, Vector2.Zero, TextureRect);
            }
            for (int i = 0; i < TextTextureList.Count; i++)
            {
                RectangleF TextureRect = new RectangleF(0, 0, TextTextureList[i].Width, TextTextureList[i].Height);
                Sprite.Draw(TextTextureList[i].Texture, TextTextureList[i].Position, new Vector2(1, 1), TextTextureList[i].Color, Vector2.Zero, TextureRect);
            }
            
            this.SwapBuffers();
        }
    }
}
