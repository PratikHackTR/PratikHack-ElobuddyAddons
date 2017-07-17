using System;
using EloBuddy.SDK;
using EloBuddy;

namespace Aka_s_Yasuo.Manager
{
    class DamageManager
    {
        public static double GetQDmg(Obj_AI_Base target)
        {
            var dmgItem = 0d;
            if (Item.HasItem((int)ItemId.Sheen) && (Item.CanUseItem((int)ItemId.Sheen) || Player.HasBuff("Sheen")))
            {
                dmgItem = Variables._Player.BaseAttackDamage;
            }
            if (Item.HasItem((int)ItemId.Trinity_Force)
                && (Item.CanUseItem((int)ItemId.Trinity_Force) || Player.HasBuff("Sheen")))
            {
                dmgItem = Variables._Player.BaseAttackDamage * 2;
            }
            if (dmgItem > 0)
            {
                dmgItem = Variables._Player.CalculateDamageOnUnit(target, DamageType.Physical, (float)dmgItem);
            }
            double dmgQ = Variables._Player.GetSpellDamage(target, SpellSlot.Q);
            if (Math.Abs(Variables._Player.Crit - 1) < float.Epsilon)
            {
                dmgQ += Variables._Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    (float)(Item.HasItem((int)ItemId.Infinity_Edge) ? 0.875 : 0.5) * Variables._Player.TotalAttackDamage);
            }
            return dmgQ + dmgItem;
        }

        public static double GetEDmg(Obj_AI_Base target)
        {
            return Variables._Player.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)((50 + 20 * SpellManager.E.Level) * (1 + Math.Max(0, Variables._Player.GetBuffCount("YasuoDashScalar") * 0.25))
                + 0.6 * Variables._Player.FlatMagicDamageMod));
        }

        public static float GetRDmg(Obj_AI_Base target) //W8 wut float? Idc tho
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new double[] { 200, 300, 400 }[Player.GetSpell(SpellSlot.R).Level - 1]
                         + 1.5 * (Player.Instance.FlatPhysicalDamageMod)));
        }
    }
}
