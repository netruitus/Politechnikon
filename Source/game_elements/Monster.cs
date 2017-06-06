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
        private int experience;
        private bool isBoss;


        public Monster(int x, int y, int id, bool Boss)
        {
            this.X = x;
            this.Y = y;
            this.Id = id;
            this.isBoss = Boss;
            LoadMonsterVariables(this.Id);
            
        }
        private void LoadMonsterVariables(int id)
        {
            XMLParser parser;
            if (isBoss == false)
            {
                parser = new XMLParser("Monsters.xml");
            }
            else
            {
                parser = new XMLParser("Bosses.xml");
            }
            if (parser != null)
            {
                ParseName(parser);
                ParsePath(parser);
                ParseDescription(parser);
                ParseSizeX(parser);
                ParseSizeY(parser);

                this.defense = Int32.Parse(parser.getElementByAttribute("id", "" + id, "defense"));
                this.attack = Int32.Parse(parser.getElementByAttribute("id", "" + id, "attack"));
                this.maxHP = Int32.Parse(parser.getElementByAttribute("id", "" + id, "life"));
                this.experience = Int32.Parse(parser.getElementByAttribute("id", "" + id, "experience"));
                this.currentHP = this.maxHP;
            }

        }

        public bool IsBoss
        {
            get { return isBoss; }
            set { this.isBoss = value; }
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

        public int Experience
        {
            get { return experience; }
            set { this.experience = value; }
        }

    }
}
