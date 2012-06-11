// (c)2011 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uLink;

/// <summary>
/// By attaching this to a game object in the Hierarchy view, it will
/// automatically be instantiated for all others over the network when
/// you start. This works for both clients and servers. You can specify
/// different prefabs for Proxy and Owner. The Owner is the original
/// player. This script requires a non-authoritative server.
/// </summary>
[AddComponentMenu("uLink Utilities/Instantiate For Others")]
[RequireComponent(typeof(uLinkNetworkView))]
public class uLinkInstantiateForOthers : uLink.MonoBehaviour
{
	public GameObject proxyPrefab;
	public GameObject ownerPrefab;
	// TODO: this script should be generalized for both owners and creators
	
	public bool appendLoginData = false;
	
	void Start()
	{		
		if (uLink.Network.status == uLink.NetworkStatus.Connected)
		{
			uLink_OnConnectedToServer();
		}
	}

	void uLink_OnConnectedToServer()
	{
		if (networkView.viewID != uLink.NetworkViewID.unassigned)
		{
			return;
		}
		
		if (uLink.Network.isAuthoritativeServer && uLink.Network.isClient)
		{
			// TODO: warn if server is authoritative && this is not the server
			return;
		}
		
		Transform trans = transform;
		uLink.NetworkPlayer owner = uLink.Network.player;
		uLink.NetworkViewID viewID = uLink.Network.AllocateViewID();
		object[] initialData = appendLoginData ? uLink.Network.loginData : new object[0];
		
		uLink.Network.Instantiate(viewID, owner, proxyPrefab, ownerPrefab, null, trans.position, trans.rotation, 0, initialData);
		
		networkView.SetViewID(viewID, owner, owner);
		networkView.SetInitialData(initialData);
	}
}
