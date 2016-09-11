using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Aka_s_Vayne.Manager;

namespace Aka_s_Vayne.Logic
{
    class AJSPositioner
    {
        public static void Execute()
        {
            if (Manager.MenuManager.AutoPos && Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
                Orbwalker.OrbwalkTo(Variables.TumblePosition);
            }
        }
    }
}