using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

public class CreateTileWizard : ScriptableWizard
{
    public MeshFilter meshFilter;
    public int Rows = 5;
    public int Columns = 10;

    [MenuItem("Tools/Create Tile Wizard...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CreateTileWizard>("Create Tile", "Create new");
    }

    void OnWizardCreate()
    {
        var m = TileGenerator.Generate(Rows, Columns);
        if (meshFilter != null)
        {
            meshFilter.sharedMesh = m;
        }
        else
        {
            var a = Selection.activeGameObject;
            if (a != null)
            {
                var mf = a.GetComponent<MeshFilter>();
                if (mf != null)
                    mf.sharedMesh = m;
            }
        }
        // Selection.SetActiveObjectWithContext(m, m);
        EditorGUIUtility.PingObject(m);
    }
}

public class TileGenerator
{
    // TODO: less LINQ
    public static Mesh Generate(int rows, int cols)
    {
        var m = new Mesh();

        var tri_verts = new Vector3[]{
            new Vector3(0.5f, 0, Mathf.Sqrt(0.75f)),
            new Vector3(0, 0, 0),
            new Vector3(1f, 0, 0),
        };

        const float margin = 0.05f;

        var verts =
            Enumerable.Range(0, rows).Select(j =>
            Enumerable.Range(0, cols).Select(i =>
            tri_verts.Select(v =>
            {
                if (i % 2 == 0)
                    v.z = -v.z + Mathf.Sqrt(0.75f);
                if (j % 2 == 0)
                    v.z = -v.z + Mathf.Sqrt(0.75f);
                v.z += j * (margin + Mathf.Sqrt(3f) / 3f * 1.5f);
                return v + i * Vector3.right * (margin + 0.5f);
            })
        ).SelectMany(x => x)).SelectMany(x => x)
        .ToArray();

        m.vertices = verts;

        int[] triangles = new int[rows * cols * 3];

        for (int i = 0; i < cols / 2; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int k = i + j * cols / 2;

                if (j % 2 == 1)
                {

                    triangles[k * 6 + 0] = 0 + k * 6;
                    triangles[k * 6 + 1] = 1 + k * 6;
                    triangles[k * 6 + 2] = 2 + k * 6;

                    triangles[k * 6 + 3] = 5 + k * 6;
                    triangles[k * 6 + 4] = 4 + k * 6;
                    triangles[k * 6 + 5] = 3 + k * 6;
                }
                else
                {
                    triangles[k * 6 + 0] = 2 + k * 6;
                    triangles[k * 6 + 1] = 1 + k * 6;
                    triangles[k * 6 + 2] = 0 + k * 6;

                    triangles[k * 6 + 3] = 3 + k * 6;
                    triangles[k * 6 + 4] = 4 + k * 6;
                    triangles[k * 6 + 5] = 5 + k * 6;
                }
            }
        }
        m.triangles = triangles;

        // m.triangles = Enumerable.Range(0, rows * cols)
        //     // .Select(i => new int[] { 2, 1, 0 }
        //     .Select(i => (i % 2 == 0 ?
        //         new int[] { 2, 1, 0 } :
        //         new int[] { 0, 1, 2 })
        //     .Select(j => j + i * 3))
        // .SelectMany(i => i)
        // .ToArray();

        m.uv = Enumerable.Range(0, rows).Select(j =>
                Enumerable.Range(0, cols).SelectMany(i =>
                {
                    var uv = new Vector2((float)i / cols, (float)j / rows);
                    return new Vector2[] { uv, uv, uv };
                }))
                .SelectMany(uv => uv)
                .ToArray();

        m.uv2 = m.uv;

        // m.normals = verts.Select(v => Vector3.up).ToArray();

        // var centers = Enumerable.Range(0, rows * cols).Select(i =>
        //     (verts[i * 3] + verts[i * 3 + 1] + verts[i * 3 + 2]) / 3f
        // ).Select(v => new Vector2(v.x, v.z)).ToArray();

        // m.uv2 = m.vertices
        //     .Select((v, i) => new Vector2(v.x, v.z) - centers[i / 3])
        //     .ToArray();

        // barycentric coords
        m.colors = Enumerable.Range(0, rows).Select(j =>
                Enumerable.Range(0, cols)
                .SelectMany(i =>
                {
                    return new Color[] {
                        Color.red,
                        Color.green,
                        Color.blue
                    };
                })
                ).SelectMany(x => x)
            .ToArray();

        m.RecalculateBounds();
        m.RecalculateNormals();
        var path = "Assets/Tiles/Tile.asset";
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(m, path);
        AssetDatabase.SaveAssets();
        return m;
    }


}


