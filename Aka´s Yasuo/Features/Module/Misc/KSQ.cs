using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Aka_s_Yasuo.Features.Module.Misc
{
    class KSQ : IModule
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
            return Manager.MenuManager.KSQ && Manager.SpellManager.Q.IsReady();
        }

        public void OnExecute()
        {
            if (Variables.IsDashing)
            {
                if (Variables.CanCastQCir)
                {
                    var targets = Variables.GetQCirTarget.Where(i => i.Health + i.AttackShield <= Manager.DamageManager.GetQDmg(i)).ToList();
                    if (Variables.CastQCir(targets))
                    {
                        return;
                    }
                }
            }
            else
            {
                var target = TargetSelector.GetTarget(Variables.SpellQ.Width / 2, DamageType.Physical);
                if (target != null && target.Health + target.AttackShield <= Manager.DamageManager.GetQDmg(target))
                {
                    if (!Variables.haveQ3)
                    {
                        if (Manager.SpellManager.Q.Cast(target))
                        {
                            return;
                        }
                    }
                    else if (Manager.SpellManager.Q2.Cast(target))
                    {
                        return;
                    }
                }
            }
        }
    }
}