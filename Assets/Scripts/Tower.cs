
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private int _totalHeight;
    public List<Block> blocks;
    public Spawner blockSpawner;

    struct GridRow
    {
        List<TowerGridSlot> gridCells;
    }

    List<GridRow> grid;

    public int gridHeight = 5;
    public int gridWidth = 5;

    public TowerGridSlot towerGridSlot;
    
	// Use this for initialization
	void Start ()
    {
        var buildDelta = new Vector3(0, 0.5f, 0);
        for (var y = 0; y < gridHeight; y++)
        {
            buildDelta.x = 0;
            for (var x = 0; x < gridWidth; x++)
            {
                var newCell = Instantiate(towerGridSlot, buildDelta, Quaternion.identity);
                newCell.gameObject.SetActive(true);
                buildDelta.x++;
            }
            buildDelta.y++;
        }

        blockSpawner = Instantiate(blockSpawner, new Vector3(5, 5), Quaternion.identity);

        blocks.Add(blockSpawner.SpawnBlock());
    }

    // Update is called once per frame
    void Update ()
    {
        if (blocks.Count != 0)
        {
            if (blocks[blocks.Count - 1].locked)
            {
                blockSpawner.hasBlock = false;
                blocks.Add(blockSpawner.SpawnBlock());
            }
        }
    }
}
