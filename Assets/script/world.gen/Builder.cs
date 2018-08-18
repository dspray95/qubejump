using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour {

    public Transform player;
    //Terrain Objects
    public Transform ground;
    public Transform belowGround;
    public Transform ceiling;
    public Transform aboveCeiling;
    public Transform ore;
    public Transform lava;
    public Transform lavaLight;
    //PowerUps
    public Transform starburst;
    public Transform coin;
    public Transform hopperCoin;
    public Transform speedBoost;
    public Transform speedReduce;
    public Transform jumpBoost;
    //Mobs
    public Transform dumbPatrol; 

    public void BuildChunk(Chunk chunk)
    {
        BuildTerrain(chunk);
        BuildTerrainFeatures(chunk);
        BuildPowerUps(chunk);
        if (chunk.ceilingHeights != null)
        {
            BuildCeiling(chunk);
        }
    }

    public Transform BuildPlayer(Chunk chunk)
    {
        Transform playerTransform = Transform.Instantiate(player, new Vector3(chunk.startPos + chunk.heights.Count -5 , 2f, 0), Quaternion.identity);
        return playerTransform;
    }

    public void BuildTerrain(Chunk chunk)
    {
        List<int> heights = chunk.heights;
        List<TerrainObject> terrain = chunk.terrainFeatures;
        int prevHeight = 0;
        BoxCollider2D currentCollider = null;
        Transform groundTransform = null;
        for (int i = 0; i < heights.Count; i++)
        {
            groundTransform = Transform.Instantiate(ground, new Vector3(i + chunk.startPos + 1, heights[i], 0), Quaternion.identity);
            Transform belowGroundTransform = Transform.Instantiate(belowGround, new Vector3(i + chunk.startPos + 1, heights[i] - (belowGround.transform.localScale.y / 2), 0), Quaternion.identity);
            chunk.gameObjects.Add(groundTransform);
            chunk.gameObjects.Add(belowGroundTransform);



            //do box collider stuff
            if (currentCollider == null)
            {
                currentCollider = groundTransform.gameObject.AddComponent<BoxCollider2D>();
            }
            if (heights[i] != prevHeight)
            {
                currentCollider = groundTransform.gameObject.AddComponent<BoxCollider2D>();
                currentCollider.size = new Vector2(currentCollider.size.x, 30);
                currentCollider.offset = new Vector2(0, 0.5f - (currentCollider.size.y / 2));
            }
            else
            {
                currentCollider.size = new Vector2(currentCollider.size.x + 1, 30);
                currentCollider.offset = new Vector2((currentCollider.size.x / 2) - 0.5f, 0.5f - (currentCollider.size.y / 2));
            }
            prevHeight = heights[i];
        }
    }

    public BoxCollider2D BuildBoxCollider(GameObject parent, int length)
    {
        BoxCollider2D boxCollider = parent.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(length, 1);
        boxCollider.offset = new Vector2(0 - (length / 2), 0);
        return boxCollider;
    }

    public void BuildTerrainFeatures(Chunk chunk)
    {
        int poolEnd = 0;

        for(int i = 0; i < chunk.terrainFeatures.Count; i++)
        {
            if (chunk.terrainFeatures[i] == TerrainObject.ORE && chunk.ceilingHeights != null && i > 1 && chunk.chunkType != CaveType.CAVERN)
            {
                List<Vector2> oreList = new List<Vector2>();
                oreList.Add(new Vector2(i + chunk.startPos, Random.Range(0, 100) < 50 ? chunk.ceilingHeights[i] + (Random.Range(5, 15)) : chunk.heights[i] - (Random.Range(5, 15))));
                bool buildingOre = true;
                string color = Random.Range(0, 100) > 50 ? "white" : "gold";
                int oreSize = 0;
                while (buildingOre)
                {
                    if (Random.Range(0, 100) > oreSize * 25)
                    {
                        Vector2 vector = new Vector2();
                        switch ((int)Random.Range(0, 4))
                        {
                            case 0:
                                vector = new Vector2(oreList[oreList.Count - 1].x + 0.5f, oreList[oreList.Count - 1].y);
                                break;
                            case 1:
                                vector = new Vector2(oreList[oreList.Count - 1].x - 0.5f, oreList[oreList.Count - 1].y);
                                break;
                            case 2:
                                vector = new Vector2(oreList[oreList.Count - 1].x, oreList[oreList.Count - 1].y + 0.5f);
                                break;
                            case 3:
                                vector = new Vector2(oreList[oreList.Count - 1].x, oreList[oreList.Count - 1].y - 0.5f);
                                break;
                            default:
                                vector = new Vector2(oreList[oreList.Count - 1].x, oreList[oreList.Count - 1].y + 0.5f);
                                break;
                        }
                        oreList.Add(vector);
                        oreSize++;
                    }
                    else
                    {
                        buildingOre = false;
                    }
                }
                float totalX = 0;
                float totalY = 0;
                foreach (Vector2 vector in oreList)
                {
                    if (vector.y < chunk.heights[i] || vector.y > chunk.ceilingHeights[i])
                    {
                        Transform oreTransform = Transform.Instantiate(ore, vector, Quaternion.identity);
                        totalX += vector.x;
                        totalY += vector.y;
                        if (color.Equals("gold"))
                        {
                            oreTransform.GetComponent<SpriteRenderer>().color = new Color(204, 204, 155);
                        }
                        chunk.gameObjects.Add(oreTransform);
                    }
                }

                Vector3 lightVector = new Vector3(totalX / oreList.Count, (totalY / oreList.Count), -2f);
                GameObject lightObj = new GameObject();
                lightObj.transform.position = lightVector;

                Light lightComponent = lightObj.AddComponent<Light>();
                if (color.Equals("gold"))
                {
                    lightComponent.color = Color.yellow;
                }
                else
                {
                    lightComponent.color = Color.white;

                }
                lightComponent.intensity = Random.Range(1f, 1.5f);
                chunk.gameObjects.Add(lightObj.transform);
            }
            else if (chunk.terrainFeatures[i] == TerrainObject.LAVA_POOL)
            {
                Transform lavaTransformTop = Transform.Instantiate(lava, new Vector3(i + chunk.startPos + 1, chunk.heights[i] + 1, 0), Quaternion.identity);
                Transform lavaTransformBottom = Transform.Instantiate(lava, new Vector3(i + chunk.startPos + 1, chunk.heights[i], 0), Quaternion.identity);
                chunk.gameObjects.Add(lavaTransformTop);
                chunk.gameObjects.Add(lavaTransformBottom);

                if (i > poolEnd)
                {
                    bool lavaPool = true;
                    int poolLength = 0;
                    while (lavaPool)
                    {
                        if (chunk.terrainFeatures[i + poolLength] == TerrainObject.LAVA_POOL)
                        {
                            poolLength++;
                        }
                        else
                        {
                            poolEnd = i + poolLength;
                            lavaPool = false;
                        }
                    }
                    Transform lavaLightTransform = Transform.Instantiate(lavaLight, new Vector3(chunk.startPos + i + (poolLength / 2), chunk.heights[i] + 1.5f, -2f), Quaternion.identity);
                    chunk.gameObjects.Add(lavaLightTransform);
                }
            }
        }
    }

    public void BuildPowerUps(Chunk chunk)
    {
        List<PowerUp> powerups = chunk.powerUps;
        for(int i = 0; i < chunk.powerUps.Count; i++)
        {
            if(powerups[i] == PowerUp.COIN)
            {
                Transform coinTransform = Transform.Instantiate(coin, new Vector3(i + chunk.startPos, chunk.heights[i] + 2, 0), Quaternion.identity);
                chunk.gameObjects.Add(coinTransform);
            }
            else if(powerups[i] == PowerUp.STARBURST)
            {
                int height = (chunk.heights[i] + chunk.ceilingHeights[i]) / 2;
                Transform starburstTransform = Transform.Instantiate(starburst, new Vector3(i + chunk.startPos, height, 0), Quaternion.identity);
                chunk.gameObjects.Add(starburstTransform);
            }
            else if(powerups[i] == PowerUp.SPEED_UP)
            {
                Transform speedUpTransform = Transform.Instantiate(speedBoost, new Vector3(i + chunk.startPos, chunk.heights[i] + 3, 0), Quaternion.identity);
                speedUpTransform.Rotate(new Vector3(0, 0, -90));
                chunk.gameObjects.Add(speedUpTransform);
            }
            else if(powerups[i] == PowerUp.SPEED_DOWN)
            {
                Transform speedDownTransform = Transform.Instantiate(speedReduce, new Vector3(i + chunk.startPos, chunk.heights[i] + 3, 0), Quaternion.identity);
                speedDownTransform.Rotate(new Vector3(0, 0, 90));
                chunk.gameObjects.Add(speedDownTransform);
            }
            else if(powerups[i] == PowerUp.JUMP_BOOST)
            {
                Transform jumpBoostTransform = Transform.Instantiate(jumpBoost, new Vector3(i + chunk.startPos, chunk.heights[i] + 3, 0), Quaternion.identity);

                chunk.gameObjects.Add(jumpBoostTransform);
            }
            else if(powerups[i] == PowerUp.HOPPER_COIN)
            {
                Transform hopperTransform = Transform.Instantiate(hopperCoin, new Vector3(i + chunk.startPos, chunk.heights[i] + 2, 0), Quaternion.identity);
                chunk.gameObjects.Add(hopperTransform);
            }
        }
    }

    public void BuildCeiling(Chunk chunk)
    {
        List<int> ceilingHieghts = chunk.ceilingHeights;
        for (int i = 0; i < ceilingHieghts.Count; i++)
        {
            try
            {
                if (chunk.terrainFeatures[i] != TerrainObject.FISSURE)
                {
                    Transform ceilingTipTransform = Transform.Instantiate(ceiling, new Vector3(i + chunk.startPos + 1, ceilingHieghts[i], 0), Quaternion.identity);
                    Transform ceilingTransform = Transform.Instantiate(aboveCeiling, new Vector3(i + chunk.startPos + 1, ceilingHieghts[i] + belowGround.transform.localScale.y / 2, 0), Quaternion.identity);
                    chunk.gameObjects.Add(ceilingTipTransform);
                    chunk.gameObjects.Add(ceilingTransform);
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Debug.Log(i);
            }
        }
    }

}
