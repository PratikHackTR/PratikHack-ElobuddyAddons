using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;
using SharpDX;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using System.Text.RegularExpressions;
using AkaCore.AkaLib.Evade;

namespace AkaCore.Manager
{
    class EvadeManager
    {
        public class EvadeSkillshot
        {
            #region Public Methods and Operators

            //public static Menu evadeMenu, evadeSpells, championmenu;

            public static void Init()
            {
                /*
                evadeMenu = MainMenu.AddMenu("Evade Skillshot", "EvadeSkillshot");
                {
                        foreach (var spell in EvadeSpellDatabase.Spells)
                        {
                            evadeSpells = evadeMenu.AddSubMenu(spell.Name + " (" + spell.Slot + ")", "ESSS_" + spell.Name);
                            {
                                if (spell.Name == "YasuoDashWrapper")
                                {
                                    evadeSpells.Add("ETower", new CheckBox("Under Tower", false));
                                }
                                else if (spell.Name == "YasuoWMovingWall")
                                {
                                    evadeSpells.Add("WDelay", new Slider("Extra Delay", 100, 0, 150));
                                }
                                evadeSpells.Add("DangerLevel", new Slider("If Danger Level >=", 2, 1, 5));
                                evadeSpells.Add("Enabled", new CheckBox("Enabled"));
                            }
                        }
                    foreach (var spell in
                        SpellDatabase.Spells.Where(i => EntityManager.Heroes.Enemies.Any(a => a.ChampionName == i.ChampionName)))
                    {
                        championmenu = evadeMenu.AddSubMenu(spell.SpellName + " (" + spell.Slot + ")", "ESS_" + spell.MenuItemName);
                        {
                            championmenu.Add("DangerLevel", new Slider("Danger Level", spell.DangerValue, 1, 5));
                            championmenu.Add("Enabled", new CheckBox("Enabled", !spell.DisabledByDefault));
                            //evadeMenu.SubMenu("EvadeSS_" + spell.ChampionName).AddSubMenu(sub);                           
                        }
                    }
                }
                */
                Collisions.Init();
                Game.OnUpdate += OnUpdateEvade;
                SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
                SkillshotDetector.OnDeleteMissile += OnDeleteMissile;
            }

            public static IsSafeResult IsSafePoint(Vector2 point)
            {
                var result = new IsSafeResult { SkillshotList = new List<Skillshot>() };
                foreach (var skillshot in
                    SkillshotDetector.DetectedSkillshots.Where(i => i.Evade && !i.IsSafePoint(point)))
                {
                    result.SkillshotList.Add(skillshot);
                }
                result.IsSafe = result.SkillshotList.Count == 0;
                return result;
            }

            #endregion

            #region Methods

            private static IEnumerable<Obj_AI_Base> GetEvadeTargets(
                EvadeSpellData spell,
                bool onlyGood = false,
                bool dontCheckForSafety = false)
            {
                var badTargets = new List<Obj_AI_Base>();
                var goodTargets = new List<Obj_AI_Base>();
                var allTargets = new List<Obj_AI_Base>();
                foreach (var targetType in spell.ValidTargets)
                {
                    switch (targetType)
                    {
                        case SpellTargets.AllyChampions:
                            allTargets.AddRange(
                                EntityManager.Heroes.Allies.Where(i => i.IsValidTarget(spell.MaxRange, false) && !i.IsMe));
                            break;
                        case SpellTargets.AllyMinions:
                            allTargets.AddRange(EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Ally, ObjectManager.Player.ServerPosition, spell.MaxRange));
                            break;
                        case SpellTargets.AllyWards:
                            allTargets.AddRange(
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        i =>
                                         i.IsValidTarget(spell.MaxRange, false) && i.Team == ObjectManager.Player.Team));
                            break;
                        case SpellTargets.EnemyChampions:
                            allTargets.AddRange(EntityManager.Heroes.Enemies.Where(i => i.IsValidTarget(spell.MaxRange)));
                            break;
                        case SpellTargets.EnemyMinions:
                            allTargets.AddRange(EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, ObjectManager.Player.ServerPosition, spell.MaxRange));
                            break;
                    }
                }
                foreach (var target in
                    allTargets.Where(i => dontCheckForSafety || IsSafePoint(i.ServerPosition.To2D()).IsSafe))
                {
                    if (spell.Name == "YasuoDashWrapper" && target.HasBuff("YasuoDashWrapper"))
                    {
                        continue;
                    }
                    var pathToTarget = new List<Vector2> { ObjectManager.Player.ServerPosition.To2D(), target.ServerPosition.To2D() };
                    if (IsSafePath(pathToTarget, Configs.EvadingFirstTimeOffset, spell.Speed, spell.Delay).IsSafe)
                    {
                        goodTargets.Add(target);
                    }
                    if (IsSafePath(pathToTarget, Configs.EvadingSecondTimeOffset, spell.Speed, spell.Delay).IsSafe)
                    {
                        badTargets.Add(target);
                    }
                }
                return goodTargets.Any() ? goodTargets : (onlyGood ? new List<Obj_AI_Base>() : badTargets);
            }

            public static List<Vector2> GetEvadePoints(int speed = -1,
    int delay = 0,
    bool isBlink = false,
    bool onlyGood = false)
            {
                speed = speed == -1 ? (int)ObjectManager.Player.MoveSpeed : speed;

                var goodCandidates = new List<Vector2>();
                var badCandidates = new List<Vector2>();

                var polygonList = new List<Geometry.Polygon>();

                foreach (var skillshot in SkillshotDetector.DetectedSkillshots)
                {
                    if (skillshot.Evade)
                    {
                        polygonList.Add(skillshot.Polygon);
                    }
                }

                //Create the danger polygon:
                var dangerPolygons = Geometry.ClipPolygons(polygonList).ToPolygons();
                var myPosition = ObjectManager.Player.Position.To2D();

                //Scan the sides of each polygon to find the safe point.
                foreach (var poly in dangerPolygons)
                {
                    for (var i = 0; i <= poly.Points.Count - 1; i++)
                    {
                        var sideStart = poly.Points[i];
                        var sideEnd = poly.Points[(i == poly.Points.Count - 1) ? 0 : i + 1];

                        var originalCandidate = myPosition.ProjectOn(sideStart, sideEnd).SegmentPoint;
                        var distanceToEvadePoint = Vector2.DistanceSquared(originalCandidate, myPosition);


                        if (distanceToEvadePoint < 600 * 600)
                        {
                            var sideDistance = Vector2.DistanceSquared(sideEnd, sideStart);
                            var direction = (sideEnd - sideStart).Normalized();

                            var s = (distanceToEvadePoint < 200 * 200 && sideDistance > 90 * 90)
                                ? Configs.DiagonalEvadePointsCount
                                : 0;
                            for (var j = -s; j <= s; j++)
                            {
                                var candidate = originalCandidate + j * Configs.DiagonalEvadePointsStep * direction;
                                var pathToPoint = ObjectManager.Player.GetPath(candidate.To3D()).ToList();

                                if (IsSafePath(pathToPoint.To2D(), Configs.EvadingFirstTimeOffset, speed, delay).IsSafe)
                                {
                                    goodCandidates.Add(candidate);
                                }

                                if (
                                    IsSafePath(pathToPoint.To2D(), Configs.EvadingSecondTimeOffset, speed, delay).IsSafe &&
                                    j == 0)
                                {
                                    badCandidates.Add(candidate);
                                }
                            }
                        }
                    }
                }
                return (goodCandidates.Count > 0) ? goodCandidates : (onlyGood ? new List<Vector2>() : badCandidates);
            }

            private static SafePathResult IsSafePath(List<Vector2> path, int timeOffset, int speed = -1, int delay = 0)
            {
                var isSafe = false;
                var intersections = new List<FoundIntersection>();
                var intersection = new FoundIntersection();
                foreach (var sResult in
                    SkillshotDetector.DetectedSkillshots.Where(i => i.Evade)
                        .Select(i => i.IsSafePath(path, timeOffset, speed, delay)))
                {
                    isSafe = sResult.IsSafe;
                    if (sResult.Intersection.Valid)
                    {
                        intersections.Add(sResult.Intersection);
                    }
                }
                return isSafe
                           ? new SafePathResult(true, intersection)
                           : new SafePathResult(
                                 false,
                                 intersections.Count > 0 ? intersections.MinOrDefault(i => i.Distance) : intersection);
            }

            private static void OnDeleteMissile(Skillshot skillshot, MissileClient missile)
            {
                if (skillshot.SpellData.SpellName != "VelkozQ"
                    || SkillshotDetector.DetectedSkillshots.Count(i => i.SpellData.SpellName == "VelkozQSplit") != 0)
                {
                    return;
                }
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                for (var i = -1; i <= 1; i = i + 2)
                {
                    SkillshotDetector.DetectedSkillshots.Add(
                        new Skillshot(
                            DetectionType.ProcessSpell,
                            spellData,
                            AkaLib.Evade.Extensions.GameTimeTickCount,
                            missile.Position.To2D(),
                            missile.Position.To2D() + i * skillshot.Perpendicular * spellData.Range,
                            skillshot.Unit));
                }
            }

            private static void OnDetectSkillshot(Skillshot skillshot)
            {
                var alreadyAdded = false;

                foreach (var i in SkillshotDetector.DetectedSkillshots)
                {
                    if (i.SpellData.SpellName == skillshot.SpellData.SpellName
                    && i.Unit.NetworkId == skillshot.Unit.NetworkId
                    && skillshot.Direction.AngleBetween(i.Direction) < 5
                    && (skillshot.Start.Distance(i.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0))

                        alreadyAdded = true;
                }
                if (skillshot.Unit.Team == ObjectManager.Player.Team)
                {
                    return;
                }
                if (skillshot.Start.Distance(ObjectManager.Player.ServerPosition.To2D())
                    > (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
                {
                    return;
                }
                if (alreadyAdded && !skillshot.SpellData.DontCheckForDuplicates)
                {
                    return;
                }
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;
                        for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                             i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                             i++)
                        {
                            SkillshotDetector.DetectedSkillshots.Add(
                                new Skillshot(
                                    skillshot.DetectionType,
                                    skillshot.SpellData,
                                    skillshot.StartTick,
                                    skillshot.Start,
                                    skillshot.Start
                                    + skillshot.SpellData.Range
                                    * originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i),
                                    skillshot.Unit));
                        }
                        return;
                    }
                    if (skillshot.SpellData.SpellName == "UFSlash")
                    {
                        skillshot.SpellData.MissileSpeed = 1600 + (int)skillshot.Unit.MoveSpeed;
                    }
                    if (skillshot.SpellData.SpellName == "SionR")
                    {
                        skillshot.SpellData.MissileSpeed = (int)skillshot.Unit.MoveSpeed;
                    }
                    if (skillshot.SpellData.Invert)
                    {
                        SkillshotDetector.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                skillshot.SpellData,
                                skillshot.StartTick,
                                skillshot.Start,
                                skillshot.Start
                                + -(skillshot.End - skillshot.Start).Normalized()
                                * skillshot.Start.Distance(skillshot.End),
                                skillshot.Unit));
                        return;
                    }
                    if (skillshot.SpellData.Centered)
                    {
                        SkillshotDetector.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                skillshot.SpellData,
                                skillshot.StartTick,
                                skillshot.Start - skillshot.Direction * skillshot.SpellData.Range,
                                skillshot.Start + skillshot.Direction * skillshot.SpellData.Range,
                                skillshot.Unit));
                        return;
                    }
                    if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                    {
                        const int Angle = 60;
                        var edge1 =
                            (skillshot.End - skillshot.Unit.ServerPosition.To2D()).Rotated(
                                -Angle / 2f * (float)Math.PI / 180);
                        var edge2 = edge1.Rotated(Angle * (float)Math.PI / 180);
                        foreach (var skillshotToAdd in from minion in ObjectManager.Get<Obj_AI_Minion>()
                                                       let v =
                                                           (minion.ServerPosition - skillshot.Unit.ServerPosition).To2D(
                                                               )
                                                       where
                                                           minion.Name == "Seed" && edge1.CrossProduct(v) > 0
                                                           && v.CrossProduct(edge2) > 0
                                                           && minion.Distance(skillshot.Unit) < 800
                                                           && minion.Team != ObjectManager.Player.Team
                                                       let start = minion.ServerPosition.To2D()
                                                       let end =
                                                           skillshot.Unit.ServerPosition.Extend(
                                                               minion.ServerPosition,
                                                               skillshot.Unit.Distance(minion) > 200 ? 1300 : 1000)
                                                       select
                                                           new Skillshot(
                                                           skillshot.DetectionType,
                                                           skillshot.SpellData,
                                                           skillshot.StartTick,
                                                           start,
                                                           end,
                                                           skillshot.Unit))
                        {
                            SkillshotDetector.DetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }
                    if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                    {
                        SkillshotDetector.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                skillshot.SpellData,
                                skillshot.StartTick,
                                skillshot.End - skillshot.Perpendicular * 400,
                                skillshot.End + skillshot.Perpendicular * 400,
                                skillshot.Unit));
                        return;
                    }
                    if (skillshot.SpellData.SpellName == "DianaArc")
                    {
                        SkillshotDetector.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                SpellDatabase.GetByName("DianaArcArc"),
                                skillshot.StartTick,
                                skillshot.Start,
                                skillshot.End,
                                skillshot.Unit));
                    }
                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.Distance(skillshot.End);
                        var d2 = d1 * 0.4f;
                        var d3 = d2 * 0.69f;
                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");
                        var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction * d3;
                        bounce1SpellData.Delay =
                            (int)(skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                        bounce2SpellData.Delay =
                            (int)(bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);
                        SkillshotDetector.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                bounce1SpellData,
                                skillshot.StartTick,
                                skillshot.End,
                                bounce1Pos,
                                skillshot.Unit));
                        SkillshotDetector.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                bounce2SpellData,
                                skillshot.StartTick,
                                bounce1Pos,
                                bounce2Pos,
                                skillshot.Unit));
                    }
                    if (skillshot.SpellData.SpellName == "ZiggsR")
                    {
                        skillshot.SpellData.Delay =
                            (int)(1500 + 1500 * skillshot.End.Distance(skillshot.Start) / skillshot.SpellData.Range);
                    }
                    if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                    {
                        var endPos = new Vector2();
                        foreach (var s in SkillshotDetector.DetectedSkillshots)
                        {
                            if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                            {
                                var extendedE = new Skillshot(
                                    skillshot.DetectionType,
                                    skillshot.SpellData,
                                    skillshot.StartTick,
                                    skillshot.Start,
                                    skillshot.End + skillshot.Direction * 100,
                                    skillshot.Unit);
                                if (!extendedE.IsSafePoint(s.End))
                                {
                                    endPos = s.End;
                                }
                                break;
                            }
                        }
                        foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            if (m.CharData.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team)
                            {
                                var extendedE = new Skillshot(
                                    skillshot.DetectionType,
                                    skillshot.SpellData,
                                    skillshot.StartTick,
                                    skillshot.Start,
                                    skillshot.End + skillshot.Direction * 100,
                                    skillshot.Unit);
                                if (!extendedE.IsSafePoint(m.Position.To2D()))
                                {
                                    endPos = m.Position.To2D();
                                }
                                break;
                            }
                        }
                        if (endPos.IsValid())
                        {
                            skillshot = new Skillshot(
                                DetectionType.ProcessSpell,
                                SpellDatabase.GetByName("JarvanIVEQ"),
                                AkaLib.Evade.Extensions.GameTimeTickCount,
                                skillshot.Start,
                                endPos + 200 * (endPos - skillshot.Start).Normalized(),
                                skillshot.Unit);
                        }
                    }
                }
                if (skillshot.SpellData.SpellName == "OriannasQ")
                {
                    SkillshotDetector.DetectedSkillshots.Add(
                        new Skillshot(
                            skillshot.DetectionType,
                            SpellDatabase.GetByName("OriannaQend"),
                            skillshot.StartTick,
                            skillshot.Start,
                            skillshot.End,
                            skillshot.Unit));
                }
                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }
                SkillshotDetector.DetectedSkillshots.Add(skillshot);
            }

            private static void OnUpdateEvade(EventArgs args)
            {
                SkillshotDetector.DetectedSkillshots.RemoveAll(i => !i.IsActive);
                foreach (var skillshot in SkillshotDetector.DetectedSkillshots)
                {
                    skillshot.OnUpdate();
                }
                if (ObjectManager.Player.IsDead)
                {
                    return;
                }
                if (ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity) || ObjectManager.Player.HasBuffOfType(BuffType.SpellShield))
                {
                    return;
                }
                var safePoint = IsSafePoint(ObjectManager.Player.ServerPosition.To2D());
                var safePath = IsSafePath(ObjectManager.Player.GetWaypoints(), 100);
                if (!safePath.IsSafe && !safePoint.IsSafe)
                {
                    TryToEvade(safePoint.SkillshotList, Game.CursorPos.To2D());
                }
            }

            private static void TryToEvade(List<Skillshot> hitBy, Vector2 to)
            {
                var dangerLevel =
                    hitBy.Select(i => Manager.MenuManager.EDLVL.Cast<Slider>().CurrentValue) //championmenu["DangerLevel"].Cast<Slider>().CurrentValue
                        .Concat(new[] { 0 })
                        .Max();
                foreach (var evadeSpell in
                    EvadeSpellDatabase.Spells.Where(i => i.Enabled && i.DangerLevel <= dangerLevel && i.IsReady)
                        .OrderBy(i => i.DangerLevel))
                {
                    if (evadeSpell.EvadeType == EvadeTypes.Dash && evadeSpell.CastType == CastTypes.Target)
                    {
                        var targets =
                            GetEvadeTargets(evadeSpell)
                                .Where(
                                    i =>
                                    IsSafePoint(Extensions.PosAfterE(i).To2D()).IsSafe
                                    && (!Extensions.PosAfterE(i).IsUnderTurret()) || Manager.MenuManager.EvadeMenu["ETower"].Cast<CheckBox>().CurrentValue)
                                .ToList();
                        if (targets.Count > 0)
                        {
                            var closestTarget = targets.MinOrDefault(i => Extensions.PosAfterE(i).To2D().Distance(to));
                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                            return;
                        }
                    }
                    if (evadeSpell.EvadeType == EvadeTypes.WindWall
                        && hitBy.Where(
                            i =>
                            i.SpellData.CollisionObjects.Contains(CollisionObjectTypes.YasuoWall)
                            && i.IsAboutToHit(
                                150 + evadeSpell.Delay - Manager.MenuManager.EvadeMenu["WDelay"].Cast<Slider>().CurrentValue,
                                ObjectManager.Player))
                               .OrderByDescending(
                                   i => Manager.MenuManager.EDLVL.Cast<Slider>().CurrentValue)
                               .Any(
                                   i =>
                                   ObjectManager.Player.Spellbook.CastSpell(
                                       evadeSpell.Slot,
                                       ObjectManager.Player.ServerPosition.Extend(i.Start.To3D(), 100).To3D(), true)))
                    {
                        return;
                    }
                    if (evadeSpell.EvadeType == EvadeTypes.Dash && evadeSpell.CastType == CastTypes.Position)
                    {
                        var points = GetEvadePoints(evadeSpell.Speed, evadeSpell.Delay, false);

                        points.RemoveAll(
                            item => item.Distance(ObjectManager.Player.ServerPosition) > evadeSpell.MaxRange);

                        if (evadeSpell.FixedRange)
                        {
                            for (var i = 0; i < points.Count; i++)
                            {
                                points[i] = ObjectManager.Player.Position
                                    .Extend(points[i], evadeSpell.MaxRange);
                            }

                            for (var i = points.Count - 1; i > 0; i--)
                            {
                                if (!IsSafePoint(points[i]).IsSafe)
                                {
                                    points.RemoveAt(i);
                                }
                            }
                        }

                        if (points.Count > 0)
                        {
                            var EvadePoint = to.Closest(points);

                            var castPoint = ObjectManager.Player.Position -
                                            (EvadePoint.To3D() - ObjectManager.Player.Position);
                            ObjectManager.Player.Spellbook.CastSpell(evadeSpell.Slot, castPoint);
                        }

                        return;
                    }
                }
            }

            #endregion

            internal struct IsSafeResult
            {
                #region Fields

                public bool IsSafe;

                public List<Skillshot> SkillshotList;

                #endregion
            }
        }

        public class EvadeTarget
        {
            #region Static Fields

            private static readonly List<Targets> DetectedTargets = new List<Targets>();

            public static readonly List<SpellData> Spells = new List<SpellData>();

            private static Vector2 wallCastedPos;

            #endregion

            #region Properties

            private static GameObject Wall
            {
                get
                {
                    return
                        ObjectManager.Get<GameObject>()
                            .FirstOrDefault(
                                i => i.IsValid && Regex.IsMatch(i.Name, "_w_windwall.\\.troy", RegexOptions.IgnoreCase));
                }
            }

            #endregion

            #region Public Methods and Operators

            //public static Menu evadeMenu2, championmenu2;

            public static void Init()
            {
                Extensions.LoadYSpells();
                LoadSpellData();
                /*
                evadeMenu2 = MainMenu.AddMenu("Evade Target", "EvadeTarget");
                {
                    evadeMenu2.Add("W", new CheckBox("Use W")); //                                    evadeSpells.Add("ETower", new CheckBox("Under Tower", false));
                    evadeMenu2.Add("E", new CheckBox("Use E (To Dash Behind WindWall)"));
                    evadeMenu2.Add("ETower", new CheckBox("-> Under Tower", false));
                    evadeMenu2.Add("BAttack", new CheckBox("Basic Attack"));
                    evadeMenu2.Add("BAttackHpU", new Slider("-> If Hp <", 35));
                    evadeMenu2.Add("CAttack", new CheckBox("Crit Attack"));
                    evadeMenu2.Add("CAttackHpU", new Slider("-> If Hp <", 40));
                        championmenu2 = evadeMenu2.AddSubMenu("Evade Point to Click");                   
                    foreach (
                        var spell in Spells.Where(i => EntityManager.Heroes.Enemies.Any(a => a.ChampionName == i.ChampionName)))
                    {
                        championmenu2.Add(spell.MissileName, new CheckBox(
                            spell.MissileName + " (" + spell.Slot + ")",
                            false));
                    }
                }
                */
                Game.OnUpdate += OnUpdateTarget;
                GameObject.OnCreate += ObjSpellMissileOnCreate;
                GameObject.OnDelete += ObjSpellMissileOnDelete;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            }

            #endregion

            #region Methods

            private static bool GoThroughWall(Vector2 pos1, Vector2 pos2)
            {
                if (Wall == null)
                {
                    return false;
                }
                var wallWidth = 300 + 50 * Convert.ToInt32(Wall.Name.Substring(Wall.Name.Length - 6, 1));
                var wallDirection = (Wall.Position.To2D() - wallCastedPos).Normalized().Perpendicular();
                var wallStart = Wall.Position.To2D() + wallWidth / 2f * wallDirection;
                var wallEnd = wallStart - wallWidth * wallDirection;
                var wallPolygon = new Geometry.Polygon.Rectangle(wallStart, wallEnd, 75);
                var intersections = new List<Vector2>();
                for (var i = 0; i < wallPolygon.Points.Count; i++)
                {
                    var inter =
                        wallPolygon.Points[i].Intersection(
                            wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0],
                            pos1,
                            pos2);
                    if (inter.Intersects)
                    {
                        intersections.Add(inter.Point);
                    }
                }
                return intersections.Any();
            }

            private static void LoadSpellData()
            {
                Spells.Add(
                    new SpellData
                    { ChampionName = "Ahri", SpellNames = new[] { "ahrifoxfiremissiletwo" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    { ChampionName = "Ahri", SpellNames = new[] { "ahritumblemissile" }, Slot = SpellSlot.R });
                Spells.Add(
                    new SpellData { ChampionName = "Akali", SpellNames = new[] { "akalimota" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData { ChampionName = "Anivia", SpellNames = new[] { "frostbite" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { ChampionName = "Annie", SpellNames = new[] { "disintegrate" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Brand",
                        SpellNames = new[] { "brandconflagrationmissile" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Brand",
                        SpellNames = new[] { "brandwildfire", "brandwildfiremissile" },
                        Slot = SpellSlot.R
                    });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Caitlyn",
                        SpellNames = new[] { "caitlynaceintheholemissile" },
                        Slot = SpellSlot.R
                    });
                Spells.Add(
                    new SpellData
                    { ChampionName = "Cassiopeia", SpellNames = new[] { "cassiopeiatwinfang" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { ChampionName = "Elise", SpellNames = new[] { "elisehumanq" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Ezreal",
                        SpellNames = new[] { "ezrealarcaneshiftmissile" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "FiddleSticks",
                        SpellNames = new[] { "fiddlesticksdarkwind", "fiddlesticksdarkwindmissile" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData { ChampionName = "Gangplank", SpellNames = new[] { "parley" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData { ChampionName = "Janna", SpellNames = new[] { "sowthewind" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData { ChampionName = "Kassadin", SpellNames = new[] { "nulllance" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Katarina",
                        SpellNames = new[] { "katarinaq", "katarinaqmis" },
                        Slot = SpellSlot.Q
                    });
                Spells.Add(
                    new SpellData
                    { ChampionName = "Kayle", SpellNames = new[] { "judicatorreckoning" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Leblanc",
                        SpellNames = new[] { "leblancchaosorb", "leblancchaosorbm" },
                        Slot = SpellSlot.Q
                    });
                Spells.Add(new SpellData { ChampionName = "Lulu", SpellNames = new[] { "luluw" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    { ChampionName = "Malphite", SpellNames = new[] { "seismicshard" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "MissFortune",
                        SpellNames = new[] { "missfortunericochetshot", "missFortunershotextra" },
                        Slot = SpellSlot.Q
                    });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Nami",
                        SpellNames = new[] { "namiwenemy", "namiwmissileenemy" },
                        Slot = SpellSlot.W
                    });
                Spells.Add(
                    new SpellData { ChampionName = "Nunu", SpellNames = new[] { "iceblast" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { ChampionName = "Pantheon", SpellNames = new[] { "pantheonq" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Ryze",
                        SpellNames = new[] { "spellflux", "spellfluxmissile" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData { ChampionName = "Shaco", SpellNames = new[] { "twoshivpoison" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { ChampionName = "Sona", SpellNames = new[] { "sonaqmissile" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData { ChampionName = "Swain", SpellNames = new[] { "swaintorment" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { ChampionName = "Syndra", SpellNames = new[] { "syndrar" }, Slot = SpellSlot.R });
                Spells.Add(
                    new SpellData { ChampionName = "Teemo", SpellNames = new[] { "blindingdart" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    { ChampionName = "Tristana", SpellNames = new[] { "detonatingshot" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData
                    { ChampionName = "TwistedFate", SpellNames = new[] { "bluecardattack" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    { ChampionName = "TwistedFate", SpellNames = new[] { "goldcardattack" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    { ChampionName = "TwistedFate", SpellNames = new[] { "redcardattack" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = "Urgot",
                        SpellNames = new[] { "urgotheatseekinghomemissile" },
                        Slot = SpellSlot.Q
                    });
                Spells.Add(
                    new SpellData { ChampionName = "Vayne", SpellNames = new[] { "vaynecondemn" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData
                    { ChampionName = "Veigar", SpellNames = new[] { "veigarprimordialburst" }, Slot = SpellSlot.R });
                Spells.Add(
                    new SpellData
                    { ChampionName = "Viktor", SpellNames = new[] { "viktorpowertransfer" }, Slot = SpellSlot.Q });
            }

            private static void ObjSpellMissileOnCreate(GameObject sender, EventArgs args)
            {
                if (!sender.IsValid<MissileClient>())
                {
                    return;
                }
                var missile = (MissileClient)sender;
                if (!missile.SpellCaster.IsValid<AIHeroClient>() || missile.SpellCaster.Team == ObjectManager.Player.Team)
                {
                    return;
                }
                var unit = (AIHeroClient)missile.SpellCaster;
                var spellData =
                    Spells.FirstOrDefault(
                        i =>
                        i.SpellNames.Contains(missile.SData.Name.ToLower())
                        && Manager.MenuManager.EvadeMenu[i.MissileName].Cast<CheckBox>().CurrentValue);
                if (spellData == null && EloBuddy.SDK.Constants.AutoAttacks.IsAutoAttack(missile.SData) //MenuManager.LaneClearMenu["E"].Cast<CheckBox>().CurrentValue
                    && (!missile.SData.Name.ToLower().Contains("crit")
                            ? Manager.MenuManager.EvadeMenu["BAttack"].Cast<CheckBox>().CurrentValue
                              && ObjectManager.Player.HealthPercent < Manager.MenuManager.EvadeMenu["BAttackHpU"].Cast<Slider>().CurrentValue
                            : Manager.MenuManager.EvadeMenu["CAttack"].Cast<CheckBox>().CurrentValue
                              && ObjectManager.Player.HealthPercent < Manager.MenuManager.EvadeMenu["CAttackHpU"].Cast<Slider>().CurrentValue))
                {
                    spellData = new SpellData
                    { ChampionName = unit.ChampionName, SpellNames = new[] { missile.SData.Name } };
                }
                if (spellData == null || !missile.Target.IsMe)
                {
                    return;
                }
                DetectedTargets.Add(new Targets { Start = unit.ServerPosition, Obj = missile });
            }

            private static void ObjSpellMissileOnDelete(GameObject sender, EventArgs args)
            {
                if (!sender.IsValid<MissileClient>())
                {
                    return;
                }
                var missile = (MissileClient)sender;
                if (missile.SpellCaster.IsValid<AIHeroClient>() && missile.SpellCaster.Team != ObjectManager.Player.Team)
                {
                    DetectedTargets.RemoveAll(i => i.Obj.NetworkId == missile.NetworkId);
                }
            }

            private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (!sender.IsValid || sender.Team != ObjectManager.Player.Team || args.SData.Name != "YasuoWMovingWall")
                {
                    return;
                }
                wallCastedPos = sender.ServerPosition.To2D();
            }

            private static void OnUpdateTarget(EventArgs args)
            {
                if (ObjectManager.Player.IsDead)
                {
                    return;
                }
                if (ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity) || ObjectManager.Player.HasBuffOfType(BuffType.SpellShield))
                {
                    return;
                }
                if (!Extensions.YasuoW.IsReady(300) && (Wall == null || !Extensions.YasuoE.IsReady(200)))
                {
                    return;
                }
                foreach (var target in
                    DetectedTargets.Where(i => ObjectManager.Player.Distance(i.Obj.Position) < 700))
                {
                    if (Extensions.YasuoE.IsReady() && Manager.MenuManager.EvadeMenu["E"].Cast<CheckBox>().CurrentValue && Wall != null
                        && !GoThroughWall(ObjectManager.Player.ServerPosition.To2D(), target.Obj.Position.To2D())
                        && Extensions.YasuoW.IsInRange(target.Obj.Position))
                    {
                        var obj = new List<Obj_AI_Base>();
                        obj.AddRange(EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, ObjectManager.Player.ServerPosition, Extensions.YasuoE.Range));
                        obj.AddRange(EntityManager.Heroes.Enemies.Where(i => i.IsValidTarget(Extensions.YasuoE.Range)));
                        if (
                            obj.Where(
                                i =>
                                Extensions.CanCastE(i) && EvadeSkillshot.IsSafePoint(i.ServerPosition.To2D()).IsSafe
                                && EvadeSkillshot.IsSafePoint(Extensions.PosAfterE(i).To2D()).IsSafe
                                && (!Extensions.PosAfterE(i).IsUnderTurret() || Manager.MenuManager.EvadeMenu["ETower"].Cast<CheckBox>().CurrentValue)
                                && GoThroughWall(ObjectManager.Player.ServerPosition.To2D(), Extensions.PosAfterE(i).To2D()))
                                .OrderBy(i => Extensions.PosAfterE(i).Distance(Game.CursorPos))
                                .Any(i => Extensions.YasuoE.Cast(i)))
                        {
                            return;
                        }
                    }
                    if (Extensions.YasuoW.IsReady() && Manager.MenuManager.EvadeMenu["W"].Cast<CheckBox>().CurrentValue && Extensions.YasuoW.IsInRange(target.Obj.Position))
                    {
                        Extensions.YasuoW.Cast((Vector3)ObjectManager.Player.ServerPosition.Extend(target.Start, 100));
                    }
                }
            }

            #endregion

            public class SpellData
            {
                #region Fields

                public string ChampionName;

                public SpellSlot Slot;

                public string[] SpellNames = { };

                #endregion

                #region Public Properties

                public string MissileName
                {
                    get
                    {
                        return this.SpellNames.First();
                    }
                }

                #endregion
            }

            private class Targets
            {
                #region Fields

                public MissileClient Obj;

                public Vector3 Start;

                #endregion
            }
        }
    }

    public class Extensions
    {
        public static Spell.Targeted YasuoE;
        public static Spell.Skillshot YasuoW;

        public static bool CanCastE(Obj_AI_Base target)
        {
            return !target.HasBuff("YasuoDashWrapper");
        }

        public static Vector3 PosAfterE(Obj_AI_Base target)
        {
            return (Vector3)ObjectManager.Player.ServerPosition.Extend(
                target.ServerPosition,
                ObjectManager.Player.Distance(target) < 410 ? 475 : ObjectManager.Player.Distance(target) + 65);
        }

        public static void LoadYSpells()
        {
            YasuoW = new Spell.Skillshot(SpellSlot.W, 400, EloBuddy.SDK.Enumerations.SkillShotType.Cone);
            YasuoE = new Spell.Targeted(SpellSlot.E, 475);
        }
    }
}
    

