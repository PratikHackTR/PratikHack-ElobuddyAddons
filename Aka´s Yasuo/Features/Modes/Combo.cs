using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy;

namespace Aka_s_Yasuo.Features.Modes
{
    class Combo
    {
        public static void Execute()
        {
            if (Manager.MenuManager.UseRC && Manager.SpellManager.R.IsReady())
            {
                var target = Variables.GetRTarget();
                if (target != null && Manager.SpellManager.R.Cast(target))
                {
                    return;
                }
            }
            if (Manager.MenuManager.SmartW && Manager.SpellManager.W.IsReady())
            {
                var target = TargetSelector.GetTarget(Manager.SpellManager.E.Range, DamageType.Physical);
                if (target != null && Math.Abs(target.AttackSpeedMod - float.MaxValue) > float.Epsilon
                    && (target.HealthPercent > Variables._Player.HealthPercent
                            ? Variables._Player.CountAlliesInRange(500) < target.CountEnemiesInRange(700)
                            : Variables._Player.HealthPercent <= 30))
                {
                    var posPred = Manager.SpellManager.W.GetPrediction(target).CastPosition;
                    if (Variables._Player.Position.Distance(posPred) > 100 && Variables._Player.Position.Distance(posPred) < 330 && Manager.SpellManager.W.Cast(posPred))
                    {
                        return;
                    }
                }
            }
            if (Manager.MenuManager.AkaData && Manager.SpellManager.Q.IsReady() && Variables.haveQ3 && AkaCore.AkaLib.Item.Flash != null && AkaCore.AkaLib.Item.Flash.IsReady())
            {
                var target = TargetSelector.GetTarget(AkaCore.AkaLib.Item.Flash.Range + Manager.SpellManager.Q3.Width, DamageType.Physical);

                if (Variables._Player.Health <= Manager.MenuManager.AkaDatamy && target.Health <= Manager.MenuManager.AkaDataEnemy && target.Distance(Variables._Player.Position) > 400)
                {
                    Variables.AkaData();
                }
            }
            if (Manager.MenuManager.SmartE && Manager.SpellManager.E.IsReady() && Variables.wallLeft != null && Variables.wallRight != null)
            {
                var target = TargetSelector.GetTarget(Manager.SpellManager.E.Range, DamageType.Magical);
                if (target != null && Math.Abs(target.AttackSpeedMod - float.MaxValue) > float.Epsilon
                    && !Variables.HaveE(target) && AkaCore.Manager.EvadeManager.EvadeSkillshot.IsSafePoint(Variables.GetPosAfterDash(target).To2D()).IsSafe)
                {
                    var listPos =
                        Variables.ListEnemies()
                            .Where(i => i.IsValidTarget(Manager.SpellManager.E.Range * 2) && !Variables.HaveE(i))
                            .Select(Variables.GetPosAfterDash)
                            .Where(
                                i =>
                                target.Distance(i) < target.Distance(Variables._Player)
                                || target.Distance(i) < target.GetAutoAttackRange() + 100)
                            .ToList();
                    if (listPos.Any(i => Variables.IsThroughWall(target.ServerPosition, i)) && Manager.SpellManager.E.Cast(target))
                    {
                        return;
                    }
                }
            }
            var targetE = Manager.MenuManager.UseEC && Manager.SpellManager.E.Level > 0 ? Variables.GetBestDashObj(Manager.MenuManager.UseECTower) : null;
            if (targetE != null && Manager.SpellManager.E.Cast(targetE))
            {
                return;
            }
            if (Manager.SpellManager.Q.IsReady())
            {
                if (Variables._Player.IsDashing())
                {
                    var target = Variables.GetRTarget(true);
                    if (target != null && Manager.SpellManager.Q3.Cast(target.ServerPosition))
                    {
                        Core.DelayAction(() => Manager.SpellManager.R.Cast(target), 5);
                    }
                }
                if (Variables.IsDashing)
                {
                    if (Variables.CanCastQCir)
                    {
                        if (Variables.CastQCir(Variables.GetQCirTarget))
                        {
                            return;
                        }
                        if (!Variables.haveQ3 && Manager.MenuManager.UseEC && Manager.MenuManager.UseECStack
                            && Variables._Player.CountEnemiesInRange(700) == 0 && Variables.CastQCir(Variables.GetQCirObj))
                        {
                            return;
                        }
                    }
                }
                else if (targetE == null && (!Variables.haveQ3 ? Variables.CastQ() : Variables.CastQ3()))
                {
                    return;
                }
            }
        }
    }
}