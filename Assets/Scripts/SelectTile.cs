using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ComeMillion;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTile : MonoBehaviour
{
    Tile[] tiles;
    Tile last;

    public Material DefaultMaterial;
    public Material SelectedMaterial;
    public Material UnSelectedMaterial;
    private Coroutine render;

    void OnEnable()
    {
        tiles = FindObjectsOfType<Tile>().Where(t => t.isActiveAndEnabled).ToArray();
        System.Array.ForEach(tiles, t => t.GetComponent<MeshRenderer>().sharedMaterial = UnSelectedMaterial);
        FindObjectsOfType<TileGroup>().ToList().ForEach(g => g.enabled = false);
    }

    void OnDisable()
    {
        FindObjectsOfType<TileGroup>().ToList().ForEach(g => g.enabled = true);

        System.Array.ForEach(tiles, t =>
        {
            // might be destroy here
            if (t)
                t.GetComponent<MeshRenderer>().sharedMaterial = DefaultMaterial;

        });
    }

    // Update is called once per frame
    void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButton(0))
        {
            var p = Input.mousePosition;
            p = Camera.main.ScreenToWorldPoint(p);
            var tile = tiles
                .Where(t => t.GetComponent<MeshRenderer>().enabled)
                .OrderBy(t => (t.GetComponent<MeshRenderer>().bounds.center - p).magnitude)
                .FirstOrDefault();

            if (tile == null)
                return;

            if (tile != last)
            {
                if (last != null)
                {
                    last.GetComponent<MeshRenderer>().sharedMaterial = UnSelectedMaterial;
                    SetTileColor(last, Color.black);
                }

                tile.GetComponent<MeshRenderer>().sharedMaterial = SelectedMaterial;
                last = tile;

                SetTileColor(tile, Color.red);

                // FindObjectsOfType<TileGroup>().ToList().ForEach(g => Graphics.Blit(Texture2D.blackTexture, g.rt));
                // Graphics.Blit(Texture2D.whiteTexture, tile.rt, tile.rect.size, tile.rect.position);
                // var rt = tile.rt;
                // var tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
                // RenderTexture.active = rt;
                // tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                // tex.Apply();
                // RenderTexture.active = null;
                // tex.SetPixel(tile.rect.x, tile.rect.y, Color.red);
                // tex.Apply();
                // Graphics.Blit(tex, rt);
            }
        }


    }

    void SetTileColor(Tile tile, Color32 color)
    {
        var client = tile.GetComponentInParent<TileGroup>().client;
        var colors = new Color32[tile.rect.width * tile.rect.height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = color;
        client.SetData(tile.stripId, (ushort)(1 + tile.pixelAddressInStrip), colors);

    }
}
