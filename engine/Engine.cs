using OpenTK;
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
    //struktura, która opisuje element listy tektur gotowych do wyrenderowania
    public struct InitializedObjectTexture
    {
        private Texture2D texture;
        private Vector2 position;
        private int width;
        private int height;
        private Color color;
        private string texturePath;

        public InitializedObjectTexture(int x, int y, int width, int height, string texturePath, Color color)
        {
            this.position.X = x;
            this.position.Y = y;
            this.width = width;
            this.height = height;
            this.texturePath = texturePath;
            this.color = color;
            this.texture = ContentPipe.LoadTexture(this.texturePath);
        }

        public string TexturePath
        {
            get
            {
                return texturePath;
            }
            set
            {
                texturePath = value;
                texture = ContentPipe.LoadTexture(texturePath);
            }
        }
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
        }
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        public int X
        {
            get
            {
                return (int)position.X;
            }
            set
            {
                position.X = (float)value;
            }
        }
        public int Y
        {
            get
            {
                return (int)position.Y;
            }
            set
            {
                position.Y = (float)value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }
    }
    public class Engine : GameWindow
    {
        private Mechanic GameMechanic;
        private List<InitializedObjectTexture> ObjectTextureList;
        private Texture2D tileset;
        private Level level;
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
            GameMechanic = new Mechanic(ObjectTextureList,this);
        }

  
        private void initDisplay()
        {
            //inicjalizacja ikonki
            Icon = (Bitmap)Image.FromFile(@"resources\\graphics\\misc\\Icon.png");
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
            
            this.SwapBuffers();
        }
    }
}
