using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Politechnikon.game_elements
{
    public class Avatar : ObjectAbstrakt
    {
        private int attack;
        private int defense;
        private int level;
        private int maxHp;
        private int currentHp;
        private int experience;
        private int levelPeak;
        private Boolean hasAKey;
        private Item[] potions;
        private Item[] equipment;
        private Item[] backpack;

        public Item[] Potions
        {
            get { return potions; }
            set { this.potions = value; }
        }

        public Item[] Equipment
        {
            get { return equipment; }
            set { this.equipment = value; }
        }

        public Item[] Backpack
        {
            get { return this.backpack; }
            set { this.backpack = value; }
        }

        public int Attack
        {
            get { return attack; }
            set { this.attack = value; }
        }

        public int Defense
        {
            get { return defense; }
            set { this.defense = value; }
        }

        public int Level
        {
            get { return level; }
            set { this.level = value; }
        }

        public int MaxHp
        {
            get { return maxHp; }
            set { this.maxHp = value; }
        }

        public int CurrentHp
        {
            get { return currentHp; }
            set 
            { 
                this.currentHp = value;
                if (this.currentHp > this.maxHp) this.currentHp = this.maxHp;
            }
        }

        public int Experience
        {
            get { return experience; }
            set 
            { 
                this.experience = value;
                if (this.experience >= this.levelPeak)
                {
                    this.levelPeak = this.levelPeak * 2;
                    this.attack += 1;
                    this.defense += 1;
                    this.level += 1;
                    this.maxHp += 10;
                    this.currentHp = this.maxHp;
                }
            }
        }

        public bool HasAKey
        {
            get { return hasAKey; }
            set { this.hasAKey = value; }
        }

        public Avatar(String name)
        {
            this.Name = name;
            LoadAvatarVariables();
        }

        private void LoadAvatarVariables()
        {
            this.potions = new Item[2];
            this.equipment = new Item[4];
            this.backpack = new Item[8];
            this.levelPeak = 100;
            this.attack = 1;
            this.defense = 1;
            this.level = 1;
            this.maxHp = 100;
            this.currentHp = 100;
            this.experience = 0;
            this.hasAKey = false;
            potions[0] = new Item(743, 248, 255, ItemType.Others);
            potions[1] = new Item(743, 300, 255, ItemType.Others);
            backpack[0] = new Item(505, 248, 255, ItemType.Others);
            backpack[1] = new Item(554, 248, 255, ItemType.Others);
            backpack[2] = new Item(604, 248, 255, ItemType.Others);
            backpack[3] = new Item(654, 248, 255, ItemType.Others);
            backpack[4] = new Item(505, 300, 255, ItemType.Others);
            backpack[5] = new Item(554, 300, 255, ItemType.Others);
            backpack[6] = new Item(604, 300, 255, ItemType.Others);
            backpack[7] = new Item(654, 300, 255, ItemType.Others);
            equipment[0] = new Item(694, 106, 255, ItemType.Others);
            equipment[1] = new Item(745, 106, 255, ItemType.Others);
            equipment[2] = new Item(745, 157, 255, ItemType.Others);
            equipment[3] = new Item(745, 55, 255, ItemType.Others);
        }



    }
}
