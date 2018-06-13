using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class CreateTileWizard : ScriptableWizard
{
    public MeshFilter meshFilter;
    public int Rows = 5;
    public int Columns = 9;

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

        m.triangles = Enumerable.Range(0, rows * cols)
        .Select(i => (i % 2 == 0 ?
            new int[] { 2, 1, 0 } :
            new int[] { 0, 1, 2 })
            .Select(j => j + i * 3))
        .SelectMany(i => i)
        .ToArray();

        m.uv = Enumerable.Range(0, rows).Select(j =>
                Enumerable.Range(0, cols).SelectMany(i =>
                {
                    var uv = new Vector2((float)i / cols, (float)j / rows);
                    return new Vector2[] { uv, uv, uv };
                }))
                .SelectMany(uv => uv)
                .ToArray();

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



public class SplitJoinMesh
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

        var combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            var c = meshFilters[i];
            var m = c.GetComponent<MeshFilter>().sharedMesh;
            var t = c.GetComponent<Transform>();
            combine[i].mesh = m;
            //https://answers.unity.com/questions/231249/instanced-meshes-are-being-offset-to-weird-positio.html
            combine[i].transform = Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
        }

        mesh.CombineMeshes(combine, true, true, false);

        for (int i = 0; i < meshFilters.Length; i++)
        {
            UnityEngine.Object.DestroyImmediate(meshFilters[i].gameObject);
        }
    }
}