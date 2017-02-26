using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpriteRenderController : BaseController
{
	static PriorityQueue<SRendObject> overlapping;

	public override void InternalSetup() {
		overlapping = new PriorityQueue<SRendObject> ();
	}

	void Update() {
		if (overlapping.Count() > 0)
			SortOverlappings ();
	}

	static void SortOverlappings() {
		// Algorithm might lower sortingOrder more and more, so raise it all if:
		int offset = 0;
		if (overlapping.Peek ().rend.sortingOrder < -50) {
			offset = 100;
		}
		overlapping.Peek ().rend.sortingOrder += offset;
		while (overlapping.Count() > 1) {
			SRendObject obj = overlapping.Dequeue ();
			SRendObject next = overlapping.Peek ();
			//print (obj.rend.gameObject + "  " + obj.rend.sortingOrder + "  " + next.rend.gameObject + "  " + next.rend.sortingOrder);
			if (obj.rend.sortingOrder <= next.rend.sortingOrder) {
				next.rend.sortingOrder = obj.rend.sortingOrder - 1 + offset;
			} else {
				next.rend.sortingOrder += offset;
			}
		}
		overlapping.Dequeue ();
	}

	public static void AddToOverlappings(SpriteRenderer sRend) {
		overlapping.Enqueue (new SRendObject (sRend));
	}

}

public class SRendObject : IComparable<SRendObject> {

	public SpriteRenderer rend;

	public SRendObject(SpriteRenderer r) {
		rend = r;
	}

	public int CompareTo(SRendObject other) {
		// Higher y, lower sorting order wanted
		if (rend.transform.position.y < other.rend.transform.position.y)
			return -1;
		else if (rend.transform.position.y == other.rend.transform.position.y)
			return 0;
		return 1;
	}
}
