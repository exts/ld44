using Godot;

namespace Gamma.Core.Scripts.Menu
{
    public class BackButton : Control
    {
        private Button _button;

        public override void _Ready()
        {
            _button = GetNode<Button>("Panel/Button");
            _button.Connect("pressed", this, nameof(GoToMainMenu));
        }

        public void GoToMainMenu()
        {
            SceneSwitcher.Switch(Scenes.MainMenu);
        }
    }
}