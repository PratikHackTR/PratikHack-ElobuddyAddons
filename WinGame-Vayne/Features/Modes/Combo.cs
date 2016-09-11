using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using Aka_s_Vayne.Manager;
using Aka_s_Vayne.Logic;
using SharpDX;

namespace Aka_s_Vayne.Features.Modes
{
    class Combo
    {
        public static void Load()
        {
            var target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);
            if (target == null) return;
            UseQ();
            UseE();
            UseR();
        }

        public static void UseQ()
        {
            var mode2 = MenuManager.UseQif;
            var target = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(), DamageType.Physical);
            if (target != null && SpellManager.Q.IsReady())
            {
                switch (mode2)
                {
                    case 0:
                        //AfterAttack
                        if (Variables.IsAfterAttack)
                        {
                            Tumble.PreCastTumble(target);
                        }
                        break;
                    case 1:
                        //BeforeAttack
                        if (Variables.IsBeforeAttack)
                        {
                            Tumble.PreCastTumble(target);
                        }
                        break;
                    case 2:
                        //Never
                        break;
                }
            }
        }

        public static void UseE()
        {
            if (MenuManager.UseE && SpellManager.E.IsReady() && !MenuManager.AutoE) //!AutoE to save FPS
            {
                Condemn.Execute();
            }
        }

        public static void UseR()
        {
            if (Manager.MenuManager.UseR && Manager.SpellManager.R.IsReady())
            {
                if (Variables._Player.CountEnemiesInRange(1000) >= Manager.MenuManager.UseRSlider)
                {
                    Manager.SpellManager.R.Cast();
                }
            }
        }
    }
}

