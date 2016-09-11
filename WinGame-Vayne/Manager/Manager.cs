using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aka_s_Vayne.Features.Utility;

namespace Aka_s_Vayne.Manager
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
            //StealthManager
            StealthManager.Load();
            //Less Fps drops i hope
            drawing.Load();
        }
    }
}
