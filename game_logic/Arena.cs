using System;
using System.Security.Cryptography;

namespace Politechnikon.game_logic
{
    class Arena
    {
        private static int playerHP;
        private static int monsterHP;
        private static int playerAP;
        private static int monsterAP;
        private static int playerDP;
        private static int monsterDP;
        private static RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();

        public static int PlayerHP
        {
            get
            {
                return playerHP;
            }

            set
            {
                playerHP = value;
            }
        }

        public static int MonsterHP
        {
            get
            {
                return monsterHP;
            }

            set
            {
                monsterHP = value;
            }
        }

        public static int PlayerAP
        {
            get
            {
                return playerAP;
            }

            set
            {
                playerAP = value;
            }
        }

        public static int MonsterAP
        {
            get
            {
                return monsterAP;
            }

            set
            {
                monsterAP = value;
            }
        }

        public static int PlayerDP
        {
            get
            {
                return playerDP;
            }

            set
            {
                playerDP = value;
            }
        }

        public static int MonsterDP
        {
            get
            {
                return monsterDP;
            }

            set
            {
                monsterDP = value;
            }
        }

        private static ushort Random(ushort min, ushort max)
        {
            byte[] random = new byte[4];
            ushort result = 0;
            while (result < min + 1 || result > max + 1)
            {
                RNG.GetBytes(random);
                result = BitConverter.ToUInt16(random, 0);
            }
            return --result;
        }

        private static float fRandom()
        {
            return Convert.ToSingle(Random(0, 100)) / 100;
        }

        public static void fight()
        {
            int Damage;

            float DamageMultiplier;
            float DefenseMultiplier;

            DamageMultiplier = (float)(fRandom() + 0.5);
            DefenseMultiplier = (float)(fRandom() + 0.2);

            Damage = Convert.ToInt16(((PlayerAP * DamageMultiplier) * (1 + DefenseMultiplier)) / (1 + ((MonsterDP * (DefenseMultiplier))) / 2));
            if (Damage > MonsterHP) Damage = MonsterHP;

            if (!ChanceRandom(10.0))
            {
                MonsterHP -= Damage;
                Console.WriteLine("Gracz zadaje obrażenie. Potwór traci " + Damage + " HP. Zostało " + MonsterHP + " życia."); //do zamiany na log
            }
            else Console.WriteLine("Gracz nie zadaje obrażeń w wyniku uniku potwora.");//do zamiany na log

            if (MonsterHP == 0) return;

            DamageMultiplier = (float)(fRandom() + 0.5);
            DefenseMultiplier = (float)(fRandom() + 0.2);

            Damage = Convert.ToInt16(((MonsterAP * DamageMultiplier) * (1 + DefenseMultiplier)) / (1 + ((PlayerDP * (DefenseMultiplier))) / 2));
            if (Damage > PlayerHP) Damage = PlayerHP;

            if (!ChanceRandom(15.0))
            {
                PlayerHP -= Damage;
                Console.WriteLine("Potwór zadaje obrażenie. Gracz traci " + Damage + " HP. Zostało " + PlayerHP + " życia.");//do zamiany na log
            }
            else Console.WriteLine("Potwór nie zadaje obrażeń w wyniku uniku gracza.");//do zamiany na log
        }

        public static bool ChanceRandom(double Chance)
        {
            if (Chance >= 100) return true;

            int Randomizer;
            int RealChance = (int)(Chance * 10);

            Randomizer = Random(0, 1000);
            if (Chance == 0 || Randomizer == 0) return false;

            if (Randomizer <= RealChance) return true;
            else return false;
        }
    }
}
