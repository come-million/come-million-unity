using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Rendering;

namespace ComeMillion
{
    public class TileGroup : MonoBehaviour
    {
        public LBClientSender client;
        public PlayableDirector director;
        public RectInt rect;
        public Material[] materials;
        public RenderTexture rt;
        public Texture2D tex;
        Tile[] tiles;

        [NonSerialized]
        private List<Color32[]> tilesPixels;

        [System.Serializable]
        public class TextureEvent : UnityEvent<Texture> { }

        public TextureEvent OnTextureReady;

        void Awake()
        {
            tiles = GetComponentsInChildren<Tile>();

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
            };
            rt.Create();
            OnTextureReady.Invoke(rt);

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

        void FillCommandBuffer(CommandBuffer cmd)
        {
            cmd.Clear();
            cmd.SetRenderTarget(rt);
            cmd.ClearRenderTarget(true, true, Color.clear);

            var resProp = Shader.PropertyToID("_Resolution");
            var texelSize = Shader.PropertyToID("_TexelSize");
            var modelMatrix = Shader.PropertyToID("_ModelMatrix");

            foreach (var m in materials)
            {
                if (m == null)
                    continue;
                foreach (var t in tiles)
                {
                    var r = t.rect;
                    var res = new Vector4(r.x, r.y, r.width, r.height);
                    cmd.SetGlobalVector(resProp, res);
                    cmd.SetGlobalVector(texelSize, new Vector4(rt.width, rt.height, 1f / rt.width, 1f / rt.height));
                    cmd.SetGlobalMatrix(modelMatrix, t.transform.localToWorldMatrix);
                    cmd.DrawMesh(t.mesh, Matrix4x4.identity, m);
                }
            }

            // cmd.SetRenderTarget(BuiltinRenderTextureType.None);
        }

        private IEnumerator Run()
        {
            // var director = GetComponent<UnityEngine.Playables.PlayableDirector>();
            var timeProp = Shader.PropertyToID("_Time");
            var groupProp = Shader.PropertyToID("_Group");
            yield return null;
            var w = new WaitForEndOfFrame();

            // var cmd = new CommandBuffer();
            // FillCommandBuffer(cmd);
            // var mats = new Material[materials.Length];
            // Array.Copy(materials, mats, materials.Length);

            while (true)
            {
                if (director != null)
                {
                    float dt = (float)director.time;
                    var time = new Vector4(dt / 20, dt, dt * 2, dt * 3);
                    Shader.SetGlobalVector(timeProp, time);
                }

                // if (!mats.SequenceEqual(materials))
                // {
                //     FillCommandBuffer(cmd);
                //     mats = new Material[materials.Length];
                //     Array.Copy(materials, mats, materials.Length);
                // }
                // Graphics.ExecuteCommandBuffer(cmd);

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
                        t.material.SetVector(groupProp, new Vector4(rect.x, rect.y, rect.width, rect.height));
                        t.Render();
                    }
                }

                RenderTexture.active = null;

                // #if UNITY_EDITOR
                //             UnityEditor.EditorUtility.SetDirty(rt);
                //             UnityEditor.SceneView.RepaintAll();
                // #endif
                yield return w;

                ReadAndSend();
            }
        }

        public void CopyRect(Color32[] src, Color32[] dst, RectInt r)
        {
            int rows = r.height;
            int w = rt.width;
            int rw = r.width;
            // try
            // {
            for (int i = 0; i < rows; i++)
            {
                Array.Copy(src, r.x + (r.y + i) * w, dst, i * rw, rw);
            }
            // }
            // catch (Exception) { }
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
                client.SetData(t.stripId, (ushort)(1 + t.pixelAddressInStrip), c);
            }

            var p = tex.GetPixels32(tex.mipmapCount - 1).Take(1).ToArray();

            var strips = tiles.Select(t => t.stripId).Distinct();
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