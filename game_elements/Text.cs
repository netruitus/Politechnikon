using OpenTK.Graphics.OpenGL;
using Politechnikon.engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.game_elements
{
    public class Text : ObjectAbstrakt
    {
        private String txt;
        private int fontSize;
        private Texture2D generatedBMP;
        private Font Font;

        public String text
        {
            get { return txt; }
            set 
            {
                this.txt = value;
                this.generatedBMP = GenerateTextTexture(this.txt, this.Font);
            }
        }

        public int FontSize
        {
            get { return fontSize; }
            set 
            { 
                this.fontSize = value;
                this.generatedBMP = GenerateTextTexture(this.txt, this.Font);
            }
        }

        public Texture2D GeneratedBMP
        {
            get { return generatedBMP; }
            set { this.generatedBMP = value; }
        }

        //konstruktor
        public Text(int x, int y, int fontsize, String t)
        {
            InitObject();
            this.generatedBMP = new Texture2D();
            this.fontSize = fontsize;
            this.X = x;
            this.Y = y;
            this.txt = t;
            this.Path = "Resources\\fonts\\gabriola.ttf";
            this.Font = LoadFont(this.Path, "gabriola", this.fontSize);
            this.generatedBMP = GenerateTextTexture(this.txt,this.Font);
        }

        //załadowanie czcionki
        private Font LoadFont(String path, String Fontname, int Fontsize)
        {
            PrivateFontCollection collection = new PrivateFontCollection();
            collection.AddFontFile(@"" + path);
            FontFamily fontFamily = new FontFamily(Fontname, collection);
            return new Font(fontFamily, FontSize);
        }

        //konwersja bitmapy na texturę
        private Texture2D GenerateTextTexture(String txtString, Font font)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            Bitmap bmp = (Bitmap)DrawText(txtString,font,Color.White,Color.FromArgb(0,0,0,0));
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width,
                data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            this.SizeX = bmp.Width;
            this.SizeY = bmp.Height;

            return new Texture2D(id, bmp.Width, bmp.Height);
        }

        //generowanie bitmapy tekstu
        private Image DrawText(String text, Font font, Color textColor, Color backColor)
        {
                Image img = new Bitmap(1, 1);
                Graphics drawing = Graphics.FromImage(img);
                SizeF textSize = drawing.MeasureString(text, font);
                img.Dispose();
                drawing.Dispose();
                img = new Bitmap((int)textSize.Width, (int)textSize.Height);
                drawing = Graphics.FromImage(img);
                drawing.Clear(backColor);
                Brush textBrush = new SolidBrush(textColor);
                drawing.DrawString(text, font, textBrush, 0, 0);
                drawing.Save();
                textBrush.Dispose();
                drawing.Dispose();
                return img;
        }
    }
}
