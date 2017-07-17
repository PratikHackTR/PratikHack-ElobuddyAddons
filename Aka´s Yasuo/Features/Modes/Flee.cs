using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aka_s_Yasuo.Features.Modes
{
    class Flee
    {
        public static void Execute()
        {
            if (Manager.MenuManager.UseEStackFlee && Manager.SpellManager.Q.IsReady() && !Variables.haveQ3 && Variables.IsDashing && Variables.CanCastQCir && Variables.CastQCir(Variables.GetQCirObj))
            {
                return;
            }

            if (!Manager.SpellManager.E.IsReady())
            {
                return;
            }

            var obj = Variables.GetBestObjToMouse();
            if (obj != null && Manager.SpellManager.E.Cast(obj))
            {
                Variables.lastE = Environment.TickCount;
            }
        }
    }
}
