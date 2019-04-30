using Godot;

namespace Gamma.Core.Scripts.Objects
{
    public class Bullet : Area2D
    {
        public int Speed = 600;

        private Vector2 _direction = Vector2.Zero;
        
        private Rect2 _hideUntil = new Rect2();

        public override void _Ready()
        {
            Visible = false;
        }

        public override void _Process(float delta)
        {
            DeleteSelf();

            if(!_hideUntil.HasPoint(Position))
            {
                Visible = true;
            }
            
            Position += _direction * delta;
        }

        public void MoveInDirection(Vector2 dest)
        {
            _direction = (dest - Position).Normalized() * Speed;
            Rotation = _direction.Angle();
        }

        public void HideUntil(Rect2 hideUntil) => _hideUntil = hideUntil;

        public void DeleteSelf()
        {
            var window = GetViewport().Size;

            if(Position.x < 0 || Position.x > window.x || Position.y < 0 ||
               Position.y > window.y)
            {
                CallDeferred("queue_free");
            }
        }
    }
}