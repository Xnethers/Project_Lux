using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
public class IFlowchart : MonoBehaviour {
	public Flowchart flowchart;
	public virtual void Start(){
		flowchart = FindObjectOfType<Flowchart>();
	}
}
