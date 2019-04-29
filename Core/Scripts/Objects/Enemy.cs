using System.Linq;
using Godot;
using Godot.Collections;

namespace Gamma.Core.Scripts.Objects
{
    public class Enemy : KinematicBody2D
    {
        public int Health = 30;
        public int Damage = 5;

        [Signal]
        public delegate void DamageDealt(int amount);
        
        public bool GamePaused;
        private int Speed = 150;
        private Vector2[] _paths = {};

        public int CurrentDamage = 5;
        
        private const int DefaultDamage = 10;

        private Vector2 _destination = Vector2.Zero;

        private int _bounceAmount = 50;

        public override void _Ready()
        {
        }

        public override void _Process(float delta)
        {
            if(GamePaused) return;

            if(_destination == Vector2.Zero) return;
            
            var velocity = (_destination - Position).Normalized() * Speed;
            var collide = MoveAndCollide(velocity * delta);
            
            if(collide == null) return;
            
            HandleCollision(collide, velocity);
        }

        private void HandleCollision(KinematicCollision2D collide, Vector2 velocity)
        {
            if(collide.Collider is KinematicBody2D e) 
            {
                var direction = new Vector2();
                if(e.Position.x > Position.x)
                {
                    direction.x -= _bounceAmount;
                }

                if(e.Position.x < Position.x)
                {
                    direction.x += _bounceAmount;
                }
                if(e.Position.y > Position.y)
                {
                    direction.y -= _bounceAmount;
                }

                if(e.Position.y < Position.y)
                {
                    direction.y += _bounceAmount;
                }

                Position += direction;
            }

            if(collide.Collider is Player)
            {
                EmitSignal(nameof(DamageDealt), Damage);
            }
                
            velocity = velocity.Slide(collide.Normal);
            MoveAndSlide(velocity);
        }

        public void SetDestination(Vector2 dest) => _destination = dest;

        /**
         * This used to be code for moving the enemy along a navigation path to mimic path finding, didn't work out so
         * well, so I stopped using it.
         */
        private void old_MoveCharacter(float delta)
        {
            var velocity = delta * Speed;            
            var spaceState = GetWorld2d().GetDirectSpaceState();
            
            var lastPosition = Position;
            for(var x = 0; x < _paths.Length; x++)
            {
                if(_paths.Length > 1)
                {
                    var playerFound = false;
                    for(var rayPath = x + 1; rayPath < _paths.Length; rayPath++)
                    {
                        var raycast = spaceState.IntersectRay(Position, _paths[rayPath], new Array {this},
                            GetNode<Area2D>("Area2D").CollisionMask, true, true);

                        if(raycast.Count > 0 && raycast["collider"] is Area2D collider)
                        {
                            if(collider.GetParent().Name == "Player")
                            {
                                playerFound = true;
                                break;
                            }
                        }
                    }

                    if(playerFound)
                    {
                        _paths = _paths.Skip(_paths.Length - 1).Take(_paths.Length - 1).ToArray();
                        break;
                    }
                    
                }

                var moveVelocity = (_paths[x] - lastPosition).Normalized() * Speed;
                var padded = lastPosition + new Vector2(16, 16);
                var distanceBetweenPoints = padded.DistanceTo(_paths[x]);
                if(velocity <= distanceBetweenPoints)
                {
//                    Position = lastPosition.LinearInterpolate(_paths[x], velocity / distanceBetweenPoints);
                    MoveAndSlide(moveVelocity);
                    break;
                }
                
                if (velocity < 0.0) 
                {
                    Position = _paths[0];
                    break;
                }

                velocity -= distanceBetweenPoints;
                lastPosition = _paths[x];
                _paths = _paths.Skip(1).Take(_paths.Length - 1).ToArray();
                GD.Print("Do we ever get here?");
            }
        }
        
        /**
         * We used this to get the paths from a navigation2d and then tell it where we wanted it to go.
         * Nav2D path finding is a little wonky because it ignores objects and it's best that you do something like
         * custom points for added padding. I'm sure custom coding could make this much nicer
         */
        public void old_SetPaths(Navigation2D nav, Vector2 dest)
        {
            var paths = nav.GetSimplePath(Position, dest, false);
            _paths = paths.Skip(1).Take(paths.Length - 1).ToArray();
        }
    }
}