using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using Math = System.Math;

namespace DataArcs.Components.ExternalInput
{
    public class PhysicsComposite : BaseComposite
    {
	    private World _world;
	    private RectFSeries _simBounds;

	    public override int Capacity { get => _world.GetBodyCount(); set{} }

	    public PhysicsComposite()
	    {
			//todo: set a pixels per meter ratio, have auto converters to FloatSeries (also account for inverted Y).
		    RectFSeries appBounds = MouseInput.MainFrameSize;
		    _simBounds = appBounds.Outset(-20f);
            // box2d aabb is LeftBottom (lowerBound) and RightTop(upperBound)
            AABB bounds = new AABB();
			bounds.LowerBound = new Vec2(appBounds.Left, appBounds.Bottom - _simBounds.Bottom);
			bounds.UpperBound = new Vec2(appBounds.Right, _simBounds.Bottom);
            _world = new World(bounds, new Vec2(0,-10), true);

			CreateGround();
			CreateBody(_simBounds.CX, _simBounds.CY);

	    }

	    public override void StartUpdate(float currentTime, float deltaTime)
	    {
            base.StartUpdate(currentTime, deltaTime);

		    //float timeStep = 1.0f / 60.0f;
		    int velocityIterations = 8;
		    int positionIterations = 1;
		    _world.Step(deltaTime/1000f, velocityIterations, positionIterations);
	    }
		
	    public override Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
	    {
		    Series result = null;
            Body body = GetBodyAtIndex(index);
            if (body != null)
            {
	            switch (propertyId)
	            {
		            case PropertyId.Location:
			            Vec2 pos = body.GetPosition();
			            result = new FloatSeries(2, pos.X, _simBounds.Height - pos.Y);
			            break;
		            case PropertyId.Orientation:
			            float normAngle = body.GetAngle() / (float)(Math.PI * 2.0f);
			            result = new FloatSeries(1, normAngle);
			            break;
                }
            }

            if (parentSeries != null)
		    {
			    if (result != null)
			    {
				    //result.CombineInto(parentSeries, store.CombineFunction, t);
			    }
			    else
			    {
				    result = parentSeries;
			    }
		    }

		    return result;
	    }
	    public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
			int index = SamplerUtils.IndexFromT(Capacity, t);
			return GetSeriesAtIndex(propertyId, index, parentSeries);
	    }
        public override ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT)
	    {
            ParametricSeries result = seriesT;
            switch (propertyId)
            {
	            case PropertyId.Location:
		            Series loc = GetSeriesAtT(PropertyId.Location, seriesT[0], null);
		            result = new ParametricSeries(2, loc.X / _simBounds.Width, (_simBounds.Y - loc.Y) / _simBounds.Height);
		            break;
	            case PropertyId.Orientation:
		            Series angle = GetSeriesAtT(PropertyId.Orientation, seriesT[0], null);
		            result = new ParametricSeries(1, angle.X);
		            break;
	            default:
		            var store = GetStore(propertyId);
		            if (store != null)
		            {
			            result = store.GetSampledTs(seriesT);
		            }
		            break;
            }

            return result;
	    }
	    public override void GetDefinedStores(HashSet<PropertyId> ids)
	    {
			base.GetDefinedStores(ids);
			ids.Add(PropertyId.Location);
			ids.Add(PropertyId.Orientation);
        }

	    private Body GetBodyAtIndex(int index)
	    {
		    int counter = 0;
		    Body result = _world.GetBodyList();
		    while (counter < index && result.GetNext() != null)
		    {
			    result = result.GetNext();
			    counter++;
		    }

		    return result;
	    }
        private void CreateGround()
		{
			BodyDef groundBodyDef = new BodyDef();
			groundBodyDef.Position.Set(50.0f, 10.0f);

			// Call the body factory which creates the ground box shape.
			// The body is also added to the world.
			Body groundBody = _world.CreateBody(groundBodyDef);

			PolygonDef groundShapeDef = new PolygonDef();
			groundShapeDef.SetAsBox(_simBounds.Width / 2.0f, 10.0f);
			groundBody.CreateShape(groundShapeDef);
		}
        private void CreateBody(float x, float y)
		{
			BodyDef bodyDef = new BodyDef();
			bodyDef.Position.Set(x, y);
			Body body = _world.CreateBody(bodyDef);

			PolygonDef shapeDef = new PolygonDef();
			shapeDef.SetAsBox(1.0f, 1.0f);
			
			shapeDef.Density = 1.0f;
			shapeDef.Friction = 0.3f;
			body.CreateShape(shapeDef);
			body.SetMassFromShapes();
		}
    }
}
