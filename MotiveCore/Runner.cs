using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;
using Motive.Components;
using Motive.Components.Libraries;
using Motive.Graphic;
using Motive.Samplers.Utils;
using Motive.SeriesData;
using Motive.Stores;
using Timer = System.Timers.Timer;
using Motive;

// #error version

namespace Motive
{
    public class Runner
    {
	    public static Runner CurrentRunner;
        public static Runner GetRunnerById(int id) => CurrentRunner;

        public static Definitions<ISeries> CurrentSeries => CurrentRunner.Series;
        public static Definitions<Sampler> CurrentSamplers => CurrentRunner.Samplers;
        public static Definitions<IStore> CurrentStores => CurrentRunner.Stores;
        public static Definitions<IRenderable> CurrentRenderables => CurrentRunner.Renderables;
        public static Definitions<IComposite> CurrentComposites => CurrentRunner.Composites;

        private readonly Form _display;

        public Definitions<ISeries> Series { get; } = new Definitions<ISeries>();
        public Definitions<Sampler> Samplers { get; } = new Definitions<Sampler>();
        public Definitions<IStore> Stores { get; } = new Definitions<IStore>();
        public Definitions<IRenderable> Renderables { get; } = new Definitions<IRenderable>();
        public Definitions<IComposite> Composites { get; } = new Definitions<IComposite>();

        private bool _isPaused;
        private static DateTime _pauseTime;
        private static TimeSpan _delayTime = new TimeSpan(0);
        public static DateTime StartTime { get; private set; }
        static Runner() { StartTime = DateTime.Now;}

        private Timer _timer;
        private TimeSpan _lastTime;
        private TimeSpan _currentTime;
        public double CurrentMs => _currentTime.TotalMilliseconds;

        public FloatSeries ExternalValue0 = new FloatSeries(1, 0);
        public FloatSeries ExternalValue1 = new FloatSeries(1, 0);

        //public IComposite this[int index] => _composites[index];

        public Runner(Form display)
		{
            CurrentRunner = this;
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

			if (_display != null)
			{
				_display.Paint += OnDraw;
			}
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
		        Composites.Update(CurrentMs, deltaTime);

		        if (_display != null)
		        {
			        _display.Invalidate();
		        }
		        _lastTime = _currentTime;
	        }
	        _isBusy = false;
        }

        private void OnDraw(object sender, PaintEventArgs e)
        {
	        if (!Composites.NeedsDestroy)
	        {
		        Composites.CanDestroy = false;
		        {
			        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			        var activeIds = Composites.ActiveIdsCopy;
			        foreach (var id in activeIds)
			        {
				        if (Composites.ContainsKey(id))
				        {
					        var element = Composites[id];
					        if (element is IDrawable drawable)
					        {
						        drawable.Draw(e.Graphics, new Dictionary<PropertyId, ISeries>());
					        }
				        }
			        }
		        }
	        }

	        Composites.CanDestroy = true;
        }

        public void ActivateComposite(int id)
        {
	        var composite = Composites[id];
	        if (composite != null)
	        {
		        if (composite is ITimeable anim)
		        {
			        anim.StartTime = (float) (DateTime.Now - Runner.StartTime).TotalMilliseconds;
		        }

		        Composites.ActivateElement(composite.Id);
	        }
        }
        public void DeactivateComposite(int id)
        {
	        Composites.DeactivateElement(id);
        }

        public void Clear()
        {
	        Composites.Clear();
			Stores.Clear();
			Samplers.Clear();
			Series.Clear();
        }
		
        public void Reset()
        {
            Composites.Reset();
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
                foreach (var id in Composites.ActiveIds)
                {
                    if (Composites.ContainsKey(id) && (Composites[id] is ITimeable))
                    {
                        ((ITimeable)Composites[id]).Pause();
                    }
                }
            }
            else
            {
                _delayTime += DateTime.Now - _pauseTime;
                _lastTime = DateTime.Now - (StartTime + _delayTime);
                foreach (var id in Composites.ActiveIds)
                {
                    if (Composites.ContainsKey(id) && (Composites[id] is ITimeable))
                    {
                        ((ITimeable)Composites[id]).Resume();
                    }
                }
            }
        }

        public static RectFSeries MainFrameRect
        {
	        get
	        {
		        RectFSeries result;
		        if (Application.OpenForms.Count > 0)
		        {
			        var form = Application.OpenForms[0];
			        result = new RectFSeries(0, 0, form.ClientSize.Width, form.ClientSize.Height);
		        }
		        else
		        {
			        result = new RectFSeries(0, 0, 800, 600);
		        }
		        return result;
	        }
        }


    }
}
