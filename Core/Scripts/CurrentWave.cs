using Godot;

namespace Gamma.Core.Scripts
{
    public class CurrentWave : Control
    {
        [Signal]
        public delegate void WaveStart();
        
        private Timer _blinkTimer;
        private Timer _countDownTimer;

        private Label _waveCounterLabel;

        public int WaveLength = 5;
        private int _currentTime;
        private bool _blinkToggle = true;

        public override void _Ready()
        {
            _currentTime = WaveLength;

            _waveCounterLabel = GetNode<Label>("HBoxContainer/VBoxContainer/CenterContainer/VBoxContainer/Num");

            _blinkTimer = GetNode<Timer>("BlinkTimer");
            _blinkTimer.Connect("timeout", this, nameof(ToggleBlink));
            
            _countDownTimer = GetNode<Timer>("CountDownTimer");
            _countDownTimer.Connect("timeout", this, nameof(CountDown));
            
            _waveCounterLabel.Text = _currentTime.ToString();
        }

        public void Start()
        {
            _currentTime = WaveLength;
            _waveCounterLabel.Text = _currentTime.ToString();
            Show();
            _countDownTimer.Start();
            _blinkTimer.Start();
        }

        public void ToggleBlink()
        {
            _waveCounterLabel.Visible = !_blinkToggle;
            _blinkToggle = !_blinkToggle;
        }

        public void CountDown()
        {
            if(_currentTime == 1)
            {
                _waveCounterLabel.Text = "GO!";
                --_currentTime;
                return;
            }

            if(_currentTime <= 0)
            {
                Hide();
                _blinkTimer.Stop();
                _countDownTimer.Stop();
                _currentTime = WaveLength;
                _waveCounterLabel.Text = _currentTime.ToString();
                EmitSignal(nameof(WaveStart));
                return;
            }

            if(_currentTime > 0)
            {
                --_currentTime;
                _waveCounterLabel.Text = _currentTime.ToString();
            }
        }
    }
}