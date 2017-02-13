using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ISavable<T> {
	T Save (T baseObj);
	void Load (T obj);
}
