using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using GuTenTak.KogMaw;
using SharpDX;

namespace GuTenTak.KogMaw
{
    internal class Program
    {
        public const string ChampionName = "KogMaw";
        public static Menu Menu, ModesMenu1, ModesMenu2, ModesMenu3, DrawMenu;
        public static int SkinBase;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);
        public static Item hextech = new Item(ItemId.Hextech_Gunblade, 700);
        //private static bool IsZombie;
        //private static bool wActive;
        //private static int LastAATick;

        //public static AIHeroClient PlayerInstance { get { return Player.Instance; } }
        private static float HealthPercent() { return (Player.Instance.Health / Player.Instance.MaxHealth) * 100; }
        
        //public static bool AutoQ { get; protected set; }
        //public static float Manaah { get; protected set; }
        //public static object GameEvent { get; private set; }

        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        //private static bool siegecount;

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }



        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != ChampionName)
            {
                return;
            }
            //IsZombie = PlayerInstance.HasBuff("kogmawicathiansurprise");
            //wActive = PlayerInstance.HasBuff("kogmawbioarcanebarrage");
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Obj_AI_Base.OnBuffGain += Common.OnBuffGain;
            Game.OnTick += (EventArgs) => Common.Skinhack();
            Gapcloser.OnGapcloser += Common.Gapcloser_OnGapCloser;
            Game.OnUpdate += Common.zigzag;
            Obj_AI_Base.OnLevelUp += OnLevelUp;
            SkinBase = Player.Instance.SkinId;
            // Item
            try
            {
                Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1650, 70);
                Q.AllowedCollisionCount = 0;
                W = new Spell.Active(SpellSlot.W, 750);
                E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Linear, 500, 1400, 120);
                E.AllowedCollisionCount = int.MaxValue;
                R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Circular, 1200, int.MaxValue, 120);
                R.AllowedCollisionCount = int.MaxValue;



                Bootstrap.Init(null);
                Chat.Print("Script yüklendi hadi gene iyisin", Color.Green);


                Menu = MainMenu.AddMenu("EzGame-KogMaw", "KogMaw");
                Menu.AddSeparator();
                Menu.AddLabel("Kusura bakmayın arada Crash verebilir.. hatayı  V2'de çözücem.");
                Menu.AddLabel("PratikHack");

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("Menu", "Modes1KogMaw");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Kombo ayarları");
                ModesMenu1.Add("ComboQ", new CheckBox("Komboda Q kullan", true));
                ModesMenu1.AddLabel("kullanma manası Q  >= 80");
                ModesMenu1.Add("ComboW", new CheckBox("Komboda W kullan", true));
                ModesMenu1.Add("ComboE", new CheckBox("Komboda E kullan", true));
                ModesMenu1.Add("ComboR", new CheckBox("Komboda R kullan", true));
                ModesMenu1.Add("LogicRn", new ComboBox(" hedefin can miktari R için % <= ", 1, "100%", "50%", "25%"));
                ModesMenu1.Add("ManaCE", new Slider("E kullanma manası %", 30));
                ModesMenu1.Add("ManaCR", new Slider("R kullanm manası %", 80));
                ModesMenu1.Add("CRStack", new Slider("R stack limiti ?", 3, 1, 10));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Oto Dürtme");
                ModesMenu1.Add("AutoHarass", new CheckBox("R ile oto dürtmeyi aç", false));
                ModesMenu1.Add("ARStack", new Slider("Otomatik R stack'ı limiti", 2, 1, 6));
                ModesMenu1.Add("ManaAuto", new Slider("Mana %", 70));

                ModesMenu1.AddLabel("Dürtme");
                ModesMenu1.Add("HarassQ", new CheckBox("Dürterken Q kullan", true));
                ModesMenu1.Add("HarassE", new CheckBox("Dürterken E kullan", true));
                ModesMenu1.Add("HarassR", new CheckBox("Dürterken R kullan", true));
                ModesMenu1.Add("ManaHE", new Slider(" E Mana %", 60));
                ModesMenu1.Add("ManaHR", new Slider(" R Mana %", 60));
                ModesMenu1.Add("HRStack", new Slider("Dürterken atılan R stack'i", 1, 1, 6));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("KS ayarları");
                ModesMenu1.AddLabel("Aslında Kil çalma türkçesi fakat Türkçeyi sikleyen yok malesef..");
                ModesMenu1.Add("KS", new CheckBox("KS at", true));
                ModesMenu1.Add("KQ", new CheckBox("KS atmak için  Q at", true));
                ModesMenu1.Add("KR", new CheckBox("KS atmak için R at", true));

                ModesMenu2 = Menu.AddSubMenu("Farm", "Modes2KogMaw");
                ModesMenu2.AddLabel("Lane temizleme");
                ModesMenu2.Add("ManaL", new Slider("Mana %", 40));
                ModesMenu2.Add("FarmQ", new CheckBox("Lane temizlerken Q kullan", true));
                ModesMenu2.Add("ManaLR", new Slider("Mana %", 40));
                ModesMenu2.Add("FarmR", new CheckBox("R kullan", true));
                ModesMenu2.Add("FRStack", new Slider("R Stack limiti", 1, 1, 6));
                ModesMenu2.AddLabel("Orman temizleme Limiti");
                ModesMenu2.Add("ManaJ", new Slider("Mana %", 40));
                ModesMenu2.Add("JungleQ", new CheckBox("Q kullan", true));
                ModesMenu2.Add("ManaJR", new Slider("Mana %", 40));
                ModesMenu2.Add("JungleR", new CheckBox("R kullan", true));
                ModesMenu2.Add("JRStack", new Slider("R Stack'i", 2, 1, 6));

                ModesMenu3 = Menu.AddSubMenu("Çeşitli", "Modes3KogMaw");
                ModesMenu3.AddLabel("Vurkaç");
                ModesMenu3.Add("FleeR", new CheckBox("R kullan", true));
                ModesMenu3.Add("FleeE", new CheckBox("E kullan", true));
                ModesMenu3.Add("ManaFlR", new Slider("R kullan %", 35));
                ModesMenu3.Add("FlRStack", new Slider("R Stack durumu", 2, 1, 6));

                ModesMenu3.AddLabel("Komboda İtem Kullan");
                ModesMenu3.Add("useYoumuu", new CheckBox("Yoummu", true));
                ModesMenu3.Add("usehextech", new CheckBox("Hextech birim", true));
                ModesMenu3.Add("useBotrk", new CheckBox("Mahvolmuş kral", true));
                ModesMenu3.Add("useQss", new CheckBox("CıvaYatağan", true));
                ModesMenu3.Add("minHPBotrk", new Slider("BotRk basmak için rakip can yüzdesi %", 80));
                ModesMenu3.Add("enemyMinHPBotrk", new Slider("MMF:S %", 80));

                ModesMenu3.AddLabel("QSS ayarları");
                ModesMenu3.Add("Qssmode", new ComboBox(" ", 0, "Otomatik", "Kombo"));
                ModesMenu3.Add("Stun", new CheckBox("Sersemletme", true));
                ModesMenu3.Add("Blind", new CheckBox("Çekme", true));
                ModesMenu3.Add("Charm", new CheckBox("Cazibe", true));
                ModesMenu3.Add("Suppression", new CheckBox("Kitle kontrol", true));
                ModesMenu3.Add("Polymorph", new CheckBox("Polimörf", true));
                ModesMenu3.Add("Fear", new CheckBox("Kışkırtma", true));
                ModesMenu3.Add("Taunt", new CheckBox("FŞB", true));
                ModesMenu3.Add("Silence", new CheckBox("Susturma", false));
                ModesMenu3.Add("QssDelay", new Slider("Kullanım gecikmesi", 250, 0, 1000));

                ModesMenu3.AddLabel("Cıva Yatağan Kullanımı ++");
                ModesMenu3.Add("ZedUlt", new CheckBox("Zed R", true));
                ModesMenu3.Add("VladUlt", new CheckBox("Vladimir R", true));
                ModesMenu3.Add("FizzUlt", new CheckBox("Fizz R", true));
                ModesMenu3.Add("MordUlt", new CheckBox("Mordekaiser R", true));
                ModesMenu3.Add("PoppyUlt", new CheckBox("Poppy R", true));
                ModesMenu3.Add("QssUltDelay", new Slider("Ultiye göre Kullanım Gecikmesi MS", 250, 0, 1000));

                ModesMenu3.AddLabel("Skin Hack");
                ModesMenu3.Add("skinhack", new CheckBox("Skin Hack Aktifleştirmek için Tıklat", false));
                ModesMenu3.Add("skinId", new ComboBox("Skin numarası", 0, "Klasik", "1", "2", "3", "4", "5", "6", "7", "8"));

                DrawMenu = Menu.AddSubMenu("Çizgiler", "DrawKogMaw");
                DrawMenu.Add("drawQ", new CheckBox(" Q çizgileri", true));
                DrawMenu.Add("drawW", new CheckBox(" W çizgileri", true));
                DrawMenu.Add("drawR", new CheckBox(" R çizgileri", false));
                DrawMenu.Add("drawXR", new CheckBox(" Çizgi göstertme R", true));
                DrawMenu.Add("drawXFleeQ", new CheckBox(" vurkaç'ta Q gösterme", false));

            }

            catch (Exception)
            {

            }

        }
        private static void Game_OnDraw(EventArgs args)
        {

            try
            {
                if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady() && Q.IsLearned)
                    {
                        Circle.Draw(Color.White, Q.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.IsReady() && W.IsLearned)
                    {
                        Circle.Draw(Color.White, W.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady() && R.IsLearned)
                    {
                        Circle.Draw(Color.White, R.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawXR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady() && R.IsLearned)
                    {
                        Circle.Draw(Color.Red, 700, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawXFleeQ"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady() && Q.IsLearned)
                    {
                        Circle.Draw(Color.Red, 400, Player.Instance.Position);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        static void Game_OnUpdate(EventArgs args)
        {
            try
            {
                var AutoHarass = ModesMenu1["AutoHarass"].Cast<CheckBox>().CurrentValue;
                var ManaAuto = ModesMenu1["ManaAuto"].Cast<Slider>().CurrentValue;
                Common.KillSteal();

                if (AutoHarass && ManaAuto <= ObjectManager.Player.ManaPercent)
                    {
                        Common.AutoR();
                    }
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        Common.Combo();
                        Common.ItemUsage();
                    }
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                    {
                        Common.Harass();
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    {

                        Common.LaneClear();

                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {

                        Common.JungleClear();
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                    {
                        Common.LastHit();

                    }
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                    {
                        Common.Flee();

                    }
            }
            catch (Exception e)
            {

            }
        }

        public static void OnLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (!sender.IsMe || args.Level != 1) return;
            Game.OnTick += SetSkillshot;
        }

        public static void SetSkillshot(EventArgs args)
        {
            if (Q.Level + W.Level + E.Level + R.Level == Player.Instance.Level)
            {
                W = new Spell.Active(SpellSlot.W, (uint)(565 + 60 + W.Level * 20 + 105));
                R = new Spell.Skillshot(SpellSlot.R, (uint)(900 + R.Level * 300), SkillShotType.Circular, 1500, int.MaxValue, 225);
                Game.OnTick -= SetSkillshot; //improve fps
            }
        }

    }
}
