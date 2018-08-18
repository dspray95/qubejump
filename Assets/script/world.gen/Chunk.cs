using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

    public CaveType chunkType; 
    public List<int> heights;
    public List<int> ceilingHeights;
    public List<TerrainObject> terrainFeatures;
    public List<Mob> mobs;
    public List<PowerUp> powerUps; 
    public List<Transform> gameObjects;
    public int startPos;

    public Chunk(CaveType chunkType, int startPos, List<int> heights, List<TerrainObject> terrainFeatures, List<PowerUp> powerUps, List<Mob> mobs) 
    {
        this.heights = heights;
        this.terrainFeatures = terrainFeatures;
        this.powerUps = powerUps;
        this.mobs = mobs;
        this.startPos = startPos + 1;
        this.gameObjects = new List<Transform>();
    }

    public void Destroy()
    {
        foreach(Transform obj in gameObjects)
        {
            try
            {
                Object.Destroy(obj.gameObject);
            }
            catch (System.Exception e)
            {
                Debug.Log("Exception: \n" + e + "\nwith GameObject " + obj.name);
            }
        }
    }
}
