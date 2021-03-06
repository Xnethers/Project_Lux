using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode]
public class OutlineObject : MonoBehaviour {

	public bool _transparent,_occlusion,_occlusionCull;
	[HideInInspector]
	public new Renderer renderer;
	[HideInInspector]
	public Material[] sharedMaterials;

	[HideInInspector]
	public int subMeshIndex = 0;

	public bool eraseRenderer;//test
	

#if UNITY_EDITOR
        void OnValidate () 
		{
			#if !UNITY_2018 //on 2017 version need disable object static batch
				var staticFlags = GameObjectUtility.GetStaticEditorFlags (gameObject);
				staticFlags &= ~(StaticEditorFlags.BatchingStatic);
				GameObjectUtility.SetStaticEditorFlags (gameObject, staticFlags);
			#endif
        }
#endif
	void OnEnable() 
	{
		renderer = GetComponent<Renderer>();
		#if UNITY_2018
			subMeshIndex = renderer is SkinnedMeshRenderer ? 0 : ((MeshRenderer)renderer).subMeshStartIndex;
		#endif
		this.sharedMaterials = renderer.sharedMaterials;
		OutlineManager.Instance.objs.Add(this);
	}

	void OnDisable()
	{
		OutlineManager.Instance.objs.Remove(this);
	}

}
