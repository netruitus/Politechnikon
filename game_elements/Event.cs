using Politechnikon.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.game_elements
{
    public class Event : ObjectAbstrakt
    {
        public Event(int x, int y, int id)
        {
            this.X = x;
            this.Y = y;
            this.Id = id;

            LoadEventVariables(this.Id);
        }

        private void LoadEventVariables(int id)
        {
            XMLParser parser = new XMLParser("Events.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseDescription(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);
        }
    }
}
