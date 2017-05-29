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
        private int gold;
        private int score;
        private int maxHp;
        private int currentHp;
        private int experience;
        private Boolean hasAKey;

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

        public int Gold
        {
            get { return gold; }
            set { this.gold = value; }
        }

        public int MaxHp
        {
            get { return maxHp; }
            set { this.maxHp = value; }
        }

        public int Score
        {
            get { return score; }
            set { this.score = value; }
        }

        public int CurrentHp
        {
            get { return currentHp; }
            set { this.currentHp = value; }
        }

        public int Experience
        {
            get { return experience; }
            set { this.experience = value; }
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
            attack = 2;
            defense = 1;
            level = 1;
            gold = 0;
            score = 0;
            maxHp = 100;
            currentHp = 100;
            experience = 0;
            hasAKey = false;
        }



    }
}
