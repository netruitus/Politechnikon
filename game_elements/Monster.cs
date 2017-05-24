using Politechnikon.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.game_elements
{
    public class Monster : ObjectAbstrakt
    {
        private int defense;
        private int attack;
        private int currentHP;
        private int maxHP;


        public Monster(int x, int y, int id)
        {
            this.X = x;
            this.Y = y;
            this.Id = id;

            LoadMonsterVariables(this.Id);
            
        }
        private void LoadMonsterVariables(int id)
        {    
            XMLParser parser = new XMLParser("Monsters.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseDescription(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);

            this.defense = Int32.Parse(parser.getElementByAttribute("id", "" + id, "defense"));
            this.attack = Int32.Parse(parser.getElementByAttribute("id", "" + id, "attack"));
            this.maxHP = Int32.Parse(parser.getElementByAttribute("id", "" + id, "life"));
            this.currentHP = this.maxHP;
        }



        public int Defense
        {
            get { return defense; }
            set { this.defense = value; }
        }

        public int Attack
        {
            get { return attack; }
            set { this.attack = value; }
        }

        public int CurrentHP
        {
            get { return currentHP; }
            set { this.currentHP = value; }
        }

        public int MaxHP
        {
            get { return maxHP; }
            set { this.maxHP = value; }
        }

    }
}
