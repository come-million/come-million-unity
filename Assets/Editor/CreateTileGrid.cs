using System.Collections;
using System.Collections.Generic;
using ComeMillion;
using UnityEditor;
using UnityEngine;

public class TileGridWizard : ScriptableWizard
{
    [Tooltip("Tile Prefab to clone")]
    public GameObject Tile;

    public Transform parent;
    public int Rows = 5;
    public int Columns = 10;

    [MenuItem("Tools/Create Tile Grid...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<TileGridWizard>("Create a grid of tiles");
    }

    void OnWizardCreate()
    {
        if (Tile == null)
        {
            isValid = false;
            errorString = "Please select a prefab";
            return;
        }
        isValid = true;

        float x = Mathf.Sqrt(1 + 0.5f * 0.5f) * 0.5f - 0.1f;

        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                // var strip = new GameObject("Strip" + (i + 1));
                // strip.transform.SetParent(parent);

                var go = Instantiate(Tile);

                go.name = string.Format("Tile{0}_{1}", i, j);
                var t = go.GetComponent<Tile>();
                t.stripId = (ushort)(i);
                t.pixelAddressInStrip = (ushort)(50 * j);

                t.rect.x += i * t.rect.width;
                t.rect.y += j * t.rect.height;

                t.transform.SetParent(parent);
                var b = t.GetComponent<MeshFilter>().sharedMesh.bounds;
                t.transform.position = new Vector3(i * (b.size.x - x) + (j % 2 == 1 ? 0.55f : 0), 0, j * (b.size.z + 0.05f));
            }
        }
    }
}
