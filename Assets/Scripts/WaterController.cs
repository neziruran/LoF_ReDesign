using UnityEngine; 
using System.Collections;

public class WaterController : MonoBehaviour 
{ 
	public int materialIndex = 0; 
	public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f ); 
	public string textureName = "_MainTex";
	public string bumpTextureName = "_BumpMap";
	
	Vector2 uvOffset = Vector2.zero;
	
	void LateUpdate()
	{
		uvOffset += ( uvAnimationRate * Time.deltaTime );
		if( GetComponent<Renderer>().enabled )
		{
			GetComponent<Renderer>().materials[ materialIndex ].SetTextureOffset( textureName, uvOffset );
			var tex = GetComponent<Renderer>().materials[ materialIndex ].GetTexture( bumpTextureName);
			if (tex)
				GetComponent<Renderer>().materials[ materialIndex ].SetTextureOffset( bumpTextureName, uvOffset );
		}
	}
}
