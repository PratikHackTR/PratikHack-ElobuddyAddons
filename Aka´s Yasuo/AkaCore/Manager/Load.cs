using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkaCore.Manager
{
    class Load
    {
        public static void Execute()
        {
            //MenuManager
            MenuManager.Load();
            //AkaLib *_*
            AkaLib.Initialize.Execute();
            //EventManager
            EventManager.Load();
            //EvadeManager
            //EvadeManager.Load();
        }
    }
}
