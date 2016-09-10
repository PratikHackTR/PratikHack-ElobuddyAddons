﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using GuTenTak.Lucian;
using SharpDX;
using EloBuddy.SDK.Constants;

namespace GuTenTak.Lucian
{
    internal class Program
    {
        public const string ChampionName = "Lucian";
        public static Menu Menu, ModesMenu1, ModesMenu2, ModesMenu3, DrawMenu;
        public static int SkinBase;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);
        public static Item hextech = new Item(ItemId.Hextech_Gunblade, 700);
        public static AIHeroClient lastTarget;
        public static float lastSeen = Game.Time;
        public static float RCast = 0;
        public static Vector3 predictedPos;
        public static AIHeroClient RTarget = null;
        public static Vector3 RCastToPosition = new Vector3();
        public static Vector3 MyRCastPosition = new Vector3();
        public static bool disableMovement = false;
        public static bool PassiveUp;


        public static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }
        private static float HealthPercent()
        {
            return (PlayerInstance.Health / PlayerInstance.MaxHealth) * 100;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool AutoQ { get; protected set; }
        public static float Manaah { get; protected set; }
        public static object GameEvent { get; private set; }

        public static Spell.Targeted Q;
        public static Spell.Skillshot Q1;
        public static Spell.Skillshot W;
        public static Spell.Skillshot W1;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
        }


        static void Game_OnStart(EventArgs args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Gapcloser.OnGapcloser += Common.Gapcloser_OnGapCloser;
            Obj_AI_Base.OnBuffGain += Common.OnBuffGain;
            Game.OnTick += OnTick;
            Orbwalker.OnPostAttack += Common.aaCombo;
            Orbwalker.OnPostAttack += Common.LJClear;
            Player.OnBasicAttack += Player_OnBasicAttack;
            SkinBase = Player.Instance.SkinId;
            // Item
            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Q = new Spell.Targeted(SpellSlot.Q, 675);
                Q1 = new Spell.Skillshot(SpellSlot.Q, 940, SkillShotType.Linear, 350, int.MaxValue, 75);
                W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Linear, 250, 1600, 100);
                W1 = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Linear, 250, 1600, 100);
                E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
                R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear, 500, 2800, 110);



                Bootstrap.Init(null);
                Chat.Print("Hadi gene iyisin, Script yüklendi", Color.Green);


                Menu = MainMenu.AddMenu("EzGame-Lucian", "Lucian");
                Menu.AddSeparator();
                Menu.AddLabel("PratikHack");
                Menu.AddLabel("Bu arada Gapslocer vb. şeyleri niye ingilizce dilde demeyin. onun Türkçesi yok googlede araştırın mekanik oldğunu anlarsınız.");

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("Menü", "Modes1Lucian");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Kombo ayarları");
                ModesMenu1.Add("CWeaving", new CheckBox("Komboda Pasifi Aktifleştir", true));
                ModesMenu1.Add("ComboQ", new CheckBox("Komboda Q kullan", true));
                ModesMenu1.Add("ComboW", new CheckBox("Komboda W kullan", true));
                ModesMenu1.Add("ComboE", new CheckBox("Komboda E kullan", true));
                ModesMenu1.Add("ManaCW", new Slider("W için mana sınırı %", 30));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Mantık ayarları");
                ModesMenu1.Add("LogicAA", new ComboBox(" Kombo Mantığı ", 1, "Hızlı", "Kite attığın hedefe Full Hasar"));
                ModesMenu1.Add("LogicW", new ComboBox(" Temel W mantıgı ", 1, "Kite attığın hedefe", "Sürekli"));
                ModesMenu1.Add("WColision", new ComboBox(" Çarpışma ayarları W ", 1, "Çarpışma", "Çarpışma yok"));
                ModesMenu1.Add("LogicE", new ComboBox(" E Mantığı ", 0, "Mouse'yi izler güvenliyse atlar", "Yanlara doğru", "Mouse yönünde Atlar"));

                ModesMenu1.AddSeparator();
                //ModesMenu1.AddLabel("AutoHarass Configs");
                //ModesMenu1.Add("AutoHarass", new CheckBox("Use Q on AutoHarass", false));
               // ModesMenu1.Add("ManaAuto", new Slider("Mana %", 80));

                ModesMenu1.AddLabel("Dürtme Ayarları");
                ModesMenu1.Add("HWeaving", new CheckBox("Dürtme ayarları Pasif kullan", true));
                ModesMenu1.Add("HarassMana", new Slider("Dürterken Mana dikkati %", 60));
                ModesMenu1.Add("HarassQ", new CheckBox("Dürterken Q kullan", true));
                ModesMenu1.Add("HarassQext", new CheckBox("Genişletilmiş mekanik Q kullan", true));
                ModesMenu1.Add("HarassW", new CheckBox("Dürterken W kullan", true));
                ModesMenu1.Add("ManaHW", new Slider("W kullanmak için max. mana sınırı %", 60));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Kill Çalma");
                ModesMenu1.Add("KS", new CheckBox("Kill Çalma Aktif", true));
                ModesMenu1.Add("KQ", new CheckBox("Kill çalmak için Q kullan", true));
                ModesMenu1.Add("KW", new CheckBox("Kill çalmak için W kullan", true));
                ModesMenu1.Add("KR", new CheckBox("Kill çalmak için R kullan", false));

                ModesMenu2 = Menu.AddSubMenu("Farm", "Modes2Lucian");
                ModesMenu2.AddLabel("Koridor Temizle");
                ModesMenu1.AddSeparator();
                ModesMenu2.Add("FarmQ", new CheckBox("Koridor Temizlerken Q kullan", true));
                ModesMenu2.Add("ManaLQ", new Slider("Mana %", 40));
                ModesMenu2.Add("FarmW", new CheckBox("Koridor Temizlerken W kullan", true));
                ModesMenu2.Add("ManaLW", new Slider("Mana %", 40));
                ModesMenu2.AddLabel("Orman temizleme Ayarları");
                ModesMenu2.Add("JungleQ", new CheckBox("Orman temizlerken Q kullan", true));
                ModesMenu2.Add("ManaJQ", new Slider("Mana %", 40));
                ModesMenu2.Add("JungleW", new CheckBox("Orman temizlerken W kullan", true));
                ModesMenu2.Add("ManaJW", new Slider("Mana %", 40));

                ModesMenu3 = Menu.AddSubMenu("Cesitli", "Modes3Lucian");
                ModesMenu3.Add("AntiGap", new CheckBox("E ile Anti-Gapslocer.", true));
                ModesMenu3.AddLabel("Vurkaç");
                ModesMenu3.Add("FleeE", new CheckBox("Vurkaç yaparken E kombosu at", true));

                ModesMenu3.AddLabel("İtem Kullanım ayarları");
                ModesMenu3.Add("useYoumuu", new CheckBox("Yoummu hayalet kılıç kullan", true));
                ModesMenu3.Add("usehextech", new CheckBox("Hextech birim tehcizat kullan", true));
                ModesMenu3.Add("useBotrk", new CheckBox("Mahvolmuş kralın kılıcı kullan", true));
                ModesMenu3.Add("useQss", new CheckBox("CıvaYatağan kullan", true));
                ModesMenu3.Add("minHPBotrk", new Slider("Mahvolmuş kralın kılıcı kaç can yüzdesinde kullanılsın %", 80));
                ModesMenu3.Add("enemyMinHPBotrk", new Slider("Mahvolmuş kralın kılıcı kaç can yüzdesinde kullanılsın  %", 80));

                ModesMenu3.AddLabel("Cıva Yatağan Ayarları");
                ModesMenu3.Add("Qssmode", new ComboBox(" ", 0, "Otomatik", "Komboda"));
                ModesMenu3.Add("Stun", new CheckBox("Stun", true));
                ModesMenu3.Add("Blind", new CheckBox("Kör", true));
                ModesMenu3.Add("Charm", new CheckBox("Cazibe", true));
                ModesMenu3.Add("Suppression", new CheckBox("Baskı", true));
                ModesMenu3.Add("Polymorph", new CheckBox("Polimörf", true));
                ModesMenu3.Add("Fear", new CheckBox("Korku", true));
                ModesMenu3.Add("Taunt", new CheckBox("Kışkırtma", true));
                ModesMenu3.Add("Silence", new CheckBox("Susturma", false));
                ModesMenu3.Add("QssDelay", new Slider("Cıva yatağan gecikme ayarı (ms)", 250, 0, 1000));

                ModesMenu3.AddLabel("CıvaYatağan Extra ayarlar");
                ModesMenu3.Add("ZedUlt", new CheckBox("Zed R", true));
                ModesMenu3.Add("VladUlt", new CheckBox("Vladimir R", true));
                ModesMenu3.Add("FizzUlt", new CheckBox("Fizz R", true));
                ModesMenu3.Add("MordUlt", new CheckBox("Mordekaiser R", true));
                ModesMenu3.Add("PoppyUlt", new CheckBox("Poppy R", true));
                ModesMenu3.Add("QssUltDelay", new Slider("Gecikme ayarı (ms) olarak", 250, 0, 1000));

                ModesMenu3.AddLabel("KOSTÜM HİLESİ");
                ModesMenu3.Add("skinhack", new CheckBox("Skin Hack Aktifleştir", false));
                ModesMenu3.Add("skinId", new ComboBox("Skin numarası", 0, "Klasik", "1", "2", "3", "Kod Adı Lucian", "5", "6", "7", "8"));

                DrawMenu = Menu.AddSubMenu("Sınır", "DrawLucian");
                DrawMenu.Add("drawA", new CheckBox(" Düz vurus Alanı", true));
                DrawMenu.Add("drawQ", new CheckBox(" Q alanı", true));
                DrawMenu.Add("drawQext", new CheckBox(" Genişletilmiş Q alanı", true));
                DrawMenu.Add("drawW", new CheckBox(" W alanı", true));
                DrawMenu.Add("drawE", new CheckBox(" E alanı", true));
                DrawMenu.Add("drawR", new CheckBox(" R alanı", false));
                
                if (ModesMenu3["skinhack"].Cast<CheckBox>().CurrentValue)
                    Player.SetSkinId(ModesMenu3["skinId"].Cast<ComboBox>().CurrentValue);
                ModesMenu3["skinId"].Cast<ComboBox>().OnValueChange += (sender, vargs) =>
                {
                    if (ModesMenu3["skinhack"].Cast<CheckBox>().CurrentValue)
                        Player.SetSkinId(vargs.NewValue);
                };
                ModesMenu3["skinhack"].Cast<CheckBox>().OnValueChange += (sender, vargs) =>
                {
                    if (vargs.NewValue)
                        Player.SetSkinId(ModesMenu3["skinId"].Cast<ComboBox>().CurrentValue);
                    else
                        Player.SetSkinId(0);
                };
            }

            catch (Exception e)
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
                if (DrawMenu["drawQext"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady() && Q.IsLearned)
                    {
                        Circle.Draw(Color.White, Q1.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.IsReady() && W.IsLearned)
                    {
                        Circle.Draw(Color.White, W.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                {
                    if (E.IsReady() && E.IsLearned)
                    {
                        Circle.Draw(Color.White, E.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady() && R.IsLearned)
                    {
                        Circle.Draw(Color.White, R.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawA"].Cast<CheckBox>().CurrentValue)
                {
                    Circle.Draw(Color.LightGreen, 560, Player.Instance.Position);
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
                //var AutoHarass = ModesMenu1["AutoHarass"].Cast<CheckBox>().CurrentValue;
                //var ManaAuto = ModesMenu1["ManaAuto"].Cast<Slider>().CurrentValue;
                Common.KillSteal();

                /*
                if (AutoHarass && ManaAuto <= _Player.ManaPercent)
                    {
                        Common.AutoQ();
                    }*/
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

                    //Common.LaneClear();

                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {

                    //Common.JungleClear();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    //Common.LastHit();

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

        public static void OnTick(EventArgs args)
        {
            //Common.Skinhack();
            if (lastTarget != null)
            {
                if (lastTarget.IsVisible)
                {
                    predictedPos = Prediction.Position.PredictUnitPosition(lastTarget, 300).To3D();
                    lastSeen = Game.Time;
                }
                if (lastTarget.Distance(Player.Instance) > 700)
                {
                    lastTarget = null;
                }
            }
        }

        private static void Player_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != Player.Instance)
                return;
            if (args.Target is AIHeroClient)
                lastTarget = (AIHeroClient)args.Target;
            else
                lastTarget = null;
        }

        public static void OnCastSpell(GameObject sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsDead || !sender.IsMe) return;
            if (args.SData.IsAutoAttack())
            {
                PassiveUp = false;
            }
            switch (args.Slot)
            {
                case SpellSlot.Q:
                case SpellSlot.W:
                    Orbwalker.ResetAutoAttack();
                    break;
            }
        }

        public static void OnProcessSpellCast(GameObject sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsDead || !sender.IsMe) return;
            {
                switch (args.Slot)
                {
                    case SpellSlot.Q:
                    case SpellSlot.W:
                    case SpellSlot.R:
                        PassiveUp = true;
                        break;
                    case SpellSlot.E:
                        PassiveUp = true;
                        Orbwalker.ResetAutoAttack();
                        break;
                }
            }
        }

    }
}