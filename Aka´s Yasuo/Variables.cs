using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Aka_s_Yasuo.Features.Module;
using Aka_s_Yasuo.Features.Module.Misc;

namespace Aka_s_Yasuo
{
    static class Variables
    {
        #region Player Variables

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion

        #region Spell Variables

        public const float QDelay = 0.39f, Q2Delay = 0.35f, QDelays = 0.22f, Q2Delays = 0.315f;

        public const int RWidth = 400;

        #endregion

        #region Static Variables

        public static int[] AbilitySequence;

        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;

        public static int cDash;

        public static bool haveQ3;

        public static bool isDash;

        public static int lastE;

        public static bool wallcasted;

        public static Vector3 posDash;

        public static MissileClient wallLeft, wallRight;

        public static Geometry.Polygon.Rectangle wallPoly;

        #endregion

        public static List<IModule> moduleList = new List<IModule>()
        {
            new AutoQ(),
            new KSE(),
            new KSQ(),
            new KSR(),
            new StackQ(),
            new FlashEQ(),
        };

        #region Extension Variables

        public static bool CanCastQCir => posDash.IsValid() && _Player.Position.Distance(posDash) < 150;

        public static List<Obj_AI_Base> GetQCirObj
            =>
                ListEnemies(true)
                    .Where(i => i.IsValid && Manager.SpellManager.Q3.CastIfItWillHit())
                    .ToList();

        public static List<Obj_AI_Base> GetQCirTarget
    =>
        EntityManager.Heroes.Enemies
            .Where(i => i.Distance(posDash) < Manager.SpellManager.Q3.Width + 20 && Manager.SpellManager.Q3.CastIfItWillHit())
            .Cast<Obj_AI_Base>()
            .ToList();

        public static bool IsDashing => Environment.TickCount - lastE <= 100 || _Player.IsDashing() || posDash.IsValid();

        public static Spell.Skillshot SpellQ => !haveQ3 ? Manager.SpellManager.Q : Manager.SpellManager.Q2;

        public static List<Obj_AI_Base> ListEnemies(bool includeClones = false)
        {
            var list = new List<Obj_AI_Base>();
            list.AddRange(EntityManager.Heroes.Enemies);
            list.AddRange(ListMinions(includeClones));
            return list;
        }

        public static List<Obj_AI_Minion> ListMinions(bool includeClones = false)
        {
            var list = new List<Obj_AI_Minion>();
            list.AddRange(EntityManager.MinionsAndMonsters.Monsters);
            list.AddRange(EntityManager.MinionsAndMonsters.EnemyMinions.Where(i => i.IsMinion()));
            return list;
        }

        #endregion

        #region more extensions :(

        private static bool CanCastDelayR(AIHeroClient target, bool isAirBlade = false)
        {
            if (target.HasBuffOfType(BuffType.Knockback))
            {
                return true;
            }
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockup);
            return buff != null
                   && Game.Time - buff.StartTime >= (!isAirBlade ? 0.89 : 0.85) * (buff.EndTime - buff.StartTime);
        }

        public static bool CanDash(
    Obj_AI_Base target,
    bool inQCir = false,
    bool underTower = true,
    Vector3 pos = new Vector3(),
    bool isAirBlade = false)
        {
            if (HaveE(target))
            {
                return false;
            }
            if (!pos.IsValid())
            {
                pos = Manager.SpellManager.E2.GetPrediction(target).CastPosition;
            }
            var posAfterE = GetPosAfterDash(target);
            return (underTower || !posAfterE.IsUnderTurret())
                   && (inQCir ? Manager.SpellManager.Q3.CastIfItWillHit() : posAfterE.Distance(pos) < (!isAirBlade ? pos.Distance(_Player.Position) : Manager.SpellManager.R.Range))
                   && AkaCore.Manager.EvadeManager.EvadeSkillshot.IsSafePoint(posAfterE.To2D()).IsSafe;
        }

        public static bool CastQ3()
        {
            var targets = EntityManager.Heroes.Enemies.Where(x => x.Position.Distance(_Player.Position) < Manager.SpellManager.Q2.Range);
            if (targets.Count() == 0)
            {
                return false;
            }
            var posCast = new Vector3();
            foreach (var pred in
                targets.Select(i => Manager.SpellManager.Q2.GetPrediction(i))
                    .Where(
                        i =>
                        i.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High || (i.HitChance >= EloBuddy.SDK.Enumerations.HitChance.Medium && i.CastPosition.CountEnemiesInRange(Manager.SpellManager.Q2.Width) > 1)).OrderByDescending(i => i.CastPosition.CountEnemiesInRange(Manager.SpellManager.Q2.Width)))
            {
                posCast = pred.CastPosition;
                break;
            }
            return posCast.IsValid() && Manager.SpellManager.Q2.Cast(posCast);
        }

        public static bool CastQ()
        {
            var besttarget = TargetSelector.GetTarget(Manager.SpellManager.Q.Range, DamageType.Physical);
            if (besttarget == null) return false;
            var pred = Manager.SpellManager.Q.GetPrediction(besttarget);
            var predpos = new Vector3();

            if (besttarget != null && pred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High)
            {
                predpos = pred.CastPosition;
            }
            
            return predpos.IsValid() && Manager.SpellManager.Q.Cast(pred.CastPosition);
        }

        public static bool CastQCir(List<Obj_AI_Base> obj)
        {
            if (obj.Count == 0)
            {
                return false;
            }
            var target = obj.FirstOrDefault();
            return target != null && Manager.SpellManager.Q3.Cast(SpellQ.GetPrediction(target).CastPosition);
        }

        public static void AkaData()
        {
            if (Variables._Player.IsDashing())
            {
                var bestHit = 0;
                var bestPos = new Vector3();
                for (var i = 0; i < 360; i += 10)
                {
                    var pos =
                        (Variables._Player.ServerPosition.To2D()
                         + AkaCore.AkaLib.Item.Flash.Range * new Vector2(1, 0).Rotated((float)(Math.PI * i / 180.0))).To3D();
                    var hits = EntityManager.Heroes.Enemies.Count(a => a.IsValidTarget(Manager.SpellManager.Q3.Width, false, pos));
                    if (hits > bestHit)
                    {
                        bestHit = hits;
                        bestPos = pos;
                    }
                }
                if (bestPos.IsValid())
                {
                    Player.CastSpell(SpellSlot.Q);
                    Core.DelayAction(() => Player.CastSpell(AkaCore.AkaLib.Item.Flash.Slot, bestPos), 5);
                }
            }
        }

        public static Obj_AI_Base GetBestDashObj(bool underTower)
        {
            if (Manager.MenuManager.UseRC && Manager.SpellManager.R.IsReady() && Manager.SpellManager.Q.IsReady(50))
            {
                var target = GetRTarget(true);
                if (target != null)
                {
                    return Manager.SpellManager.E.IsInRange(target) && CanDash(target, true, underTower, target.ServerPosition, true)
                               ? target
                               : (GetBestDashObjToUnit(target, true, underTower, true)
                                  ?? GetBestDashObjToUnit(target, false, underTower, true));
                }
            }
            if (Manager.MenuManager.UseEC)
            {
                var target = Manager.SpellManager.E.GetTarget();
                if (target != null && haveQ3 && Manager.SpellManager.Q.IsReady(50))
                {
                    var nearObj = GetBestDashObjToUnit(target, true, underTower);
                    if (nearObj != null
                        && (GetPosAfterDash(nearObj).CountEnemiesInRange(Manager.SpellManager.Q3.Width) > 1
                            || _Player.CountEnemiesInRange(Manager.SpellManager.Q.Range + Manager.SpellManager.E.Range / 2) == 1))
                    {
                        return nearObj;
                    }
                }
                target = Manager.SpellManager.E.GetTarget();
                if (target != null
                    && ((cDash > 0 && CanDash(target, false, underTower))
                        || (haveQ3 && Manager.SpellManager.Q.IsReady(50) && CanDash(target, true, underTower))))
                {
                    return target;
                }
                target = Manager.SpellManager.Q.GetTarget() ?? Manager.SpellManager.Q2.GetTarget();
                if (target != null && (!_Player.Spellbook.IsAutoAttacking || _Player.HealthPercent < 40))
                {
                    var nearObj = GetBestDashObjToUnit(target, false, underTower);
                    var canDash = cDash == 0 && nearObj != null && !HaveE(target);
                    if (Manager.SpellManager.Q.IsReady(50))
                    {
                        var nearObjQ3 = GetBestDashObjToUnit(target, true, underTower);
                        if (nearObjQ3 != null)
                        {
                            nearObj = nearObjQ3;
                            canDash = true;
                        }
                    }
                    if (!canDash && target.Distance(_Player.Position) > target.GetAutoAttackRange() * 0.7)
                    {
                        canDash = true;
                    }
                    if (canDash)
                    {
                        if (nearObj == null && Manager.SpellManager.E.IsInRange(target) && CanDash(target, false, underTower))
                        {
                            nearObj = target;
                        }
                        if (nearObj != null)
                        {
                            return nearObj;
                        }
                    }
                }
            }
            return null;
        }

        private static Obj_AI_Base GetBestDashObjToUnit(
    AIHeroClient target,
    bool inQCir,
    bool underTower,
    bool isAirBlade = false)
        {
            var pos = !isAirBlade ? Manager.SpellManager.E2.GetPrediction(target).CastPosition : target.ServerPosition;
            return
                ListEnemies()
                    .Where(
                        i =>
                        i.IsValidTarget(Manager.SpellManager.E.Range) && !(i == target)
                        && CanDash(i, inQCir, underTower, pos, isAirBlade))
                    .MinOrDefault(i => GetPosAfterDash(i).Distance(pos));
        }


        public static AIHeroClient GetRTarget(bool isAirBlade = false)
        {
            var result = new Tuple<AIHeroClient, List<AIHeroClient>>(null, new List<AIHeroClient>());
            var targets = EntityManager.Heroes.Enemies.Where(x => x.Distance(_Player.Position) <= Manager.SpellManager.R.Range && HaveR(x));
            foreach (var target in targets)
            {
                var nears =
                    EntityManager.Heroes.Enemies.Where(
                        i => i.IsValidTarget(RWidth, true, target.ServerPosition) && !(i == target) && HaveR(i))
                        .ToList();
                nears.Add(target);
                if (nears.Count > result.Item2.Count
                    && ((nears.Count > 1 && nears.Any(i => i.Health + i.AttackShield <= Manager.DamageManager.GetRDmg(i) + Manager.DamageManager.GetQDmg(i)))
                        || nears.Sum(i => i.HealthPercent) / nears.Count < Manager.MenuManager.UseRCHP
                        || nears.Count >= Manager.MenuManager.UseRCEnemies))
                {
                    result = new Tuple<AIHeroClient, List<AIHeroClient>>(target, nears);
                }
            }
            return Manager.MenuManager.UseRCDelay
                   && (_Player.HealthPercent >= 15
                       || EntityManager.Heroes.Enemies.Count(i => i.IsValidTarget(600) && !HaveR(i)) == 0)
                       ? (result.Item2.Any(i => CanCastDelayR(i, isAirBlade)) ? result.Item1 : null)
                       : result.Item1;
        }

        public static Obj_AI_Base GetBestObjToMouse(bool underTower = true)
        {
            var pos = Game.CursorPos;
            return
                GetDashObj(underTower)
                    .Where(i => CanDash(i, false, true, pos))
                    .MinOrDefault(i => GetPosAfterDash(i).Distance(pos));
        }

        public static List<Obj_AI_Base> GetDashObj(bool underTower = false)
        {
            return
                ListEnemies()
                    .Where(i => i.IsValidTarget(Manager.SpellManager.E.Range) && (underTower || !GetPosAfterDash(i).IsUnderTurret()))
                    .ToList();
        }

        public static Vector3 GetPosAfterDash(Obj_AI_Base target)
        {
            return _Player.ServerPosition.Extend(target.ServerPosition, Manager.SpellManager.E.Range).To3D();
        }

        public static float GetQDelay(bool isQ3)
        {
            var delayOri = !isQ3 ? QDelay : Q2Delay;
            var delayMax = !isQ3 ? QDelays : Q2Delays;
            var perReduce = 1 - delayMax / delayOri;
            var delayReal =
                Math.Max(
                    delayOri * (1 - Math.Min((_Player.AttackSpeedMod - 1) * (perReduce / 1.1f), perReduce)),
                    delayMax);
            return (float)Math.Round((decimal)delayReal, 3, MidpointRounding.AwayFromZero);
        }

        public static bool IsThroughWall(Vector3 from, Vector3 to)
        {
            if (wallLeft == null || wallRight == null)
            {
                return false;
            }
            wallPoly = new Geometry.Polygon.Rectangle(wallLeft.Position, wallRight.Position, 75);
            for (var i = 0; i < wallPoly.Points.Count; i++)
            {
                var inter = wallPoly.Points[i].Intersection(
                    wallPoly.Points[i != wallPoly.Points.Count - 1 ? i + 1 : 0],
                    from.To2D(),
                    to.To2D());
                if (inter.Intersects)
                {
                    return true;
                }
            }
            return false;
        }

        public static T MinOrDefault<T, TR>(this IEnumerable<T> container, Func<T, TR> valuingFoo)
    where TR : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var minElem = enumerator.Current;
            var minVal = valuingFoo(minElem);
            while (enumerator.MoveNext())
            {
                var currVal = valuingFoo(enumerator.Current);
                if (currVal.CompareTo(minVal) >= 0)
                {
                    continue;
                }

                minVal = currVal;
                minElem = enumerator.Current;
            }

            return minElem;
        }

        public static bool HaveE(Obj_AI_Base target)
        {
            return target.HasBuff("YasuoDashWrapper");
        }

        public static bool HaveR(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup);
        }
        #endregion
    }
}
