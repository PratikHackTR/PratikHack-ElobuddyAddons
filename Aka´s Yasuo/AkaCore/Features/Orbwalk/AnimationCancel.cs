using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using EloBuddy.SDK.Rendering;

namespace AkaCore.Features.Orbwalk
{
    class AnimationCancel : IModule
    {
        public void OnLoad()
        {
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
        }

        private void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;
            if (ObjectManager.Player.ChampionName == "Riven")
            {
                switch (args.Animation)
                {
                    case "Spell1a":
                        if (Manager.MenuManager.AnimationCancelQ)
                        {
                            Chat.Say("/d");
                        }
                        break;
                    case "Spell1b":
                        if (Manager.MenuManager.AnimationCancelQ)
                        {
                            Chat.Say("/d");
                        }
                        break;
                    case "Spell1c":
                        if (Manager.MenuManager.AnimationCancelQ)
                        {
                            Chat.Say("/d");
                        }
                        break;
                    case "Spell3":
                        if (Manager.MenuManager.AnimationCancelE)
                        {
                            Chat.Say("/d");
                        }
                        break;
                    case "Spell4a":
                        if (Manager.MenuManager.AnimationCancelR)
                        {
                            Chat.Say("/d");
                        }
                        break;
                    case "Spell4b":
                        if (Manager.MenuManager.AnimationCancelR)
                        {
                            Chat.Say("/d");
                        }
                        break;
                }
            }
            else
            {
                switch (args.Animation)
                {
                    case "Spell1":
                        if (Manager.MenuManager.AnimationCancelQ)
                        {
                            Chat.Say("/d");
                        }
                        break;
                    case "Spell2":
                        if (Manager.MenuManager.AnimationCancelW)
                        {
                            Chat.Say("/d");
                        }
                        break;
                    case "Spell3":
                        if (Manager.MenuManager.AnimationCancelE)
                        {
                            Chat.Say("/d");
                        }
                        break;
                    case "Spell4":
                        if (Manager.MenuManager.AnimationCancelR)
                        {
                            Chat.Say("/d");
                        }
                        break;
                }
            }
        }
    

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return false;
        }

        public void OnExecute()
        {
        }
    }
}
