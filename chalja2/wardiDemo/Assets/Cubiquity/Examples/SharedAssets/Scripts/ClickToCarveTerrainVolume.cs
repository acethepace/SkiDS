using UnityEngine;
using System.Collections;

using Cubiquity;
using UnityEditor;
using Valve.VR;
using Valve.VR.InteractionSystem;

using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Globalization;

public class ClickToCarveTerrainVolume : MonoBehaviour
{
	private TerrainVolume terrainVolume;
    public SteamVR_Behaviour_Pose trackedObj;
    //public GameObject trackedObj;
    private RaycastHit hit;

    private static float brushOuterRadius = 3.0f;
    private static float brushOpacity = 1.0f;
    private static int selectedBrush = 2;
    private const int NoOfBrushes = 5;

    //variables for POP
    //UdpClient listener;
    //IPEndPoint ipep;
    //public GameObject rightHand;

    // Bit of a hack - we want to detect mouse clicks rather than the mouse simply being down,
    // but we can't use OnMouseDown because the voxel terrain doesn't have a collider (the
    // individual pieces do, but not the parent). So we define a click as the mouse being down
    // but not being down on the previous frame. We'll fix this better in the future...
    private bool isMouseAlreadyDown = false;
    private GameObject dirt;
	// Use this for initialization
	void Start ()
	{
		// We'll store a reference to the colored cubes volume so we can interact with it later.
		terrainVolume = gameObject.GetComponent<TerrainVolume>();
		if(terrainVolume == null)
		{
			Debug.LogError("This 'ClickToCarveTerrainVolume' script should be attached to a game object with a TerrainVolume component");
		}
        //make dirtmound invisible at the beginning
        dirt = GameObject.FindWithTag("dirtmound");
        dirt.SetActive(false);

        //code for POP
        //ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 33023);
        //listener = new UdpClient(ipep);


    }
	
	// Update is called once per frame
	void Update ()
	{
		// Bail out if we're not attached to a terrain.
		if(terrainVolume == null)
		{
			return;
		}

        //code for POP
        /*try
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = listener.Receive(ref remote);
            string s = Encoding.ASCII.GetString(data);
            String[] tokens = s.Split(',');

            String ax0 = tokens[0];
            String ax1 = tokens[1];
            String ax2 = tokens[2];
            String ax3 = tokens[3];
            String ax4 = tokens[4];
            String ax5 = tokens[5];
            String ax6 = tokens[6];

            float val1 = float.Parse(ax0, CultureInfo.InvariantCulture.NumberFormat) / 5f;
            float val2 = float.Parse(ax1, CultureInfo.InvariantCulture.NumberFormat) / 5f;
            float val3 = float.Parse(ax2, CultureInfo.InvariantCulture.NumberFormat) / 5f;
            float val4 = float.Parse(ax3, CultureInfo.InvariantCulture.NumberFormat);
            float val5 = float.Parse(ax4, CultureInfo.InvariantCulture.NumberFormat);
            float val6 = float.Parse(ax5, CultureInfo.InvariantCulture.NumberFormat);
            float val7 = float.Parse(ax6, CultureInfo.InvariantCulture.NumberFormat);


            //Vector3 difference = trackedObj.transform.position - (new Vector3(val1, val2, -val3) * 1.15f);
            //Debug.Log("position difference : " + difference);
            //trackedObj.transform.position = (new Vector3((val1 - 168), val2 - 459, -val3 + 195)) * 1.15f;
            //trackedObj.transform.localEulerAngles = new Vector3(-val5, -val6 - 90, -val4 + 90);
            //transform.localEulerAngles = new Vector3(-val5, -val6 - 90, val4 + 90);
            //Debug.Log("positions: "+ new Vector3(val1, val2, -val3) +"     "+ trackedObj.transform.position);
            //Debug.Log("angles: "+transform.localEulerAngles);
        }
        catch (Exception e)
        {
            print(e.ToString());
        }
        //code for POP ends*/

        for (int handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
        {
            Debug.Log(Player.instance.hands.Length);
            Hand hand = Player.instance.hands[handIndex];
            if (hand != null)
            {
                hand.HideController(true);
                Debug.Log("hide2");
            }
            else
            {
                Debug.Log("not able to hide controller");
            }
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
                if (SteamVR_Input._default.inActions.dig.GetStateDown(trackedObj.inputSource)){
                    DestroyVoxels((int)((pickResult.volumeSpacePos.x)), (int)pickResult.volumeSpacePos.y, (int)((pickResult.volumeSpacePos.z)), range);
                    dirt.SetActive(true);
                }
                float multiplier = 1.0f;
                //if (e.modifiers == EventModifiers.Shift)
                //{
                //    multiplier = -1.0f;
                //}
                
                // Selected brush is in the range 0 to NoOfBrushes - 1. Convert this to a 0 to 1 range.
                float brushInnerScaleFactor = (float)selectedBrush / ((float)(NoOfBrushes - 1));
                // Use this value to compute the inner radius as a proportion of the outer radius.
                float brushInnerRadius = brushOuterRadius * brushInnerScaleFactor;
                if (SteamVR_Input._default.inActions.dig.GetStateUp(trackedObj.inputSource)){
                    TerrainVolumeEditor.SculptTerrainVolume(terrainVolume, pickResult.volumeSpacePos.x, pickResult.volumeSpacePos.y, pickResult.volumeSpacePos.z, brushInnerRadius, brushOuterRadius, brushOpacity * multiplier);
                    dirt.SetActive(false);
                }
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
