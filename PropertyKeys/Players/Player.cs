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

		private readonly Dictionary<int, CompositeBase> _elements = new Dictionary<int, CompositeBase>();

		private readonly Form _display;
        private Timer _timer;
        private TimeSpan _lastTime;
        private TimeSpan _currentTime;
        public float CurrentMs => (float)_currentTime.TotalMilliseconds;
		
        private readonly Dictionary<int, CompositeBase> _toAdd = new Dictionary<int, CompositeBase>();
        public readonly List<int> _toRemove = new List<int>();

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
	            _elements.Remove(_toRemove[i]);
            }
			_toRemove.Clear();

			foreach (var item in _toAdd)
			{
				_elements.Add(item.Key, item.Value);
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
            foreach (var element in _elements.Values)
            {
	            element.Update(CurrentMs, dt);
            }
	        _display.Invalidate();

	        _lastTime = _currentTime;
        }

        private void OnDraw(object sender, PaintEventArgs e)
        {
	        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
	        foreach (var element in _elements.Values)
	        {
		        element.Draw(e.Graphics);
	        }
        }
        public CompositeBase this[int index] => _elements[index];

        public void AddElement(CompositeBase composite) => _toAdd.Add(composite.CompositeId, composite);
        public void RemoveElement(CompositeBase composite)
        {
	        if (_elements.ContainsKey(composite.CompositeId))
	        {
		        _toRemove.Add(composite.CompositeId);
	        }
        }
        public void RemoveElementById(int id)
        {
	        if (_elements.ContainsKey(id))
	        {
		        _toRemove.Add(id);
	        }
        }
        public void Clear() => _toRemove.AddRange(_elements.Keys);

    }
}
