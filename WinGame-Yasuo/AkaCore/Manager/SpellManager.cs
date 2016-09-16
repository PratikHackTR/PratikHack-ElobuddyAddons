using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace AkaCore.Manager
{
    class SpellManager
    {
        public static Spell.Targeted YasuoE;
        public static Spell.Skillshot YasuoW, VayneQ;

        private static void Spells()
        {
            if (ObjectManager.Player.ChampionName == "Yasuo")
            {
                YasuoW = new Spell.Skillshot(SpellSlot.W, 400, EloBuddy.SDK.Enumerations.SkillShotType.Cone);
                YasuoE = new Spell.Targeted(SpellSlot.E, 475);
            }
            if (ObjectManager.Player.ChampionName == "Vayne")
            {
                VayneQ = new Spell.Skillshot(SpellSlot.Q, 300, EloBuddy.SDK.Enumerations.SkillShotType.Linear);
            }
        }

        public static void Load()
        {
            Spells();
        }
    }
}
