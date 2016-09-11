using System;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aka_s_Vayne.Manager
{
    class MenuManager
    {
        private static Menu VMenu,
            Hotkeymenu,
            Qsettings,
            ComboMenu,
            CondemnMenu,
            HarassMenu,
            FleeMenu,
            LaneClearMenu,
            JungleClearMenu,
            MiscMenu,
            DrawingMenu;

        public static void Load()
        {
            Mainmenu();
            Hotkeys();
            Combomenu();
            QSettings();
            Condemnmenu();
            Harassmenu();
            Fleemenu();
            LaneClearmenu();
            JungleClearmenu();
            Miscmenu();
            Drawingmenu();
        }

        private static void Mainmenu()
        {
            VMenu = MainMenu.AddMenu("WinGame-Vayne", "akavayne");
            VMenu.AddGroupLabel("Hak yediğini unutma ve iyi Eğlenceler :)");
            VMenu.AddGroupLabel("Made by PratikHack & AkaV. *-*");
        }

        private static void Hotkeys()
        {
            Hotkeymenu = VMenu.AddSubMenu("Tuslar", "Hotkeys");
            Hotkeymenu.Add("flashe", new KeyBind("Flash Kına!", false, KeyBind.BindTypes.HoldActive, 'Y'));
            Hotkeymenu.Add("insece", new KeyBind("Flash Insec!", false, KeyBind.BindTypes.HoldActive, 'Z'));
            Hotkeymenu.Add("rote", new KeyBind("Zz'Rot Kına!", false, KeyBind.BindTypes.HoldActive, 'N'));
            Hotkeymenu.Add("autopos", new KeyBind("Otomatik pozisyon al(beta)", false, KeyBind.BindTypes.HoldActive, 'K'));
            Hotkeymenu.Add("insecmodes", new ComboBox("Insec Modu", 0, "Hepsi", "Kuleler", "Mouse kit"));
            Hotkeymenu.Add("RnoAA", new KeyBind("Ellemeyin Stabil kalsın", false, KeyBind.BindTypes.PressToggle, 'T'));
            Hotkeymenu.Add("RnoAAif", new Slider("Stabill", 2, 0, 5));
        }

        private static void Combomenu()
        {
            ComboMenu = VMenu.AddSubMenu("Kombo", "Combo");
            ComboMenu.AddGroupLabel("Kombo");
            ComboMenu.AddGroupLabel("Q modu");
            ComboMenu.AddLabel("Zeki Mod geri!");
            var qmode = ComboMenu.Add("Qmode", new ComboBox("Q modu", 1, "Mouse", "Zeki", "Kite", "Eski", "Yeni"));
            qmode.OnValueChange += delegate
            {
                if (qmode.CurrentValue == 1)
                {
                    Qsettings["UseSafeQ"].IsVisible = true;
                    Qsettings["UseQEnemies"].IsVisible = true;
                    Qsettings["UseQSpam"].IsVisible = true;
                    ComboMenu["Qmode2"].IsVisible = true;
                    Qsettings["QNMode"].IsVisible = false;
                    Qsettings["QNEnemies"].IsVisible = false;
                    Qsettings["QNWall"].IsVisible = false;
                    Qsettings["QNTurret"].IsVisible = false;
                }
                if (qmode.CurrentValue == 3)
                {
                    ComboMenu["Qmode2"].IsVisible = false;
                    Qsettings["UseSafeQ"].IsVisible = false;
                    Qsettings["UseQEnemies"].IsVisible = false;
                    Qsettings["UseQSpam"].IsVisible = false;
                    Qsettings["QNMode"].IsVisible = false;
                    Qsettings["QNEnemies"].IsVisible = false;
                    Qsettings["QNWall"].IsVisible = false;
                    Qsettings["QNTurret"].IsVisible = false;
                }
                if (qmode.CurrentValue == 2)
                {
                    ComboMenu["Qmode2"].IsVisible = false;
                    Qsettings["UseSafeQ"].IsVisible = false;
                    Qsettings["UseQEnemies"].IsVisible = false;
                    Qsettings["UseQSpam"].IsVisible = false;
                    Qsettings["QNMode"].IsVisible = false;
                    Qsettings["QNEnemies"].IsVisible = false;
                    Qsettings["QNWall"].IsVisible = false;
                    Qsettings["QNTurret"].IsVisible = false;
                }
                if (qmode.CurrentValue == 0)
                {
                    ComboMenu["Qmode2"].IsVisible = false;
                    Qsettings["UseSafeQ"].IsVisible = false;
                    Qsettings["UseQEnemies"].IsVisible = false;
                    Qsettings["UseQSpam"].IsVisible = false;
                    Qsettings["QNMode"].IsVisible = false;
                    Qsettings["QNEnemies"].IsVisible = false;
                    Qsettings["QNWall"].IsVisible = false;
                    Qsettings["QNTurret"].IsVisible = false;
                }
                if (qmode.CurrentValue == 4)
                {
                    ComboMenu["Qmode2"].IsVisible = false;
                    Qsettings["UseSafeQ"].IsVisible = false;
                    Qsettings["UseQEnemies"].IsVisible = false;
                    Qsettings["UseQSpam"].IsVisible = false;
                    Qsettings["QNMode"].IsVisible = true;
                    Qsettings["QNEnemies"].IsVisible = true;
                    Qsettings["QNWall"].IsVisible = true;
                    Qsettings["QNTurret"].IsVisible = true;
                }
            };
            ComboMenu.Add("Qmode2", new ComboBox("Zeki mod", 0, "Agrasif", "Defansif")).IsVisible = true;
            ComboMenu.Add("UseQwhen", new ComboBox("Q kullan", 0, "Saldırıdan önce", "Saldırıdan sonra", "Asla"));
            ComboMenu.AddGroupLabel("W ayarları");
            ComboMenu.Add("UseW", new CheckBox("W focus", false));
            ComboMenu.AddGroupLabel("E ayarları");
            ComboMenu.Add("UseE", new CheckBox("E kullan"));
            ComboMenu.Add("UseEKill", new CheckBox("Kil almak için E kullan"));
            ComboMenu.AddGroupLabel("R ayarları");
            ComboMenu.Add("UseR", new CheckBox("R kullan", false));
            ComboMenu.Add("UseRif", new Slider("R kullan hedefe baglı", 2, 1, 5));
        }

        public static void QSettings()
        {
            Qsettings = VMenu.AddSubMenu("Q ayarları", "Q Settings");
            Qsettings.AddGroupLabel("Q ayarları");
            Qsettings.AddLabel("Patlama modu için Hızlı takla reset ayarı bulunmaktadır..");
            Qsettings.Add("UseMirinQ", new CheckBox("Patlama modu"));
            Qsettings.Add("UseQE", new CheckBox("Q+E tekrarla", false));
            //smart
            Qsettings.Add("UseSafeQ", new CheckBox("Güvenli ve Dinamik oyun Q", false)).IsVisible = true;
            Qsettings.Add("UseQEnemies", new CheckBox("Düşmanlara Q atmama", false)).IsVisible = true;
            Qsettings.Add("UseQSpam", new CheckBox("Görmezden Gel", false)).IsVisible = true;
            //new
            Qsettings.Add("QNMode", new ComboBox("Yeni mod", 1, "Yanlara", "Güvenli Pozisyon")).IsVisible = false;
            Qsettings.Add("QNEnemies", new Slider("Düşman bloklarına Q", 3, 5, 0)).IsVisible = false;
            Qsettings.Add("QNWall", new CheckBox("Düşman duvarlarına Q", true)).IsVisible = false;
            Qsettings.Add("QNTurret", new CheckBox("Kule altı Block Q", true)).IsVisible = false;
        }

        public static void Condemnmenu()
        {
            CondemnMenu = VMenu.AddSubMenu("Baskı", "Condemn");
            CondemnMenu.AddGroupLabel("Baskı");
            CondemnMenu.AddLabel("ZMA>En iyi>Parlak>Eski>Nişancı");
            CondemnMenu.Add("Condemnmode", new ComboBox("Baskı modu", 4, "En İyi", "Eski", "Nişancı", "Parlak", "ZMA"));
            CondemnMenu.Add("UseEAuto", new CheckBox("Otomatik E"));
            CondemnMenu.Add("UseETarget", new CheckBox("Seçilen hedefi E ile duvara zımbala", false));
            CondemnMenu.Add("UseEHitchance", new Slider("Baskı hedefi", 2, 1, 4));
            CondemnMenu.Add("UseEPush", new Slider("Baskı ile ittirme mesafesi", 420, 350, 470));
            CondemnMenu.Add("UseEAA", new Slider("Seçilmeyen kite atılmayan hedefleri E ile öldürme", 0, 0, 4));
            CondemnMenu.Add("AutoTrinket", new CheckBox("Biblo çalı kullanılsınmı"));
            CondemnMenu.Add("J4Flag", new CheckBox("Jarvan 4 Bayragına E at çevir."));
        }

        private static void Harassmenu()
        {
            HarassMenu = VMenu.AddSubMenu("Dürtme", "Harass");
            HarassMenu.Add("HarassCombo", new CheckBox("Dürtme Kombosu"));
            HarassMenu.Add("HarassMana", new Slider("Dürtme kombosu mana sınırı", 40));
        }

        private static void LaneClearmenu()
        {
            LaneClearMenu = VMenu.AddSubMenu("Koridor temizleme ", "LaneClear");
            LaneClearMenu.Add("UseQ", new CheckBox("Q kullan"));
            LaneClearMenu.Add("UseQMana", new Slider("Q için Mana sınırı ({0}%)", 40));
        }

        private static void JungleClearmenu()
        {
            JungleClearMenu = VMenu.AddSubMenu("Orman Temizleme", "JungleClear");
            JungleClearMenu.Add("UseQ", new CheckBox("Q kullan"));
            JungleClearMenu.Add("UseE", new CheckBox("E kullan"));
        }

        private static void Fleemenu()
        {
            FleeMenu = VMenu.AddSubMenu("Kaçma", "Flee");
            FleeMenu.Add("UseQ", new CheckBox("kaçarken Q kullan"));
            FleeMenu.Add("UseE", new CheckBox("kaçarken E kullan"));
        }

        private static void Miscmenu()
        {
            MiscMenu = VMenu.AddSubMenu("Çeşitli", "Misc");
            MiscMenu.AddGroupLabel("SP");
            MiscMenu.Add("GapcloseQ", new CheckBox("Gapclose Q"));
            MiscMenu.Add("GapcloseE", new CheckBox("Gapclose E"));
            MiscMenu.Add("InterruptE", new CheckBox("E kesintisi"));
            MiscMenu.Add("LowLifeE", new CheckBox("Az canlı hedefe E", false));
            MiscMenu.Add("LowLifeES", new Slider("Adamın canı kaç iken E atılsın öldürmek için. =>", 20));

        }

        private static void Drawingmenu()
        {
            DrawingMenu = VMenu.AddSubMenu("Alan-Sınır-Wall", "Drawings");
            DrawingMenu.Add("DrawQ", new CheckBox("Q sınırı göster", false));
            DrawingMenu.Add("DrawE", new CheckBox("E sınırı göster", false));
            DrawingMenu.Add("DrawOnlyReady", new CheckBox("Hazır saldırı Q"));
            DrawingMenu.AddGroupLabel("Tahmin");
            DrawingMenu.Add("DrawCondemn", new CheckBox("E sınırı-fırlicağı yer."));
            DrawingMenu.Add("DrawTumble", new CheckBox("Q atınca gidiceği yer"));
            DrawingMenu.Add("DrawAutoPos", new CheckBox("Otomatik Pozisyon"));
        }

        #region checkvalues
        #region checkvalues:hotkeys
        public static bool AutoPos
        {
            get { return (Hotkeymenu["autopos"].Cast<KeyBind>().CurrentValue); }
        }
        public static bool FlashE
        {
            get { return (Hotkeymenu["flashe"].Cast<KeyBind>().CurrentValue); }
        }
        public static bool InsecE
        {
            get { return (Hotkeymenu["insece"].Cast<KeyBind>().CurrentValue); }
        }
        public static bool RotE
        {
            get { return (Hotkeymenu["rote"].Cast<KeyBind>().CurrentValue); }
        }
        public static int InsecPositions
        {
            get { return (Hotkeymenu["insecmodes"].Cast<ComboBox>().CurrentValue); }
        }
        public static bool RNoAA
        {
            get { return (Hotkeymenu["RnoAA"].Cast<KeyBind>().CurrentValue); }
        }
        public static int RNoAASlider
        {
            get { return (Hotkeymenu["RnoAAif"].Cast<Slider>().CurrentValue); }
        }
        #endregion checkvalues:hotkeys
        #region checkvalues:qsettings
        public static bool Burstmode
        {
            get { return (Qsettings["UseMirinQ"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool UseQE
        {
            get { return (Qsettings["UseQE"].Cast<CheckBox>().CurrentValue); }
        }
        //smart
        public static bool Dynamicsafety
        {
            get { return (Qsettings["UseSafeQ"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool DontuseQintoenemies
        {
            get { return (Qsettings["UseQEnemies"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool SpamQ
        {
            get { return (Qsettings["UseQSpam"].Cast<CheckBox>().CurrentValue); }
        }
        //new
        public static int QNMode
        {
            get { return (Qsettings["QNMode"].Cast<ComboBox>().CurrentValue); }
        }
        public static int QNEnemies
        {
            get { return (Qsettings["QNEnemies"].Cast<Slider>().CurrentValue); }
        }
        public static bool QNWall
        {
            get { return (Qsettings["QNWall"].Cast<CheckBox>().CurrentValue); }
        }
        public static bool QNTurret
        {
            get { return (Qsettings["QNTurret"].Cast<CheckBox>().CurrentValue); }
        }
        #endregion
        #region checkvalues:Combo
        public static int UseQMode
        {
            get { return (ComboMenu["Qmode"].Cast<ComboBox>().CurrentValue); }
        }

        public static int UseQMode2
        {
            get { return (ComboMenu["Qmode2"].Cast<ComboBox>().CurrentValue); }
        }

        public static int UseQif
        {
            get { return (ComboMenu["UseQwhen"].Cast<ComboBox>().CurrentValue); }
        }

        public static bool FocusW
        {
            get { return (ComboMenu["UseW"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool UseE
        {
            get { return (ComboMenu["UseE"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool UseEKill
        {
            get { return (ComboMenu["UseEKill"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool UseR
        {
            get { return (ComboMenu["UseR"].Cast<CheckBox>().CurrentValue); }
        }

        public static int UseRSlider
        {
            get { return (ComboMenu["UseRif"].Cast<Slider>().CurrentValue); }
        }
        //Condemn
        #endregion checkvalues:Combo
        #region checkvalues:Condemn
        public static int CondemnMode
        {
            get { return (CondemnMenu["Condemnmode"].Cast<ComboBox>().CurrentValue); }
        }

        public static bool AutoE
        {
            get { return (CondemnMenu["UseEAuto"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool OnlyStunCurrentTarget
        {
            get { return (CondemnMenu["UseETarget"].Cast<CheckBox>().CurrentValue); }
        }

        public static int CondemnHitchance
        {
            get { return (CondemnMenu["UseEHitchance"].Cast<Slider>().CurrentValue); }
        }

        public static int CondemnPushDistance
        {
            get { return (CondemnMenu["UseEPush"].Cast<Slider>().CurrentValue); }
        }

        public static int CondemnBlock
        {
            get { return (CondemnMenu["UseEAA"].Cast<Slider>().CurrentValue); }
        }

        public static bool AutoTrinket
        {
            get { return (CondemnMenu["AutoTrinket"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool J4Flag
        {
            get { return (CondemnMenu["J4Flag"].Cast<CheckBox>().CurrentValue); }
        }

        #endregion checkvalues:Condemn
        #region checkvalues:Harass

        public static bool HarassCombo
        {
            get { return (HarassMenu["HarassCombo"].Cast<CheckBox>().CurrentValue); }
        }

        public static int HarassMana
        {
            get { return (HarassMenu["HarassMana"].Cast<Slider>().CurrentValue); }
        }


        #endregion checkvalues:Harass
        #region checkvalues:LC

        public static bool UseQLC
        {
            get { return (LaneClearMenu["UseQ"].Cast<CheckBox>().CurrentValue); }
        }

        public static int UseQLCMana
        {
            get { return (LaneClearMenu["UseQMana"].Cast<Slider>().CurrentValue); }
        }


        #endregion checkvalues:LC
        #region checkvalues:JC

        public static bool UseQJC
        {
            get { return (JungleClearMenu["UseQ"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool UseEJC
        {
            get { return (JungleClearMenu["UseE"].Cast<CheckBox>().CurrentValue); }
        }

        #endregion checkvalues:JC
        #region checkvalues:Flee
        public static bool UseQFlee
        {
            get { return (FleeMenu["UseQ"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool UseEFlee
        {
            get { return (FleeMenu["UseE"].Cast<CheckBox>().CurrentValue); }
        }

        #endregion checkvalues:Flee
        #region checkvalues:Misc

        public static bool GapcloseQ
        {
            get { return (MiscMenu["GapcloseQ"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool GapcloseE
        {
            get { return (MiscMenu["GapcloseE"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool InterruptE
        {
            get { return (MiscMenu["InterruptE"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool LowLifeE
        {
            get { return (MiscMenu["LowLifeE"].Cast<CheckBox>().CurrentValue); }
        }

        public static int LowLifeESlider
        {
            get { return (MiscMenu["LowLifeES"].Cast<Slider>().CurrentValue); }
        }

        public static bool Skinhack
        {
            get { return (MiscMenu["Skinhack"].Cast<CheckBox>().CurrentValue); }
        }

        public static int SkinId
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

        public static bool AutobuyStarters
        {
            get { return (MiscMenu["Autobuy"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool AutobuyTrinkets
        {
            get { return (MiscMenu["Autobuyt"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool AutoLantern
        {
            get { return (MiscMenu["Autolantern"].Cast<CheckBox>().CurrentValue); }
        }

        public static int AutoLanternS
        {
            get { return (MiscMenu["AutolanternHP"].Cast<Slider>().CurrentValue); }
        }
        #endregion checkvalues:Misc
        #region checkvalues:Drawing
        public static bool DrawQ
        {
            get { return (DrawingMenu["DrawQ"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool DrawE
        {
            get { return (DrawingMenu["DrawE"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool DrawCondemn
        {
            get { return (DrawingMenu["DrawCondemn"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool DrawTumble
        {
            get { return (DrawingMenu["DrawTumble"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool DrawAutoPos
        {
            get { return (DrawingMenu["DrawAutoPos"].Cast<CheckBox>().CurrentValue); }
        }

        public static bool DrawOnlyRdy
        {
            get { return (DrawingMenu["DrawOnlyReady"].Cast<CheckBox>().CurrentValue); }
        }
        #endregion checkvalues:Drawing
        #endregion checkvalues
    }
}
