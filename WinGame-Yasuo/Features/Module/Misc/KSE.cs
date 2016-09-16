using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Aka_s_Yasuo.Features.Module.Misc
{
    class KSE : IModule
    {
        public void OnLoad()
        {

        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.MenuManager.KSE && Manager.SpellManager.E.IsReady();
        }

        public void OnExecute()
        {
            var targets = EntityManager.Heroes.Enemies.Where(i => i.Distance(Variables._Player.Position) <= Manager.SpellManager.E.Range && !Variables.HaveE(i)).ToList();
            if (targets.Count > 0)
            {
                var target = targets.FirstOrDefault(i => i.Health + i.MagicShield <= Manager.DamageManager.GetEDmg(i));
                if (target != null)
                {
                    if (Manager.SpellManager.E.Cast(target))
                    {
                        Variables.lastE = Environment.TickCount;
                        return;
                    }
                }
                else if (Manager.MenuManager.KSQ && Manager.SpellManager.Q.IsReady(50))
                {
                    target =
                        targets.Where(i => i.Distance(Variables.GetPosAfterDash(i)) < Manager.SpellManager.Q3.Width)
                            .FirstOrDefault(
                                i =>
                                i.Health - Math.Max(Manager.DamageManager.GetEDmg(i) - i.MagicShield, 0) + i.AttackShield
                                <= Manager.DamageManager.GetQDmg(i));
                    if (target != null && Manager.SpellManager.E.Cast(target))
                    {
                        Variables.lastE = Environment.TickCount;
                        return;
                    }
                }
            }

        }
    }
}