using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour, IComparable<Waypoint>
{
	/// <summary>
	/// Other Waypoints you can get to from this one with a straight-line path
	/// </summary>
	[NonSerialized]
	public List<Waypoint> Neighbors = new List<Waypoint>();

	/// <summary>
	/// Used in path planning; next closest node to the start node
	/// </summary>
	private Waypoint predecessor;

	Waypoint[] AllWaypoints;

	int indexW;
	private float sScore = Mathf.Infinity;
	private float eScore = Mathf.Infinity;

	/// <summary>
	/// Compute the Neighbors list
	/// </summary>
	public void Setup(Waypoint[] allWaypoints)
	{
		AllWaypoints = allWaypoints;
		var position = transform.position;
		if (AllWaypoints == null)
		{
			AllWaypoints = FindObjectsOfType<Waypoint>();
		}

		for (int i = 0; i < AllWaypoints.Length; i++) {
			Waypoint wp = AllWaypoints [i];
			if (wp == this)
				indexW = i;
			if (wp != this && CanReachPoints(position, wp.transform.position))
				Neighbors.Add (wp);
		}
	}

	public static bool CanReachPoints(Vector3 pnt1, Vector3 pnt2) {
		RaycastHit2D hit = Physics2D.Raycast ((Vector2)pnt1, (Vector2)(pnt2 - pnt1), Vector3.Distance (pnt1, pnt2), LayerMask.GetMask (new string[1] { "Wall" }));
		return hit.collider == null;
	}

	/// <summary>
	/// Nearest waypoint to specified location that is reachable by a straight-line path.
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public static Waypoint NearestWaypointTo(Waypoint[] waypointNetwork, Vector2 position)
	{
		Waypoint nearest = null;
		var minDist = float.PositiveInfinity;
		for (int i = 0; i < waypointNetwork.Length; i++)
		{
			var wp = waypointNetwork[i];
			var p = wp.transform.position;
			var d = Vector2.Distance(position, p);
			if (d < minDist && CanReachPoints(p, position))
			{
				nearest = wp;
				minDist = d;
			}
		}
		return nearest;
	}

	/// <summary>
	/// Returns a series of waypoints to take to get to the specified position
	/// </summary>
	/// <param name="start">Starting position</param>
	/// <param name="end">Desired endpoint</param>
	/// <returns></returns>
	public static List<Waypoint> FindPath(Waypoint[] waypointNetwork, Vector2 start, Vector2 end)
	{
		return FindPathAStar(waypointNetwork, NearestWaypointTo(waypointNetwork, start), NearestWaypointTo(waypointNetwork, end));
	}

	static List<Waypoint> FindPathAStar(Waypoint[] network, Waypoint start, Waypoint end) {
		List<Waypoint> checkedW = new List<Waypoint> ();
		bool[] withinChecked = new bool[network.Length]; 
		for (int i = 0; i < withinChecked.Length; i++) {
			withinChecked [i] = false;
		}

		PriorityQueue<Waypoint> queuedW = new PriorityQueue<Waypoint> ();
		queuedW.Enqueue (start);

		start.sScore = 0;
		start.eScore = Vector3.Distance (start.transform.position, end.transform.position);

		Waypoint current;
		while (queuedW.Count() != 0) {
			current = queuedW.Dequeue ();
			if (current == end) {
				return ReconstructPath (current, start);
			}

			checkedW.Add (current);
			withinChecked [current.indexW] = true;
			foreach (Waypoint wp in current.Neighbors) {
				if (withinChecked [wp.indexW])
					continue;

				float new_s = current.sScore + Vector3.Distance (current.transform.position, wp.transform.position);
				if (!queuedW.Contains (wp))
					queuedW.Enqueue (wp);
				else if (new_s >= wp.sScore)
					continue;

				wp.predecessor = current;
				wp.sScore = new_s;
				wp.eScore = new_s + Vector3.Distance (wp.transform.position, end.transform.position);
			}
		}
		return null;
	}

	static List<Waypoint> ReconstructPath(Waypoint end, Waypoint start) {
		List<Waypoint> reversed = new List<Waypoint>() {end};
		while (end != start) {
			end = end.predecessor;
			reversed.Insert (0, end);
		}
		return reversed;
	}

	public int CompareTo(Waypoint other) {
		if (this.eScore < other.eScore)
			return -1;
		else if (this.eScore == other.eScore)
			return 0;
		return 1;
	}
}
