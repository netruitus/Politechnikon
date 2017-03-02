using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.engine
{
    public class MainRun
    {
        private static Engine Game;
        public static void Main(string[] args)
        {
            Game = new Engine(800, 500);
            Game.Run();
        }
    }
}
