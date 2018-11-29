using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Globalization;
public class popChalja : MonoBehaviour
{

    //variables for POP
    UdpClient listener;
    IPEndPoint ipep;
    //public GameObject rightHand;

    // Use this for initialization
    void Start()
    {
        //code for POP
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 33023);
        listener = new UdpClient(ipep);
    }

    // Update is called once per frame
    void Update()
    {
        //code for POP
        try
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

            float val1 = float.Parse(ax0, CultureInfo.InvariantCulture.NumberFormat)/400f;
            float val2 = float.Parse(ax1, CultureInfo.InvariantCulture.NumberFormat)/3000f;
            float val3 = float.Parse(ax2, CultureInfo.InvariantCulture.NumberFormat)/3000f;
            float val4 = float.Parse(ax3, CultureInfo.InvariantCulture.NumberFormat);
            float val5 = float.Parse(ax4, CultureInfo.InvariantCulture.NumberFormat);
            float val6 = float.Parse(ax5, CultureInfo.InvariantCulture.NumberFormat);
            float val7 = float.Parse(ax6, CultureInfo.InvariantCulture.NumberFormat);


            //Vector3 difference = trackedObj.transform.position - (new Vector3(val1, val2, -val3) * 1.15f);
            //Debug.Log("position difference : " + difference);
            //transform.position = (new Vector3((val1 - 168), val2 - 459, -val3 + 195)) * 1.15f;
            transform.position = (new Vector3(val1, -val2, val3));
            //trackedObj.transform.localEulerAngles = new Vector3(-val5, -val6 - 90, -val4 + 90);
            transform.localEulerAngles = new Vector3(-val5, -val6 - 90, val4 + 90);
            //Debug.Log("positions: "+ new Vector3(val1, val2, -val3) +"     "+ trackedObj.transform.position);
            //Debug.Log("angles: "+transform.localEulerAngles);
        }
        catch (Exception e)
        {
            print(e.ToString());
        }
        //code for POP ends
    }
}
