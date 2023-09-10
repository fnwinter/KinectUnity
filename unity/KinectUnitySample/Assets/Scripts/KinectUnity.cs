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
    // Kinect
    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_main();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static void kinect_render();

    // Color API
    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static bool kinect_get_color_buffer(IntPtr refBuffer, int bufferSize);

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_color_buffer_size();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_get_color_width();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_get_color_height();

    // Depth API
    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static bool kinect_get_depth_buffer(IntPtr refBuffer, int bufferSize);

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_depth_buffer_size();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_get_depth_width();

    [DllImport("KinectDLL.dll", CallingConvention = CallingConvention.Cdecl)]
    extern static int kinect_get_depth_height();

    [SerializeField]
    public bool updateDepth = true;
    [SerializeField]
    public bool updateColor = true;

    // Color Variables
    [SerializeField]
    public GameObject colorQuad;

    private IntPtr colorBufferPtr;
    private byte[] colorBufferBytes;
    private int color_buffer_size = 0;
    private Texture2D colorTexture;

    // Depth Variables
    [SerializeField]
    public GameObject depthQuad;

    private IntPtr depthBufferPtr;
    private byte[] depthBufferBytes;
    private int depth_buffer_size = 0;
    private Texture2D depthTexture;

    [SerializeField]
    private GameObject colorDepthQuad;

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

        if (updateDepth) updateDepthBuffer();

        if (updateColor) updateColorBuffer();
    }

    void updateColorBuffer()
    {
        if (color_buffer_size == 0)
        {
            color_buffer_size = kinect_color_buffer_size();
            Debug.LogFormat("color buffer size {0}", color_buffer_size);
        }

        unsafe
        {
            if (colorBufferPtr.ToPointer() == null)
            {
                colorBufferPtr = Marshal.AllocHGlobal(color_buffer_size);
                Debug.LogFormat("colorBufferPtr created");
            }
        }

        if (colorBufferBytes == null) colorBufferBytes = new byte[color_buffer_size];

        kinect_get_color_buffer(colorBufferPtr, color_buffer_size);

        Marshal.Copy(colorBufferPtr, colorBufferBytes, 0, color_buffer_size);

        if (colorTexture == null)
        {
            int color_width = kinect_get_color_width();
            int color_height = kinect_get_color_height();
            Debug.LogFormat("kinect_get_color width / height {0} / {1}", color_width, color_height);
            colorTexture = new Texture2D(color_width, color_height, TextureFormat.BGRA32, false);
        }

        if (colorTexture != null)
        {
            colorTexture.LoadRawTextureData(colorBufferBytes);
            colorTexture.Apply();
        }

        if (colorQuad != null)
        {
            Renderer colorQuadRenderer = colorQuad.GetComponent<Renderer>();
            colorQuadRenderer.material.mainTexture = colorTexture;
        }

        if (colorDepthQuad != null)
        {
            Renderer colorDepthQuadRenderer = colorDepthQuad.GetComponent<Renderer>();
            colorDepthQuadRenderer.material.mainTexture = colorTexture;
        }
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

        if (depthTexture == null)
        {
            int depth_width = kinect_get_depth_width();
            int depth_height = kinect_get_depth_height();
            Debug.LogFormat("kinect_get_depth width / height {0} / {1}", depth_width, depth_height);
            depthTexture = new Texture2D(depth_width, depth_height, TextureFormat.R16, false);
        }

        if (depthTexture != null)
        {
            depthTexture.LoadRawTextureData(depthBufferBytes);
            depthTexture.Apply();
        }

        if (depthQuad != null)
        {
            Renderer depthQuadRenderer = depthQuad.GetComponent<Renderer>();
            depthQuadRenderer.material.mainTexture = depthTexture;
        }

        if (colorDepthQuad != null)
        {
            Renderer colorDepthQuadRenderer = colorDepthQuad.GetComponent<Renderer>();
            colorDepthQuadRenderer.material.SetTexture("_ParallaxMap", depthTexture);
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
