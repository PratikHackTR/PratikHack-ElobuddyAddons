using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Aka_s_Yasuo.Features.Module.Misc
{
    class KSR : IModule
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
            return Manager.MenuManager.KSR && Manager.SpellManager.R.IsReady();
        }

        public void OnExecute()
        {
            var target = Variables.GetRTarget();
            if (target != null)
            {
                Manager.SpellManager.R.Cast();
            }
        }
    }
}
