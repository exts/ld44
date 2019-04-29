using Godot;

namespace Gamma.Core.Scripts
{
    public class CreditsMenu : Control
    {
//        private Timer _creditsTimer;
        private RichTextLabel _creditsRichText;

        private int _linkSeek;
        private int _maxSeekLines;

        public override void _Ready()
        {
            _creditsRichText = GetNode<RichTextLabel>("CreditsContainer/CenterContainer/VBoxContainer/Credits");

            // make sure bbcode is enabled & set the credits value
            _creditsRichText.BbcodeEnabled = true;
            _creditsRichText.SetBbcode(Game.Autoload().Credits);
            _creditsRichText.Connect("meta_clicked", this, nameof(Clicked));
            
            _linkSeek = _creditsRichText.GetVisibleLineCount();
            _maxSeekLines = _creditsRichText.GetLineCount() - 1;

            // was trying to use scrolling credits, but it sucked.
//            _creditsTimer = GetNode<Timer>("CreditsTimer");
//            _creditsTimer.Connect("timeout", this, nameof(ScrollCredits));
        }

        public override void _Input(InputEvent @event)
        {
            if(Input.IsActionPressed("ui_cancel"))
            {
                SceneSwitcher.Switch(Scenes.MainMenu);
            }
        }

        public void Clicked(string link)
        {
            OS.ShellOpen(link);
        }

//        public void ScrollCredits()
//        {
//            _linkSeek += 1;
//            _linkSeek = Mathf.Clamp(_linkSeek, 0, _maxSeekLines);
//            _creditsRichText.ScrollToLine(_linkSeek);
//
//            if(_linkSeek != _maxSeekLines) return;
//            
//            _linkSeek = _creditsRichText.GetVisibleLineCount();
//            _creditsTimer.Stop();
//        }
    }
}