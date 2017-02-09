using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
	private List<T> data;

	public PriorityQueue()
	{
		this.data = new List<T>();
	}

	public void Enqueue(T item)
	{
		data.Add(item);
		int ci = data.Count - 1;
		while (ci > 0)
		{
			int pi = (ci - 1) / 2;
			if (data[ci].CompareTo(data[pi]) >= 0) 
				break;
			T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
			ci = pi;
		}
	}

	public T Dequeue()
	{
		if (data.Count == 0)
			return default(T);
		int li = data.Count - 1;
		T frontItem = data[0];
		data[0] = data[li];
		data.RemoveAt(li);

		--li;
		int pi = 0;
		while (true)
		{
			int ci = pi * 2 + 1;
			if (ci > li) 
				break;
			int rc = ci + 1;
			if (rc <= li && data[rc].CompareTo(data[ci]) < 0)
				ci = rc;
			if (data[pi].CompareTo(data[ci]) <= 0) 
				break;
			T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
			pi = ci;
		}
		return frontItem;
	}

	public bool Contains(T item) {
		foreach (T datum in data) {
			if (datum.Equals (item))
				return true;
		}
		return false;
	}

	public bool ContainsByCompare(T item) {
		foreach (T datum in data) {
			if (datum.CompareTo (item) == 0)
				return true;
		}
		return false;
	}

	public T Peek()
	{
		T frontItem = data[0];
		return frontItem;
	}

	public int Count()
	{
		return data.Count;
	}

	public override string ToString()
	{
		string s = "";
		for (int i = 0; i < data.Count; ++i)
			s += data[i].ToString() + " ";
		s += "count = " + data.Count;
		return s;
	}

}