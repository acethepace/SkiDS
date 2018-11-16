using UnityEngine;
using System.Collections;

using Cubiquity;
using UnityEditor;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ClickToCarveTerrainVolume : MonoBehaviour
{
	private TerrainVolume terrainVolume;
    public SteamVR_Behaviour_Pose trackedObj;
    private RaycastHit hit;

    private static float brushOuterRadius = 3.0f;
    private static float brushOpacity = 1.0f;
    private static int selectedBrush = 2;
    private const int NoOfBrushes = 5;

    // Bit of a hack - we want to detect mouse clicks rather than the mouse simply being down,
    // but we can't use OnMouseDown because the voxel terrain doesn't have a collider (the
    // individual pieces do, but not the parent). So we define a click as the mouse being down
    // but not being down on the previous frame. We'll fix this better in the future...
    private bool isMouseAlreadyDown = false;
	
	// Use this for initialization
	void Start ()
	{
		// We'll store a reference to the colored cubes volume so we can interact with it later.
		terrainVolume = gameObject.GetComponent<TerrainVolume>();
		if(terrainVolume == null)
		{
			Debug.LogError("This 'ClickToCarveTerrainVolume' script should be attached to a game object with a TerrainVolume component");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Bail out if we're not attached to a terrain.
		if(terrainVolume == null)
		{
			return;
		}
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 1000f))
        {
            Debug.DrawRay(trackedObj.transform.position, trackedObj.transform.forward, Color.green); 
            Debug.Log("hello");
            Vector3 pixel = hit.point;
            //pixel.x -= 10;
            //print(pixel);


            //Ray ray = Camera.main.ScreenPointToRay(new Vector3(pixel.x, pixel.z,0 ));
            Ray ray = new Ray(trackedObj.transform.position, trackedObj.transform.forward);
            // Perform the raycasting.
            PickSurfaceResult pickResult;
            bool hit1 = Picking.PickSurface(terrainVolume, ray, 100.0f, out pickResult);

            // If we hit a solid voxel then create an explosion at this point.
            if (hit1)
            {
                Debug.Log("Hell2");
                int range = 2;
                if (SteamVR_Input._default.inActions.dig.GetStateDown(trackedObj.inputSource))
                    DestroyVoxels((int)((pickResult.volumeSpacePos.x)), (int)pickResult.volumeSpacePos.y, (int)((pickResult.volumeSpacePos.z)), range);
                float multiplier = 1.0f;
                //if (e.modifiers == EventModifiers.Shift)
                //{
                //    multiplier = -1.0f;
                //}
                
                // Selected brush is in the range 0 to NoOfBrushes - 1. Convert this to a 0 to 1 range.
                float brushInnerScaleFactor = (float)selectedBrush / ((float)(NoOfBrushes - 1));
                // Use this value to compute the inner radius as a proportion of the outer radius.
                float brushInnerRadius = brushOuterRadius * brushInnerScaleFactor;
                if (SteamVR_Input._default.inActions.dig.GetStateUp(trackedObj.inputSource))
                    TerrainVolumeEditor.SculptTerrainVolume(terrainVolume, pickResult.volumeSpacePos.x, pickResult.volumeSpacePos.y, pickResult.volumeSpacePos.z, brushInnerRadius, brushOuterRadius, brushOpacity * multiplier);
                    //TerrainVolumeEditor.BlurTerrainVolume(terrainVolume, pickResult.volumeSpacePos.x, pickResult.volumeSpacePos.y, pickResult.volumeSpacePos.z, brushInnerRadius, brushOuterRadius, brushOpacity);
            }
        }
    }
	
	void DestroyVoxels(int xPos, int yPos, int zPos, int range)
	{
		// Initialise outside the loop, but we'll use it later.
		int rangeSquared = range * range;
		MaterialSet emptyMaterialSet = new MaterialSet();
		
		// Iterage over every voxel in a cubic region defined by the received position (the center) and
		// the range. It is quite possible that this will be hundreds or even thousands of voxels.
		for(int z = zPos - range; z < zPos + range; z++) 
		{
			for(int y = yPos - range; y < yPos + range; y++)
			{
				for(int x = xPos - range; x < xPos + range; x++)
				{			
					// Compute the distance from the current voxel to the center of our explosion.
					int xDistance = x - xPos;
					int yDistance = y - yPos;
					int zDistance = z - zPos;
					
					// Working with squared distances avoids costly square root operations.
					int distSquared = xDistance * xDistance + yDistance * yDistance + zDistance * zDistance;
					
					// We're iterating over a cubic region, but we want our explosion to be spherical. Therefore 
					// we only further consider voxels which are within the required range of our explosion center. 
					// The corners of the cubic region we are iterating over will fail the following test.
					if(distSquared < rangeSquared)
					{	
						terrainVolume.data.SetVoxel(x, y, z, emptyMaterialSet);
					}
				}
			}
		}
		
		range += 2;
		
		TerrainVolumeEditor.BlurTerrainVolume(terrainVolume, new Region(xPos - range, yPos - range, zPos - range, xPos + range, yPos + range, zPos + range));
		//TerrainVolumeEditor.BlurTerrainVolume(terrainVolume, new Region(xPos - range, yPos - range, zPos - range, xPos + range, yPos + range, zPos + range));
		//TerrainVolumeEditor.BlurTerrainVolume(terrainVolume, new Region(xPos - range, yPos - range, zPos - range, xPos + range, yPos + range, zPos + range));
	}
}
