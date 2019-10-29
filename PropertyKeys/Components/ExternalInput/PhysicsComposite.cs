using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using DataArcs.SeriesData;

namespace DataArcs.Components.ExternalInput
{
    public class PhysicsComposite : BaseComposite
    {
	    private World _world;
	    private RectFSeries _simBounds;


        public PhysicsComposite()
	    {
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

		private void CreateGround()
		{
			BodyDef groundBodyDef = new BodyDef();
			groundBodyDef.Position.Set(0.0f, -10.0f);

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
