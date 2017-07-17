using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;

namespace Aka_s_Yasuo.Features.Module
{
    class StackQ : IModule
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
            return Manager.MenuManager.AutoStackQ && (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) && Manager.SpellManager.Q.IsReady() && !Variables.haveQ3 && !Variables.IsDashing;
        }

        public void OnExecute()
        {
            Variables.CastQ();

            var minions = Variables.ListMinions().Where(Manager.SpellManager.Q.IsInRange).OrderByDescending(i => i.MaxHealth).ToList();
            if (minions.Count == 0)
            {
                return;
            }
            var minion = minions.FirstOrDefault(i => i.Health <= Manager.DamageManager.GetQDmg(i)) ?? minions.FirstOrDefault();
            if (minion == null)
            {
                return;
            }
            Manager.SpellManager.Q.Cast(minion);
        }
    }
}