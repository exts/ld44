using Gamma.Core.Helpers;
using Godot;

namespace Gamma.Core.Scripts
{
    public class Player : KinematicBody2D
    {
        private int _speed = 250;
        public bool CanMove = true;
        
        public int MaxAmmo = 15;
        public int CurrentAmmo;

        private string _defaultFrame = "moving";

        [Signal]
        public delegate void SpawnBullet();
        
        [Signal]
        public delegate void Reloading();
        
        [Signal]
        public delegate void ReloadingDone();

        private Timer _reloadTimer;

        public override void _Ready()
        {
            CurrentAmmo = MaxAmmo;
            
            _reloadTimer = GetNode<Timer>("ReloadTimer");
            _reloadTimer.Connect("timeout", this, nameof(Reload));
        }

        public override void _PhysicsProcess(float delta)
        {
            if(!CanMove) return;
            
            MovePlayer();
        }

        public override void _Input(InputEvent @event)
        {
            if(Input.IsMouseButtonPressed((int) ButtonList.Left) && @event.IsPressed() && CurrentAmmo > 0)
            {
                --CurrentAmmo;

                if(CurrentAmmo < 1)
                {
                    _reloadTimer.Start();
                    EmitSignal(nameof(Reloading));
                }

                EmitSignal(nameof(SpawnBullet));
            }
        }

        private void MovePlayer()
        {
            var velocity = new Vector2();

            if(InputHelper.AnyKeyPressed(KeyList.A, KeyList.Left))
            {
                velocity.x -= 1;
            }
            
            if(InputHelper.AnyKeyPressed(KeyList.D, KeyList.Right))
            {
                velocity.x += 1;
            }

            if(InputHelper.AnyKeyPressed(KeyList.W, KeyList.Up))
            {
                velocity.y -= 1;
            }
            
            if(InputHelper.AnyKeyPressed(KeyList.S, KeyList.Down))
            {
                velocity.y += 1;
            }

            velocity = velocity * _speed;
            MoveAndSlide(velocity);
        }

        public void CenterPlayer()
        {
            var viewport = GetViewport();
            
            Position = new Vector2(viewport.Size.x / 2, viewport.Size.y / 2);
        }

        public void Reload()
        {
            CurrentAmmo = MaxAmmo;
            _reloadTimer.Stop();
            EmitSignal(nameof(ReloadingDone));
        }

        public Rect2 GetRect()
        {
            var texture = GetNode<AnimatedSprite>("Sprite").Frames.GetFrame(_defaultFrame, 0);
            return new Rect2(Position, texture.GetSize());
        }
    }
}