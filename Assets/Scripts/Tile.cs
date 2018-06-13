using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Space]
    public ushort tileId;
    public int width;
    public int height;
    public Material material;

    RenderTexture rt;
    Texture2D tex;
    Mesh mesh;

    void Start()
    {
        rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
        };

        tex = new Texture2D(width, height, TextureFormat.ARGB32, false, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
        };

        material = Instantiate(material);
        GetComponent<Renderer>().material.mainTexture = rt;
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

    void Update()
    {
        if (material.SetPass(0))
        {
            material.SetVector("_Resolution", new Vector4(width, height, 0, 0));

            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear, 0);
            Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
            tex.Apply();
            RenderTexture.active = null;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rt);
#endif

            var colors = tex.GetPixels32();

            LBClientSender.Instance.SetData(tileId, 0, colors);
        }
    }
}
