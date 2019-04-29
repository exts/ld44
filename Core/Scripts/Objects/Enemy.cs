using System.Linq;
using Godot;
using Godot.Collections;

namespace Gamma.Core.Scripts.Objects
{
    public class Enemy : KinematicBody2D
    {
        public bool Moving;
        private int Speed = 300;
        private Vector2[] _paths = {};
        
        public override void _Ready()
        {
        }

        public override void _Process(float delta)
        {
            if(Moving)
            {
                MoveCharacter(delta);
            }
        }

        public void SetPaths(Navigation2D nav, Vector2 dest)
        {
            var paths = nav.GetSimplePath(Position, dest, false);
            _paths = paths.Skip(1).Take(paths.Length - 1).ToArray();
        }

        private void MoveCharacter(float delta)
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
                    Moving = false;
                    break;
                }

                velocity -= distanceBetweenPoints;
                lastPosition = _paths[x];
                _paths = _paths.Skip(1).Take(_paths.Length - 1).ToArray();
                GD.Print("Do we ever get here?");
            }
        }
    }
}