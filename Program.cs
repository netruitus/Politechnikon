using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Politechnikon
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindow Window = new GameWindow(600, 600);
            Window.Run();
        }
    }
}
