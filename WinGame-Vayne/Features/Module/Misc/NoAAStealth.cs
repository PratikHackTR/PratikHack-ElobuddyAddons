using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Aka_s_Vayne.Features.Module.Misc
{
    class NoAAStealth : IModule
    {
        public void OnLoad()
        {
            Player.OnIssueOrder += IssueOrder;
        }

        void IssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && ShouldGetExecuted()
                && (args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AttackTo)
                &&
                (Variables._Player.CountEnemiesInRange(1000f) >
                 Manager.MenuManager.RNoAASlider)
                && Variables.UltActive() || Variables._Player.HasBuffOfType(BuffType.Invisibility)
                && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
            }
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.Other;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.MenuManager.RNoAA;
        }

        public void OnExecute() { }
    }
}
