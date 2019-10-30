using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Components.Transitions;
using DataArcs.SeriesData;
using Timer = System.Timers.Timer;

namespace DataArcs.Players
{
    public class Player
    {
        private static Player _currentPlayer;
        public static Player GetPlayerById(int id) => _currentPlayer;

        private readonly Form _display;

	    private readonly Dictionary<int, IComposite> _allComposites = new Dictionary<int, IComposite>();
        private readonly List<int> _activeElementIds = new List<int>();
        private readonly Dictionary<int, IComposite> _toAddActive = new Dictionary<int, IComposite>();
        private readonly List<int> _toRemoveActive = new List<int>();
        private readonly List<int> _toDestroy = new List<int>();
        private bool _needsDestroy = false;
        private bool _canDestroy = true;

        private static DateTime _pauseTime;
        private static TimeSpan _delayTime = new TimeSpan(0);
        public static DateTime StartTime { get; private set; }
        static Player() { StartTime = DateTime.Now;}

        private Timer _timer;
        private TimeSpan _lastTime;
        private TimeSpan _currentTime;
        public float CurrentMs => (float)_currentTime.TotalMilliseconds;

        public IComposite this[int index] => _allComposites.ContainsKey(index) ? _allComposites[index] : null;

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
                    int id = _toRemoveActive[i];
                    if (_allComposites.ContainsKey(id))
                    {
                        _allComposites[id].OnDeactivate();
                    }
                    _activeElementIds.Remove(id);
                }
                if (_canDestroy)
                {
                    for (int i = 0; i < _toDestroy.Count; i++)
                    {
                        int id = _toDestroy[i];
                        _activeElementIds.Remove(id);
                        _allComposites.Remove(id);
                    }
                    _needsDestroy = false;
                }

                foreach (var item in _toAddActive)
                {
                    // todo: activeElements should allow multiple copies, currently unable to distinguish them with deletion
                    _activeElementIds.Add(item.Key);
                    item.Value.OnActivate();
                }
                _toAddActive.Clear();
                _toRemoveActive.Clear();
                _toDestroy.Clear();

                _currentTime = e.SignalTime - (StartTime + _delayTime);
                float dt = (float)(_currentTime - _lastTime).TotalMilliseconds;
                foreach (var id in _activeElementIds)
                {
                    if (_allComposites.ContainsKey(id))
                    {
                        _allComposites[id].Update(CurrentMs, dt);
                    }
                }
                _display.Invalidate();

                _lastTime = _currentTime;
            }
        }

        private void OnDraw(object sender, PaintEventArgs e)
        {
            if (!_needsDestroy)
            {
                _canDestroy = false;
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    var elementIds = new List<int>(_activeElementIds);
                    foreach (var id in elementIds)
                    {
                        if (_allComposites.ContainsKey(id))
                        {
                            var element = _allComposites[id];
                            if (element is IDrawable drawable)
                            {
                                drawable.Draw(e.Graphics, new Dictionary<PropertyId, SeriesData.Series>());
                            }
                        }
                    }
                }
            }
            _canDestroy = true;
        }

        public void AddActiveElement(IComposite composite)
        {
	        if (composite is ITimeable anim)
	        {
		        anim.StartTime = (float) (DateTime.Now - Player.StartTime).TotalMilliseconds;
	        }

	        _toAddActive.Add(composite.CompositeId, composite);
        }

        public void RemoveActiveElement(IComposite composite)
        {
	        _toRemoveActive.Add(composite.CompositeId);
        }
        public void RemoveActiveElementById(int id)
        {
	        _toRemoveActive.Add(id);
        }
        public void Clear()
        {
            _toRemoveActive.AddRange(_activeElementIds);
        }

        public void AddCompositeToLibrary(IComposite composite)
        {
			// todo: use ref counting to remove dead elements.
			_allComposites.Add(composite.CompositeId, composite);
        }
        public void Reset()
        {
            _needsDestroy = false;
            Clear();
	        _toDestroy.AddRange(_allComposites.Keys);
        }

        private bool _isPaused;

        public void Pause()
        {
	        if (!_isPaused)
	        {
		        OnPause(this, null);
	        }
        }

        public void Unpause()
        {
	        if (_isPaused)
	        {
		        OnPause(this, null);
	        }
        }
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
