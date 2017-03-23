using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Politechnikon.engine
{
    public class View
    {
        public Vector2 position; //pozycja tekstury
        public double rotation; //kąt obrotu tekstury w radianach
        public double zoom;//powiększenie

        public View(Vector2 StartPosition, double startZoom = 1.0, double startRotation = 0.0){
            this.position = StartPosition;
            this.zoom = startZoom;
            this.rotation = startRotation;
        }

        public void Update(){

        }

        public void ApplyTransform(){
            Matrix4 transform = Matrix4.Identity;
            transform = Matrix4.Mult(transform,Matrix4.CreateTranslation(-position.X,-position.Y,0));//position x i y dla innej perspektywy jako temp
            transform = Matrix4.Mult(transform,Matrix4.CreateRotationZ(-(float)rotation));
            transform = Matrix4.Mult(transform,Matrix4.CreateScale((float)zoom, (float)zoom, 1.0f));

            GL.MultMatrix(ref transform);
        }

    }
}
