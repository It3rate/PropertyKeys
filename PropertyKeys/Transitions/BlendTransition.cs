using DataArcs.Components;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Transitions
{
    public class BlendTransition : CompositeBase
    {
        private float _delay;
        private float _startTime;
        private float _duration;
        private Dictionary<PropertyID, BlendStore> _blends = new Dictionary<PropertyID, BlendStore>();

        public Composite Start { get; }
        public Composite End { get; }

        public BlendTransition(Composite start, Composite end, float startTime, float duration, float delay = 0)
        {
            Start = start;
            End = end;
            _delay = delay;
            _duration = duration;
            _startTime = startTime + delay;
            GenerateBlends(); // eventaully immutable after creation, so no new blends
        }

        private void GenerateBlends()
        {
            _blends.Clear();
            HashSet<PropertyID> commonKeys = new HashSet<PropertyID>();
            HashSet<PropertyID> endKeys = new HashSet<PropertyID>();
            Start.GetDefinedProperties(commonKeys);
            End.GetDefinedProperties(endKeys);
            commonKeys.IntersectWith(endKeys);
            foreach (var key in commonKeys)
            {
                _blends.Add(key, (BlendStore)GetStore(key));
            }
        }

        public override void Update(float time)
        {
            float t = time < _startTime ? 0 : time > _startTime + _duration ? 1f : (time - _startTime) / _duration;
            foreach (var item in _blends.Values)
            {
                item.Update(t);
            }
        }

        public override IStore GetStore(PropertyID propertyID)
        {
            IStore result;
            if (_blends.ContainsKey(propertyID))
            {
                result = _blends[propertyID];
            }
            else
            {
                result = Start.GetStore(propertyID);
                IStore end = End.GetStore(propertyID);
                if (result == null)
                {
                    result = end;
                }
                else if (result != null && end != null)
                {
                    result = new BlendStore(result, end);
                }
            }
            return result;
        }

        public override void GetDefinedProperties(HashSet<PropertyID> ids)
        {
            Start.GetDefinedProperties(ids);
            End.GetDefinedProperties(ids);
        }

        public override CompositeBase CreateChild()
        {
            throw new NotImplementedException();
        }
    }
}
