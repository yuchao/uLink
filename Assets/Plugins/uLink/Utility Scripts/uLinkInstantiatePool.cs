// (c)2011 MuchDifferent. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using uLink;

/// <summary>
/// Use this script to make a pool of pre-instantiated prefabs that will be used 
/// when prefabs are network instantiated via uLink.Network.Instantiate().
/// </summary>
///
/// <remarks>
/// This wonderful script makes it possible to pool game objects (instantiated prefabs)
/// and uLink will use this pool of objects as soon as the gameplay code makes a call to
/// Network.Instantiate.
///
/// The wonderful part is that you do not have to change your code at all to use the pool
/// mechanism. This makes it very easy to check if your game is faster and more smooth
/// with a pool.
///
/// In some games, with lots of spawning NPCs, guided missiles etc, the pool
/// is very convenient to avoid the overhead of calling Object.Instantiate() and Object.Destroy().
///
/// The need for a pool is mainly to increase performance, Instantiating objects is a 
/// heavy operation in Unity. By load-testing the game with the uTsung tool it is easier to
/// make decisions when to use a pool for prefabs.
///
/// A normal scenario is to pool creator prefabs in the server scene and pool proxy prefabs
/// in the client scene.
/// 
/// The value minSize is the number of prefabs that will be instantiated at startup.
/// If the number of Instantiate calls does go above this value, the pool will be increased 
/// at run-time.
/// </remarks>
[AddComponentMenu("uLink Utilities/Instantiate Pool")]
public class uLinkInstantiatePool : uLink.MonoBehaviour
{
	public uLink.NetworkView prefab;

	public int minSize = 50; // This number of prefabs will be instantiated at startup

	private readonly Stack<uLink.NetworkView> pool = new Stack<uLink.NetworkView>();

	private Transform parent;
	
	void Awake()
	{
		if (enabled) CreatePool();
	}

	void Start()
	{
		// this is here just so the componet can be enabled/disabled.
	}

	void OnDisable()
	{
		DestroyPool();
	}

	public void CreatePool()
	{
		if (prefab._manualViewID != 0)
		{
			Debug.LogError("Prefab viewID must be set to Allocated or Unassigned", prefab);
			return;
		}

		parent = new GameObject(name + "-Pool").transform;

		for (int i = 0; i < minSize; i++)
		{
			uLink.NetworkView instance = (uLink.NetworkView)Instantiate(prefab);
			instance.transform.parent = parent;
			instance.gameObject.SetActiveRecursively(false);
			pool.Push(instance);
		}

		uLink.NetworkInstantiator.Add(prefab.name, PreInstantiator, PostInstantiator, Destroyer);
	}

	public void DestroyPool()
	{
		if (parent == null) return;

		uLink.NetworkInstantiator.Remove(prefab.name);
		pool.Clear();

		Destroy(parent.gameObject);
		parent = null;
	}

	private uLink.NetworkView PreInstantiator(string prefabName, Vector3 position, Quaternion rotation)
	{
		if (pool.Count > 0)
		{
			uLink.NetworkView instance = pool.Pop();
			instance.transform.position = position;
			instance.transform.rotation = rotation;
			instance.gameObject.SetActiveRecursively(true);
			return instance;
		}
		else
		{
			uLink.NetworkView instance = (uLink.NetworkView)Instantiate(prefab);
			instance.transform.parent = parent;
			instance.transform.position = position;
			instance.transform.rotation = rotation;
			return instance;
		}
	}

	private void PostInstantiator(uLink.NetworkView instance, uLink.NetworkMessageInfo info)
	{
		instance.BroadcastMessage("uLink_OnNetworkInstantiate", info, SendMessageOptions.DontRequireReceiver);
	}

	private void Destroyer(uLink.NetworkView instance)
	{
		instance.gameObject.SetActiveRecursively(false);
		pool.Push(instance);
	}
}
