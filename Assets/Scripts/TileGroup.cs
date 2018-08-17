using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;

namespace ComeMillion
{
    public class TileGroup : MonoBehaviour
    {
        public LBClientSender client;
        public PlayableDirector director;
        public RectInt rect;
        public Material[] materials;

        public RenderTexture rt;
        public RenderTexture posmap;
        public RenderTexture uvmap;
        public Texture2D tex;

        Tile[] tiles;

        [NonSerialized]
        private List<Color32[]> tilesPixels;

        [System.Serializable]
        public class TextureEvent : UnityEvent<Texture> { }

        public TextureEvent OnTextureReady;

        bool rendering;

        ushort[] strips;

        void Awake()
        {
            tiles = GetComponentsInChildren<Tile>();
            strips = tiles.Select(t => t.stripId).Distinct().ToArray();

            if (rect.size == Vector2Int.zero)
            {
                Debug.LogWarning("Invalid rect size");
                return;
            }

            rt = new RenderTexture(rect.width, rect.height, 0, RenderTextureFormat.ARGB32)
            {
                name = this.name + "_RT",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                useMipMap = false,
            };
            rt.Create();
            OnTextureReady.Invoke(rt);

            uvmap = new RenderTexture(rect.width, rect.height, 0, RenderTextureFormat.RGFloat)
            {
                name = this.name + "_UV",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                useMipMap = false,

            };
            uvmap.Create();


            posmap = new RenderTexture(rect.width, rect.height, 0, RenderTextureFormat.ARGBFloat)
            {
                name = this.name + "_Pos",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                useMipMap = false,
            };
            posmap.Create();


            // Array.ForEach(tiles, t => t.rt = rt);
            // tiles = GetComponentsInChildren<Tile>().Where(t => t.isActiveAndEnabled).ToArray();

            tex = new Texture2D(rect.width, rect.height, TextureFormat.ARGB32, true, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };
            tex.Apply();

            tilesPixels = new List<Color32[]>(tiles.Length);
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].rt = rt;
                var r1 = tiles[i].rect;
                tilesPixels.Add(new Color32[r1.width * r1.height]);
            }

            StartCoroutine(Run());
        }

        void RenderUV()
        {
            RenderTexture.active = uvmap;

            var m = new Material(Shader.Find("Custom/UV"));
            var groupProp = Shader.PropertyToID("_Group");
            var g = new Vector4(rect.x, rect.y, rect.width, rect.height);
            m.SetVector(groupProp, g);

            foreach (var t in tiles)
            {
                t.material = m;
                t.Render();
            }

            RenderTexture.active = null;
            Destroy(m);
        }


        void RenderPos()
        {
            RenderTexture.active = posmap;

            var m = new Material(Shader.Find("Custom/Position"));
            var modelMatrix = Shader.PropertyToID("_ModelMatrix");

            foreach (var t in tiles)
            {
                t.material = m;
                m.SetMatrix(modelMatrix, t.transform.localToWorldMatrix);
                t.Render();
            }

            RenderTexture.active = null;
            Destroy(m);
        }

        private IEnumerator Run()
        {
            yield return null;
            RenderUV();
            RenderPos();

            // var director = GetComponent<UnityEngine.Playables.PlayableDirector>();
            var timeProp = Shader.PropertyToID("_Time");
            // var groupProp = Shader.PropertyToID("_Group");
            yield return null;
            var w = new WaitForEndOfFrame();

            var tgs = FindObjectsOfType<TileGroup>();
            var w1 = new WaitWhile(() => tgs.All(t => t.rendering));

            while (true)
            {
                if (director != null)
                {
                    float dt = (float)director.time;
                    var time = new Vector4(dt / 20, dt, dt * 2, dt * 3);
                    Shader.SetGlobalVector(timeProp, time);
                }

                yield return w;
                rendering = true;

                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);

                // var g = new Vector4(rect.x, rect.y, rect.width, rect.height);
                // foreach (var m in materials)
                // {
                //     if (m == null)
                //         continue;
                //     m.SetVector(groupProp, g);
                //     foreach (var t in tiles)
                //     {
                //         t.rt = rt;
                //         t.material = m;
                //         if (t.isActiveAndEnabled)
                //             t.Render();
                //     }
                // }

                foreach (var m in materials)
                {
                    if (m == null)
                        continue;
                    m.SetTexture("_UVMap", uvmap);
                    m.SetTexture("_PosMap", posmap);
                    m.SetVector("_MainTex_Offset", new Vector4(m.mainTextureScale.x, m.mainTextureScale.y, m.mainTextureOffset.x, m.mainTextureOffset.y));
                    Graphics.Blit(m.mainTexture, rt, m);
                }

                RenderTexture.active = null;



                // #if UNITY_EDITOR
                //             UnityEditor.EditorUtility.SetDirty(rt);
                //             UnityEditor.SceneView.RepaintAll();
                // #endif

                rendering = false;
                yield return w1;

                ReadAndSend();
            }
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MethodImpl(256)]
        public void CopyRect(Color32[] src, Color32[] dst, RectInt r, int w)
        {
            int rows = r.height;
            int rw = r.width;
            int rx = r.x;
            int ry = r.y;

            for (int i = 0; i < rows; i++)
            {
                Array.Copy(src, rx + (ry + i) * w, dst, i * rw, rw);
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

            int w = rt.width;

            for (int i = 0; i < tiles.Length; i++)
            {
                var t = tiles[i];
                if (t.isActiveAndEnabled)
                {
                    RectInt r1 = t.rect;
                    var c = tilesPixels[i];
                    CopyRect(colors, c, r1, w);
                    // t.tex.SetPixels32(c);
                    // t.tex.Apply();
                    client.SetData(t.stripId, (ushort)(1 + t.pixelAddressInStrip), c);
                }
            }

            // var p = tex.GetPixels32(tex.mipmapCount - 1).Take(1).ToArray();
            // last mipmap is always 1x1
            var p = tex.GetPixels32(tex.mipmapCount - 1);

            var tmp = p[0].r;
            p[0].r = p[0].g;
            p[0].g = tmp;
            foreach (var strip in strips)
            {
                client.SetData(strip, 0, p);
            }
        }
    }
}