using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Aka_s_Vayne.Manager
{
    class SpellManager
    {
        //Spells
        public static Spell.Active Q;
        public static Spell.Skillshot Q2;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Skillshot E2;
        public static Spell.Active R;
        //Items
        public static Item totem, zzrot;

        private static void SpellsItems()
        {
            Q = new Spell.Active(SpellSlot.Q, 300);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 300, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Targeted(SpellSlot.E, (uint)(650 + Variables._Player.BoundingRadius));
            E2 = new Spell.Skillshot(
                SpellSlot.E,
                (uint)(650 + Variables._Player.BoundingRadius),
                SkillShotType.Linear,
                250,
                1200);
            R = new Spell.Active(SpellSlot.R);

            totem = new Item((int)ItemId.Warding_Totem_Trinket);
            zzrot = new Item(ItemId.ZzRot_Portal, 400);
        }

        public static void Load()
        {
            SpellsItems();
        }
    }
}
