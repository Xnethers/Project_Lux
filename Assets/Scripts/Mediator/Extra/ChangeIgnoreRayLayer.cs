using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeIgnoreRayLayer : MonoBehaviour {
	//public LayerMask layerMask;

	public List<Collider> IgnoreObj = new List<Collider>();
	public static Dictionary<Collider, int> IgnoreObjs= new Dictionary<Collider, int>();
	public bool isWall;
	/* void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")){
			IgnoreObj.Add(col);
			//IgnoreObjs[col.gameObject] = col.gameObject.layer;
			IgnoreObjs.Add(col,col.gameObject.layer);
			col.gameObject.layer=this.gameObject.layer;
			//col.gameObject.layer=LayerMask.NameToLayer("Ignore Raycast");
		}
		
	}
	void OnTriggerExit(Collider col)
	{
		foreach (KeyValuePair<Collider, int> objs in IgnoreObjs)
		{
			if(col==objs.Key){
				col.gameObject.layer=objs.Value;
				IgnoreObj.Remove(col);
				IgnoreObjs.Remove(objs.Key);
				break;
			 	//IgnoreObjs[col];
			}
				
		}
	}*/
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Wall"))
			isWall =true;
	}
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Wall"))
			isWall=false;
	}
	/// <summary>
	/// 物体透明化
	/// </summary>
	/// <param name="go"></param>
	/// <param name="mat"></param>
	public static void Convert2Virtual(GameObject go, Material mat) {
		if(go == null || mat == null) {
			Debug.Log("The GameObject or Material is NULL");
			return;
		}
		Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
		Material[] materials = null;
		for(int i = 0; i < renderers.Length; i++) {
			materials = new Material[renderers[i].materials.Length];
			for (int j = 0; j < materials.Length; j++) {
				materials[j] = mat;
			}
			renderers[i].materials = materials;
		}
	}

	/// <summary>
	/// 物体不透明
	/// </summary>
	/// <param name="go"></param>
	/// <param name="mats"></param>
	public static void Convert2Real(GameObject go, Material[][] mats) {
		if(go == null || mats.Length == 0) {
			Debug.Log("The GameObject or Material is NULL");
			return;
		}
		Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
		if(mats.Length != renderers.Length) {
			Debug.Log("The count of material is NOT true");
			return;
		}
		for(int i = 0; i < renderers.Length; i++) {
			renderers[i].materials = mats[i];
		}
	}
}
