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
            var elements = new List<CompositeBase>(_elements.Values);
	        foreach (var element in elements)
	        {
		        element.Draw(e.Graphics);
	        }
        }
        public CompositeBase this[int index]
        {
            get
            {
                CompositeBase result = null;
                foreach (var item in _elements)
                {
                    var value = item.Value;
                    if (value.CompositeId == index)
                    {
                        result = value;
                        break;
                    }
                    else
                    {
                        // todo: make element search properly recursive. May want to store unused elements that aren't dead in collection.
                        if (item.Value is BlendTransition)
                        {
                            var bt = (BlendTransition)item.Value;
                            if (bt.Start.CompositeId == index)
                            {
                                result = bt.Start;
                                break;
                            }
                            else if (bt.End.CompositeId == index)
                            {
                                result = bt.End;
                                break;
                            }
                        }
                    }
                }
                
                return result;
            }
        }

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
