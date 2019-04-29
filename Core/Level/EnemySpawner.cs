using System.Collections.Generic;
using Godot;

namespace Gamma.Core.Level
{
    public class EnemySpawner
    {
        // spawn points are located in 4 corners, we want to store that information and use it later
        private List<List<Vector2>> _spawnPoints = new List<List<Vector2>>();

        private List<int> _spawnOrder = new List<int>();

        public void AddSpawnPoints(List<Vector2> points)
        {
            _spawnPoints.Add(points);
        }

        public void RandomizeSpawnOrder()
        {
            var list = new List<int>();
            for(var i = 0; i < _spawnPoints.Count; i++)
            {
                list.Add(i);
            }

            var spawnPoints = new List<int>();
            while(list.Count > 0)
            {
                var rng = Game.RNG.Next(0, list.Count);
                spawnPoints.Add(list[rng]);
                list.RemoveAt(rng);
            }

            _spawnOrder = spawnPoints;
        }

        public Vector2 SelectSpawnPoint()
        {
            if(_spawnOrder.Count < 1)
            {
                RandomizeSpawnOrder();
            }

            var idx = _spawnOrder[0];
            _spawnOrder.RemoveAt(0);

            return _spawnPoints[idx][Game.RNG.Next(0, _spawnPoints[idx].Count)];
        }
    }
}