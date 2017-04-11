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
    public enum TweenType
    {
        Instant,
        Linear,
        QuadraticInOut,
        CubicInOut,
        QuarticOut
    }
    public class View
    {
        private Vector2 position; //pozycja tekstury
        public double rotation; //kąt obrotu tekstury w radianach
        public double zoom;//powiększenie

        private Vector2 positionGoto, positionFrom;
        private TweenType tweenType;
        private int currentStep, tweenSteps;

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
        }

        public Vector2 ToWorld(Vector2 input)
        {
            input /= (float)zoom;
            Vector2 dX = new Vector2((float)Math.Cos(rotation),(float)Math.Sin(rotation));
            Vector2 dY = new Vector2((float)Math.Cos(rotation + MathHelper.PiOver2), (float)Math.Sin(rotation + MathHelper.PiOver2));
            return (this.position + dX * input.X + dY * input.Y);
        }


        public View(Vector2 StartPosition, double startZoom = 1.0, double startRotation = 0.0){
            this.position = StartPosition;
            this.zoom = startZoom;
            this.rotation = startRotation;
        }

        public void Update()
        {
            if (currentStep < tweenSteps)
            {
                switch (tweenType)
                {
                    case TweenType.Linear:
                        position = positionFrom + (positionGoto - positionFrom) * GetLinear((float)currentStep/tweenSteps);
                        break;
                    case TweenType.QuadraticInOut:
                        position = positionFrom + (positionGoto - positionFrom) * GetQuadraticInOut((float)currentStep / tweenSteps);
                        break;
                    case TweenType.CubicInOut:
                        position = positionFrom + (positionGoto - positionFrom) * GetCubicInOut((float)currentStep / tweenSteps);
                        break;
                    case TweenType.QuarticOut:
                        position = positionFrom + (positionGoto - positionFrom) * GetQuarticOut((float)currentStep / tweenSteps);
                        break;
                }

                currentStep++;
            }
            else
            {
                position = positionGoto;
            }
        }

        public void SetPosition(Vector2 newPosition)
        {
            this.position = newPosition;
            this.positionFrom = newPosition;
            this.positionGoto = newPosition;
            tweenType = TweenType.Instant;
            currentStep = 0;
            tweenSteps = 0;
        }

        public void SetPosition(Vector2 newPosition, TweenType type, int numSteps)
        {
            this.positionFrom = position;
            this.position = newPosition; 
            this.positionGoto = newPosition;
            tweenType = type;
            currentStep = 0;
            tweenSteps = numSteps;
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
