using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class KinectUnity : MonoBehaviour
{
    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_main();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static void kinect_render();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static bool kinect_get_color_buffer(IntPtr refBuffer, int bufferSize);

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static bool kinect_get_depth_buffer(IntPtr refBuffer, int bufferSize);

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_color_buffer_size();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_depth_buffer_size();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_get_color_width();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_get_color_height();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_get_depth_width();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_get_depth_height();

    //private IntPtr pnt = Marshall.AllocHGlobal(512);
    private IntPtr depthBufferPtr;
    private byte[] depthBufferBytes;

    private int depth_buffer_size = 0;

    Texture2D tex;

    // Start is called before the first frame update
    void Start()
    {
        kinect_main();
    }

    void OnDestroy()
    {
        if (depthBufferPtr != null) Marshal.FreeHGlobal(depthBufferPtr);
    }

    // Update is called once per frame
    void Update()
    {
        kinect_render();

        updateDepthBuffer();
    }

    void updateColorBuffer()
    {   
    }

    void updateDepthBuffer()
    {
        if (depth_buffer_size == 0)
        {
            depth_buffer_size = kinect_depth_buffer_size();
        }
        if (depth_buffer_size == 0) return;

        if (depthBufferPtr == null)
        {
            depthBufferPtr = Marshal.AllocHGlobal(depth_buffer_size);
        }
        if (depthBufferPtr == null) return;

        kinect_get_depth_buffer(depthBufferPtr, depth_buffer_size);

        if (depthBufferBytes == null) depthBufferBytes = new byte[depth_buffer_size];
        if (depthBufferBytes == null) return;

        Marshal.Copy(depthBufferPtr, depthBufferBytes, 0, depth_buffer_size);

        if (tex == null)
        {
            int depth_width = kinect_get_depth_width();
            int depth_height = kinect_get_depth_height();
            tex = new Texture2D(depth_width, depth_height, TextureFormat.PVRTC_RGBA4, false);
        }

        if (tex != null)
        {
            tex.LoadRawTextureData(depthBufferBytes);
            tex.Apply();
        }
    }
}
