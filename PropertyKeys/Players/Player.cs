using System;
using System.Collections.Generic;
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
	    public static DateTime StartTime { get; }
	    static Player() { StartTime = DateTime.Now;}

		private Dictionary<int, CompositeBase> _elements = new Dictionary<int, CompositeBase>();

		private Form _display;
        private Timer _timer;
        private DateTime _lastTime;
        private DateTime _currentTime;

        public Player(Form display)
		{
			_display = display;
			Initialize();
		}

        private void Initialize()
        {
	        _lastTime = new DateTime(0);

            _timer = new Timer();
			_timer.Elapsed += Tick;
			_timer.Interval = 8;
			_timer.Enabled = true;

			_display.Paint += OnDraw;
        }

        public void AddElement(CompositeBase composite)
        {
	        _elements[composite.CompositeId] = composite;
        }
        public void RemoveElement(CompositeBase composite)
        {
	        if (_elements.ContainsKey(composite.CompositeId))
	        {
		        _elements.Remove(composite.CompositeId);
	        }
        }
        public void RemoveElementById(int id)
        {
	        if (_elements.ContainsKey(id))
	        {
		        _elements.Remove(id);
	        }
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
	        if (_lastTime.Ticks == 0)
	        {
		        _lastTime = e.SignalTime;
	        }
            _currentTime = e.SignalTime;

	        foreach (var element in _elements.Values)
	        {
		        element.Update((float)TimeSpan.FromTicks(_currentTime.Ticks).TotalMilliseconds, (float)(_currentTime - _lastTime).TotalMilliseconds);
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

    }
}
