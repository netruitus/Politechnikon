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
        private bool explored;
        private String textBuffor;

        public String TextBuffor
        {
            get { return textBuffor; }
            set { this.textBuffor = value;}
        }
        public bool Explored
        {
            get { return explored; }
            set 
            { 
                this.explored = value;
                XMLParser parser = new XMLParser("Fields.xml");
                if (explored == true)
                {
                    ParsePath(parser);
                    ParseDescription(parser);
                }
                else
                {
                    this.Path = "Graphics/Fields/nieodkryte.png";
                    this.Description = "Pole nieodkryte";
                }
            }
        }
        public Field(int x, int y, int id, bool expl)
        {
            this.explored = expl;
            this.X = x;
            this.Y = y;
            this.Id = id;
            this.textBuffor = " ";

            LoadFieldVariables(this.Id);    
        }

        private void LoadFieldVariables(int id)
        {
            XMLParser parser = new XMLParser("Fields.xml");
            ParseName(parser);
            if (explored == true)
            {
                ParsePath(parser);
                ParseDescription(parser);
            }
            else
            {
                this.Path = "Graphics/Fields/nieodkryte.png";
                this.Description = "Pole nieodkryte";
            }
            
            ParseSizeX(parser);
            ParseSizeY(parser);
        }



    }
}
