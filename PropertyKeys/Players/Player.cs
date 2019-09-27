using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Transitions;
using Timer = System.Timers.Timer;

namespace DataArcs.Players
{
    public class Player
    {
        private static Player _currentPlayer;
        public static Player GetPlayerById(int id) => _currentPlayer;

	    public static DateTime StartTime { get; }
	    static Player() { StartTime = DateTime.Now;}

	    private readonly Dictionary<int, IComposite> _allComposites = new Dictionary<int, IComposite>();

	    private readonly Dictionary<int, IComposite> _activeElements = new Dictionary<int, IComposite>();
        private readonly Dictionary<int, IComposite> _toAdd = new Dictionary<int, IComposite>();
        private readonly List<int> _toRemove = new List<int>();

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

        private float t = 0;
        private void Tick(object sender, ElapsedEventArgs e)
        {
            _currentTime = e.SignalTime - StartTime;

            for (int i = 0; i < _toRemove.Count; i++)
            {
	            _activeElements.Remove(_toRemove[i]);
            }
			_toRemove.Clear();

			foreach (var item in _toAdd)
			{
				_activeElements.Add(item.Key, item.Value);
			}
			_toAdd.Clear();

            t += 0.01f;
            var floorT = (int)t;
            float time = t - floorT;
            if (floorT % 2 == 0)
            {
	            time = 1.0f - time;
            }

            float dt = (float) (_currentTime - _lastTime).TotalMilliseconds;
            foreach (var element in _activeElements.Values)
            {
	            element.Update(CurrentMs, dt);
            }
	        _display.Invalidate();

	        _lastTime = _currentTime;
        }

        private void OnDraw(object sender, PaintEventArgs e)
        {
	        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var elements = new List<IComposite>(_activeElements.Values);
	        foreach (var element in elements)
	        {
		        if (element is IDrawable drawable)
		        {
					drawable.Draw(e.Graphics);
		        }
	        }
        }

        public void AddActiveElement(IComposite composite) => _toAdd.Add(composite.CompositeId, composite);
        public void RemoveActiveElement(IComposite composite)
        {
	        if (_activeElements.ContainsKey(composite.CompositeId))
	        {
		        _toRemove.Add(composite.CompositeId);
	        }
        }
        public void RemoveActiveElementById(int id)
        {
	        if (_activeElements.ContainsKey(id))
	        {
		        _toRemove.Add(id);
	        }
        }
        public void Clear() => _toRemove.AddRange(_activeElements.Keys);

        public IComposite this[int index] => _allComposites[index];
        public void AddCompositeToLibrary(IComposite composite)
        {
			_allComposites.Add(composite.CompositeId, composite);
        }
        public void Reset()
        {
			Clear();
			_allComposites.Clear();
        }

    }
}
