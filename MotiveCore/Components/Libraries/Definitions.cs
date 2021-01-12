using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MotiveCore.Components.Libraries
{
    public class Definitions<T> where T : class, IDefinition
    {
	    private int _idCounter = 1;

        private readonly Dictionary<int, T> _allItems = new Dictionary<int, T>();
        private readonly List<int> _activeIds = new List<int>();
        private readonly Dictionary<int, T> _toAddActive = new Dictionary<int, T>();
        private readonly List<int> _toRemoveActive = new List<int>();
        private readonly List<int> _toDestroy = new List<int>();
        public bool CanDestroy = true;
        public bool NeedsDestroy { get; private set; } = false;
		
        public T this[int index]
        {
	        get => _allItems.ContainsKey(index) ? _allItems[index] : null;
            set => _allItems[index] = value;
        }

        public Definitions()
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public List<int> ActiveIds => _activeIds;
        public List<int> ActiveIdsCopy => new List<int>(_activeIds);
        public bool ContainsKey(int key) => _allItems.ContainsKey(key);

        private bool _isBusy = false;

        public void Update(double currentTime, double deltaTime)
        {
	        if (!_isBusy)
	        {
		        _isBusy = true;
		        lock (_activeIds)
		        {
			        for (int i = 0; i < _toRemoveActive.Count; i++)
			        {
				        int id = _toRemoveActive[i];
				        if (_allItems.ContainsKey(id))
				        {
					        _allItems[id].OnDeactivate();
				        }

				        _activeIds.Remove(id);
			        }

			        if (CanDestroy)
			        {
				        for (int i = 0; i < _toDestroy.Count; i++)
				        {
					        int id = _toDestroy[i];
					        _activeIds.Remove(id);
					        _allItems.Remove(id);
				        }
				        NeedsDestroy = false;
			        }

			        foreach (var item in _toAddActive)
			        {
				        // todo: activeElements should allow multiple copies, currently unable to distinguish them with deletion
				        _activeIds.Add(item.Key);
				        item.Value.OnActivate();
			        }

			        _toAddActive.Clear();
			        _toRemoveActive.Clear();
			        _toDestroy.Clear();
					
			        foreach (var id in _activeIds)
			        {
				        if (_allItems.ContainsKey(id))
				        {
					        _allItems[id].Update(currentTime, deltaTime);
				        }
			        }
                }
	        }
	        _isBusy = false;
        }

        public int AddToLibrary(T item)
        {
	        if (item.Id == 0)
	        {
		       _idCounter += item.AssignIdIfUnset(_idCounter + 1) ? 1 : 0;
	        }

            if (_allItems.ContainsKey(item.Id))
            {
	            _allItems[item.Id] = item;
            }
            else
            {
	            _allItems.Add(item.Id, item);
            }

            return item.Id;
        }
        public void ActivateElement(int id)
        {
	        if (_allItems.ContainsKey(id))
	        {
		        _toAddActive.Add(id, _allItems[id]);
	        }
        }
		
        public void DeactivateElement(int id)
        {
            _toRemoveActive.Add(id);
        }
        public void Clear()
        {
            _toRemoveActive.AddRange(_activeIds);
        }

        public void Reset()
        {
	        NeedsDestroy = false;
            Clear();
            _toDestroy.AddRange(_allItems.Keys);
        }

    }
}
