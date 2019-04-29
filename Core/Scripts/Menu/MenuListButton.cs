using System;
using Godot;

namespace Gamma.Core.Scripts.Menu
{
    public class MenuListButton : Label
    {
        public int Order = -1;
        public Action ClickEvent;

        [Signal]
        public delegate void Selected(int order);

        private const string FontColorProperty = "custom_colors/font_color";

        private string _hoverColor = "#c0c11e";
        private string _defaultColor = "#FFFFFF";

        private bool _selected;
        private bool _mouseHasntLeft;
        
        public override void _Ready()
        {
            // allows signals to be emit
            this.SetMouseFilter(MouseFilterEnum.Stop);

            this.Connect("mouse_exited", this, nameof(HandleMouseExit));
            this.Connect("mouse_entered", this, nameof(HandleMouseOver));
        }

        public override void _Process(float delta)
        {
        }

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton mouse && mouse.ButtonIndex == (int) ButtonList.Left && @event.IsPressed())
            {
                HandleButtonEvent();
            }
        }

        public void SetFontColor(string hex)
        {
            this.Set(FontColorProperty, hex);
        }

        public void HandleButtonEvent()
        {
            if(ClickEvent != null && _selected && _mouseHasntLeft)
            {
                ClickEvent.Invoke();
            }
        }

        public void HandleMouseOver()
        {
            _mouseHasntLeft = true;
            EmitSignal(nameof(Selected), Order);
        }

        public void HandleMouseExit()
        {
            _mouseHasntLeft = false;
        }

        public void SelectButton()
        {
            _selected = true;
            SetFontColor(_hoverColor);
        }

        public void DeselectButton()
        {
            _selected = false;
            SetFontColor(_defaultColor);
        }
    }
}