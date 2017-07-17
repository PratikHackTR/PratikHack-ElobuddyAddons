using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using System.Linq;
using Aka_s_Vayne.Features.Module;
using AkaCore.Features.Utility.Modules;

namespace Aka_s_Vayne.Manager
{
    class EventManager
    {
        private static Logic.AJSProvider TumbleProvider = new Logic.AJSProvider();

        public static void Load()
        {
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Game.OnPostTick += delegate { Variables.IsAfterAttack = false; Variables.IsBeforeAttack = false; };
            Game.OnUpdate += Game_OnUpdate;
            Logic.Mechanics.LoadFlash();
            Traps.Load();

            foreach (var module in Variables.moduleList)
            {
                module.OnLoad();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (FPSProtection.CheckFps())
            {
                return;
            }

            foreach (var module in Variables.moduleList.Where(module => module.GetModuleType() == ModuleType.OnUpdate
    && module.ShouldGetExecuted()))
            {
                module.OnExecute();
            }

            Logic.Mechanics.Insec();
            Logic.Mechanics.RotE();

            //Reset the positions
            if (MenuManager.DrawAutoPos)
            {
                Variables.TumblePosition = TumbleProvider.AkaQPosition();
            }

            Logic.AJSPositioner.Execute();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Features.Modes.Harass.HarassCombo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) Features.Modes.JungleClear.Load();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) Features.Modes.Flee.Load();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Features.Modes.Combo.Load();
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            Features.Modes.LaneClear.SpellCast(sender, args);
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            Variables.IsAfterAttack = true;
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            Variables.IsBeforeAttack = true;
        }
    }
}
