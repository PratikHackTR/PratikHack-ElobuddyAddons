using EloBuddy.SDK;
using EloBuddy;

namespace Aka_s_Vayne.Features.Module.Misc
{
    class Gapcloser : IModule
    {
        public void OnLoad()
        {
            EloBuddy.SDK.Events.Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
        }

        private static void Gapcloser_OnGapCloser(AIHeroClient sender, EloBuddy.SDK.Events.Gapcloser.GapcloserEventArgs e)
        {
            if (sender == null || sender.IsAlly) return;

            if ((e.End.Distance(Variables._Player) <= sender.GetAutoAttackRange()) && Manager.MenuManager.GapcloseE && Manager.SpellManager.E.IsReady())
            {
                Manager.SpellManager.E.Cast(sender);
            }

            if ((e.End.Distance(Variables._Player) <= sender.GetAutoAttackRange()) && Manager.MenuManager.GapcloseQ && Manager.SpellManager.Q.IsReady())
            {
                var QPos = sender.Position.Extend(Variables._Player.Position, Manager.SpellManager.Q.Range);
                Player.CastSpell(SpellSlot.Q, QPos.To3D());
            }
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.Other;
        }

        public bool ShouldGetExecuted()
        {
            return true;
        }

        public void OnExecute() { }
    }
}
