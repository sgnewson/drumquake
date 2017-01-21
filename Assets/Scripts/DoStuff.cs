using UnityEngine;

public class DoStuff : MonoBehaviour {

    public Tower tower;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseUp()
    {
        print(this.tower.blockMatrix);
        var maxWidthBlocks = 0;
        for( var x = 0; x < tower.gridWidth; x++)
        {
            if (this.tower.blockMatrix[0,x] != null)
            {
                maxWidthBlocks++;
            }
        }

        var y = 1;
        for (; y < tower.gridHeight; y++)
        {
            var localWidthBlocks = 0;
            for (var x = 0; x < tower.gridWidth; x++)
            {
                if (this.tower.blockMatrix[y, x] != null)
                {
                    localWidthBlocks++;
                }
            }

            if(localWidthBlocks < maxWidthBlocks)
            {
                break;
            }
        }
        for (; y < tower.gridHeight; y++)
        {
            for (var x = 0; x < tower.gridWidth; x++)
            {
                if (this.tower.blockMatrix[y, x] != null)
                {
                    var block = this.tower.blockMatrix[y, x];
                    var rb = block.GetComponent<Rigidbody>();
                    rb.useGravity = true;
                    rb.isKinematic = false;

                    block.collapsed = true;
                    if(Random.Range(0,1)==0)
                    {
                        rb.AddForce(-100, 0, 0);
                    }
                    {
                        rb.AddForce(100, 0, 0);
                    }
                }
            }
        }
        /*
        foreach (var i in tower.blocks)
        {
            var rb = i.GetComponent<Rigidbody>();
            rb.AddForce(100, 0, 0);
        }*/
    }
}
