using Godot;

namespace Gamma.Core.Scripts
{
    public class AmmoReloading : Node2D
    {
        private Sprite _normalSprite;
        private Sprite _greyscaleSprite;
        private AnimationPlayer _player;


        public override void _Ready()
        {
            _player = GetNode<AnimationPlayer>("AnimationPlayer");
            _normalSprite = GetNode<Sprite>("Normal");
            _greyscaleSprite = GetNode<Sprite>("Greyscale");
            
            Reset();
        }

        public void Reset()
        {
            _player.Stop();
            _player.Seek(0, true);
            
            _normalSprite.Hide();
            _greyscaleSprite.Show();
        }

        public void Animate()
        {
            _normalSprite.Show();
            _greyscaleSprite.Hide();
            
            _player.Play("Spinner");
        }
    }
}