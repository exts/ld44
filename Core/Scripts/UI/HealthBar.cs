using Godot;

namespace Gamma.Core.Scripts.UI
{
    public class HealthBar : Node2D
    {
        private Label _amount;
        private TextureProgress _progress;

        public override void _Ready()
        {
            _amount = GetNode<Label>("Amount");
            _progress = GetNode<TextureProgress>("Sprite/TextureProgress");
        }

        public void SetAmount(int amount)
        {
            _amount.Text = amount.ToString();
            _progress.Value = amount;
        }
    }
}