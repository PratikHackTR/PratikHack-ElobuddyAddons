using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;
using AkaCore.AkaLib.Evade;
using EloBuddy;

namespace Aka_s_Yasuo.Features.Module
{
    class FlashEQ : IModule
    {
        //My name is Aka... data Kappa
        public void OnLoad()
        {

        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.MenuManager.FlashEQ && Manager.SpellManager.Q.IsReady() && Variables.haveQ3 && AkaCore.AkaLib.Item.Flash != null && AkaCore.AkaLib.Item.Flash.IsReady();
        }

        public void OnExecute()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            Variables.AkaData();

            if (!Manager.SpellManager.E.IsReady())
            {
                return;
            }
            var obj =
                Variables.ListEnemies(true)
                    .Where(i => i.IsValidTarget(Manager.SpellManager.E.Range) && !Variables.HaveE(i))
                    .MaxOrDefault(
                        i =>
                        EntityManager.Heroes.Enemies.Count(
                            a =>
                            EloBuddy.SDK.Extensions.IsValidTarget(a) && !(a == i)
                            && (a.Distance(i) < Manager.SpellManager.Q3.Width + AkaCore.AkaLib.Item.Flash.Range - 50
                                || a.Distance(Variables.GetPosAfterDash(i)) < Manager.SpellManager.Q3.Width + AkaCore.AkaLib.Item.Flash.Range - 50)));
            if (obj != null)
            {
                Manager.SpellManager.E.Cast(obj);
            }
        }
    }
}