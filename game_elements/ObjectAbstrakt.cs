using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Politechnikon.engine;

namespace Politechnikon.game_elements
{
    public class ObjectAbstrakt
    {
        private String name;
        private String path;
        private String description;
        private int sizeX;
        private int sizeY;
        private int x;
        private int y;
        private int id;

        public void InitObject()
        {
            //allokuje tylko adres w pamięci przypisując zmienne, które nie mają znaczenia
            this.name = "";
            this.path = "";
            this.description = "";
            this.sizeX = -1;
            this.sizeY = -1;
            this.x = -1;
            this.y = -1;
            this.id = -1;
        }

        protected void ParseName(XMLParser parser)
        {
            this.name = parser.getElementByAttribute("id", "" + id, "name");
        }

        protected void ParsePath(XMLParser parser)
        {
            this.path = parser.getElementByAttribute("id", "" + id, "path");
        }

        protected void ParseDescription(XMLParser parser)
        {
            this.description = parser.getElementByAttribute("id", "" + id, "description");
        }

        protected void ParseSizeX(XMLParser parser)
        {
            this.sizeX = Int32.Parse(parser.getElementByAttribute("id", "" + id, "sizeX"));
        }

        protected void ParseSizeY(XMLParser parser)
        { 
            this.sizeY = Int32.Parse(parser.getElementByAttribute("id", "" + id, "sizeY"));
        }

        public String Name
        {
            get { return name; }
            set { this.name = value; }
        }

        public String Path
        {
            get { return path; }
            set { this.path = value; }
        }

        public String Description
        {
            get { return description; }
            set { this.description = value; }
        }

        public int SizeX
        {
            get { return sizeX; }
            set { this.sizeX = value; }
        }

        public int SizeY
        {
            get { return sizeY; }
            set { this.sizeY = value; }
        }

        public int X
        {
            get { return x; }
            set { this.x = value; }
        }

        public int Y
        {
            get { return y; }
            set { this.y = value; }
        }

        public int Id
        {
            get { return id; }
            set { this.id = value; }
        }
    }
}
