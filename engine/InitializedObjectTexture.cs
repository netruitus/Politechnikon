using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.engine
{
    //struktura, która opisuje element listy tektur gotowych do wyrenderowania
    public class InitializedObjectTexture
    {
        private int id;
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

        public InitializedObjectTexture(int x, int y, int width, int height, Texture2D Texture, Color color)
        {
            this.position.X = x;
            this.position.Y = y;
            this.width = width;
            this.height = height;
            this.color = color;
            this.texture = Texture;
        }

        public int Id
        {
            get { return id; }
            set { this.id = value; }
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
}
