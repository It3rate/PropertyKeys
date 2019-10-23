﻿using DataArcs.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components;
using DataArcs.Components.ExternalInput;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Tests.GraphicTests
{
    public class UserInputTest : ITestScreen
    {
	    private readonly Player _player;
	    private MouseInput _mouseInput;

        public UserInputTest(Player player)
	    {
		    _player = player;
	    }

	    public void NextVersion()
	    {
		    _mouseInput = new MouseInput();
			_player.AddActiveElement(_mouseInput);

            IComposite comp = GetHexGrid();
		    _player.AddActiveElement(comp);


		    //comp.EndTimedEvent += CompOnEndTransitionEvent;
	    }

	    private void CompOnEndTransitionEvent(object sender, EventArgs e)
	    {
		    BlendTransition bt = (BlendTransition)sender;
		    bt.Runner.Reverse();
		    bt.Runner.Restart();
	    }

        IComposite GetHexGrid()
        {
	        var composite = new Container(Store.CreateItemStore(56));

	        Store loc = new Store(new FloatSeries(2, 200f, 100f, 600f, 400f), new HexagonSampler(new int[] { 7, 9 }));
	        composite.AddProperty(PropertyId.Location, loc);
	        composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 1f, 1f, 0.1f).Store);
			
	        var mouseStore = new Store(new FloatSeries(1, 5f, 32f));
	        var mouseLink = new LinkingStore(_mouseInput.CompositeId, PropertyId.SampleAtT, SeriesUtils.X, mouseStore);
            composite.AddProperty(PropertyId.Radius, mouseLink);
	        var mouseOrientationStore = new Store(new FloatSeries(1, 0f, 1f));
	        var mouseOrient = new LinkingStore(_mouseInput.CompositeId, PropertyId.SampleAtT, SeriesUtils.Y, mouseOrientationStore);
            composite.AddProperty(PropertyId.Orientation, mouseOrient);


            composite.AddProperty(PropertyId.PointCount, new IntSeries(1, 5).Store);
	        composite.AddProperty(PropertyId.FillColor, new FloatSeries(3, 0.5f, 0, 0,  0, 0.5f, 0.5f).Store);
            composite.Renderer = new PolyShape();


	        return composite;
        }

        private static void AddGraphic(Container container)
        {
	        container.AddProperty(PropertyId.Radius, new FloatSeries(2, 10f, 10f).Store);
	        container.AddProperty(PropertyId.PointCount, new IntSeries(1, 4, 8).Store);
	        container.AddProperty(PropertyId.PenColor, new FloatSeries(3, 0.5f, 0, 0, 0f, 0, 0.5f).Store);

	        var lineStore = new Store(new FloatSeries(1, .05f, .2f), new LineSampler(), CombineFunction.Multiply);
	        var lineLink = new LinkingStore(container.CompositeId, PropertyId.Radius, SeriesUtils.X, lineStore);
	        container.AddProperty(PropertyId.PenWidth, lineLink);
	        container.Renderer = new PolyShape();
        }

    }
}
