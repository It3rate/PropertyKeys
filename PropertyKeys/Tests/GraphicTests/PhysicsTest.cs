using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
	public class PhysicsTest : ITestScreen
	{
		private readonly Player _player;
		private MouseInput _mouseInput;
		private PhysicsComposite _physicsComposite;

        public PhysicsTest(Player player)
		{
			_player = player;
			_player.Pause();
		}

		public void NextVersion()
        {
            _player.Pause();

            _mouseInput = new MouseInput();
            _player.AddActiveElement(_mouseInput);

            _physicsComposite = new PhysicsComposite();
            _player.AddActiveElement(_physicsComposite);

            IComposite comp = GetHexGrid();
            _player.AddActiveElement(comp);

   //         var target = AddTargetBody();
   //         _player.AddActiveElement(target);

   //         var body = AddCompositeBody();
			//_player.AddActiveElement(body);

            //comp.EndTimedEvent += CompOnEndTransitionEvent;
            _player.Unpause();
        }

        IComposite AddTargetBody()
        {
            var body = new Container(new IntSeries(1, 0).Store);
            body.AddProperty(PropertyId.Location, new FloatSeries(2, 200f, 100f).Store);
            body.AddProperty(PropertyId.Radius, new FloatSeries(1, 20f).Store);
            body.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);
            body.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 0.1f, 0.1f).Store);
            body.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
            body.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
            body.Renderer = new PolyShape();
            return body;
        }
        IComposite AddCompositeBody()
		{
			var body = new Container(new IntSeries(1, 0).Store);
			LinkingStore ls = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Location, SlotUtils.XY, null);
			body.AddProperty(PropertyId.Location, ls);

			body.AddProperty(PropertyId.Radius, new FloatSeries(1, 20f).Store);
			body.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);
			body.AddProperty(PropertyId.FillColor, new FloatSeries(3,  0.4f, 0.3f, 0.4f).Store);
			body.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
			body.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
			body.Renderer = new PolyShape();
            return body;
		}

        private bool is2D = true;
		IComposite GetHexGrid()
		{
			var mouseLink = new LinkSampler(_mouseInput.CompositeId, PropertyId.MouseLocationT, SlotUtils.XY);
			var cols = 10;
            var rows =  5;
            var capacity = cols * rows;
			var composite = new Container(Store.CreateItemStore(capacity));
			composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 20f).Store);
            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5, 8, 3, 6).Store);
			composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 0.3f, 0.4f, 0.3f, 0.4f, 1f).Store);
			composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
			composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
            composite.Renderer = new PolyShape();

            //var sampler = new HexagonSampler(new int[] { cols, rows });
            var sampler = new RingSampler(new int[] { 25,15,10 });
            Store loc = new Store(MouseInput.MainFrameSize.Outset(-150f, -50f), sampler);
			if (is2D)
			{
				for (int i = 0; i < rows * cols; i++)
				{
                    float tIndex = i / (capacity - 1f);
                    var pos = loc.GetValuesAtIndex(i);
                    var pointCount = composite.GetSeriesAtT(PropertyId.PointCount, tIndex, null);
                    Debug.WriteLine(pointCount.X);
                    var radius = composite.GetSeriesAtT(PropertyId.Radius, tIndex, null);

                    var bezier = PolyShape.GeneratePolyShape(0f, (int)pointCount.X, 0, radius.X, radius.Y, 0);
                    _physicsComposite.CreateBezierBody(pos.X, pos.Y, bezier, false);
				}

				LinkingStore ls = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Location, SlotUtils.XY, null);
				composite.AddProperty(PropertyId.Location, ls);
				LinkingStore lso = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Orientation, SlotUtils.X, null);
				composite.AddProperty(PropertyId.Orientation, lso);
			}
            else
			{
				composite.AppendProperty(PropertyId.Location, loc);
			}



			return composite;
		}
	}
}
