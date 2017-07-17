using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Aka_s_Yasuo.Manager
{
    class SpellManager
    {
        public static Spell.Skillshot Q, Q2, Q3, W, E2;
        public static Spell.Targeted R;
        public static Spell.Targeted E;

        public static Item Qss, Mercurial, HPPot, Biscuit, Trinity, Sheen, Hydra, Tiamat;

        private static void SpellsItems()
        {
            //Spells
            Q = new Spell.Skillshot(SpellSlot.Q, 505, SkillShotType.Linear, (int)Variables.QDelay, int.MaxValue, 20)
            {
                AllowedCollisionCount = int.MaxValue
            };
            Q2 = new Spell.Skillshot(SpellSlot.Q, 1100, Q.Type, (int)Variables.Q2Delay, 1200, 90)
            {
                AllowedCollisionCount = int.MaxValue
            };
            Q3 = new Spell.Skillshot(SpellSlot.Q, 250, SkillShotType.Circular, (int)0.001f, int.MaxValue, 220)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Skillshot(SpellSlot.W, 400, SkillShotType.Cone, (int)0.25f, int.MaxValue)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Targeted(SpellSlot.E, 475);
            E2 = new Spell.Skillshot(E.Slot, E.Range, SkillShotType.Linear, Q3.CastDelay, 1200)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Targeted(SpellSlot.R, 1200);


            //Items
            Qss = new Item((int)ItemId.Quicksilver_Sash);
            Mercurial = new Item((int)ItemId.Mercurial_Scimitar);
            HPPot = new Item(2003);
            Biscuit = new Item(2010);
            Trinity = new Item((int)ItemId.Trinity_Force);
            Sheen = new Item((int)ItemId.Sheen);
            Hydra = new Item((int)ItemId.Ravenous_Hydra);
            Tiamat = new Item((int)ItemId.Tiamat);
        }

        public static void Load()
        {
            SpellsItems();
        }
    }
}
