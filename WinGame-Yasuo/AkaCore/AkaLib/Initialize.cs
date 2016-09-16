using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaCore.AkaLib.Evade;

namespace AkaCore.AkaLib
{
    class Initialize
    {
        public static void Execute()
        {
            Item.Load();
            Orbwalk.Load();
            //SkillshotDetector.Load();
        }
    }
}
