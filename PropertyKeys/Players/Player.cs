using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using Timer = System.Timers.Timer;

namespace DataArcs.Players
{
    public class Player
    {
        private static Player _currentPlayer;
        public static Player GetPlayerById(int id) => _currentPlayer;

        private static DateTime _pauseTime;
        private static TimeSpan _delayTime = new TimeSpan(0);
        public static DateTime StartTime { get; private set; }
        static Player() { StartTime = DateTime.Now;}

	    private readonly Dictionary<int, IComposite> _allComposites = new Dictionary<int, IComposite>();

	    private readonly Dictionary<int, IComposite> _activeElements = new Dictionary<int, IComposite>();
        private readonly Dictionary<int, IComposite> _toAddActive = new Dictionary<int, IComposite>();
        private readonly List<int> _toRemoveActive = new List<int>();
        private readonly List<int> _toDestroy = new List<int>();

        private readonly Form _display;
        private Timer _timer;
        private TimeSpan _lastTime;
        private TimeSpan _currentTime;
        public float CurrentMs => (float)_currentTime.TotalMilliseconds;
		

        public Player(Form display)
		{
            _currentPlayer = this;
			_display = display;
			Initialize();
		}

        private void Initialize()
        {
	        _currentTime = DateTime.Now - StartTime;
	        _lastTime = _currentTime;

            _timer = new Timer();
			_timer.Elapsed += Tick;
			_timer.Interval = 8;
			_timer.Enabled = true;

			_display.Paint += OnDraw;
        }

        //private float t = 0;
        private void Tick(object sender, ElapsedEventArgs e)
        {
            if (!_isPaused)
            {

                for (int i = 0; i < _toRemoveActive.Count; i++)
                {
                    _activeElements.Remove(_toRemoveActive[i]);
                }
                for (int i = 0; i < _toDestroy.Count; i++)
                {
                    _activeElements.Remove(_toDestroy[i]);
                    _allComposites.Remove(_toDestroy[i]);
                }

                foreach (var item in _toAddActive)
                {
                    _activeElements.Add(item.Key, item.Value);
                }
                _toAddActive.Clear();
                _toRemoveActive.Clear();
                _toDestroy.Clear();

                _currentTime = e.SignalTime - (StartTime + _delayTime);
                float dt = (float)(_currentTime - _lastTime).TotalMilliseconds;
                foreach (var element in _activeElements.Values)
                {
                    element.Update(CurrentMs, dt);
                }
                _display.Invalidate();

                _lastTime = _currentTime;
            }
        }

        private void OnDraw(object sender, PaintEventArgs e)
        {
	        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var elements = new List<IComposite>(_activeElements.Values);
	        foreach (var element in elements)
	        {
		        element.Draw(element, e.Graphics);
	        }
        }

        public void AddActiveElement(IComposite composite) => _toAddActive.Add(composite.CompositeId, composite);
        public void RemoveActiveElement(IComposite composite)
        {
	        if (_activeElements.ContainsKey(composite.CompositeId))
	        {
		        _toRemoveActive.Add(composite.CompositeId);
	        }
        }
        public void RemoveActiveElementById(int id)
        {
	        if (_activeElements.ContainsKey(id))
	        {
		        _toRemoveActive.Add(id);
	        }
        }
        public void Clear() => _toRemoveActive.AddRange(_activeElements.Keys);

        public IComposite this[int index] => _allComposites.ContainsKey(index) ? _allComposites[index] : null;
        public void AddCompositeToLibrary(IComposite composite)
        {
			// todo: use ref counting to remove dead elements.
			_allComposites.Add(composite.CompositeId, composite);
        }
        public void Reset()
        {
            Clear();
	        _toDestroy.AddRange(_allComposites.Keys);
        }

        private bool _isPaused;
        public void OnPause(object sender, EventArgs e)
        {
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                _pauseTime = DateTime.Now;
            }
            else
            {
                _delayTime += DateTime.Now - _pauseTime;
            }
        }

    }
}
