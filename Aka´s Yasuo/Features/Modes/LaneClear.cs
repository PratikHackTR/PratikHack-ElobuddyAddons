using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Aka_s_Yasuo.Features.Modes
{
    class LaneClear
    {
        public static void Execute()
        {
            if (Manager.SpellManager.E.IsReady() && Manager.MenuManager.UseELC)
            {
                var minions =
                    Variables.ListMinions()
                        .Where(
                            i =>
                            i.IsValid && !Variables.HaveE(i)
                            && Manager.MenuManager.UseELCTower || !Variables.GetPosAfterDash(i).IsUnderTurret()
                            && AkaCore.Manager.EvadeManager.EvadeSkillshot.IsSafePoint(Variables.GetPosAfterDash(i).To2D()).IsSafe)
                        .OrderByDescending(i => i.MaxHealth)
                        .ToList();
                if (minions.Count > 0)
                {
                    var minion = minions.FirstOrDefault(i => i.Health <= Manager.DamageManager.GetEDmg(i));
                    if (Manager.MenuManager.UseQLC && minion == null && Manager.SpellManager.Q.IsReady(50)
                        && (!Variables.haveQ3 || Manager.MenuManager.UseQ3LC))
                    {
                        var sub = new List<Obj_AI_Minion>();
                        foreach (var mob in minions)
                        {
                            if ((mob.Health < Manager.DamageManager.GetQDmg(mob) + Manager.DamageManager.GetEDmg(mob) || mob.Team == GameObjectTeam.Neutral)
                                && mob.Distance(Variables.GetPosAfterDash(mob)) < Manager.SpellManager.Q3.Width)
                            {
                                sub.Add(mob);
                            }
                            if (Manager.MenuManager.UseELC)
                            {
                                continue;
                            }
                            var nearMinion =
                                Variables.ListMinions()
                                    .Where(i => i.IsValidTarget(Manager.SpellManager.Q3.Width, true, Variables.GetPosAfterDash(mob)))
                                    .ToList();
                            if (nearMinion.Count > 2 || nearMinion.Count(i => mob.Health <= Manager.DamageManager.GetQDmg(mob)) > 1)
                            {
                                sub.Add(mob);
                            }
                        }
                        minion = sub.FirstOrDefault();
                    }
                    if (minion != null && Manager.SpellManager.E.Cast(minion))
                    {
                        return;
                    }
                }
            }
            if (Manager.MenuManager.UseQLC && Manager.SpellManager.Q.IsReady() && (!Variables.haveQ3 || Manager.MenuManager.UseQ3LC))
            {
                if (Variables.IsDashing)
                {
                    if (Variables.CanCastQCir)
                    {
                        var minions = Variables.GetQCirObj.Where(i => i is Obj_AI_Minion).ToList();
                        if (minions.Any(i => i.Health <= Manager.DamageManager.GetQDmg(i) || i.Team == GameObjectTeam.Neutral)
                            || minions.Count > 2)
                        {
                            Variables.CastQCir(minions);
                        }
                    }
                }
                else
                {
                    var minions =
                        Variables.ListMinions()
                            .Where(i => !Variables.haveQ3 ? Manager.SpellManager.Q.IsInRange(i) : i.IsValidTarget(Manager.SpellManager.Q2.Range - i.BoundingRadius / 2))
                            .OrderByDescending(i => i.MaxHealth)
                            .ToList();
                    if (minions.Count == 0)
                    {
                        return;
                    }
                    if (!Variables.haveQ3)
                    {
                        var minion = minions.FirstOrDefault(i => i.Health < Manager.DamageManager.GetQDmg(i));
                        if (minion != null)
                        {
                            Manager.SpellManager.Q.Cast(minion);
                        }
                    }
                    else
                    {
                        var minion = minions.FirstOrDefault(i => i.Health < Manager.DamageManager.GetQDmg(i));
                        if (minion != null)
                        {
                            Manager.SpellManager.Q2.CastOnBestFarmPosition(3);
                        }
                    }
                }
            }
        }
    }
}
