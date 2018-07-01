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

        for (int j = 0; j < Rows; j++)
        {
            for (int i = 0; i < Columns; i++)
            {
                var go = Instantiate(Tile);

                int id = i + j * Rows;
                go.name = "Tile" + (1 + id);
                var t = go.GetComponent<Tile>();
                t.tileId = (ushort)id;
                
                t.rect.x += i * t.rect.width;
                t.rect.y += j * t.rect.height;

                t.transform.SetParent(group.transform);
                t.transform.position = new Vector3(t.rect.x, 0, t.rect.y);
            }
        }
		
    }
}
