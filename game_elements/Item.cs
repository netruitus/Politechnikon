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

    public class Item : ObjectAbstrakt
    {
        private ItemType Type;
        private int itemAttributeValue;
        public Item(int x, int y, int id, ItemType type)
        {
            this.X = x;
            this.Y = y;
            this.Type = type;
            this.Id = id;
            this.itemAttributeValue = -1;

            if (this.Type == ItemType.Armor)
            {
                LoadArmorVariables(this.Id);
            }
            else if (this.Type == ItemType.Weapon)
            {
                LoadWeaponVariables(this.Id);
            }
            else if (this.Type == ItemType.Others)
            {
                LoadOtherVariables(this.Id);
            }

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
        }


    }
}
