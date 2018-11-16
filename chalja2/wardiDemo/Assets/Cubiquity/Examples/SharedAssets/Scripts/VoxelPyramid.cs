using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;

public class VoxelPyramid : MonoBehaviour
{

    // Use this for initialization
    //void Start()
    //{
    public Texture soilTexture;

		
	//}
	void Update()
	{ 
		if(Input.GetMouseButtonDown(0)) Debug.Log("Pressed left click.");
		if (Input.GetMouseButtonDown(1))
		{
			Debug.Log("Pressed right click.");
			float maxHeight = 15;
			for (float height = 0; height < maxHeight; height=height+0.5f)
			{
				float length = maxHeight - height;
				for (float x = -length; x <= length; x++)
				{
					for (float z = -length; z <= length; z++)
					{
						if (Mathf.Abs(x) == length || Mathf.Abs(z) == length)
						{
							Color color= new Color(Random.Range(0.6f,0.65f),Random.Range(0.32f,0.33f),0.1f,0.15f);
							float size = Random.Range(1f, 1f);
                            VoxelTools.soilTexture = soilTexture;
                            VoxelTools.MakeCube(x, height, z,color,size);
						}
					}
				}

			}
		}
		if(Input.GetMouseButtonDown(2)) Debug.Log("Pressed middle click.");
	}
}
