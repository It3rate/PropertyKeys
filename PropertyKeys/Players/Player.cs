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
		private Dictionary<int, CompositeBase> _elements = new Dictionary<int, CompositeBase>();

		private Form _display;
        private Timer _timer;
        private DateTime _startTime;
        private DateTime _currentTime;

		public Player(Form display)
		{
			_display = display;
			Initialize();
		}

        private void Initialize()
		{
			_startTime = DateTime.Now;
			_currentTime = DateTime.Now;

			_timer = new Timer();
			_timer.Elapsed += Tick;
			_timer.Interval = 8;
			_timer.Enabled = true;

			_display.Paint += OnDraw;
        }

        public void AddElement(CompositeBase composite)
        {
	        _elements.Add(composite.Id, composite);
        }
        public void RemoveElement(CompositeBase composite)
        {
	        if (_elements.ContainsKey(composite.Id))
	        {
		        _elements.Remove(composite.Id);
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
            _currentTime = e.SignalTime;
	        foreach (var element in _elements.Values)
	        {
		        element.Update((float)(_currentTime - _startTime).TotalMilliseconds);
	        }
	        _display.Invalidate();
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
