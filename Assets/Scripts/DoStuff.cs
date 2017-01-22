using UnityEngine;

public class DoStuff : MonoBehaviour {

//    public Tower tower;

//	// Use this for initialization
//	void Start () {
		
//	}
	
//	// Update is called once per frame
//	void Update () {
		
//	}

//    private void OnMouseUp()
//    {
//        for (var j = 0; j < tower.gridHeight; j++)
//        {
//            var str = "";
//            for (var i = 0; i < tower.gridWidth; i++)
//            {
//                if(tower.blockMatrix[j,i] == null)
//                {
//                    str += "O";
//                }
//                else
//                {
//                    str += "X";
//                }
//            }
//            print(str);
//        }
//        var maxWidthBlocks = 0;
//        for( var x = 0; x < tower.gridWidth; x++)
//        {
//            if (this.tower.blockMatrix[0,x] != null)
//            {
////                this.tower.blockMatrix[0, x].rb.useGravity = false;
////                this.tower.blockMatrix[0, x].rb.isKinematic = true;
//                maxWidthBlocks++;
//            }
//        }

//        var y = 1;
//        var localWidthBlocks = 0;
//        for (; y < tower.gridHeight; y++)
//        {
//            localWidthBlocks = 0;
//            for (var x = 0; x < tower.gridWidth; x++)
//            {
//                if (this.tower.blockMatrix[y, x] != null)
//                {
////                    this.tower.blockMatrix[y, x].rb.useGravity = false;
////                    this.tower.blockMatrix[y, x].rb.isKinematic = true;
//                    localWidthBlocks++;
//                }
//            }

//            if(localWidthBlocks < maxWidthBlocks)
//            {
//                break;
//            }
//        }
//        print(localWidthBlocks + " - " + maxWidthBlocks);
//        if(localWidthBlocks >= maxWidthBlocks)
//        {
//            return;
//        }
        
//        // go to lower level and start marking things as collapsed
//        for (y -= 1; y < tower.gridHeight; y++)
//        {
//            for (var x = 0; x < tower.gridWidth; x++)
//            {
//                if (this.tower.blockMatrix[y, x] != null)
//                {
//                    var block = this.tower.blockMatrix[y, x];
//                    var rb = block.GetComponent<Rigidbody>();
//                    rb.useGravity = true;
//                    rb.isKinematic = false;
//                    print("here" + maxWidthBlocks);
//                    block.collapsed = true;
//                    if(Random.Range(0,1)==0)
//                    {
//                        rb.AddForce(-100, 0, 0);
//                    }
//                    {
//                        rb.AddForce(100, 0, 0);
//                    }
//                }
//            }
//        }
//        /*
//        foreach (var i in tower.blocks)
//        {
//            var rb = i.GetComponent<Rigidbody>();
//            rb.AddForce(100, 0, 0);
//        }*/
//    }
}
