using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ComponentUtils
{
	public class Pool<T> where T : Component //useful
	{
		private readonly DiContainer _diContainer;
		private readonly T _prefab;
		private readonly Transform _parent;
		private readonly Stack<T> _pool = new();

		public Pool(T prefab, Transform parent = null, DiContainer diContainer = null)
		{
			_diContainer = diContainer;
			_prefab = prefab;
			_prefab.Deactivate();
			_parent = parent ?? prefab.transform.parent;
		}

		public Pool(IEnumerable<T> items, Transform parent = null, DiContainer diContainer = null)
		{
			_diContainer = diContainer;
			foreach (var item in items)
			{
				item.Deactivate();
				if (_prefab == null)
				{
					_prefab = item;
				}
				else
				{
					_pool.Push(item);
				}
			}

			if (_prefab == null)
			{
				Debug.LogError($"No prefab for {typeof(T)}");
				return;
			}

			_prefab.Deactivate();
			_prefab.transform.SetAsFirstSibling();
			_parent = parent ?? _prefab.transform.parent;
		}

		public Pool(Transform parent, int startSize = 0, DiContainer diContainer = null)
		{
			_diContainer = diContainer;

			foreach (var child in parent.GetComponentsInChildren<T>(true))
			{
				child.transform.SetParent(parent);
				_pool.Push(child.Deactivate());
			}

			_prefab = _pool.Pop().Deactivate();
			_prefab.transform.SetAsFirstSibling();
			_parent = parent;

			while (startSize-- > 0) SpawnOneCopy();
		}

		public T Pop()
		{
			if (_pool.Count == 0)
			{
				SpawnOneCopy();
			}

			return _pool.Pop().Activate();
		}

		public void Push(T item)
		{
			_pool.Push(item.Deactivate());
		}

		public void Push(List<T> items)
		{
			if (items == null)
			{
				Debug.LogError($"No items to push");
				return;
			}

			foreach (var item in items)
			{
				Push(item);
			}

			items.Clear();
		}

		private void SpawnOneCopy()
		{
			T copy;
			if (_diContainer != null)
			{
				copy = _diContainer.InstantiatePrefabForComponent<T>(_prefab, _parent);
			}
			else
			{
				copy = _prefab.Copy(_parent);
			}

			_pool.Push(copy.Deactivate());
		}
	}
}