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
        private Boolean HasAKey;


        public Avatar(String name)
        {
            this.Name = name;
            LoadAvatarVariables();
        }

        private void LoadAvatarVariables()
        {
            attack = 0;
            defense = 0;
            level = 1;
            gold = 0;
            score = 0;
            maxHp = 100;
            currentHp = 100;
            experience = 0;
            HasAKey = false;
        }


    }
}
