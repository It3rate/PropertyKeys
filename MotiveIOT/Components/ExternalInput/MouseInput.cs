using System;
using System.Windows.Forms;
using Motive.SeriesData;

namespace Motive.Components.ExternalInput
{
    public class MouseInput : BaseComposite, ITimeable
    {
	    public float InterpolationT { get; set; }
		public int ClickCount { get; private set; }
	    public double StartTime { get; set; }
	    public ISeries Duration { get; } = new FloatSeries(1,0);
	    public ISeries Delay { get; } = new FloatSeries(1, 0);
        public event TimedEventHandler StartTimedEvent;
	    public event TimedEventHandler StepTimedEvent;
	    public event TimedEventHandler EndTimedEvent;

	    private float _mouseX;
	    private float _mouseY;
	    public Action MouseClick { get; set; }
        private IComposite _container;

        public MouseInput(IComposite container = null)
	    {
		    _container = container;
	    }

        public override void OnActivate()
        {
	        Application.OpenForms[0].MouseMove += OnMouseMove;
	        Application.OpenForms[0].MouseClick += OnMouseClick;
	        StartTimedEvent?.Invoke(this, EventArgs.Empty);
        }
        public override void OnDeactivate()
        {
            Application.OpenForms[0].MouseMove -= OnMouseMove;
            Application.OpenForms[0].MouseClick -= OnMouseClick;
            EndTimedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
	        _mouseX = args.X;
	        _mouseY = args.Y;
            //Debug.WriteLine(args.X + " : " + args.Y);
            StepTimedEvent?.Invoke(this, EventArgs.Empty);
        }
        private void OnMouseClick(object sender, MouseEventArgs args)
        {
	        ClickCount++;
	        MouseClick?.Invoke();
        }

        public override ParametricSeries GetNormalizedPropertyAtT(PropertyId propertyId, ParametricSeries seriesT)
	    {
			//todo: accomodate seriesT, maybe?
		    ParametricSeries result;
		    var rect = Runner.MainFrameRect;

            switch (propertyId)
		    {
			    case PropertyId.MouseX:
				    result = new ParametricSeries(1, _mouseX / rect.FloatValueAt(2));
				    break;
			    case PropertyId.MouseY:
				    result = new ParametricSeries(1, _mouseY / rect.FloatValueAt(3));
				    break;
			    case PropertyId.MouseLocationT:
				    result = new ParametricSeries(2, _mouseX / rect.FloatValueAt(2), _mouseY / rect.FloatValueAt(3));
                    break;
			    case PropertyId.MouseClickCount:
				    result = new ParametricSeries(1, ClickCount);
				    break;
                case PropertyId.Mouse:
			    case PropertyId.MouseLocation:
                default:
				    result = new ParametricSeries(2, _mouseX / rect.FloatValueAt(2), _mouseY/ rect.FloatValueAt(3));
				    break;
            }
		    return result;
	    }

	    public override ISeries GetSeriesAtT(PropertyId propertyId, float t, ISeries parentSeries)
	    {
		    FloatSeries result; // return current mouse atm, eventually will be able to scrub history if saved.
		    switch (propertyId)
		    {
			    case PropertyId.MouseX:
				    result = new FloatSeries(1, _mouseX);
				    break;
			    case PropertyId.MouseY:
				    result = new FloatSeries(1, _mouseY);
				    break;
			    case PropertyId.MouseLocation:
				    result = new FloatSeries(2, _mouseX, _mouseY);
				    break;
                case PropertyId.MouseClickCount:
	                result = new FloatSeries(1, (float)ClickCount);
	                break;
			    case PropertyId.EasedT:
			    case PropertyId.EasedTCombined:
			    case PropertyId.SampleAtT:
                case PropertyId.SampleAtTCombined:
                case PropertyId.MouseLocationT:
                    result = new ParametricSeries(2, _mouseX / Runner.MainFrameRect.FloatValueAt(2), _mouseY / Runner.MainFrameRect.FloatValueAt(3));
				    break;
                case PropertyId.Mouse:
			    default:
				    result = new ParametricSeries(2, _mouseX, _mouseY);
				    break;
		    }
		    return result;
        }

        
        public void Restart()
	    {
		    throw new NotImplementedException();
	    }

	    public void Reverse()
	    {
		    throw new NotImplementedException();
	    }

        public void Pause()
        {

        }
        public void Resume()
        {

        }
    }
}
