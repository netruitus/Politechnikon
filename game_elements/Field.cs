using Politechnikon.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.game_elements
{
    public class Field : ObjectAbstrakt
    {
        public Field(int x, int y, int id)
        {
            this.X = x;
            this.Y = y;
            this.Id = id;

            LoadFieldVariables(this.Id);    
        }

        private void LoadFieldVariables(int id)
        {
            XMLParser parser = new XMLParser("Fields.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseDescription(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);
        }


    }
}
