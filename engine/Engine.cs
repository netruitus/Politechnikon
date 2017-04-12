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
using Politechnikon.game_logic;

namespace Politechnikon.engine
{
    struct InitializedObject
    {
        private Texture2D texture;
        private Vector2 position;
        private int width;
        private int height;
        private Color color;
        private string texturePath;

        public InitializedObject(int x, int y, int width, int height, string texturePath, Color color)
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
        public static int GRIDSIZE = 64, TILESIZE = 64;

        Mechanic GameMechanic;
        List<InitializedObject> ObjectList;
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
            ObjectList = new List<InitializedObject>();
            GameMechanic = new Mechanic();
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
            GameMechanic.LoadObjects();

            view.SetPosition(new Vector2(0 + this.Width / 2, 0 + this.Height / 2));
            InitializedObject TempObject = new InitializedObject(500, 0, 300, 500, "GUI\\ekran_startowy.png", Color.White);
            ObjectList.Add(TempObject);

            texture = ContentPipe.LoadTexture("graphics\\misc\\Icon.png");
            tileset = ContentPipe.LoadTexture("graphics\\misc\\Icon.png");
            level = new Level(20, 20);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            GameMechanic.GetInput();
            if (Input.MousePress(OpenTK.Input.MouseButton.Left))
            {
                Vector2 pos = new Vector2(Mouse.X, Mouse.Y) - new Vector2(this.Width,this.Height) / 2f;
                pos = view.ToWorld(pos);

                view.SetPosition(pos, TweenType.QuarticOut, 60);
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

            for (int i = 0; i < ObjectList.Count; i++)
            {
                RectangleF TextureRect = new RectangleF(0, 0, ObjectList[i].Width, ObjectList[i].Height);
                Sprite.Draw(ObjectList[i].Texture, ObjectList[i].Position, new Vector2(1,1), ObjectList[i].Color, Vector2.Zero, TextureRect);
            }



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
