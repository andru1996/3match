using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_List : MonoBehaviour
{
    public List<GameObject> tiles;
    
    public int FindIndexForTexture(Texture target)
    {

        for(int i=0;i<tiles.Count;i++)
        {
            if (tiles[i].GetComponent<Tile>().GetRawImage().texture == target) return i;
        }

        return -1;
    }
}
