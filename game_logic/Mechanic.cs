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
        private List<InitializedObjectTexture> ObjectsToRender;
        private List<InitializedObjectTexture> ObjectsLoaded;
        private GameState gameState = GameState.InMenu;
        private OpenTK.Input.MouseDevice Mouse;
        private Engine engine;

        public Mechanic(List<InitializedObjectTexture> objectList,Engine eng)
        {
            ObjectsLoaded = new List<InitializedObjectTexture>();
            this.ObjectsToRender = objectList;        
            this.engine = eng;
            this.Mouse = engine.Mouse;          
        }

        public void InitObjects()
        {
            //inicjalizacja obiektów i zmiennych

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
                        //engine.Exit();     
                    }
                }
                
                 
            }
        }
        private void LoadToRenderObjects()
        {
            for (int i = 0; i < ObjectsToRender.Count; i++)
            {
                ObjectsToRender.Remove(ObjectsToRender[i]);
            }         
            for (int i = 0; i < ObjectsLoaded.Count; i++)
            {
                ObjectsToRender.Add(ObjectsLoaded[i]);
            }
        }

        public void update()
        {
            if (gameState == GameState.InMenu)
            {
                InitializedObjectTexture Menu = new InitializedObjectTexture(500, 0, 300, 500, "GUI\\ekran_startowy.png", Color.White);
                ObjectsLoaded.Add(Menu);
                LoadToRenderObjects();
                gameState = GameState.Loaded;
            }
            else if (gameState == GameState.InSave)
            {

            }
        }


    }
}
