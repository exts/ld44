using System;
using System.Collections.Generic;
using Gamma.Core.Scripts;

namespace Gamma.Core
{
    public enum Scenes
    {
        MainMenu,
        Options,
        Credits,
    }
    public class SceneSwitcher
    {
        private static readonly Dictionary<Scenes, string> ScenesDict = new Dictionary<Scenes, string>
        {
            {Scenes.MainMenu, "MainMenu"},
            {Scenes.Options, ""},
            {Scenes.Credits, "Credits"},
        };

        public static void Switch(Scenes scene)
        {
            if(!ScenesDict.ContainsKey(scene))
            {
                throw new Exception($"Invalid scene you are trying to switch to: {scene}");
            }
            
            Game.Autoload().SwitchScene($"res://Data/Scenes/{ScenesDict[scene]}.tscn");
        }
    }
}