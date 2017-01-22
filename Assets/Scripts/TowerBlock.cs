using System.Collections.Generic;
using UnityEngine;

public class TowerBlock : MonoBehaviour
{
    public class GroupEntity
    {
        public List<TowerBlock> memberBlocks;
    }

    public int gridX { get; set; }
    public int gridY { get; set; }
    public int type { get; set; }

    public Tower tower;

    public GroupEntity blockGroup { get; set; }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (this.transform.position.y < -1)
        {
            tower.glueBlockMatrix[gridY, gridX] = null;
            Destroy(this);
        }
	}
}
