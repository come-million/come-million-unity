using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UVMapping : MonoBehaviour
{
    public Texture2D texture;

    [ContextMenu("Apply")]
    void Apply()
    {
        int w = Camera.main.pixelWidth,
            h = Camera.main.pixelHeight;

        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
        Camera.main.targetTexture = rt;
        Camera.main.Render();
        Camera.main.targetTexture = null;

        if (texture != null)
            DestroyImmediate(texture);

        texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, w, h), 0, 0, false);
        texture.Apply();
        RenderTexture.active = null;

        var mesh = GetComponent<MeshFilter>().sharedMesh;
        var tris = mesh.triangles;
        var verts = mesh.vertices;
        var t = tris.Length;
        var pixels = texture.GetPixels32();

        var uvs0 = mesh.uv;
        var uvs = mesh.uv;

        var colors0 = mesh.colors;
        var colors = mesh.colors;

        System.Action<int, int> swap = (int i, int j) =>
        {
            Debug.LogFormat("{0} -> {1}", i / 3, j / 3);
            var tmp = Enumerable.Range(i, 3).Select(x => uvs[tris[x]]).ToArray();

            for (int x = 0; x < 3; x++) {
                uvs[tris[j + x]] = uvs0[tris[i + x]];
            }
            for (int x = 0; x < 3; x++) {
                uvs[tris[i + x]] = tmp[x];
            }
        };

        for (int i = 0; i < t; i += 3)
        {
            int a = tris[i], b = tris[i + 1], c = tris[i + 2];
            Vector3 v0 = verts[a], v1 = verts[b], v2 = verts[c];
            v0 = transform.TransformPoint(v0);
            v1 = transform.TransformPoint(v1);
            v2 = transform.TransformPoint(v2);
            var v = (v0 + v1 + v2) / 3f;
            var s = Camera.main.WorldToScreenPoint(v);
            var p = pixels[(int)s.x + (int)s.y * w];

            int idx = p.r;

            swap(i, idx * 3);

            // Debug.LogFormat("#{0} ({1}, {2}) = {3} ({4})", i / 3, (int)s.x, (int)s.y, idx, p.r);

            Font6x8.Draw(texture, (int)s.x + 6, (int)s.y, idx.ToString(), Color.green);
            Font6x8.Draw(texture, (int)s.x - 6, (int)s.y, (i / 3).ToString(), Color.cyan);

            texture.SetPixel((int)s.x, (int)s.y, Color.magenta);

        }

        mesh.uv = uvs;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(mesh);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        texture.Apply();
        RenderTexture.ReleaseTemporary(rt);
    }
}
