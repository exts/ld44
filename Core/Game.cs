using Godot;

namespace Gamma.Core
{
    public class Game
    {
        public static Game Instance => _instance ?? (_instance = new Game());
        private static Game _instance;

        private bool _initiated;
        private Viewport _root;

        private Game()
        {
            Init();
        }

        public void Init()
        {
            if(_initiated) return;

            _initiated = true;
            _root = ((SceneTree) Engine.GetMainLoop()).GetRoot();
        }

        public static Viewport Root()
        {
            return Instance._root;
        }

        public static Node CurrentScene()
        {
            var root = Root();
            return root?.GetChild(root.GetChildCount() - 1);
        }

        public static Autoload Autoload()
        {
            return Root().GetNode<Autoload>("Autoload");
        }

        public static void ExitGame()
        {
            Autoload().Quit();
        }
    }
}