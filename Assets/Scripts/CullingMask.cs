using UnityEngine;
using System.Collections;

public class CullingMask : MonoBehaviour {

	private bool fog = true;

	void Update () {
	
		if (fog == true)
		{
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				if (Camera.allCameras.Length > 1) 
				{
					Camera camera0Obj = Camera.allCameras [0];
					Camera camera1Obj = Camera.allCameras [1];
					
					camera0Obj.cullingMask = camera0Obj.cullingMask ^ 2048;
					camera1Obj.cullingMask = camera1Obj.cullingMask ^ 2048;

					fog = false;
				}else{
					GetComponent<Camera>().cullingMask = GetComponent<Camera>().cullingMask ^ 2048;
					fog = false;
				}
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				if (Camera.allCameras.Length > 1) 
				{
					Camera camera0Obj = Camera.allCameras [0];
					Camera camera1Obj = Camera.allCameras [1];
					
					camera0Obj.cullingMask = camera0Obj.cullingMask | 2048;
					camera1Obj.cullingMask = camera1Obj.cullingMask | 2048;
					
					fog = true;
				}else{
					GetComponent<Camera>().cullingMask = GetComponent<Camera>().cullingMask | 2048;
					fog = true;
				}
			}
		}

}
}
