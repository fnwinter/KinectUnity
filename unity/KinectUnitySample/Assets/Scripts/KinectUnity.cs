using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class KinectUnity : MonoBehaviour
{
    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_main();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static void exit_loop(bool _exit);

    // Start is called before the first frame update
    void Start()
    {
        kinect_main();
    }

    void OnDestroy()
    {
        exit_loop(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
