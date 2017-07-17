using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Events;
using SharpDX;

namespace Aka_s_Vayne.Features.Module.Condemn
{
    class FlashCondemn : IModule
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
            return Manager.MenuManager.FlashE && Manager.SpellManager.E.IsReady();
        }

        public void OnExecute()
        {
            if (Variables._Player.IsDashing())
            {
                return;
            }

            foreach (AIHeroClient target in
                EntityManager.Heroes.Enemies.Where(
                t =>
                    !t.IsValidTarget(Variables._Player.BoundingRadius) &&
                    Variables._Player.Distance(Variables._Player.ServerPosition.Extend(t.ServerPosition, 425)) > Variables._Player.Distance(t) + t.BoundingRadius))
            {
                for (int i = 1; i < 10; i++)
                {
                    if ((target.ServerPosition - Vector3.Normalize(target.ServerPosition - Variables._Player.ServerPosition) * (float)(i * 42.5)).IsWall() &&
                        (Manager.SpellManager.E2.GetPrediction(target).UnitPosition - Vector3.Normalize(target.ServerPosition - Variables._Player.ServerPosition) * (float)(i * 42.5)).IsWall() &&
                        (target.ServerPosition - Vector3.Normalize(target.ServerPosition - Variables._Player.ServerPosition) * i * 44).IsWall() &&
                        (Manager.SpellManager.E2.GetPrediction(target).UnitPosition - Vector3.Normalize(target.ServerPosition - Variables._Player.ServerPosition) * i * 44).IsWall())
                    {
                        Manager.SpellManager.E.Cast(target);
                        Variables._Player.Spellbook.CastSpell(Variables.FlashSlot, (Vector3)Variables._Player.ServerPosition.Extend(target.ServerPosition, 425));
                    }
                }
            }
        }
    }
}
