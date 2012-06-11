// (c)2011 MuchDifferent. All Rights Reserved.

using System;
using UnityEngine;
using uLink;

/// <summary>
/// A graphical tool for the game client. Perfect for testers.
/// </summary>
/// <remarks>
/// Add this script component to one of the game objects. After that, testers will be able to
/// press the "enabledByKey" and bring up a window showing some important numbers from uLink:
/// ping time, bandwidth in both directions, number of networkViews (objects), and more.
/// </remarks>

[AddComponentMenu("uLink Utilities/Statistics GUI")]
public class uLinkStatisticsGUI : uLink.MonoBehaviour
{
	public bool showOnlyInEditor = false;

	public KeyCode enabledByKey = KeyCode.Tab;
	public bool isEnabled = false;

	public bool dontDestroyOnLoad = false;

	public int guiDepth = 0;

	public bool showFrameRate = false;

	void Awake()
	{
		if (dontDestroyOnLoad) DontDestroyOnLoad(this);
	}

	void Update()
	{
		if (Input.GetKeyDown(enabledByKey)) isEnabled = !isEnabled;
	}

	void OnGUI()
	{
		if (!isEnabled || (showOnlyInEditor && !Application.isEditor)) return;
		
		GUI.depth = guiDepth;
		DrawGUI(showFrameRate);
	}

	public static void DrawGUI(bool showFrameRate)
	{
		uLink.NetworkPlayer[] connections = uLink.Network.connections;

		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		GUILayout.BeginVertical();
		GUILayout.Space(10);

		GUILayout.BeginVertical(GUILayout.Width(300));

		GUILayout.BeginVertical("Box");

		if (showFrameRate)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Frame Rate:", GUILayout.Width(150));
			GUILayout.Label(Mathf.RoundToInt(1.0f / Time.smoothDeltaTime) + " FPS");
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("Status:", GUILayout.Width(150));
		GUILayout.Label(uLink.NetworkUtility.GetStatusString(uLink.Network.peerType, uLink.Network.status));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Last Error:", GUILayout.Width(150));
		GUILayout.Label(uLink.NetworkUtility.GetErrorString(uLink.Network.lastError));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Network Time:", GUILayout.Width(150));
		GUILayout.Label(uLink.Network.time + " s");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Network Views:", GUILayout.Width(150));
		GUILayout.Label(uLink.Network.networkViewCount.ToString());
		GUILayout.EndHorizontal();

		if (uLink.Network.isServer)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Connections:", GUILayout.Width(150));
			GUILayout.Label(connections.Length.ToString());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Name in Master Server:", GUILayout.Width(150));
			GUILayout.Label(uLink.MasterServer.isRegistered ? uLink.MasterServer.gameName : "Not Registered");
			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();

		foreach (uLink.NetworkPlayer player in connections)
		{
			uLink.NetworkStatistics stats = player.statistics;
			if (stats == null) continue;

			GUILayout.BeginVertical("Box");

			GUILayout.BeginHorizontal();
			GUILayout.Label("Connection:", GUILayout.Width(150));
			GUILayout.Label(player.ToString());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Ping (average):", GUILayout.Width(150));
			GUILayout.Label(player.lastPing + " (" + player.averagePing + ") ms");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Sent:", GUILayout.Width(150));
			GUILayout.Label((int) Math.Round(stats.bytesSentPerSecond) + " B/s");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Receive:", GUILayout.Width(150));
			GUILayout.Label((int) Math.Round(stats.bytesReceivedPerSecond) + " B/s");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Encryption:", GUILayout.Width(150));
			GUILayout.Label(player.hasSecurity ? "On" : "Off");
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}

		GUILayout.EndVertical();

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
