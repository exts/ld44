using Godot;

namespace Gamma.Core.Helpers
{
    public class ObjectLoader
    {
        public static PackedScene Load(string path)
        {
            path = path.Replace(".tscn", "");
            
            return GD.Load<PackedScene>($"res://{path}.tscn");
        }
    }
}