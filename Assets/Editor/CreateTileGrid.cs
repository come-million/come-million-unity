using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileGridWizard : ScriptableWizard
{
    [Tooltip("Tile Prefab to clone")]
    public GameObject Tile;
    public int Rows = 10;
    public int Columns = 10;

    [MenuItem("Tools/Create Tile Grid...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<TileGridWizard>("Create a grid of tiles");
    }

    void OnWizardCreate()
    {
        var group = new GameObject("TileGroup", typeof(TileGroup));

        float x = Mathf.Sqrt(1 + 0.5f * 0.5f) * 0.5f - 0.1f;

        for (int j = 0; j < Rows; j++)
        {
            for (int i = 0; i < Columns; i++)
            {
                var go = Instantiate(Tile);

                int id = i + j * Rows;
                go.name = "Tile" + (1 + id);
                var t = go.GetComponent<Tile>();
                t.stripId = (ushort)(id / 5);
                t.pixelAddressInStrip = (ushort)(id % (t.rect.width * t.rect.height));

                t.rect.x += i * t.rect.width;
                t.rect.y += j * t.rect.height;

                t.transform.SetParent(group.transform);
                var b = t.GetComponent<MeshFilter>().sharedMesh.bounds;
                t.transform.position = new Vector3(i * (b.size.x - x) + (j % 2 == 1 ? 0.55f : 0), 0, j * (b.size.z + 0.05f));
            }
        }

    }
}
