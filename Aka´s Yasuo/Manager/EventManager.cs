using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using System.Linq;
using SharpDX;
using Aka_s_Yasuo.Features.Module;

namespace Aka_s_Yasuo.Manager
{
    class EventManager
    {
        public static void Load()
        {
            Game.OnUpdate += Game_OnUpdate;
            Dash.OnDash += Dash_OnDash;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            Obj_AI_Base.OnBuffUpdate += Obj_AI_Base_OnBuffUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            foreach (var module in Variables.moduleList)
            {
                module.OnLoad();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {

            foreach (var module in Variables.moduleList.Where(module => module.GetModuleType() == ModuleType.OnUpdate
    && module.ShouldGetExecuted()))
            {
                module.OnExecute();
            }

            if (Variables._Player.IsDead)
            {
                if (Variables.isDash)
                {
                    Variables.isDash = false;
                    Variables.posDash = new Vector3();
                }
                return;
            }

            if (Variables.isDash && !Variables._Player.IsDashing())
            {
                Variables.isDash = false;
                Core.DelayAction(() =>
                {
                    if (!Variables.isDash)
                    {
                        Variables.posDash = new Vector3();
                    }
                },
                    50);
            }
            SpellManager.Q.CastDelay = (int)Variables.GetQDelay(false);
            SpellManager.Q2.CastDelay = (int)Variables.GetQDelay(true);
            SpellManager.E2.Speed = 1200 + (int)(Variables._Player.MoveSpeed - 345);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Features.Modes.Harass.Execute();
            //if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) Features.Modes.JungleClear.Load();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Features.Modes.Flee.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Features.Modes.Combo.Execute();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) Features.Modes.LaneClear.Execute();
        }

        private static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            if (!sender.IsMe)
            {
                return;
            }
            Variables.isDash = true;
            Variables.posDash = e.EndPos;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            if (args.Slot == SpellSlot.E)
            {
                Variables.lastE = Environment.TickCount;
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            switch (args.Buff.DisplayName)
            {
                case "YasuoQ3W":
                    Variables.haveQ3 = true;
                    break;
                case "YasuoDashScalar":
                    Variables.cDash = 1;
                    break;
                case "yasuoeqcombosoundmiss":
                case "YasuoEQComboSoundHit":    
                    Core.DelayAction(
                        () =>
                        Player.IssueOrder(
                            GameObjectOrder.AttackTo,
                            (Vector3)Variables._Player.ServerPosition.Extend(Game.CursorPos, Variables._Player.BoundingRadius)), 90);
                    break;
            }
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            switch (args.Buff.DisplayName)
            {
                case "YasuoQ3W":
                    Variables.haveQ3 = false;
                    break;
                case "YasuoDashScalar":
                    Variables.cDash = 0;
                    break;
            }
        }

        private static void Obj_AI_Base_OnBuffUpdate(Obj_AI_Base sender, Obj_AI_BaseBuffUpdateEventArgs args)
        {
            if (!sender.IsMe || args.Buff.DisplayName != "YasuoDashScalar")
            {
                return;
            }
            Variables.cDash = 2;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender is MissileClient)
            {
                MissileClient missle = (MissileClient)sender;
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    Variables.wallLeft = missle;
                }
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    Variables.wallcasted = true;
                }
                if (missle.SData.Name == "yasuowmovingwallmisr")
                {
                    Variables.wallRight = missle;
                }
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender is MissileClient)
            {
                MissileClient missle = (MissileClient)sender;
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    Variables.wallLeft = missle;
                }
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    Variables.wallcasted = true;
                }
                if (missle.SData.Name == "yasuowmovingwallmisr")
                {
                    Variables.wallRight = missle;
                }
            }
        }
    }
}
