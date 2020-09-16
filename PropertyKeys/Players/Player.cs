using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;
using DataArcs.Components;
using DataArcs.Components.Libraries;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;
using Timer = System.Timers.Timer;

namespace DataArcs.Players
{
    public class Player
    {
        private static Player _currentPlayer;
        public static Player GetPlayerById(int id) => _currentPlayer;

        private readonly Form _display;

        //private Definitions<Series> _series;
        //private Definitions<Sampler> _samplers;
        private Definitions<Store> _stores;
        private Definitions<IComposite> _composites = new Definitions<IComposite>();

        private bool _isPaused;
        private static DateTime _pauseTime;
        private static TimeSpan _delayTime = new TimeSpan(0);
        public static DateTime StartTime { get; private set; }
        static Player() { StartTime = DateTime.Now;}

        private Timer _timer;
        private TimeSpan _lastTime;
        private TimeSpan _currentTime;
        public double CurrentMs => _currentTime.TotalMilliseconds;

        public IComposite this[int index] => _composites[index];

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
        private bool _isBusy = false;
        private void Tick(object sender, ElapsedEventArgs e)
        {
	        if (!_isPaused && !_isBusy)
	        {
		        _isBusy = true;

		        _currentTime = e.SignalTime - (StartTime + _delayTime);
		        double deltaTime = (_currentTime - _lastTime).TotalMilliseconds;
		        _composites.Update(CurrentMs, deltaTime);

		        _display.Invalidate();
		        _lastTime = _currentTime;
	        }
	        _isBusy = false;
        }

        private void OnDraw(object sender, PaintEventArgs e)
        {
	        if (!_composites.NeedsDestroy)
	        {
		        _composites.CanDestroy = false;
		        {
			        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			        var elementIds = _composites.ActiveIdsCopy;
			        foreach (var id in elementIds)
			        {
				        if (_composites.ContainsKey(id))
				        {
					        var element = _composites[id];
					        if (element is IDrawable drawable)
					        {
						        drawable.Draw(e.Graphics, new Dictionary<PropertyId, SeriesData.Series>());
					        }
				        }
			        }
		        }
	        }

	        _composites.CanDestroy = true;
        }

        public void AddActiveElement(IComposite composite)
        {
	        if (composite is ITimeable anim)
	        {
		        anim.StartTime = (float) (DateTime.Now - Player.StartTime).TotalMilliseconds;
	        }
			_composites.AddActiveElement(composite);
        }

        public void RemoveActiveElement(IComposite composite)
        {
	        _composites.RemoveActiveElement(composite);
        }
        public void RemoveActiveElementById(int id)
        {
	        _composites.RemoveActiveElementById(id);
        }
        public void Clear()
        {
	        _composites.Clear();
        }

        public void AddCompositeToLibrary(IComposite composite)
        {
            // todo: use ref counting to remove dead elements.
            _composites.AddToLibrary(composite);
        }
        public void Reset()
        {
            _composites.Reset();
        }


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
                foreach (var id in _composites.ActiveIds)
                {
                    if (_composites.ContainsKey(id) && (_composites[id] is ITimeable))
                    {
                        ((ITimeable)_composites[id]).Pause();
                    }
                }
            }
            else
            {
                _delayTime += DateTime.Now - _pauseTime;
				_lastTime += DateTime.Now - _pauseTime;
                foreach (var id in _composites.ActiveIds)
                {
                    if (_composites.ContainsKey(id) && (_composites[id] is ITimeable))
                    {
                        ((ITimeable)_composites[id]).Resume();
                    }
                }
            }
        }

    }
}
