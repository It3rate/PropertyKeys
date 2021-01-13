using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components;
using Motive.SeriesData;
using Motive.Commands;
using Motive.Graphic;
using Motive.Samplers;
using Motive.Stores;

namespace Motive.Tests.GraphicTests
{
    class LayoutTest : ITestScreen
    {
	    private readonly Runner _runner;

	    public LayoutTest(Runner runner)
	    {
		    _runner = runner;
	    }

	    public void NextVersion()
	    {
            var props = new LayoutElementProperties[]
	            {
		            new LayoutElementProperties(0.3f,0.2f,0.4f,0.1f),
		            new LayoutElementProperties(0.5f,0.8f,0.2f,0.3f),
		            new LayoutElementProperties(0.9f,0.5f,0.1f,0.4f),
                }
            ;
			LayoutContainer lc = new LayoutContainer(props);
			var loc = new RectFSeries(250f, 100f, 650f, 400f).Store();
			lc.AddProperty(PropertyId.Location, loc);
            Runner.CurrentRunner.ActivateComposite(lc.Id);
	    }
    }
}
