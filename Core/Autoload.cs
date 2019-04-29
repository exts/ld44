using System;
using Gamma.Core.Scripts.Credits;
using Godot;

namespace Gamma.Core
{
    public class Autoload : Node
    {
        private Node _scene;
        
        public string Credits = String.Empty;

        public override void _Ready()
        {
            _scene = Game.CurrentScene();
            
            // load credits
            Credits = new CreditsLoader().LoadCredits();
        }
        
        public override void _Notification(int what)
        {
            if (what == MainLoop.NotificationWmQuitRequest)
                Quit();
        }

        public void Quit()
        {
            GetTree().Quit(); // default behavior
        }

        public void SwitchScene(string path)
        {
            CallDeferred(nameof(SwitchSceneCallback), path);
        }

        public void SwitchSceneCallback(string path)
        {
            _scene.Free();
            _scene = GD.Load<PackedScene>(path).Instance();
            Game.Root().AddChild(_scene);
            GetTree().SetCurrentScene(_scene);
        }
    }
}