using System;
using System.Collections.Generic;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Motive.Samplers;
using Motive.SeriesData;
using Motive.Components.ExternalInput;
using Math = System.Math;

namespace Motive.Components.Simulators
{
    public class PhysicsComposite : BaseComposite, IDisposable
    {
	    private World _world;
		private readonly Dictionary<int, int> _bodyMap = new Dictionary<int, int>(256);
		private int _bodyCounter = 0;
	    private readonly RectFSeries _simBounds;
        private readonly float _simX;
        private readonly float _simY;

        public const float PixelsPerMeter = 50f;
        float thickness = 10;

        public override int Capacity { get => _bodyMap.Count; set{} }

	    public PhysicsComposite()
	    {
            RectFSeries appBounds = Runner.MainFrameRect;
            _simBounds = appBounds.Outset(thickness);// new RectFSeries(20f, 0, appBounds.Width - 40f, appBounds.Height + 20);
            _simX = _simBounds.X - appBounds.X;
            _simY = _simBounds.Y - appBounds.Y;

            // box2d aabb is LeftBottom (lowerBound) and RightTop(upperBound)
            AABB bounds = new AABB {
	            LowerBound = GlobalPixelToMeters(_simBounds.Left, _simBounds.Bottom),
	            UpperBound = GlobalPixelToMeters(_simBounds.Right, _simBounds.Top)
            };
            _world = new World(bounds, new Vec2(0, -10f), true);

			SealWorldEdges();
        }
        
        private Vec2 GlobalPixelToMeters(float px, float py) => new Vec2((px - _simX) / PixelsPerMeter, (_simBounds.Height - (py - _simY)) / PixelsPerMeter);
        private FloatSeries MetersToGlobalPixels(float mx, float my) => new FloatSeries(2,  mx * PixelsPerMeter + _simY,  _simBounds.Height - my * PixelsPerMeter + _simY);
        private Vec2 SizeToMeters(float w, float h) => new Vec2(w / PixelsPerMeter, h / PixelsPerMeter);
        private FloatSeries SizeToPixels(float w, float h) => new FloatSeries(2, w * PixelsPerMeter, h * PixelsPerMeter);
        private float MeterToPixel(float value) => value * PixelsPerMeter;
        private float PixelToMeter(float value) => value / PixelsPerMeter;

        public override void StartUpdate(double currentTime, double deltaTime)
	    {
            base.StartUpdate(currentTime, deltaTime);

		    //float timeStep = 1.0f / 60.0f;
		    int velocityIterations = 8;
		    int positionIterations = 1;
		    _world.Step((float)(deltaTime/1000.0), velocityIterations, positionIterations);
	    }

        public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
			Series result = null;
			int index = SamplerUtils.IndexFromT(Capacity, t);
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
						result = new FloatSeries(1, 1f - normAngle);
						break;
				}
			}

			if (parentSeries != null)
			{
				if (result != null)
				{
					// todo: atm physics has no 'store' object, so can't combine functionally. Maybe needs this, or maybe external input is a source only, not an interm step?
					//result.CombineInto(parentSeries, store.CombineFunction, t);
				}
				else
				{
					result = parentSeries;
				}
			}

			return result;
	    }
        public override ParametricSeries GetNormalizedPropertyAtT(PropertyId propertyId, ParametricSeries seriesT)
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
		    Body result = _world.GetBodyList();
		    while (result != null && result.GetUserData() != index)
		    {
			    result = result.GetNext();
		    }
		    return result;
	    }

        public void CreateBezierBody(int index, IContainer composite, bool isStatic = false)
        {
	        float tIndex = index / (composite.Capacity - 1f);
	        var dict = new Dictionary<PropertyId, Series>
	        {
		        { PropertyId.PointCount, null },
		        { PropertyId.Radius, null },
		        { PropertyId.Location, null }
            };
	        composite.QueryPropertiesAtT(dict, tIndex, false);
	        var bezier = composite.Renderer.GetDrawable(dict);

            var pos = GlobalPixelToMeters(dict[PropertyId.Location].X, dict[PropertyId.Location].Y);

	        BodyDef bodyDef = new BodyDef();
	        bodyDef.Position.Set(pos.X, pos.Y);
	        Body body = _world.CreateBody(bodyDef);

			body.SetUserData(_bodyCounter++);
			_bodyMap[index] = body.GetUserData();

	        ShapeDef shapeDef;
	        int count = (int)(bezier.Count - 1);
	        if (count <= 10)
	        {
		        var polyDef = new PolygonDef();

                polyDef.VertexCount = count;
		        polyDef.Vertices = new Vec2[count];
		        for (int i = count - 1; i >= 0; i--)
		        {
			        float xp = bezier.FloatValueAt(i * 2); // skip the midpoints
			        float yp = bezier.FloatValueAt(i * 2 + 1);
			        polyDef.Vertices[i] = new Vec2(PixelToMeter(xp), -PixelToMeter(yp));
		        }
		        shapeDef = polyDef;
	        }
	        else
	        {
				var circDef = new CircleDef();
				int midX = (int)(count / 2f);
				float difX = bezier.FloatValueAt(midX * 2) - bezier.FloatValueAt(0);
				float difY = bezier.FloatValueAt(midX * 2 + 1) - bezier.FloatValueAt(1);
				circDef.Radius = PixelToMeter((float)Math.Sqrt(difX * difX + difY * difY) / 2.0f);
				shapeDef = circDef;
	        }

	        shapeDef.Density = isStatic ? 0.0f : 1.0f;
	        shapeDef.Friction = 0.1f;
	        body.CreateShape(shapeDef);
	        body.SetMassFromShapes();
        }

        private void SealWorldEdges()
        {
	        CreateGround(_simBounds.CX, thickness / 2.0f, _simBounds.Width * 2f, thickness);
	        CreateGround(_simBounds.CX, _simBounds.Height - thickness / 2.0f, _simBounds.Width * 2f, thickness);
            CreateGround(thickness / 2.0f, _simBounds.CY, thickness, _simBounds.Height * 2f);
	        CreateGround(_simBounds.Width - thickness / 2.0f, _simBounds.CY, thickness, _simBounds.Height * 2f);
        }
        private void CreateGround(float x, float y, float w, float h)
        {
            var pos = GlobalPixelToMeters(x + _simX, y + _simY);
            var box = SizeToMeters(w / 2.0f, h / 2.0f);
            BodyDef groundBodyDef = new BodyDef();
            groundBodyDef.Position.Set(pos.X, pos.Y);
            Body groundBody = _world.CreateBody(groundBodyDef);
            PolygonDef groundShapeDef = new PolygonDef();
            groundShapeDef.SetAsBox(box.X, box.Y);
            groundBody.CreateShape(groundShapeDef);
        }
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool state)
        {
            if (state)
            {
                _world.Dispose();
                _world = null;
            }
        }
    }
}
