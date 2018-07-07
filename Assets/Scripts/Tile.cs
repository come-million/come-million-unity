using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ComeMillion
{
    public class Tile : MonoBehaviour
    {
        [Space]
        public ushort stripId;
        public ushort pixelAddressInStrip;
        public RectInt rect;

        public Material material;

        public RenderTexture rt;

        public Texture2D tex;

        [HideInInspector]
        public Mesh mesh;
        Vector4 res;
        private int resProp;
        private int texelSize;
        private int modelMatrix;
        private Material rendererMat;

        Transform m_transform;

        void Start()
        {
            // rt = rt ?? new RenderTexture(rect.width, rect.height, 0, RenderTextureFormat.ARGB32)
            // {
            //     filterMode = FilterMode.Point,
            //     wrapMode = TextureWrapMode.Clamp,
            // };

            tex = new Texture2D(rect.width, rect.height, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };

            mesh = GetComponent<MeshFilter>().sharedMesh;

            res = new Vector4(rect.x, rect.y, rect.width, rect.height);
            resProp = Shader.PropertyToID("_Resolution");

            texelSize = Shader.PropertyToID("_TexelSize");
            modelMatrix = Shader.PropertyToID("_ModelMatrix");

            var propertyBlock = new MaterialPropertyBlock();
            rendererMat = GetComponent<MeshRenderer>().sharedMaterial;
            // mat2 = GetComponent<Renderer>().material;
            // mat2.mainTexture = rt;
            // mat2.SetVector(resProp, res);
            propertyBlock.SetTexture("_MainTex", rt);
            propertyBlock.SetVector(resProp, res);
            GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);

            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;

            m_transform = transform;
        }

        // void Update()
        // {
        //     mat2.SetVector(resProp, res);
        // }

        public void Render()
        {
            // RenderTexture.active = rt;
            // GL.Clear(true, true, Color.clear);

            res = new Vector4(rect.x, rect.y, rect.width, rect.height);
            material.SetVector(resProp, res);
            material.SetVector(texelSize, new Vector4(rt.width, rt.height, 1f / rt.width, 1f / rt.height));
            material.SetMatrix(modelMatrix, m_transform.localToWorldMatrix);
            rendererMat.SetVector(resProp, res);
            material.SetPass(0);
            // Graphics.SetRenderTarget(rt);
            Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
            // Graphics.SetRenderTarget(null);
            // RenderTexture.active = null;

            // #if UNITY_EDITOR
            //         UnityEditor.EditorUtility.SetDirty(rt);
            // #endif
        }
    }
}