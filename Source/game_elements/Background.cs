using Politechnikon.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.game_elements
{
    public enum BackgroundType
    {
        Board,
        GUI
    }

    public class Background : ObjectAbstrakt
    {
        private BackgroundType Type;

        public Background(int x, int y, int id, BackgroundType type)
        {
            this.Id = id;
            this.X = x;
            this.Y = y;
            this.Type = type;

            if (this.Type == BackgroundType.Board)
            {
                LoadBoardVariables(this.Id);
            }
            else if (this.Type == BackgroundType.GUI)
            {
                LoadGUIVariables(this.Id);
            }
        }

        private void LoadBoardVariables(int id)
        {
            XMLParser parser = new XMLParser("Backgrounds.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);
        }

        private void LoadGUIVariables(int id)
        {
            XMLParser parser = new XMLParser("GUIs.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);
        }
    }
}
