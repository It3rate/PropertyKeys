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
	    public float StartTime { get; set; }
	    public Series Duration { get; }
	    public Series Delay { get; }
	    public event TimedEventHandler StartTimedEvent;
	    public event TimedEventHandler StepTimedEvent;
	    public event TimedEventHandler EndTimedEvent;

	    private float _mouseX;
	    private float _mouseY;
	    public RectFSeries MainFrameSize
	    {
		    get
		    {
				var form = Application.OpenForms[0];
				return new RectFSeries(0, 0, form.ClientSize.Width, form.ClientSize.Height);
		    }
	    } 
	    private IComposite _container;

	    public MouseInput(IComposite container = null)
	    {
		    _container = container;
	    }

        public override void OnActivate()
        {
            Application.OpenForms[0].MouseMove += OnMouseMove;
        }
        public override void OnDeactivate()
        {
            Application.OpenForms[0].MouseMove -= OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
	    {
		    _mouseX = args.X;
		    _mouseY = args.Y;
			//Debug.WriteLine(args.X + " : " + args.Y);
        }

	    public override ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT)
	    {
			//todo: accomodate seriesT, maybe?
		    ParametricSeries result;
		    switch (propertyId)
		    {
			    case PropertyId.MouseX:
				    result = new ParametricSeries(1, _mouseX / MainFrameSize.FloatDataAt(2));
				    break;
			    case PropertyId.MouseY:
				    result = new ParametricSeries(1, _mouseY / MainFrameSize.FloatDataAt(3));
				    break;
			    case PropertyId.MouseLocationT:
				    result = new ParametricSeries(2, _mouseX / MainFrameSize.FloatDataAt(2), _mouseY / MainFrameSize.FloatDataAt(3));
                    break;
                case PropertyId.Mouse:
			    case PropertyId.MouseLocation:
                default:
				    result = new ParametricSeries(2, _mouseX / MainFrameSize.FloatDataAt(2), _mouseY/ MainFrameSize.FloatDataAt(3));
				    break;
            }
		    return result;
	    }

	    public override Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
	    {
		    return GetSeriesAtT(propertyId, 1f, null);
	    }
	    public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
	    {
		    ParametricSeries result; // return current mouse atm, eventually will be able to scrub history if saved.
		    switch (propertyId)
		    {
			    case PropertyId.MouseX:
				    result = new ParametricSeries(1, _mouseX);
				    break;
			    case PropertyId.MouseY:
				    result = new ParametricSeries(1, _mouseY);
				    break;
			    case PropertyId.MouseLocation:
				    result = new ParametricSeries(2, _mouseX, _mouseY);
				    break;
			    case PropertyId.EasedT:
			    case PropertyId.EasedTCombined:
			    case PropertyId.SampleAtT:
                case PropertyId.SampleAtTCombined:
                case PropertyId.MouseLocationT:
                    result = new ParametricSeries(2, _mouseX / MainFrameSize.FloatDataAt(2), _mouseY / MainFrameSize.FloatDataAt(3));
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
    }
}
