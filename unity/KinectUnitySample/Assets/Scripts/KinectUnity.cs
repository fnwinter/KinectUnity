using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

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

    [SerializeField]
    public GameObject quad;

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
            Debug.LogFormat("depth buffer size {0}", depth_buffer_size);
        }

        unsafe
        {
            if (depthBufferPtr.ToPointer() == null)
            {
                depthBufferPtr = Marshal.AllocHGlobal(depth_buffer_size);
                Debug.LogFormat("depthBufferPtr created");
            }
        }

        if (depthBufferBytes == null) depthBufferBytes = new byte[depth_buffer_size];

        kinect_get_depth_buffer(depthBufferPtr, depth_buffer_size);

        Marshal.Copy(depthBufferPtr, depthBufferBytes, 0, depth_buffer_size);

        // For Test
        // ByteArrayToFile("depth.img", depthBufferBytes);

        if (tex == null)
        {
            int depth_width = kinect_get_depth_width();
            int depth_height = kinect_get_depth_height();
            Debug.LogFormat("kinect_get_depth width / height {0} / {1}", depth_width, depth_height);
            tex = new Texture2D(depth_width, depth_height, TextureFormat.R16, false);
        }

        if (tex != null)
        {
            tex.LoadRawTextureData(depthBufferBytes);
            tex.Apply();
        }

        if (quad != null)
        {
            Renderer m_Renderer = quad.GetComponent<Renderer>();
            m_Renderer.material.mainTexture = tex;
        }
    }

    public bool ByteArrayToFile(string fileName, byte[] byteArray)
    {
        try
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(byteArray, 0, byteArray.Length);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught in process: {0}", ex);
            return false;
        }
    }
}
