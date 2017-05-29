using Politechnikon.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.game_elements
{
    public enum ItemType
    {
        Armor,
        Weapon,
        Others
    }

    public enum EffectType
    {
        Nothing,
        Healing,
        Learning
    }

    public class Item : ObjectAbstrakt
    {
        private ItemType ItType;
        private EffectType efType;
        private int itemAttributeValue;
        
        public Item(int x, int y, int id, ItemType type)
        {
            this.X = x;
            this.Y = y;
            this.ItType = type;
            this.Id = id;
            this.itemAttributeValue = -1;

            if (this.ItType == ItemType.Armor)
            {
                LoadArmorVariables(this.Id);
            }
            else if (this.ItType == ItemType.Weapon)
            {
                LoadWeaponVariables(this.Id);
            }
            else if (this.ItType == ItemType.Others)
            {
                LoadOtherVariables(this.Id);
            }

        }

        public EffectType EfType
        {
            get { return efType; }
            set { this.efType = value; }
        }

        public int ItemAttributeValue
        {
            get { return itemAttributeValue; }
            set { this.itemAttributeValue = value; }
        }

        private void LoadArmorVariables(int id)
        {
            XMLParser parser = new XMLParser("Armors.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseDescription(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);
            this.itemAttributeValue = Int32.Parse(parser.getElementByAttribute("id", "" + id, "defense"));
        }

        private void LoadWeaponVariables(int id)
        {
            XMLParser parser = new XMLParser("Weapons.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseDescription(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);
            this.itemAttributeValue = Int32.Parse(parser.getElementByAttribute("id", "" + id, "attack"));

        }

        private void LoadOtherVariables(int id)
        {
            XMLParser parser = new XMLParser("Items.xml");
            ParseName(parser);
            ParsePath(parser);
            ParseDescription(parser);
            ParseSizeX(parser);
            ParseSizeY(parser);
            var temp = Int32.Parse(parser.getElementByAttribute("id", "" + id, "effectType"));
            this.itemAttributeValue = Int32.Parse(parser.getElementByAttribute("id", "" + id, "effectStrength")); 
            if (temp == 0)
            {
                EfType = EffectType.Nothing;
            }
            else if (temp == 1)
            {
                EfType = EffectType.Healing;
            }
            else if (temp == 2)
            {
                EfType = EffectType.Learning;
            }
        }


    }
}
