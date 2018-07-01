using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroup : MonoBehaviour
{
    public RectInt rect;
    public Material[] materials;

    public RenderTexture rt;
    public Texture2D tex;
    Tile[] tiles;
    List<Color32[]> tilesPixels;


    void Awake()
    {
        tiles = GetComponentsInChildren<Tile>();

        rt = new RenderTexture(rect.width, rect.height, 0, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
        };

        tex = new Texture2D(rect.width, rect.height, TextureFormat.ARGB32, false, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
        };

        tilesPixels = new List<Color32[]>(tiles.Length);
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].rt = rt;
            var r1 = tiles[i].rect;
            tilesPixels.Add(new Color32[r1.width * r1.height]);
        }

        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        var director = GetComponent<UnityEngine.Playables.PlayableDirector>();
        var timeProp = Shader.PropertyToID("_Time");
        yield return null;
        var w = new WaitForEndOfFrame();
        while (true)
        {
            if (director != null)
            {
                float dt = (float)director.time;
                var time = new Vector4(dt / 20, dt, dt * 2, dt * 3);
                Shader.SetGlobalVector(timeProp, time);
            }

            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);

            foreach (var m in materials)
            {
                if (m == null)
                    continue;
                foreach (var t in tiles)
                {
                    t.rt = rt;
                    t.material = m;
                    t.Render();
                }
            }

            RenderTexture.active = null;
            // UnityEditor.EditorUtility.SetDirty(rt);

            yield return w;

            ReadAndSend();
        }
    }

    static void CopyRect(Color32[] src, Color32[] dst, RectInt r)
    {
        int rows = r.height;
        int w = r.width;
        for (int i = 0; i < rows; i++)
        {
            Array.Copy(src, r.x + (r.y + i) * w, dst, i * w, w);
        }
    }

    void ReadAndSend()
    {
        RenderTexture.active = rt;
        var r = new Rect(0, 0, rt.width, rt.height);
        tex.ReadPixels(r, 0, 0, false);
        tex.Apply();
        RenderTexture.active = null;

        var colors = tex.GetPixels32();

        for (int i = 0; i < tiles.Length; i++)
        {
            var t = tiles[i];
            RectInt r1 = t.rect;
            var c = tilesPixels[i];
            CopyRect(colors, c, r1);
            // t.tex.SetPixels32(c);
            // t.tex.Apply();
            LBClientSender.Instance.SetData(t.tileId, 0, c);
        }

    }
}
