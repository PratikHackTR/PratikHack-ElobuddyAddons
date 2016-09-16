using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;

namespace AkaCore.AkaLib
{
    class Orbwalk
    {
        private static float lastaa = 1f;

        public static void Load()
        {
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                lastaa = Game.Time * 1000;
            }
        }

        private static void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsMe && (Game.Time * 1000) - lastaa < ObjectManager.Player.AttackCastDelay * 1000 + 50f && !args.ForceStop)
            {
                lastaa = 0f;
            }
        }

        public static bool AfterAttack
        {
            get
            {
                if (Game.Time * 1000 < lastaa + ObjectManager.Player.AttackDelay * 1000 - ObjectManager.Player.AttackDelay * 1000 / 2.35 && Game.Time * 1000 > lastaa + ObjectManager.Player.AttackCastDelay * 1000)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
