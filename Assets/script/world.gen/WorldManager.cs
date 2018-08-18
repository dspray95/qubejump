using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public int numChunks = 6;
    public int chunkSize = 40;
    int totalChunks;
    int lastNewChunk;
    bool buildingChunk;
    List<Chunk> chunks; 
    Builder builder;
    Generator generator;
    PlayerController player;

	// Use this for initialization
	void Start () {
        totalChunks = 0;
        chunks = new List<Chunk>();
        generator = new Generator(chunkSize);
        builder = (Builder)transform.GetComponent("Builder");
        initLevel();
        buildingChunk = false;
	}
	
	void Update () {
        //If the player is halfway towards the the end of current chunks and we havn't already built a new one and removed the oldest
        //we need to do so
        //Example:  Player is at chunk 10. NumChunks is 4, totalChunks is 12. 
        //              playerChunk >= totalChunks - numChunks/2 = 10 >= 12 - 2 = true
        //              lastNewChunk = 8 < playerChunk = true
        //              so build new chunk...
        //          But if playerchunk = 10, numchunks = 4, totalChunks = 12 and lastnewChunk = 10
        //          then... 
        //              10 > 12 - 2 = true
        //              10 < 10 = false
        //              so do nothing...
        if (player.GetCurrentChunk() >= totalChunks - (numChunks / 2) && lastNewChunk < player.GetCurrentChunk() && !buildingChunk)
        {
            buildingChunk = true;
            ChunkCycle();
        }
	}

    void ChunkCycle()
    {
        //Destroy first chunk
        chunks[0].Destroy();
        chunks[0] = null;
        chunks.Remove(chunks[0]);
        //Generate and build new chunk
        Chunk newChunk = generator.GenerateChunk(chunks[chunks.Count - 1], totalChunks);
        builder.BuildChunk(newChunk);
        chunks.Add(newChunk);
        //Sort chunk related variables;
        totalChunks++;
        lastNewChunk = player.GetCurrentChunk();
        buildingChunk = false;
    }

    void initLevel()
    {
        chunks.Add(generator.GenerateFirstChunk());

        for(int i = 1; i <= numChunks; i++)
        {
            chunks.Add(generator.GenerateChunk(chunks[i - 1], i));
        }

        foreach (Chunk chunk in chunks)
        {
            builder.BuildChunk(chunk);
            totalChunks++;
        }

        CameraController cameraController = (CameraController)Camera.main.GetComponent("CameraController");
        Transform playerTransfrom = builder.BuildPlayer(chunks[0]);
        player = (PlayerController)playerTransfrom.GetComponent("PlayerController");
        player.parentWorld = this;
        cameraController.player = playerTransfrom;;
    }
}
