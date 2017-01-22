﻿
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private int _totalHeight;
    public Spawner blockSpawner;

    public TowerGridSlot[,] gridCells;
    public Block[,] blockMatrix;
    public List<Block> blocks;
    private Block lastBlock;

    public int gridHeight = 5;
    public int gridWidth = 5;

    public TowerGridSlot towerGridSlot;
    
    private void BuildGridCells()
    {
        gridCells = new TowerGridSlot[gridHeight, gridWidth];

        var buildDelta = new Vector3(0, 0, 0);
        for (var y = 0; y < gridHeight; y++)
        {
            buildDelta.x = 0;
            for (var x = 0; x < gridWidth; x++)
            {
                var newCell = Instantiate(towerGridSlot, buildDelta, Quaternion.identity);
                newCell.gameObject.SetActive(true);
                buildDelta.x++;

                newCell.gridX = x;
                newCell.gridY = y;
                gridCells[y, x] = newCell;
            }
            buildDelta.y++;
        }
    }

    public bool AddBlock(Block block)
    {
        var x = block.gridX;
        var y = block.gridY;

        if (gridCells[x, y].isFilled)
        {
            return false;
        }

        if(x < 0 || x >= gridWidth)
        {
            return false;
        }

        if (y < 0 || y >= gridHeight)
        {
            return false;
        }

        this.blockMatrix[y, x] = block;
        gridCells[x, y].isFilled = true;
        return true;
    }

	// Use this for initialization
	void Start ()
    {
        blockMatrix = new Block[gridHeight, gridWidth];

        BuildGridCells();

        blockSpawner = Instantiate(blockSpawner, new Vector3(5, 5), Quaternion.identity);
        lastBlock = blockSpawner.SpawnBlock();
        blocks.Add(lastBlock);
    }

    // Update is called once per frame
    void Update ()
    {
        if (blocks.Count != 0)
        {
            if (blocks[blocks.Count - 1].locked)
            {
                //if (gridCells[lastBlock.gridX, lastBlock.gridY].isFilled && blockMatrix[lastBlock.gridY, lastBlock.gridX] != lastBlock)
                //{
                //    Block tempBlock = lastBlock;
                //    tempBlock.gameObject.transform.position = new Vector3(-100, -100, 0);
                //    blockSpawner.hasBlock = false;
                //    lastBlock = blockSpawner.SpawnBlock();
                //    blocks.Add(lastBlock);
                //    DestroyImmediate(tempBlock);
                //    print("Reset block");
                //    return;
                //}
                if (AddBlock(lastBlock))
                {
                    blockSpawner.hasBlock = false;
                   // AddBlock(lastBlock);
                    lastBlock = blockSpawner.SpawnBlock();
                    blocks.Add(lastBlock);
                }
                else
                {
                    Block tempBlock = lastBlock;
                    blockSpawner.hasBlock = false;
                    lastBlock = blockSpawner.SpawnBlock();
                    blocks.Add(lastBlock);
                    DestroyImmediate(tempBlock);
                }
            }
        }
    }
}
