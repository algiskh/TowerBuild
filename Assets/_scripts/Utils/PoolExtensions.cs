using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ComponentUtils
{
	public static class PoolExtensions
	{
		public static T Activate<T>(this T component) where T : Component
		{
			return component.SetActive(true);
		}

		public static T Deactivate<T>(this T component) where T : Component
		{
			return component.SetActive(false);
		}

		public static T SetActive<T>(this T component, bool active) where T : Component
		{
			component.gameObject.SetActive(active);
			return component;
		}

		public static GameObject Activate(this GameObject gameObject)
		{
			gameObject.SetActive(true);
			return gameObject;
		}

		public static GameObject Deactivate(this GameObject gameObject)
		{
			gameObject.SetActive(false);
			return gameObject;
		}

		#region Prefabs

		public static T Copy<T>(this T source, Transform parent = null) where T : Component
		{
			if (source == null)
			{
				Debug.LogError("Null prefab");
				return null;
			}

#if UNITY_EDITOR
			if (!Application.isPlaying && source.InstantiatePrefab(parent) is { } copy)
			{
				return copy;
			}
#endif
			return source.gameObject.Copy(parent).GetComponent(source.GetType()) as T;
		}

		public static GameObject Copy(this GameObject source, Transform parent = null)
		{
			if (source == null)
			{
				Debug.LogError("Null prefab");
				return null;
			}

#if UNITY_EDITOR
			if (!Application.isPlaying && source.InstantiatePrefab(parent) is { } copy)
			{
				return copy;
			}
#endif

			var obj = UnityEngine.Object.Instantiate(source, parent != null ? parent : source.transform.parent, false);
			obj.SetActive(true);

			obj.name = source.name;

			return obj;
		}

		public static T InstantiatePrefab<T>(this T source, Transform parent = null) where T : Component
		{
			return source.gameObject.InstantiatePrefab(parent) is { } copy ? copy.GetComponent<T>() : default;
		}

		public static GameObject InstantiatePrefab(this GameObject source, Transform parent = null)
		{
			if (!source.IsPrefab() && !source.IsPrefabCopy(out source))
			{
				return default;
			}

#if UNITY_EDITOR
			if (PrefabUtility.InstantiatePrefab(source, parent) is GameObject copy)
			{
				copy.SetActive(true);
				return copy;
			}
#endif

			return default;
		}

		public static bool IsPrefab(this GameObject obj)
		{
			return string.IsNullOrEmpty(obj.scene.name);
		}

		public static bool IsPrefabCopy(this GameObject obj, out GameObject prefab)
		{
#if UNITY_EDITOR
			var root = PrefabUtility.GetCorrespondingObjectFromSource(obj);

			if (root != null)
			{
				var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
				prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
				return true;
			}
#endif

			prefab = null;
			return false;
		}

		#endregion

		public static T GetRandomElement<T>(this IEnumerable<T> source, bool throwException = true)
		{
			if (source == null || !source.Any())
			{
				if (throwException)
				{
					throw new InvalidOperationException("Cannot select a random element from an empty or null collection.");
				}

				return default;
			}

			var random = new System.Random();

			var index = random.Next(0, source.Count());
			return source.ElementAt(index);
		}
	}
}