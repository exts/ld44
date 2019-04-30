using System.Collections.Generic;
using System.Linq;
using Gamma.Core.Helpers;
using Gamma.Core.Level;
using Gamma.Core.Scripts.Objects;
using Gamma.Core.Scripts.UI;
using Godot;

namespace Gamma.Core.Scripts
{
    public class Arena : Node2D
    {
        private Node2D _cursor;
        private Player _player;
        private Node2D _enemyContainer;
        private CurrentWave _currentWave;
        private HBoxContainer _ammoContainer;
        private AmmoReloading _ammoReloading;
        private HealthBar _hpbar;
        
        private Timer _waveTimer;
        private Timer _spawnTimer;

        private PackedScene _enemyObject;
        private PackedScene _bulletObject;
        private PackedScene _ammoEmptyObject;
        private PackedScene _ammoAvailableObject;
        
        private EnemySpawner _spawner = new EnemySpawner();

        private int _currentAmmo;

        private bool _waveStart;

        private Node2D _spawnPoints;

        private int MaxWaveTime = 30;
        private int CurrentWaveTime;

        private int _spawnsPerWave = 1;

        public override void _Ready()
        {
            Game.WavesCleared = 0;
            Game.EnemiesCleared = 0;
            
            CurrentWaveTime = MaxWaveTime;
            
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

            _enemyContainer = GetNode<Node2D>("Enemies");

            _waveTimer = GetNode<Timer>("Timers/WaveTimer");
            _waveTimer.Connect("timeout", this, nameof(HandleCurrentWave));
            
            _spawnTimer = GetNode<Timer>("Timers/SpawnTimer");
            _spawnTimer.Connect("timeout", this, nameof(SpawnMonsters));

            _hpbar = GetNode<HealthBar>("HealthBar");

            _spawnPoints = GetNode<Node2D>("SpawnPoints");
            SetupSpawnPointsInSpawner();

            _enemyObject = ObjectLoader.Load("Data/Objects/Enemy");
            _bulletObject = ObjectLoader.Load("Data/Objects/Bullet");
            _ammoEmptyObject = ObjectLoader.Load("Data/Objects/UI/AmmoEmpty");
            _ammoAvailableObject = ObjectLoader.Load("Data/Objects/UI/AmmoAvailable");
            
            DrawAmmoUiElements();
            _currentWave.Start();
        }

        public override void _Process(float delta)
        {
            _cursor.Position = GetGlobalMousePosition();

            if(_player.Health <= 0)
            {
                _waveStart = false;
                SceneSwitcher.Switch(Scenes.GameOver);
                return;
            }

            if(_waveStart)
            {
                UpdateEnemyDestination(_player.Position);
            }
        }

        public override void _Input(InputEvent @event)
        {
            if(Input.IsKeyPressed((int) KeyList.Escape) && @event.IsPressed())
            {
                _waveStart = false;
                SceneSwitcher.Switch(Scenes.MainMenu);
            }
        }

        public void SpawnBullet()
        {
            var bullet = (Bullet) _bulletObject.Instance();
            bullet.Connect(nameof(Bullet.EnemyHit), this, nameof(EnemyDamageDealt));
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
            CurrentWaveTime = MaxWaveTime;
            
            _waveTimer.Start();
            _spawnTimer.Start();
        }

        public void SpawnMonsters()
        {
            for(var x = 0; x < _spawnsPerWave; x++)
            {
                var enemy = (Enemy) _enemyObject.Instance();
                enemy.Connect(nameof(Enemy.Dead), this, nameof(EnemyDead));
                enemy.Connect(nameof(Enemy.DamageDealt), this, nameof(DamageDealt));
                enemy.Position = _spawner.SelectSpawnPoint();
                _enemyContainer.AddChild(enemy);
            }
        }

        public void HandleCurrentWave()
        {
            if(CurrentWaveTime > 1)
            {
                --CurrentWaveTime;
            }
            else
            {
                _waveStart = false;
                _waveTimer.Stop();
                _spawnTimer.Stop();
                
                DeleteEnemies();
                CurrentWaveTime = 0;
                _currentWave.Start();
            }
        }

        public void UpdateEnemyDestination(Vector2 dest)
        {
            foreach(var node in _enemyContainer.GetChildren())
            {
                if(node is Enemy enemy)
                {
                    enemy.SetDestination(dest);
                }
            }
        }

        public void DeleteEnemies()
        {
            foreach(var node in _enemyContainer.GetChildren())
            {
                if(node is Enemy enemy)
                {
                    enemy.CallDeferred("queue_free");
                }
            }
        }

        public void DamageDealt(int amount)
        {
            if(_player.Health > 0)
            {
                _player.Health -= amount;
            }

            _player.Health = Mathf.Clamp(_player.Health, 0, 100);
            _hpbar.SetAmount(_player.Health);
        }

        public void EnemyDamageDealt(Enemy enemy)
        {
            enemy.Health -= _player.Damage;
        }

        public void EnemyDead()
        {
            Game.EnemiesCleared += 1;
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

        private void SetupSpawnPointsInSpawner()
        {
            foreach(var corner in _spawnPoints.GetChildren().ToList())
            {
                if(!(corner is Node2D points)) continue;
                
                var list = new List<Vector2>();
                foreach(var point in points.GetChildren().ToList())
                {
                    if(point is Position2D position2D)
                    {
                        list.Add(position2D.GlobalPosition);
                    }
                }
                    
                _spawner.AddSpawnPoints(list);
            }
            
            _spawner.RandomizeSpawnOrder();
        }
    }
}