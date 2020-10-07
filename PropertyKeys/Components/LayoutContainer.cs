using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public struct LayoutElementProperties
	{
        public float[] Position;
		public float[] Area;
		//public float[] Spacing;
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

		public LayoutContainer(LayoutElementProperties[] layoutElementProperties)
		{
			LayoutProps = layoutElementProperties;

		    var box = new UIBox();
		    int len = layoutElementProperties.Length;
			int[] itemIds = new int[len];
            for (int i = 0; i < layoutElementProperties.Length; i++)
		    {
				var element = new Container(null, this, box);
				element.AddProperty(PropertyId.Location, new FloatSeries(2, 0, 0).Store());
				element.AddProperty(PropertyId.Size, new FloatSeries(2, 1, 1).Store());
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
			    var child = Player.CurrentComposites[_children[i]];
			    var loc = child.GetStore(PropertyId.Location).GetSeriesRef().FloatDataRef;
			    loc[0] = LayoutProps[i].Position[0];
			    loc[1] = LayoutProps[i].Position[1];
			    LayoutProps[i].Position[0] = (LayoutProps[i].Position[0] + 0.001f * (i+1f)) % 1;
			    LayoutProps[i].Position[1] = (LayoutProps[i].Position[1] + 0.003f * (LayoutProps[i].Position[0])) % 1;

                var sz = child.GetStore(PropertyId.Size).GetSeriesRef().FloatDataRef;
                sz[0] = LayoutProps[i].Area[0];
                sz[1] = LayoutProps[i].Area[1];

                var scale = child.GetStore(PropertyId.Scale).GetSeriesRef().FloatDataRef;
				scale[0] = selfLoc[2] - selfLoc[0];
				scale[1] = selfLoc[3] - selfLoc[1];
		    }
	    }

	    public override void Draw(Graphics g, Dictionary<PropertyId, Series> dict)
	    {
		    for (int i = 0; i < _children.Count; i++)
		    {
			    var child = Player.CurrentComposites[_children[i]];
			    if (child is IDrawable drawable)
			    {
				    drawable.Draw(g, new Dictionary<PropertyId, SeriesData.Series>());
			    }
            }
			
        }
    }
}
