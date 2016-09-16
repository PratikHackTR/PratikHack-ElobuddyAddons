using System;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aka_s_Yasuo.Manager
{
    class MenuManager
    {
        private static Menu VMenu,
                            Hotkeymenu,
                            ComboMenu,
                            HarassMenu,
                            LaneClearMenu,
                            FleeMenu,
                            MiscMenu,
                            DrawingMenu;

        public static void Load()
        {
            Mainmenu();
            Hotkeys();
            Combomenu();
            Harassmenu();
            Fleemenu();
            LaneClearmenu();
            Miscmenu();
            Drawingmenu();
        }

        private static void Mainmenu()
        {
            VMenu = MainMenu.AddMenu("WinGame-Yasuo", "akayas");
            VMenu.AddGroupLabel("Unutma Kul Hakkına giriyorsun.");
            VMenu.AddGroupLabel("Scriptteki Hataları ve oyun crashları olursa: bana ");
            VMenu.AddGroupLabel("www.facebook.com/PratikHackOfficial/  sayfamdan atınız.");
            VMenu.AddGroupLabel("PratikHack-");
        }

        private static void Hotkeys()
        {
            Hotkeymenu = VMenu.AddSubMenu("Tuslar", "Hotkeys");
            Hotkeymenu.Add("autoq", new KeyBind("Otomatik Q Stack kas", true, KeyBind.BindTypes.PressToggle, 'T'));
            Hotkeymenu.Add("eqflash", new KeyBind("WinGameData", false, KeyBind.BindTypes.HoldActive, 'Y'));
        }

        private static void Combomenu()
        {
            ComboMenu = VMenu.AddSubMenu("WinGame-Kombo", "Combo");
            ComboMenu.AddGroupLabel("Kombo");
            ComboMenu.Add("UseQ", new CheckBox("Q kullan"));
            ComboMenu.Add("UseI", new CheckBox("Tutustur kullan"));
            ComboMenu.AddGroupLabel("Zeki Kombo ayarları");
            ComboMenu.AddLabel("Not: Evade kullanmıyorsanız bunları açın.");
            ComboMenu.Add("SUseW", new CheckBox("W kullan", false));
            ComboMenu.Add("SUseE", new CheckBox("E kullan", false));
            ComboMenu.AddGroupLabel("E ayarları");
            ComboMenu.Add("UseE", new CheckBox("E kullan"));
            ComboMenu.Add("UseETower", new CheckBox("Kule Altı", false));
            ComboMenu.Add("UseEStack", new CheckBox("Q Stackla", false));
            ComboMenu.AddGroupLabel("R ayarları");
            ComboMenu.Add("UseR", new CheckBox("R kullan"));
            ComboMenu.Add("UseRDelay", new CheckBox("Gecikmeli R at."));
            ComboMenu.Add("UseRHP", new Slider("hedeflerin HP miktarı <", 60));
            ComboMenu.Add("UseRif", new Slider("Kaç hedefe Atılsın >=", 2, 1, 5));
            ComboMenu.AddGroupLabel("WinGame-Data");
            ComboMenu.Add("AkaDataC", new CheckBox("Komboda Kullan"));
            ComboMenu.Add("AkaDataMHp", new Slider("Benim Can değerim <=", 10));
            ComboMenu.Add("AkaDataEHp", new Slider("Rakibin Can değeri <=", 20));
        }

        private static void Harassmenu()
        {
            HarassMenu = VMenu.AddSubMenu("Dürtme", "Harass");
            HarassMenu.AddGroupLabel("Dürtme");
            HarassMenu.Add("UseQ", new CheckBox("Q kullan"));
            HarassMenu.Add("UseQ3", new CheckBox("Hortum kullan"));
            HarassMenu.Add("UseQLH", new CheckBox("Q ile sonvurus"));
            HarassMenu.AddGroupLabel("Otomatik dürtme");
            HarassMenu.Add("UseQAuto", new CheckBox("Otomatik Q "));
            HarassMenu.Add("UseQ3Auto", new CheckBox("Otomatik Hortum"));
        }

        private static void LaneClearmenu()
        {
            LaneClearMenu = VMenu.AddSubMenu("Temizleme", "clear");
            LaneClearMenu.AddGroupLabel("Q ayarları");
            LaneClearMenu.Add("UseQ", new CheckBox("Q kullan"));
            LaneClearMenu.Add("UseQ3", new CheckBox("Hortum Kullan"));
            LaneClearMenu.AddGroupLabel("E ayarları");
            LaneClearMenu.Add("UseE", new CheckBox("E kullan"));
            LaneClearMenu.Add("UseELH", new CheckBox("Sadece SonVurus", false));
            LaneClearMenu.Add("UseETower", new CheckBox("Kule altı E at", false));
        }

        private static void Fleemenu()
        {
            FleeMenu = VMenu.AddSubMenu("Vurkaç", "Flee");
            FleeMenu.Add("UseE", new CheckBox("E kullan"));
            FleeMenu.Add("UseEStack", new CheckBox("Q stack kas"));
        }

        private static void Miscmenu()
        {
            MiscMenu = VMenu.AddSubMenu("Kill Çalma ", "Misc");
            MiscMenu.AddGroupLabel("KS");
            MiscMenu.Add("KSQ", new CheckBox("Q ile KillÇal"));
            MiscMenu.Add("KSE", new CheckBox("E ile KillÇal"));
            MiscMenu.Add("KSR", new CheckBox("R ile KillÇal"));
        }

        private static void Drawingmenu()
        {
            DrawingMenu = VMenu.AddSubMenu("Gösterge", "Drawings");
            DrawingMenu.Add("DrawQ", new CheckBox("Q Göster "));
            DrawingMenu.Add("DrawE", new CheckBox("E göster", false));
            DrawingMenu.Add("DrawR", new CheckBox("R göster", false));
            DrawingMenu.Add("DrawOnlyReady", new CheckBox("Skiller hazır olduğunda Gösterge açılır"));
            DrawingMenu.Add("DrawStatus", new CheckBox("Otomatik Stack göstergesi "));
        }

        #region checkvalues
        #region checkvalues:hotkeys
        public static bool AutoStackQ
        {
            get { return (Hotkeymenu["autoq"].Cast<KeyBind>().CurrentValue); }
        }
        public static bool FlashEQ
        {
            get { return (Hotkeymenu["eqflash"].Cast<KeyBind>().CurrentValue); }
        }
        #endregion
        #region checkvalues:combo
        public static bool UseQC
        {
            get { return (ComboMenu["UseQ"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseEC
        {
            get { return (ComboMenu["UseE"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseECTower
        {
            get { return (ComboMenu["UseETower"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseECStack
        {
            get { return (ComboMenu["UseEStack"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseRC
        {
            get { return (ComboMenu["UseR"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseRCDelay
        {
            get { return (ComboMenu["UseRDelay"].Cast<CheckBox>().CurrentValue); }
        }
        public static int UseRCHP
        {
            get { return (ComboMenu["UseRHP"].Cast<Slider>().CurrentValue); }
        }
        public static int UseRCEnemies
        {
            get { return (ComboMenu["UseRif"].Cast<Slider>().CurrentValue); }
        }
        public static bool UseIgnite
        {
            get { return (ComboMenu["UseI"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool SmartW
        {
            get { return (ComboMenu["SUseW"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool SmartE
        {
            get { return (ComboMenu["SUseE"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool AkaData
        {
            get { return (ComboMenu["AkaDataC"].Cast<CheckBox>().CurrentValue); }
        }
        public static int AkaDataEnemy
        {
            get { return (ComboMenu["AkaDataEHp"].Cast<Slider>().CurrentValue); }
        }
        public static int AkaDatamy
        {
            get { return (ComboMenu["AkaDataMHp"].Cast<Slider>().CurrentValue); }
        }
        #endregion
        #region checkvalues:harass
        public static bool UseQH
        {
            get { return (HarassMenu["UseQ"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseQ3H
        {
            get { return (HarassMenu["UseQ3"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseQHLH
        {
            get { return (HarassMenu["UseQLH"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseQHAuto
        {
            get { return (HarassMenu["UseQAuto"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseQ3HAuto
        {
            get { return (HarassMenu["UseQ3Auto"].Cast<CheckBox>().CurrentValue); }
        }
        #endregion
        #region checkvalues:laneclear
        public static bool UseQLC
        {
            get { return (LaneClearMenu["UseQ"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseQ3LC
        {
            get { return (LaneClearMenu["UseQ3"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseELC
        {
            get { return (LaneClearMenu["UseE"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseELCLH
        {
            get { return (LaneClearMenu["UseELH"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseELCTower
        {
            get { return (LaneClearMenu["UseETower"].Cast<CheckBox>().CurrentValue); }
        }
        #endregion
        #region checkvalues:flee
        public static bool UseEFlee
        {
            get { return (FleeMenu["UseE"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseEStackFlee
        {
            get { return (FleeMenu["UseEStack"].Cast<CheckBox>().CurrentValue); }
        }
        #endregion
        #region checkvalues:misc
        public static bool KSQ
        {
            get { return (MiscMenu["KSQ"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool KSE
        {
            get { return (MiscMenu["KSE"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool KSR
        {
            get { return (MiscMenu["KSR"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool Skinhack
        {
            get { return (MiscMenu["Skinhack"].Cast<CheckBox>().CurrentValue); }
        }
        public static int SkinID
        {
            get { return (MiscMenu["SkinId"].Cast<ComboBox>().CurrentValue); }
        }
        public static bool Autolvl
        {
            get { return (MiscMenu["Autolvl"].Cast<CheckBox>().CurrentValue); }
        }
        public static int AutolvlSlider
        {
            get { return (MiscMenu["AutolvlS"].Cast<ComboBox>().CurrentValue); }
        }
        #endregion
        #region checkvalues:draw
        public static bool DrawQ
        {
            get { return (DrawingMenu["DrawQ"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool DrawE
        {
            get { return (DrawingMenu["DrawE"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool DrawR
        {
            get { return (DrawingMenu["DrawR"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool DrawOnlyRdy
        {
            get { return (DrawingMenu["DrawOnlyReady"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool DrawStatus
        {
            get { return (DrawingMenu["DrawStatus"].Cast<CheckBox>().CurrentValue); }
        }
        #endregion
        #endregion
    }
}
