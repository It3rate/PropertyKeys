using System;
using System.Collections.Generic;
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
			_mouseInput = new MouseInput();
			_player.AddActiveElement(_mouseInput);

            IComposite comp = GetHexGrid();
            _player.AddActiveElement(comp);

            _physicsComposite = new PhysicsComposite();
			_player.AddActiveElement(_physicsComposite);

			var body = AddCompositeBody();
			_player.AddActiveElement(body);

            //comp.EndTimedEvent += CompOnEndTransitionEvent;
            _player.Unpause();
        }

		IComposite AddCompositeBody()
		{
			var body = new Container(new IntSeries(1, 0).Store);
			LinkingStore ls = new LinkingStore(_physicsComposite.CompositeId, PropertyId.Location, SlotUtils.XY, null);
			body.AddProperty(PropertyId.Location, ls);

			body.AddProperty(PropertyId.Radius, new FloatSeries(1, 30f).Store);
			body.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);
			body.AddProperty(PropertyId.FillColor, new FloatSeries(3,  0.4f, 0.3f, 0.4f).Store);
			body.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
			body.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
			body.Renderer = new PolyShape();
            return body;
		}

		IComposite GetHexGrid()
		{
			var mouseLink = new LinkSampler(_mouseInput.CompositeId, PropertyId.MouseLocationT, SlotUtils.XY);

			var composite = new Container(Store.CreateItemStore(20 * 11));
			Store loc = new Store(MouseInput.MainFrameSize.Outset(-50f), new HexagonSampler(new int[] {16, 10}));
			composite.AppendProperty(PropertyId.Location, loc);

			composite.AddProperty(PropertyId.Radius, new FloatSeries(1, 20f).Store);
			composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 6).Store);
			composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 0.3f, 0.4f, 0.3f, 0.4f, 1f).Store);
			composite.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.2f, 0.1f, 0.1f).Store);
			composite.AddProperty(PropertyId.PenWidth, new FloatSeries(1, 1.5f).Store);
            composite.Renderer = new PolyShape();

			return composite;
		}
	}
}
