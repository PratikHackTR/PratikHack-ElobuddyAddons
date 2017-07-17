using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;

namespace Aka_s_Yasuo.Features.Module
{
    class AutoQ : IModule
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
            return (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) && (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) && Manager.MenuManager.UseQHAuto && Manager.SpellManager.Q.IsReady() && !Variables.IsDashing;
        }

        public void OnExecute()
        {
            if (!Variables.haveQ3)
            {
                Variables.CastQ();
            }
            else if (Variables.haveQ3 && Manager.MenuManager.UseQ3HAuto)
            {
                Variables.CastQ3();
            }
        }
    }
}