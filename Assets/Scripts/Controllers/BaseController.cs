using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour {
	
	// Called by scene controller to initialize this controller
	public virtual void InternalSetup() {}
	public virtual void ExternalSetup() {}
}
