using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Politechnikon.engine;

namespace Politechnikon.game_logic
{
    public class Mechanic
    {
        private List<InitializedObjectTexture> ObjectList;
        private bool IsGameLoaded = false, IsGameStarted = false;
       
        public Mechanic(List<InitializedObjectTexture> objectList)
        {
            this.ObjectList = objectList;
        }

        public void InitObjects()
        {
            //inicjalizacja obiektów - ładowanie poszczególnych tekstur wyświetlanych do listy obiektów do wyrenderowania przez silnik
            InitializedObjectTexture TempObject = new InitializedObjectTexture(500, 0, 300, 500, "GUI\\ekran_startowy.png", Color.White);
            ObjectList.Add(TempObject);
        }

        public void GetInput()
        {

        }


        public void update()
        {

        }


    }
}
