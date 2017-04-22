using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Politechnikon.engine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace Politechnikon.game_logic
{
    public enum GameState
    {
        InMenu,
        Loading,
        Loaded,
        Running,
        InEnd,
        InSave,
        InLoad,
        InHighScore,
        InCreation
    }
    public class Mechanic
    {
        private List<InitializedObjectTexture> ObjectList;
        private List<InitializedObjectTexture> TemporaryObjectList;
        private GameState gameState = GameState.InMenu;
        private OpenTK.Input.MouseDevice Mouse;
        private Engine engine;

        public Mechanic(List<InitializedObjectTexture> objectList,Engine eng)
        {
            this.ObjectList = objectList;        
            this.engine = eng;
            this.Mouse = engine.Mouse;
        }

        public void InitObjects()
        {
            //inicjalizacja obiektów - ładowanie poszczególnych tekstur wyświetlanych do listy obiektów do wyrenderowania przez silnik
            InitializedObjectTexture Menu = new InitializedObjectTexture(500, 0, 300, 500, "GUI\\ekran_startowy.png", Color.White);
            ObjectList.Add(Menu);
        }

        public void GetInput()
        {
            //zbieranie inputów myszy
            if (Input.MousePress(OpenTK.Input.MouseButton.Left))
            {
                //stan inputów w menu
                if (gameState == GameState.InMenu)
                {
                    if (Mouse.X > 700 && Mouse.X < 800)
                    {
                        Console.WriteLine("Koniec\n");
                        engine.Exit();     
                    }
                }
                
                 
            }
        }


        public void update()
        {

        }


    }
}
