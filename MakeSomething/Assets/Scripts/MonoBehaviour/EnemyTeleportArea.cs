using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyTeleportArea : MonoBehaviour
{
    private static EnemyTeleportArea currentArea;
    private static Vector2[] AllTilePosition => currentArea.tilePos;

    [SerializeField] private Tilemap tile;
    private Vector2[] tilePos;


    private void Awake() 
    {
        if(currentArea == null)
            currentArea = this;
            
        List<Vector2> p = new List<Vector2>();

        foreach(var pos in currentArea.tile.cellBounds.allPositionsWithin)
        {
            if(!currentArea.tile.HasTile(pos))
                continue;

            p.Add(pos + currentArea.tile.gameObject.transform.position + currentArea.tile.tileAnchor);
        }
        tilePos = p.ToArray();
    }
}
