using Politechnikon.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.game_elements
{
    public class Shop : ObjectAbstrakt
    {
        public Shop(int x, int y, int id)
        {
            this.X = x;
            this.Y = y;
            this.Id = id;

            LoadShopVariables(this.Id);
        }

        private void LoadShopVariables(int id)
        {
            XMLParser parser = new XMLParser("Shops.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseDescription(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);
        }
    }
}
