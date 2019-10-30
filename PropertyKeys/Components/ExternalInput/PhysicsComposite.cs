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
        private float _simX;
        private float _simY;

        public const float PixelsPerMeter = 100f;

	    public override int Capacity { get => _world.GetBodyCount(); set{} }

	    public PhysicsComposite()
	    {
			//todo: set a pixels per meter ratio, have auto converters to FloatSeries (also account for inverted Y).
		    RectFSeries appBounds = MouseInput.MainFrameSize;
            _simBounds = appBounds.Outset(-50f);// new RectFSeries(20f, 0, appBounds.Width - 40f, appBounds.Height + 20);
            _simX = _simBounds.X - appBounds.X;
            _simY = _simBounds.Y - appBounds.Y;
            // box2d aabb is LeftBottom (lowerBound) and RightTop(upperBound)
            AABB bounds = new AABB();
			bounds.LowerBound = GlobalPixelToMeters(_simBounds.Left, _simBounds.Bottom);
			bounds.UpperBound = GlobalPixelToMeters(_simBounds.Right, _simBounds.Top);
            _world = new World(bounds, new Vec2(0,-10f), true);

			CreateGround();
            //CreateBody(200f, 100f, true);
            CreateBody(_simBounds.CX, _simBounds.Top);
        }

        private Vec2 GlobalPixelToMeters(float px, float py) => new Vec2((px - _simX) / PixelsPerMeter, (_simBounds.Height - (py - _simY)) / PixelsPerMeter);
        private FloatSeries MetersToGlobalPixels(float mx, float my) => new FloatSeries(2,  mx * PixelsPerMeter + _simY,  _simBounds.Height - my * PixelsPerMeter + _simY);
        private Vec2 SizeToMeters(float w, float h) => new Vec2(w / PixelsPerMeter, h / PixelsPerMeter);
        private FloatSeries SizeToPixels(float w, float h) => new FloatSeries(2, w * PixelsPerMeter, h * PixelsPerMeter);
        private float MeterToPixel(float value) => value * PixelsPerMeter;
        private float PixelToMeter(float value) => value / PixelsPerMeter;

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
			            result = MetersToGlobalPixels(pos.X, pos.Y);
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
            var pos = GlobalPixelToMeters(_simBounds.CX + _simX, _simBounds.Height+_simY);
            var box = SizeToMeters(_simBounds.CX, 4f);

			BodyDef groundBodyDef = new BodyDef();
			groundBodyDef.Position.Set(pos.X, pos.Y);

			// Call the body factory which creates the ground box shape.
			// The body is also added to the world.
			Body groundBody = _world.CreateBody(groundBodyDef);

			PolygonDef groundShapeDef = new PolygonDef();
			groundShapeDef.SetAsBox(box.X, box.Y);
			groundBody.CreateShape(groundShapeDef);
		}
        private void CreateBody(float x, float y, bool isStatic = false)
        {
            var pos = GlobalPixelToMeters(x, y);

            BodyDef bodyDef = new BodyDef();
			bodyDef.Position.Set(pos.X, pos.Y);
			Body body = _world.CreateBody(bodyDef);

			PolygonDef shapeDef = new PolygonDef();
            var box = SizeToMeters(10f, 10f);
            shapeDef.SetAsBox(box.X, box.Y);
			
			shapeDef.Density = isStatic ? 0.0f : 1.0f;
			shapeDef.Friction = 0.3f;
			body.CreateShape(shapeDef);
			body.SetMassFromShapes();
		}
    }
}
