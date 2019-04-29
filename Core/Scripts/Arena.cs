using Gamma.Core.Helpers;
using Gamma.Core.Scripts.Objects;
using Godot;

namespace Gamma.Core.Scripts
{
    public class Arena : Node2D
    {
        private Node2D _cursor;
        private Player _player;
        private CurrentWave _currentWave;
        private HBoxContainer _ammoContainer;
        private AmmoReloading _ammoReloading;
        
        private PackedScene _bulletObject;
        private PackedScene _ammoEmptyObject;
        private PackedScene _ammoAvailableObject;

        private int _currentAmmo;

        private bool _waveStart;

        public override void _Ready()
        {
            // custom cursor
            Input.SetMouseMode(Input.MouseMode.Hidden);
            _cursor = GetNode<Node2D>("Cursor");
            
            _player = GetNode<Player>("Player");
            _player.Connect(nameof(Player.Reloading), this, nameof(ShowReloadingIcon));
            _player.Connect(nameof(Player.SpawnBullet), this, nameof(SpawnBullet));
            _player.Connect(nameof(Player.ReloadingDone), this, nameof(HideReloadingIcon));
            _player.CenterPlayer();
            _currentAmmo = _player.CurrentAmmo;

            _currentWave = GetNode<CurrentWave>("CurrentWave");
            _currentWave.Connect("WaveStart", this, nameof(WaveStart));

            _ammoContainer = GetNode<HBoxContainer>("Ammo/Container");
            _ammoReloading = GetNode<AmmoReloading>("Ammo/Reloading");

            _bulletObject = ObjectLoader.Load("Data/Objects/Bullet");
            _ammoEmptyObject = ObjectLoader.Load("Data/Objects/UI/AmmoEmpty");
            _ammoAvailableObject = ObjectLoader.Load("Data/Objects/UI/AmmoAvailable");
            
            DrawAmmoUiElements();
            _currentWave.Start();
        }

        public override void _Process(float delta)
        {
            _cursor.Position = GetGlobalMousePosition();
        }

        public void SpawnBullet()
        {
            var bullet = (Bullet) _bulletObject.Instance();
            bullet.Position = _player.Position;
            bullet.MoveInDirection(GetGlobalMousePosition());
            bullet.HideUntil(_player.GetRect());
            GetNode<Node2D>("SpawnedNodes").AddChild(bullet);

            if(_currentAmmo != _player.CurrentAmmo)
            {
                _currentAmmo = _player.CurrentAmmo;
                
                DeleteAmmoUiElements();
                DrawAmmoUiElements();
            }
        }

        public void ShowReloadingIcon()
        {
            _ammoReloading.Animate();
        }

        public void HideReloadingIcon()
        {
            _ammoReloading.Reset();
                
            DeleteAmmoUiElements();
            DrawAmmoUiElements();
            
            _player.SetReloading(false);
        }

        public void WaveStart()
        {
            _waveStart = true;
            _player.GamePaused = false;
        }

        private void DeleteAmmoUiElements()
        {
            foreach(var ammo in _ammoContainer.GetChildren())
            {
                if(ammo is Node node)
                {
                    node.CallDeferred("queue_free");
                }
            }
        }

        private void DrawAmmoUiElements()
        {
            var empty = _player.MaxAmmo - _player.CurrentAmmo;
            for(var e = 0; e < empty; e++)
            {
                var emptyObject = (TextureRect) _ammoEmptyObject.Instance();
                _ammoContainer.AddChild(emptyObject);
            }

            for(var c = 0; c < _player.CurrentAmmo; c++)
            {
                var currentObject = (TextureRect) _ammoAvailableObject.Instance();
                _ammoContainer.AddChild(currentObject);
            }
        }
    }
}