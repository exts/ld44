using Godot;

namespace Gamma.Core.Scripts
{
    public class GameOver : Node2D
    {
        private Label _gameOverText;

        public override void _Ready()
        {
            _gameOverText = GetNode<Label>("GameOverText");
            
            SetGameOverText(Game.WavesCleared, Game.EnemiesCleared);
        }

        public void SetGameOverText(int waves, int kills)
        {
            _gameOverText.Text = $"You killed [{kills}] enemies and survived [{waves}] waves!\n\nThanks for playing!";
        }
    }
}