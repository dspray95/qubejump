using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator {

    int chunkSize; 

	public Generator(int chunkSize)
    {
        this.chunkSize = chunkSize;
    }

    public Chunk GenerateChunk(Chunk previousChunk, int currentPosition)
    {
        CaveType caveType = RollCaveType();
        List<int> groundHeights = GenerateHeights(previousChunk);
        List<Mob> mobs = new List<Mob>();
        List<PowerUp> powerUps = GeneratePowerUps(groundHeights);
        List<TerrainObject> terrainObjects = GenerateTerrainFeatures(groundHeights, caveType);
        Chunk chunk = new Chunk(caveType, currentPosition * chunkSize + 1, groundHeights, terrainObjects, powerUps, mobs);

        if (caveType != CaveType.CAVERN)
        {
            chunk.ceilingHeights = GenerateCeiling(groundHeights, caveType);
        }
        else if(caveType == CaveType.CAVERN)
        {
            chunk.ceilingHeights = GenerateCavern(previousChunk, groundHeights);
        }

        return chunk;
    }

    public List<int> GenerateHeights(Chunk previousChunk)
    {
        List<int> heights = new List<int>();

        for(int i = 0; i < chunkSize; i++)
        {
            //If we're on the first few squares of the chunk, make sure the transition between this one and the previous is smooth
            if(i < 2)
            {
                heights.Add(previousChunk.heights[previousChunk.heights.Count - 1]);
            }
            else
            {
                heights.Add(Random.Range(heights[i - 1] - 2, heights[i - 1] + 3));
            }
        }
        return SmoothHeights(heights);
    }

    public List<int> SmoothHeights(List<int> heights)
    {
        for(int i = 0; i < heights.Count; i++)
        {
            if(i > 1 && i < heights.Count - 1)
            {
                if(heights[i - 1] < heights[i] && heights[i + 1] < heights[i])
                {
                    heights[i] = heights[i - 1];
                }
                else if(heights[i - 1] > heights[i] && heights[i + 1] > heights[i])
                {
                    heights[i] = heights[i - 1];
                }
            }
        }
        return heights;
    }

    public List<int> GenerateCeiling(List<int> groundHeights, CaveType caveType)
    {
        int[] heightRange; 
        if(caveType == CaveType.LOW)
        {
            heightRange = new int[]{ 6, 10};
        }
        else if(caveType == CaveType.MID)
        {
            heightRange = new int[] { 9, 15 };
        }
        else
        {
            heightRange = new int[] { 12, 16 };
        }

        List<int> ceiling = new List<int>();
        for (int i = 0; i < chunkSize; i++)
        {
            int heightDifference = 10;
            if (i % 5 == 0)
            {
                heightDifference = Random.Range(heightRange[0], heightRange[1]);
            }
            ceiling.Add(Random.Range(groundHeights[i] - 1, groundHeights[i] + 2) + heightDifference);
        }
        if(caveType == CaveType.MID)
        {
            return SmoothHeights(ceiling);
        }
        else
        { 
            return ceiling;
        }
        
    }

    public List<int> GenerateCavern(Chunk previousChunk, List<int> heights)
    {
        //If the current tile is five after the beginning or five before the end then smooth the transition to a large
        //cavern otherwise build a cavern ceiling
        List<int> ceilingHeights = new List<int>();
        for (int i = 0; i < chunkSize; i++) {
            if (i < 5)
            {
                //transition up to cavern height
                ceilingHeights.Add(previousChunk.ceilingHeights[previousChunk.ceilingHeights.Count - 1] + (i*3));
            }
            else if (i > chunkSize - 5)
            {
                //transition back to a normal height
                int h = ceilingHeights[i - 1] - ((chunkSize - i) * 3) > heights[i] + 5 ? ceilingHeights[i - 1] - ((chunkSize - i) * 3) : heights[i] + 5;
                ceilingHeights.Add(h);
            }
            else if(i < chunkSize/2)
            {
                //Cavern tends to slope upwards
                int lowerBound = ceilingHeights[i - 1] - 3 > heights[i] + 5 ? ceilingHeights[i - 1] : heights[i] + 5;
                int upperBound = ceilingHeights[i - 1] + 3 > lowerBound ? ceilingHeights[i - 1] + 2 : lowerBound + 2;
                ceilingHeights.Add(Random.Range(lowerBound, upperBound));
            }
            else
            {
                //Cavern tends to slope downwards
                int lowerBound = ceilingHeights[i - 1] - 3 > heights[i] + 5 ? ceilingHeights[i - 1] - 3 : heights[i] + 5;
                int upperBound = ceilingHeights[i - 1] + 1 > heights[i] + 5 ? ceilingHeights[i - 1] + 1 : heights[i] + 5;
                ceilingHeights.Add(Random.Range(lowerBound, upperBound));
            }
        }
        return ceilingHeights;
    }

    public CaveType RollCaveType()
    {
        int typeRoll = Random.Range(0, 100);
        if (typeRoll < 40)
        {
            return CaveType.LOW;
        }
        else if (typeRoll >= 40 && typeRoll < 80)
        {
            return CaveType.MID;
        }
        else
        {
            return CaveType.CAVERN;
        }
    }

    /*
     * Generate terrain objects such as chasms, fissures, ore, and gold
    */
    public List<TerrainObject> GenerateTerrainFeatures(List<int> heights, CaveType caveType)
    {
        List<TerrainObject> features = new List<TerrainObject>();
        int chasmChance = 2;
        int fissureChance = chasmChance + 2;
        int oreChance = fissureChance + 3;
        int lavaPoolChance = oreChance + 5;
        int waterPoolChance = lavaPoolChance + 5;
        int waterFallChance = waterPoolChance + 2;

        for (int i = 0; i < chunkSize; i++)
        {

            if (i < heights.Count - 4)
            {
                int roll = Random.Range(0, 100);
                if (roll < chasmChance)
                {
                    features.AddRange(CreateTerrainObject(3, TerrainObject.CHASM));
                    i += 2;
                }
                else if (roll > chasmChance && roll < fissureChance)
                {
                    features.AddRange(CreateTerrainObject(3, TerrainObject.FISSURE));
                    i += 2;
                }
                else if (roll > fissureChance && roll < oreChance)
                {
                    features.AddRange(CreateTerrainObject(2, TerrainObject.ORE));
                    i += 1;
                }
                else if (roll > oreChance && roll < lavaPoolChance && caveType != CaveType.LOW) 
                {
                    try
                    {
                        if (heights[i - 1] == heights[i + 4])
                        {
                            for(int j = i; j < i + 4; j++)
                            {
                                heights[j] = heights[i] - 2;
                            }
                            features.Add(TerrainObject.TERRAIN);
                            features.AddRange(CreateTerrainObject(3, TerrainObject.LAVA_POOL));
                            i += 3;
                        }
                        else
                        {
                            features.Add(TerrainObject.TERRAIN);
                        }
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        features.Add(TerrainObject.TERRAIN);
                    }
                }
                else
                {
                    features.Add(TerrainObject.TERRAIN);
                }
            }
            else
            {
                features.Add(TerrainObject.TERRAIN);
            }
        }
        return features;
    }

    public List<TerrainObject> CreateTerrainObject(int length, TerrainObject tObj)
    {
        List<TerrainObject> newTerrainObject = new List<TerrainObject>();
        for (int i = 0; i < length; i++)
        {
            newTerrainObject.Add(tObj);
        }
        return newTerrainObject;
    }

    public List<PowerUp> GeneratePowerUps(List<int> heights) 
    {
        //SPEED_UP, SPEED_DOWN, JUMP_BOOST, JUMP_MUSHROOM, COIN, HOPPER_COIN, GRAVITY_SWITCH
        List<PowerUp> powerUps = new List<PowerUp>();
        float speedUpChance = 0.05f;
        float speedDownChance = speedUpChance + 0.05f;
        float jumpBoostChance = speedDownChance + 0.1f;
        float jumpMushroomChance = jumpBoostChance + 1;
        float coinChance = jumpMushroomChance + 4;
        float hopperCoinChance = coinChance + 2;
        float gravitySwitchChance = hopperCoinChance + 0.5f;
        float starburstChance = gravitySwitchChance + 0.5f;

        for(int i = 0; i < chunkSize; i++)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= speedUpChance)
            {
                powerUps.Add(PowerUp.SPEED_UP);
            }
            else if (roll > speedUpChance && roll <= speedDownChance)
            {
                powerUps.Add(PowerUp.SPEED_DOWN);
            }
            else if (roll > speedDownChance && roll <= jumpBoostChance)
            {
                powerUps.Add(PowerUp.JUMP_BOOST);
            }
            else if (roll > jumpBoostChance && roll <= jumpMushroomChance)
            {
                powerUps.Add(PowerUp.JUMP_MUSHROOM);
            }
            else if (roll > jumpBoostChance && roll <= coinChance)
            {
                if (isValidCoin(heights, i))
                {
                    powerUps.Add(PowerUp.COIN);
                }
                else
                {
                    powerUps.Add(PowerUp.EMPTY);
                }
            }
            else if (roll > coinChance && roll <= hopperCoinChance)
            {
                if (isValidCoin(heights, i))
                { 
                    powerUps.Add(PowerUp.HOPPER_COIN);
                }
                else
                {
                    powerUps.Add(PowerUp.EMPTY);
                }
            }
            else if (roll > hopperCoinChance && roll <= gravitySwitchChance)
            {
                powerUps.Add(PowerUp.GRAVITY_SWITCH);
            }
            else if(roll > gravitySwitchChance && roll <= starburstChance)
            {
                try
                {
                    bool recentStarburst = false;
                    for(int j = i - 10; j < i; j++)
                    {
                        if(powerUps[j] == PowerUp.STARBURST)
                        {
                            recentStarburst = true;
                        }
                    }
                    if (recentStarburst)
                    {
                        powerUps.Add(PowerUp.EMPTY);
                    }
                    else {
                        powerUps.Add(PowerUp.STARBURST);
                    }
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    powerUps.Add(PowerUp.EMPTY);
                }
            }
            else
            {
                powerUps.Add(PowerUp.EMPTY);
            }
        }
        return powerUps;
    }

    private bool isValidCoin(List<int> heights, int index)
    {
        return heights[index - 1] == heights[index] && heights[index + 1] == heights[index];
    }
    //Create the initial chunk, the player starts here.
    public Chunk GenerateFirstChunk()
    {
        List<int> heights = new List<int>();
        List<Mob> mobs = new List<Mob>();
        List<PowerUp> powerUps = new List<PowerUp>();
        List<TerrainObject> terrainFeatures = new List<TerrainObject>();
        for(int i = 0; i <= chunkSize; i++)
        {
            heights.Add(0);
            terrainFeatures.Add(TerrainObject.TERRAIN);
        }
        int chunkStart = 0;
        Chunk chunk = new Chunk(CaveType.MID, chunkStart, heights, terrainFeatures, powerUps, mobs);
        chunk.ceilingHeights = GenerateCeiling(heights, CaveType.HIGH);
        return chunk;
    }
}
