using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;
using System.Windows.Forms;

namespace DataArcs.Components.ExternalInput
{
    public class MouseInput : BaseComposite, ITimeable
    {
	    public float InterpolationT { get; set; }
		public int ClickCount { get; private set; }
	    public double StartTime { get; set; }
	    public Series Duration { get; } = new FloatSeries(1,0);
	    public Series Delay { get; } = new FloatSeries(1, 0);
        public event TimedEventHandler StartTimedEvent;
	    public event TimedEventHandler StepTimedEvent;
	    public event TimedEventHandler EndTimedEvent;

	    private float _mouseX;
	    private float _mouseY;
	    public Action MouseClick { get; set; }
        private IComposite _container;

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

        public override ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT)
	    {
			//todo: accomodate seriesT, maybe?
		    ParametricSeries result;
		    switch (propertyId)
		    {
			    case PropertyId.MouseX:
				    result = new ParametricSeries(1, _mouseX / MainFrameRect.FloatDataAt(2));
				    break;
			    case PropertyId.MouseY:
				    result = new ParametricSeries(1, _mouseY / MainFrameRect.FloatDataAt(3));
				    break;
			    case PropertyId.MouseLocationT:
				    result = new ParametricSeries(2, _mouseX / MainFrameRect.FloatDataAt(2), _mouseY / MainFrameRect.FloatDataAt(3));
                    break;
			    case PropertyId.MouseClickCount:
				    result = new ParametricSeries(1, ClickCount);
				    break;
                case PropertyId.Mouse:
			    case PropertyId.MouseLocation:
                default:
				    result = new ParametricSeries(2, _mouseX / MainFrameRect.FloatDataAt(2), _mouseY/ MainFrameRect.FloatDataAt(3));
				    break;
            }
		    return result;
	    }

	    public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
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
                    result = new ParametricSeries(2, _mouseX / MainFrameRect.FloatDataAt(2), _mouseY / MainFrameRect.FloatDataAt(3));
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
