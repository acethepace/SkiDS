
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cubiquity;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Drag : MonoBehaviour
{

    //Initialize Variables
    GameObject getTarget;
    bool isMouseDragging;
    Vector3 offsetValue;
    Vector3 positionOfScreen;
    public SteamVR_Behaviour_Pose trackedObj;

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {

        //Mouse Button Press Down
        //if (Input.GetMouseButtonDown(0))
        if(SteamVR_Input._default.inActions.drag.GetStateDown(trackedObj.inputSource))
        {
            Debug.Log("Press detected");
            RaycastHit hitInfo;
            getTarget = ReturnClickedObject(out hitInfo);
            if (getTarget != null)
            {
                isMouseDragging = true;
                //Converting world position to screen position.
                Destroy(getTarget.GetComponent<Rigidbody>());
                positionOfScreen = Camera.main.WorldToScreenPoint(getTarget.transform.position);
                offsetValue = getTarget.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, positionOfScreen.z));
            }
        }

        //Mouse Button Up
        //if (Input.GetMouseButtonUp(0))
        if (SteamVR_Input._default.inActions.drag.GetStateUp(trackedObj.inputSource))
        {
            if (isMouseDragging)
            {
                getTarget.AddComponent<Rigidbody>();
            }
            isMouseDragging = false;
        }

        //Is mouse Moving
        if (isMouseDragging)
        {
            //tracking mouse position.
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, positionOfScreen.z);

            //converting screen position to world position with offset changes.
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offsetValue;

            //It will update target gameobject's current postion.
            getTarget.transform.position = currentPosition;
        }


    }

    //Method to Return Clicked Object
    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        if (Physics.Raycast(trackedObj.transform.position, trackedObj.transform.forward, out hit, 1000f))
        {
            target = hit.collider.gameObject;
            Debug.Log(target.tag);
            if (!target.CompareTag("plant"))
            {
                Debug.Log(target.name);
                target = null;

            }
        }
        if (target != null)
            Debug.Log(target.GetType().ToString());
        Debug.Log("target found");
        return target;
    }

}

