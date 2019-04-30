using System;
using System.Collections.Generic;
using Gamma.Core.Scripts;

namespace Gamma.Core
{
    public enum Scenes
    {
        Start,
        MainMenu,
        Credits,
        GameOver
    }
    public class SceneSwitcher
    {
        private static readonly Dictionary<Scenes, string> ScenesDict = new Dictionary<Scenes, string>
        {
            {Scenes.Start, "Level/Arena"},
            {Scenes.MainMenu, "MainMenu"},
            {Scenes.Credits, "Credits"},
            {Scenes.GameOver, "Level/GameOver"},
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