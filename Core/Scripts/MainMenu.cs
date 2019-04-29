using System.Collections.Generic;
using Gamma.Core.Scripts.Menu;
using Godot;

namespace Gamma.Core.Scripts
{
    public class MainMenu : Control
    {
        private int _buttonSelected = 1;
        private List<MenuListButton> _buttons = new List<MenuListButton>();

        public override void _Ready()
        {
            var buttons = GetNode<VBoxContainer>("MenuContainer/ButtonContainer/Buttons");
            SetupButtonOrder(buttons);
            UpdateButtonSelect(_buttonSelected);
        }

        public override void _Input(InputEvent @event)
        {
            HandleKeyboardMenuActions();
            HandleMenuNavigation();
        }

        private void HandleMenuNavigation()
        {
            var updateMenuButtons = false;
            if(Input.IsActionPressed("ui_up"))
            {
                _buttonSelected -= 1;
                updateMenuButtons = true;
            } else if(Input.IsActionPressed("ui_down"))
            {
                _buttonSelected += 1;
                updateMenuButtons = true;
            }

            // if button select is less than index 0, select the end menu item; else check if we're doing the opposite
            // and it's greater than the list go to the top of the menu else select the correct value. This is mostly
            // for when we're using the keyboard to switch keys
            var original = _buttonSelected;
            _buttonSelected = _buttonSelected < 0 ? _buttons.Count - 1 :
                _buttonSelected >= _buttons.Count ? 0 : _buttonSelected;

            if(original != _buttonSelected || updateMenuButtons)
            {
                UpdateButtonSelect(_buttonSelected);
            }
        }

        private void HandleKeyboardMenuActions()
        {
            if(Input.IsActionPressed("ui_accept"))
            {
                if(_buttons[_buttonSelected] != null)
                {
                    _buttons[_buttonSelected]?.ClickEvent?.Invoke();
                }
            }
        }

        private void SetupButtonOrder(Node btnContainer)
        {
            var order = 0;
            foreach(var button in btnContainer.GetChildren())
            {
                if(button is MenuListButton btn)
                {
                    btn.Order = order;
                    btn.Connect("Selected", this, nameof(UpdateButtonSelect));
                    SetupMenuBtnClickEvents(btn);
                    
                    _buttons.Add(btn);
                }
                ++order;
            }
        }

        private void SetupMenuBtnClickEvents(MenuListButton btn)
        {
            switch(btn.Text)
            {
                case "Play":
                    break;
                case "Options":
                    break;
                case "Credits":
                    btn.ClickEvent = () => SceneSwitcher.Switch(Scenes.Credits);
                    break;
                case "Quit":
                    btn.ClickEvent = () => Game.ExitGame();
                    break;
            }
        }

        public void UpdateButtonSelect(int order)
        {
            if(_buttons[order] == null) return;
            
            _buttonSelected = order;
            foreach(var button in _buttons.ToArray())
            {
                button.DeselectButton();
            }

            _buttons[order].SelectButton();
        }
    }
}