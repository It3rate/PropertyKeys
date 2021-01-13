using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Graphic;
using Motive.SeriesData;
using Motive.Components;
using Motive.Stores;

namespace Motive.Components
{
	public struct LayoutElementProperties
	{
        public float[] Position;
		public float[] Area;
        //public float[] Spacing;
        //public float[] Compressibility;
        //public float[] Prominence;
        //public float[] Legibility;
        //public float[] Elevation;
        //public float[] HueHints;
        public LayoutElementProperties(float posX, float posY, float areaW, float areaH)
		{
			Position = new[] { posX, posY };
			Area = new[] { areaW, areaH };
		}
    }
    public class LayoutContainer : Container
    {
	    private LayoutElementProperties[] LayoutProps;
        public float[] Vibrancy;
		public float[] Similarity;
		public float[] Density;
		public float[] Entropy;

		private LayoutElementProperties[] _speeds;
		private Random _rand = new Random();

        public LayoutContainer(LayoutElementProperties[] layoutElementProperties)
		{
			LayoutProps = layoutElementProperties;
			_speeds = new LayoutElementProperties[LayoutProps.Length];
			for (int i = 0; i < LayoutProps.Length; i++)
			{
				_speeds[i] = new LayoutElementProperties(0,0,0,0);
			}

		    var box = new UIBox();
		    int len = layoutElementProperties.Length;
			int[] itemIds = new int[len];
            for (int i = 0; i < layoutElementProperties.Length; i++)
		    {
				var element = new Container(null, this, box);
				element.AddProperty(PropertyId.Location, new FloatSeries(2, 0, 0).Store());
				element.AddProperty(PropertyId.Size, new FloatSeries(2, 1, 1).Store());
				element.AddProperty(PropertyId.Origin, new FloatSeries(2, 0, 0).Store());
				element.AddProperty(PropertyId.Scale, new FloatSeries(2, 1, 1).Store());
                element.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, i / (float)len, 1f - i / (float)len).Store());
                AddChild(element);
				itemIds[i] = element.Id;
		    }
		    AddProperty(PropertyId.Items, new IntSeries(1, itemIds).Store());
	    }
	    public override void StartUpdate(double currentTime, double deltaTime)
	    {
            base.StartUpdate(currentTime, deltaTime);

            var selfLoc = GetStore(PropertyId.Location).GetSeriesRef().FloatDataRef;

            for (int i = 0; i < _children.Count; i++)
		    {
			    var child = Runner.CurrentComposites[_children[i]];
			    var loc = child.GetStore(PropertyId.Location).GetSeriesRef().FloatDataRef;
			    loc[0] = LayoutProps[i].Position[0];
			    loc[1] = LayoutProps[i].Position[1];
			    LayoutProps[i].Position[0] = Math.Abs((LayoutProps[i].Position[0] + _speeds[i].Position[0]) % 1);
			    LayoutProps[i].Position[1] = Math.Abs((LayoutProps[i].Position[1] + _speeds[i].Position[1]) % 1);

                var sz = child.GetStore(PropertyId.Size).GetSeriesRef().FloatDataRef;
                sz[0] = Math.Abs((LayoutProps[i].Area[0] + _speeds[i].Area[0]) % 1);
                sz[1] = Math.Abs((LayoutProps[i].Area[1] + _speeds[i].Area[1]) % 1);

                var origin = child.GetStore(PropertyId.Origin).GetSeriesRef().FloatDataRef;
                origin[0] = selfLoc[0];
                origin[1] = selfLoc[1];
                var scale = child.GetStore(PropertyId.Scale).GetSeriesRef().FloatDataRef;
                scale[0] = selfLoc[2] - selfLoc[0];
                scale[1] = selfLoc[3] - selfLoc[1];
            }

            for (int i = 0; i < _speeds.Length; i++)
            {
	            for (int j = 0; j < _speeds[i].Position.Length; j++)
	            {
		            _speeds[i].Position[j] = 0.9f * (_speeds[i].Position[j] + _rand.Next(-1000, 1000) / 1000000f);
		            _speeds[i].Area[j] += 0.9f * (_rand.Next(-1000, 1000) / 100000f);
	            }
            }
	    }

	    public override void Draw(Graphics g, Dictionary<PropertyId, Series> dict)
	    {
		    var selfLoc = GetStore(PropertyId.Location).GetSeriesRef().FloatDataRef;
            g.DrawRectangle(new Pen(Brushes.Beige, 1), new Rectangle((int)selfLoc[0], (int)selfLoc[1], 
	            (int)(selfLoc[2] - selfLoc[0]), (int)(selfLoc[3] - selfLoc[1])));
		    for (int i = 0; i < _children.Count; i++)
		    {
			    var child = Runner.CurrentComposites[_children[i]];
			    if (child is IDrawable drawable)
			    {
				    drawable.Draw(g, new Dictionary<PropertyId, SeriesData.Series>());
			    }
            }
			
        }
    }
}
