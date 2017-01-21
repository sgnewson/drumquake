
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private int _totalHeight;
    public List<TowerBlock> blocks;

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
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
