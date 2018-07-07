using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ComeMillion;
using UnityEditor;
using UnityEngine;

public static class SplitJoinMesh
{
    [MenuItem("CONTEXT/MeshFilter/Split mesh")]
    static void SplitMesh(MenuCommand cmd)
    {
        MeshFilter mf = cmd.context as MeshFilter;

        var mesh = mf.sharedMesh;
        var transform = mf.GetComponent<Transform>();
        var renderer = mf.GetComponent<MeshRenderer>();
        renderer.enabled = false;
        var mat = renderer.sharedMaterial;

        var verts = mesh.vertices;
        var tris = mesh.triangles;
        var uvs = mesh.uv;
        var uvs2 = mesh.uv2;
        var colors = mesh.colors;

        var l = tris.Length;
        for (int i = 0; i < l; i += 3)
        {
            var tri = tris.Skip(i).Take(3).ToArray();
            var go = new GameObject("Tri" + i / 3, typeof(MeshFilter), typeof(MeshRenderer));

            var m = new Mesh();
            m.vertices = tri.Select(t => verts[t]).ToArray();
            m.uv = tri.Select(t => uvs[t]).ToArray();
            m.uv2 = tri.Select(t => uvs2[t]).ToArray();
            m.colors = tri.Select(t => colors[t]).ToArray();
            m.triangles = new int[] { 2, 1, 0 };
            m.RecalculateBounds();
            m.RecalculateNormals();

            go.GetComponent<MeshFilter>().sharedMesh = m;
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }


    }

    [MenuItem("CONTEXT/MeshFilter/Combine child meshes")]
    static void CombineChildMeshes(MenuCommand cmd)
    {
        MeshFilter mf = cmd.context as MeshFilter;
        var mesh = mf.sharedMesh;
        var meshFilters = mf.GetComponentsInChildren<MeshFilter>().Where(i => i != mf).ToArray();
        // var transform = mf.GetComponent<Transform>();

        var tile = mf.GetComponent<Tile>();

        var combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            var c = meshFilters[i];
            Debug.Log(c.gameObject.name);
            var m = c.GetComponent<MeshFilter>().sharedMesh;
            var t = c.GetComponent<Transform>();

            var x = i % tile.rect.width;
            var y = i / tile.rect.width;
            var uv = new Vector2((float)x / tile.rect.width, (float)y / tile.rect.height);
            m.uv = new Vector2[] { uv, uv, uv };

            combine[i].mesh = m;
            //https://answers.unity.com/questions/231249/instanced-meshes-are-being-offset-to-weird-positio.html
            combine[i].transform = Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
        }

        mesh.colors = Enumerable.Range(0, mesh.triangles.Length / 3)
                .SelectMany(i =>
                {
                    return new Color[] {
                        Color.red,
                        Color.green,
                        Color.blue
                    };
                })
            .ToArray();

        mesh.CombineMeshes(combine, true, true, false);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mf.GetComponent<MeshRenderer>().enabled = true;

        for (int i = 0; i < meshFilters.Length; i++)
        {
            UnityEngine.Object.DestroyImmediate(meshFilters[i].gameObject);
        }
    }
}