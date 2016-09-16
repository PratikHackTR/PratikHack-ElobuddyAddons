using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace AkaCore.Features.Utility.Modules
{
    class Humanizer : IModule
    {
        public static Dictionary<SpellSlot, int> LastCast = new Dictionary<SpellSlot, int>();

        public static int TimeSince(int time)
        {
            return Environment.TickCount - time;
        }

        public void OnLoad()
        {
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (ShouldGetExecuted())
            {
                var senderValid = sender != null && sender.Owner != null && sender.Owner.IsMe;
                var spell = args.Slot;
                if (!senderValid) return;

                var qdelay = Manager.MenuManager.HumanizeQ;
                var wdelay = Manager.MenuManager.HumanizeW;
                var edelay = Manager.MenuManager.HumanizeE;
                var rdelay = Manager.MenuManager.HumanizeR;

                if (spell == SpellSlot.Q && TimeSince(LastCast[SpellSlot.Q]) < qdelay)
                {
                    args.Process = false;
                    return;
                }
                if (spell == SpellSlot.W && TimeSince(LastCast[SpellSlot.W]) < wdelay)
                {
                    args.Process = false;
                    return;
                }
                if (spell == SpellSlot.E && TimeSince(LastCast[SpellSlot.E]) < edelay)
                {
                    args.Process = false;
                    return;
                }
                if (spell == SpellSlot.R && TimeSince(LastCast[SpellSlot.R]) < rdelay)
                {
                    args.Process = false;
                    return;
                }

                LastCast[spell] = Environment.TickCount;
            }
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.MenuManager.EnableH;
        }

        public void OnExecute()
        {
        }
    }
}
