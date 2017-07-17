using System;
using EloBuddy;
using SharpDX;
using EloBuddy.SDK;
using System.Linq;
using System.Collections.Generic;
using Aka_s_Vayne.Features.Module;
using Aka_s_Vayne.Features.Module.Condemn;
using Aka_s_Vayne.Features.Module.Misc;
using Aka_s_Vayne.Features.Module.Tumble;

namespace Aka_s_Vayne
{
    static class Variables
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool IsAfterAttack;

        public static bool IsBeforeAttack;

        public static float lastTick, fps, delay, lastTickdelay, autoFpsBalancer, frameRate; //FPS Protection

        public static Vector3 TumblePosition = Vector3.Zero; //Tumble Prediction

        public static int currentSkin = 0; //Skin

        public static bool bought = false; //Autobuy

        public static int ticks = 0; //Autobuy

        public static float LastCondemnFlashTime { get; set; } //Flash E

        public static bool VayneUltiIsActive { get; set; } //Ult

        public static SpellSlot FlashSlot;

        public static int[] AbilitySequence; //Autolvl

        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0; //autolvl

        public static List<IModule> moduleList = new List<IModule>()
        {
            new AutoE(),
            new EKS(),
            new LowLifeE(),
            new NoAAStealth(),
            new QKS(),
            new FocusW(),
            new Reveal(),
            new AntiFlash(),
            new EInterrupt(),
            new FlashCondemn(),
        };

        public static bool ThreshInGame() //Autolantern
        {
            return ObjectManager.Get<AIHeroClient>().Any(h => h.IsAlly && !h.IsMe && h.ChampionName == "Thresh");
        }

        public static bool UltActive()
        {
            return (Variables._Player.HasBuff("vaynetumblefade") && !UnderEnemyTower((Vector2)_Player.Position));
        }

        public static bool UnderEnemyTower(Vector2 pos)
        {
            return EntityManager.Turrets.Enemies.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(pos) < 950);
        }

        public static bool UnderAllyTurret_Ex(this Vector3 position)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(t => t.IsAlly && !t.IsDead);
        }

        public static IEnumerable<AIHeroClient> ValidTargets { get { return EntityManager.Heroes.Enemies.Where(enemy => enemy.Health > 5 && enemy.IsVisible); } }

        public static bool IsValidTargetEx(
    this AttackableUnit unit,
    float range,
    bool checkTeam = true,
    Vector3 from = default(Vector3))
        {
            var ai = unit as Obj_AI_Base;

            if ((ai != null && ai.HasBuff("kindredrnodeathbuff") && ai.HealthPercent <= 10.0)
                || checkTeam && unit.Team == ObjectManager.Player.Team)
            {
                return false;
            }

            var targetPosition = ai != null ? ai.ServerPosition : unit.Position;
            var fromPosition = from.To2D().IsValid() ? from.To2D() : ObjectManager.Player.ServerPosition.To2D();

            var distance2 = Vector2.DistanceSquared(fromPosition, targetPosition.To2D());
            return distance2 <= range * range;
        }

        public static bool IsJ4Flag(Vector3 endPosition, Obj_AI_Base target)
        {
            return Manager.MenuManager.J4Flag
                && ObjectManager.Get<Obj_AI_Base>().Any(m => m.Distance(endPosition) <= target.BoundingRadius && m.Name == "Beacon");
        }

        public static IEnumerable<AIHeroClient> MeleeEnemiesTowardsMe
        {
            get
            {
                return
                    EntityManager.Heroes.Enemies.FindAll(
                        m => m.IsMelee && m.Distance(Variables._Player) <= _Player.GetAutoAttackRange(m)
                            && (m.ServerPosition.To2D() + (m.BoundingRadius + 25f) * m.Direction.To2D().Perpendicular()).Distance(Variables._Player.ServerPosition.To2D()) <= m.ServerPosition.Distance(Variables._Player.ServerPosition)
                            && m.IsValidTarget(1200, false));
            }
        }

        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    EntityManager.Heroes.Enemies.Where(
                        m =>
                            m.Distance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.IsValidTarget(1500, false) &&
                            m.CountEnemiesInRange(m.IsMelee ? m.AttackRange * 1.5f : m.AttackRange + 20 * 1.5f) > 0);
            }
        }

        public static List<AIHeroClient> GetLhEnemiesNear(this Vector3 position, float range, float healthpercent)
        {
            return EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(range, true, position) && hero.HealthPercent <= healthpercent).ToList();
        }

        #region MenuOptions

        public static bool Combo = false;
        public static bool Harass = false;
        public static bool Condemn = false;
        public static bool LC = false;
        public static bool JC = false;
        public static bool Misc = false;
        public static bool Activator = false;
        public static bool Flee = false;
        public static bool Draw = false;

        #endregion MenuOptions


    }
}
