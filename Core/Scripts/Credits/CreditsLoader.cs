using Godot;

namespace Gamma.Core.Scripts.Credits
{
    public class CreditsLoader
    {
        private const string CreditsFile = "res://Data/CREDITS";

        public string LoadCredits()
        {
            var content = string.Empty;
            
            using(var file = new File())
            {
                if(file.Open(CreditsFile, (int) File.ModeFlags.Read) == Error.Ok)
                {
                    content = file.GetAsText();
                    file.Close();
                }
            }

            return content;
        }
    }
}