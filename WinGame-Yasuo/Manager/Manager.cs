using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aka_s_Yasuo.Manager
{
    class Manager
    {
        public static void Load()
        {
            //SpellManager
            SpellManager.Load();
            //MenuManager
            MenuManager.Load();
            //EventManager
            EventManager.Load();
            //Less Fps drops i hope
            Features.Modes.drawing.Load();
        }
    }
}
