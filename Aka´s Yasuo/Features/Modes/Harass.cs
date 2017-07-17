using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace Aka_s_Yasuo.Features.Modes
{
    class Harass
    {
        public static void Execute()
        {
            if (!Manager.SpellManager.Q.IsReady() || Variables.IsDashing)
            {
                return;
            }
            if (!Variables.haveQ3)
            {
                Manager.SpellManager.Q.Cast();

                if (Manager.MenuManager.UseQHLH && (TargetSelector.GetTarget(Manager.SpellManager.Q.Range, DamageType.Physical) == null)
                    && !Variables._Player.Spellbook.IsAutoAttacking)
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                            i => (i.IsMinion()) && Manager.SpellManager.Q.IsInRange(i) && Manager.DamageManager.GetQDmg(i) > i.Health);
                    if (minion != null)
                    {
                        Manager.SpellManager.Q.Cast((Obj_AI_Base)minion);
                    }
                }
            }
            else if (Manager.MenuManager.UseQ3H)
            {
                Variables.CastQ3();
            }
        }
    }
}
