using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Aka_s_Vayne.Logic
{
    public static class Condemn
    {
        public static void Execute()
        {
            if (!Manager.SpellManager.E.IsReady())
            {
                return;
            }

            if (Manager.MenuManager.CondemnMode == 0 || Manager.MenuManager.CondemnMode == 1 || Manager.MenuManager.CondemnMode == 2 || Manager.MenuManager.CondemnMode == 3)
            {
                var CondemnTarget = GetCondemnTarget(Variables._Player.ServerPosition);

                if (CondemnTarget.IsValidTarget())
                {
                    Manager.SpellManager.E.Cast(CondemnTarget);
                }
            }
            if (Manager.MenuManager.CondemnMode == 4)
            {
                Aka.Execute();
            }
        }

        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender != null && sender.Owner != null && sender.Owner.IsMe && args.Slot == SpellSlot.E && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
            {
                if (!(args.Target is AIHeroClient))
                {
                    args.Process = false;
                    return;
                }

                if (GetCondemnTarget(Variables._Player.ServerPosition).IsValidTarget())
                {
                    if (!Shine.GetTarget(Variables._Player.ServerPosition).IsValidTarget())
                    {
                        args.Process = false;
                    }
                }
            }
        }

        public static Obj_AI_Base GetCondemnTarget(Vector3 fromPosition)
        {
            switch (Manager.MenuManager.CondemnMode)
            {
                case 0:
                    //VH Revolution
                    return Shine.GetTarget(fromPosition);
                case 1:
                    //VH Reborn
                    return VHReborn.GetTarget(fromPosition);
                case 2:
                    //Marksman / Gosu
                    return Marksman.GetTarget(fromPosition);
                case 3:
                    //Shine#
                    return VHRevolution.GetTarget(fromPosition);
            }
            return null;
        }
    }

    class Marksman
    {
        public static AIHeroClient GetTarget(Vector3 fromPosition)
        {
            foreach (var target in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget(Manager.SpellManager.E.Range)))
            {
                var pushDistance = Manager.MenuManager.CondemnPushDistance;
                var targetPosition = Manager.SpellManager.E2.GetPrediction(target).UnitPosition;
                var finalPosition = targetPosition.Extend(fromPosition, -pushDistance);
                var finalPosition2 = targetPosition.Extend(fromPosition, -(pushDistance / 2f));
                var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                var collFlags2 = NavMesh.GetCollisionFlags(finalPosition2);
                var j4Flag = Manager.MenuManager.J4Flag && (Variables.IsJ4Flag(finalPosition.To3D(), target) || Variables.IsJ4Flag(finalPosition.To3D(), target));
                if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building) || collFlags2.HasFlag(CollisionFlags.Wall) || collFlags2.HasFlag(CollisionFlags.Building) || j4Flag)
                {
                    if (Manager.MenuManager.OnlyStunCurrentTarget &&
                        TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical) != null &&
                        !target.NetworkId.Equals(TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical).NetworkId))
                    {
                        return null;
                    }

                    if (target.Health + 10 <=
                        Variables._Player.GetAutoAttackDamage(target) *
                        Manager.MenuManager.CondemnBlock)
                    {
                        return null;
                    }

                    return target;
                }
            }
            return null;
        }
    }

    class Shine
    {
        public static Obj_AI_Base GetTarget(Vector3 fromPosition)
        {
            foreach (var target in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget(Manager.SpellManager.E.Range)))
            {
                var pushDistance = Manager.MenuManager.CondemnPushDistance;
                var targetPosition = Manager.SpellManager.E2.GetPrediction(target).UnitPosition;
                var pushDirection = (targetPosition - Variables._Player.ServerPosition).Normalized();
                float checkDistance = pushDistance / 40f;
                for (int i = 0; i < 40; i++)
                {
                    Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                    var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                    var j4Flag = Manager.MenuManager.J4Flag && (Variables.IsJ4Flag(finalPosition, target));
                    if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building) || j4Flag) //not sure about building, I think its turrets, nexus etc
                    {
                        if (Manager.MenuManager.OnlyStunCurrentTarget && TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical) != null &&
                                        !target.NetworkId.Equals(TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical).NetworkId))
                        {
                            return null;
                        }

                        if (target.Health + 10 <=
                                        Variables._Player.GetAutoAttackDamage(target) *
                                        Manager.MenuManager.CondemnBlock)
                        {
                            return null;
                        }

                        return target;
                    }
                }
            }

            return null;
        }
    }

    class VHReborn
    {
        public static AIHeroClient GetTarget(Vector3 fromPosition)
        {
            if (Variables.UnderEnemyTower((Vector2)Variables._Player.ServerPosition))
            {
                return null;
            }

            var pushDistance = Manager.MenuManager.CondemnPushDistance;

            foreach (var target in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget(Manager.SpellManager.E.Range) && !h.HasBuffOfType(BuffType.SpellShield) && !h.HasBuffOfType(BuffType.SpellImmunity)))
            {
                var targetPosition = target.ServerPosition;
                var finalPosition = targetPosition.Extend(fromPosition, -pushDistance);
                var numberOfChecks = (float)Math.Ceiling(pushDistance / 30f);


                if (Manager.MenuManager.OnlyStunCurrentTarget && TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical) != null &&
                            !target.NetworkId.Equals(TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical).NetworkId))
                {
                    continue;
                }

                for (var i = 1; i <= 30; i++)
                {
                    var v3 = (targetPosition - fromPosition).Normalized();
                    var extendedPosition = targetPosition + v3 * (numberOfChecks * i);
                    var j4Flag = Manager.MenuManager.J4Flag && (Variables.IsJ4Flag(extendedPosition, target));
                    //var underTurret = MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.condemn.condemnturret") && (Helpers.UnderAllyTurret_Ex(finalPosition) || Helpers.IsFountain(finalPosition));
                    var collFlags = NavMesh.GetCollisionFlags(extendedPosition);
                    if ((collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building) || j4Flag) && (target.Path.Count() < 2) && !target.IsDashing())
                    {

                        if (target.Health + 10 <=
                            Variables._Player.GetAutoAttackDamage(target) *
                            Manager.MenuManager.CondemnBlock)
                        {
                            return null;
                        }

                        return target;
                    }
                }
            }
            return null;
        }
    }

    class Aka
    {
        //The First One ~Aka

        public static long LastCheck;
        public static List<Vector2> Points = new List<Vector2>();

        public static void Execute()
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        x =>
                            x.IsValidTarget(Manager.SpellManager.E.Range) && !x.HasBuffOfType(BuffType.SpellShield) &&
                            !x.HasBuffOfType(BuffType.SpellImmunity) &&
                            IsCondemable(x)))

                Manager.SpellManager.E.Cast(enemy);
        }



        public static bool IsCondemable(AIHeroClient unit, Vector2 pos = new Vector2())
        {
            if (unit.HasBuffOfType(BuffType.SpellImmunity) || unit.HasBuffOfType(BuffType.SpellShield) || LastCheck + 50 > Environment.TickCount || Variables._Player.IsDashing()) return false;
            var prediction = Manager.SpellManager.E2.GetPrediction(unit);
            var predictionsList = pos.IsValid() ? new List<Vector3>() { pos.To3D() } : new List<Vector3>
                        {
                            unit.ServerPosition,
                            unit.Position,
                            prediction.CastPosition,
                            prediction.UnitPosition
                        };

            var wallsFound = 0;
            Points = new List<Vector2>();
            foreach (var position in predictionsList)
            {
                for (var i = 0; i < Manager.MenuManager.CondemnPushDistance; i += (int)unit.BoundingRadius)
                {
                    var cPos = ObjectManager.Player.Position.Extend(position, ObjectManager.Player.Distance(position) + i).To3D();
                    Points.Add(cPos.To2D());
                    if (NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(cPos).HasFlag(CollisionFlags.Building))
                    {
                        wallsFound++;
                        break;
                    }
                }
            }
            if ((wallsFound / predictionsList.Count) >= 33 / 100f)
            {
                return true;
            }
            return false;
        }

    }

    class VHRevolution
    {
        public static Obj_AI_Base GetTarget(Vector3 fromPosition)
        {
            var HeroList = EntityManager.Heroes.Enemies.Where(
                                    h =>
                                        h.IsValidTarget(Manager.SpellManager.E.Range) &&
                                        !h.HasBuffOfType(BuffType.SpellShield) &&
                                        !h.HasBuffOfType(BuffType.SpellImmunity));
            //dz191.vhr.misc.condemn.rev.accuracy
            //dz191.vhr.misc.condemn.rev.nextprediction
            var MinChecksPercent = Manager.MenuManager.CondemnHitchance;
            var PushDistance = Manager.MenuManager.CondemnPushDistance;

            if (PushDistance >= 410)
            {
                var PushEx = PushDistance;
                PushDistance -= (10 + (PushEx - 410) / 2);
            }

            if (Variables.UnderEnemyTower((Vector2)Variables._Player.ServerPosition))
            {
                return null;
            }

            foreach (var Hero in HeroList)
            {
                if (Manager.MenuManager.OnlyStunCurrentTarget &&
                    Hero.NetworkId != TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical).NetworkId)
                {
                    continue;
                }

                if (Hero.Health + 10 <=
                    Variables._Player.GetAutoAttackDamage(Hero) *
                    Manager.MenuManager.CondemnBlock)
                {
                    continue;
                }


                var targetPosition = Manager.SpellManager.E2.GetPrediction(Hero).UnitPosition;
                var finalPosition = targetPosition.Extend(Variables._Player.ServerPosition, -PushDistance);
                var finalPosition_ex = Hero.ServerPosition.Extend(Variables._Player.ServerPosition, -PushDistance);

                var condemnRectangle = new AJSGeometry.AJSPolygon(AJSGeometry.AJSPolygon.Rectangle(targetPosition.To2D(), finalPosition, Hero.BoundingRadius));
                var condemnRectangle_ex = new AJSGeometry.AJSPolygon(AJSGeometry.AJSPolygon.Rectangle(Hero.ServerPosition.To2D(), finalPosition_ex, Hero.BoundingRadius));

                if (IsBothNearWall(Hero))
                {
                    return null;
                }

                if (condemnRectangle.Points.Count(point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >= condemnRectangle.Points.Count() * (MinChecksPercent / 100f)
                    && condemnRectangle_ex.Points.Count(point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >= condemnRectangle_ex.Points.Count() * (MinChecksPercent / 100f))
                {
                    return Hero;
                }
            }
            return null;
        }

        private static bool IsBothNearWall(Obj_AI_Base target)
        {
            var positions =
                GetWallQPositions(target, 110).ToList().OrderBy(pos => pos.Distance(target.ServerPosition, true));
            var positions_ex =
            GetWallQPositions(Variables._Player, 110).ToList().OrderBy(pos => pos.Distance(Variables._Player.ServerPosition, true));

            if (positions.Any(p => NavMesh.GetCollisionFlags(p).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(p).HasFlag(CollisionFlags.Building)) && positions_ex.Any(p => NavMesh.GetCollisionFlags(p).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(p).HasFlag(CollisionFlags.Building)))
            {
                return true;
            }
            return false;
        }

        private static Vector3[] GetWallQPositions(Obj_AI_Base player, float Range)
        {
            Vector3[] vList =
            {
                (player.ServerPosition.To2D() + Range * player.Direction.To2D()).To3D(),
                (player.ServerPosition.To2D() - Range * player.Direction.To2D()).To3D()

            };

            return vList;
        }
    }
}


