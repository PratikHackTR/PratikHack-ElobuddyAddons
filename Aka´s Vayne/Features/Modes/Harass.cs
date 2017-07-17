using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using Aka_s_Vayne.Logic;

namespace Aka_s_Vayne.Features.Modes
{
    class Harass
    {
        public static void HarassCombo()
        {
            foreach (AIHeroClient qTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(550)))
            {
                if (qTarget.GetBuffCount("vaynesilvereddebuff") == 1)
                {
                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                }

                if (qTarget.GetBuffCount("vaynesilvereddebuff") == 2)
                {
                    Manager.SpellManager.E.Cast(qTarget);
                }
            }
        }
    }
}
