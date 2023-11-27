using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class KinectVFX : MonoBehaviour
{
    public VisualEffect m_VisualEffect;
    public Texture m_Texture;
    public Texture m_Depth;

    // Update is called once per frame
    void Update()
    {
        if (m_Texture != null)
            m_VisualEffect.SetTexture("Texture", m_Texture);
        if (m_Depth != null)
            m_VisualEffect.SetTexture("Depth", m_Depth);
    }
}
