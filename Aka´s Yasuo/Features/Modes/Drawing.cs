using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Rendering;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;

namespace Aka_s_Yasuo.Features.Modes
{
    class drawing
    {
        public static void Load()
        {
            Drawing.OnDraw += OnDraw;
        }

        public static void OnDraw(EventArgs args)
        {
            if (Variables._Player.IsDead) return;

             
            if (Manager.MenuManager.DrawE)
            {
                if (!(Manager.MenuManager.DrawOnlyRdy && !Manager.SpellManager.E.IsReady()))
                {
                    Circle.Draw(Color.Blue, Manager.SpellManager.E.Range, Variables._Player.Position);
                }
            }
            if (Manager.MenuManager.DrawQ)
            {
                if (!(Manager.MenuManager.DrawOnlyRdy && !Manager.SpellManager.Q.IsReady()))
                {
                    Circle.Draw(Color.Blue, Variables.IsDashing ? Manager.SpellManager.Q3.Width : (int)Variables.SpellQ.Range, Variables._Player.Position);
                }
            }
            if (Manager.MenuManager.DrawR)
            {
                if (!(Manager.MenuManager.DrawOnlyRdy && !Manager.SpellManager.R.IsReady()))
                {
                    Circle.Draw(Color.Blue, Manager.SpellManager.R.Range, Variables._Player.Position);
                }
            }
            if (Manager.MenuManager.DrawStatus)
            {
                var menu = Manager.MenuManager.AutoStackQ;
                var test = $"Auto Stack Q: {(menu ? (Variables.haveQ3 ? "Full" : (Manager.SpellManager.Q.IsReady() ? "Ready" : "Not Ready")) : "Off")}";
                var pos = Variables._Player.Position.WorldToScreen();

                Drawing.DrawText(pos.X - (float)Drawing.GetTextEntent(test, 2).Width / 2, pos.Y + 20, menu ? System.Drawing.Color.White : System.Drawing.Color.Gray, test);
            }
        }
    }
}
