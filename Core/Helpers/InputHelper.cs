using Godot;

namespace Gamma.Core.Helpers
{
    public class InputHelper
    {
        public static bool AnyKeyPressed(params KeyList[] keyList)
        {
            foreach(var key in keyList)
            {
                if(Input.IsKeyPressed((int) key))
                {
                    return true;
                }
            }

            return false;
        }
    }
}