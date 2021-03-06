﻿
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XboxCtrlrInput;

public class Tower : MonoBehaviour
{
    private int _totalHeight;
//    public Spawner blockSpawner;

    public TowerGridSlot[,] gridCells;
    public Block[,] glueBlockMatrix;
    //public Block[,] blockMatrix;
    public List<Block> blocks;
	private Block lastBlock;

    public int gridHeight;
    public int gridWidth;

	SoundEffectManager soundEffectManager;

    //for organisation; contains all gridSlot instances
	public GameObject GridContainer;

    public TowerGridSlot towerGridSlot;

    public Block[] towerBlocks;

    public Block currentBlock { get; set; }

    private Dictionary<XboxButton, int> dict;

	void Awake() {
		soundEffectManager = GameObject.Find ("SoundEffectManager").GetComponent<SoundEffectManager> ();
	}

    private void BuildGridCells()
    {
        gridCells = new TowerGridSlot[gridHeight, gridWidth];

        var buildDelta = new Vector3(0, 0.5f, 0);
        for (var y = 2; y < gridHeight - 2; y++)
        {
            buildDelta.x = 0;
            for (var x = 2; x < gridWidth - 2; x++)
            {
                var newCell = Instantiate(towerGridSlot, buildDelta, Quaternion.identity);
				if (Random.Range (0, 10) == 1) {
					newCell.isFilled = true;
					newCell.gameObject.GetComponent<MeshRenderer> ().enabled = false;
				}

                newCell.gameObject.SetActive(true);
				newCell.gameObject.transform.parent = GridContainer.transform;
                buildDelta.x++;

                newCell.gridX = x;
                newCell.gridY = y;
                gridCells[y, x] = newCell;
            }
            buildDelta.y++;
        }
    }

	public void HandleBeforeEarthquake() {
		if (currentBlock) {
			Destroy (currentBlock.gameObject);
			Destroy (currentBlock);
		}
//		lastBlock.gameObject.SetActive (false);
	}

	public void HandleAfterEarthquake() {
//		lastBlock.gameObject.SetActive (true);
//		lastBlock.gameObject.GetComponent<Rigidbody> ().useGravity = false;
//		lastBlock.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
	}

 	// Use this for initialization
	void Start ()
    {
        dict = new Dictionary<XboxButton, int>();
        dict.Add(XboxButton.A, 0);
        dict.Add(XboxButton.X, 1);
        dict.Add(XboxButton.Y, 2);
        dict.Add(XboxButton.B, 3);

        gridHeight = 15 + 4;
        gridWidth = 8 + 4;

        //blockMatrix = new Block[gridHeight, gridWidth];
        glueBlockMatrix = new Block[gridHeight, gridWidth];

        BuildGridCells();
//        if (!blockSpawner) { return; }
//        
//        lastBlock = blockSpawner.SpawnBlock();
//        glueBlockMatrix[lastBlock.gridY, lastBlock.gridX] = lastBlock;
//        AddGlues(lastBlock);
//        blocks.Add(lastBlock);
    }

	void PrintAllBlocks() {
		int x = 0;
		string output = "";

		foreach (Block b in blocks) {
			output += " b " + x + " locked:" + b.locked;
			x++;
		}

		Debug.Log (output);
	}

    // Update is called once per frame
    void Update ()
    {
		if (!GameManager.PlayOn) {
			return;
		}

        if (Input.GetMouseButton(0))
        {
            print(this.currentBlock);
            if (this.currentBlock)
            {
                if(!this.currentBlock.wasClicked)
                {
                    return;
                }
                this.currentBlock.DoMove();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            print(this.currentBlock);
            if (this.currentBlock)
            {
                this.currentBlock.DoPlace();
				soundEffectManager.PlayPlaceBlockSound ();
            }
        }

        if (blocks.Count != 0)
        {
			PrintAllBlocks ();
            /*
            if (blocks[blocks.Count - 1].locked)
            {
                if (AddBlock(lastBlock))
                {
                    blockSpawner.hasBlock = false;
					lastBlock.Score = lastBlock.gridY / 2;
					Debug.Log ("Score: " + lastBlock.Score);
                    lastBlock = blockSpawner.SpawnBlock();
                    glueBlockMatrix[lastBlock.gridY, lastBlock.gridX] = lastBlock;

                    //AddGlues(lastBlock);
                    blocks.Add(lastBlock);
					Debug.Log ("Add block success. Last block: " + lastBlock.gameObject.name);
                }
                else
                {
					Debug.Log ("Add block false so reset the block. Last block: " + lastBlock.gameObject.name);
                    Block tempBlock = lastBlock;
                    blockSpawner.hasBlock = false;
                    lastBlock = blockSpawner.SpawnBlock();
                    glueBlockMatrix[lastBlock.gridY, lastBlock.gridX] = lastBlock;
                    //AddGlues(lastBlock);
                    blocks.Add(lastBlock);
                    blocks.Remove(tempBlock);
                    DestroyImmediate(tempBlock.gameObject);
                }
            }
            */
        }

        //should be handled by block collision
        /*
        foreach (Block block in blocks)
        {
            if (block.transform.position.y + 2 < -1)
            {
                glueBlockMatrix[block.gridY, block.gridX] = null;
                Destroy(this);
            }
        }
        */
    }

    public bool AddBlock(Block block)
    {
        var x = block.gridX;
        var y = block.gridY;

		Debug.Log ("AddBlock x:" + x + " y:" + y);

        //out of bounds x
        if (x < 2 || x >= gridWidth - 2)
        {
            return false;
        }

        //out of bounds y
        if (y < 2 || y >= gridHeight - 2)
        {
            return false;
        }

        //out of bounds (extra)
        if (gridCells[y, x] == null)
        {
            return false;
        }

        //cell is already ocupied
        if (gridCells[y, x].isFilled)
        {
            return false;
        }

        if (!glueBlockMatrix[y, x-1]
            && !glueBlockMatrix[y, x+1]
            && !glueBlockMatrix[y+1, x]
            && !glueBlockMatrix[y-1, x]
            && y != 2)
        {
            return false;
        }

        print("adding block now");

        gridCells[y, x].isFilled = true;
        gridCells[y, x].block = block;
        //gridCells[y, x].block.InitialPosition = gridCells[y, x].block.gameObject.transform.position;
        this.glueBlockMatrix[y, x] = block;
        block.locked = true;
        this.AddGlues(block);
        blocks.Add(block);
		block.Score = block.gridY - 1;

        CreateNewBlock();

        return true;
    }

    private void CreateNewBlock()
    {
        int towerBlockIndex = dict[GameManager.LastHitType];
        var newCube = Instantiate(towerBlocks[towerBlockIndex], new Vector3(-5.37f, -0.64f, -2.89f), Quaternion.identity);
        newCube.type = towerBlockIndex;
        newCube.gameObject.SetActive(true);
    }

    private bool CheckBlock(Block oldBlock, Block newBlock)
    {
        if(!oldBlock)
        {
            return false;
        }

        if(oldBlock.type != newBlock.type)
        {
            return false;
        }

        return true;
    }

    private bool CheckBlocks(List<Block> oldBlocks, Block newBlock )
    {
        //check if right color and existance
        foreach(var b in oldBlocks)
        {
            //check existance and type
            if(!CheckBlock(b, newBlock))
            {
                return false;
            }

            //check if block is part of a grouping that doesn't have all members in the checked list
            if (b.blockGroup != null
                && b.blockGroup.memberBlocks.Any(mb => oldBlocks.All(ob => ob != mb)))
            {
                return false;
            }
        }

        return true;
    }

    private List<Block> CreateBlockList(int x, int y, int newX, int newY, bool removeCenter = true)
    {
        var blockList = new List<Block> {
                this.glueBlockMatrix[y-1, x-1],
                this.glueBlockMatrix[y-1, x],
                this.glueBlockMatrix[y-1, x+1],
                this.glueBlockMatrix[y, x-1],
                this.glueBlockMatrix[y, x],
                this.glueBlockMatrix[y, x+1],
                this.glueBlockMatrix[y+1, x-1],
                this.glueBlockMatrix[y+1, x],
                this.glueBlockMatrix[y+1, x+1]
            };

        if (removeCenter)
        {
            blockList.Remove(blockList.Single(b => b != null && b.gridX == newX && b.gridY == newY));
        }

        return blockList;
    }

    private void ProcessGroupEntity(List<Block> oldBlocks, Block newBlock)
    {
        oldBlocks.Add(newBlock);

        var newGroup = new Block.GroupEntity
        {
            memberBlocks = oldBlocks
        };

        foreach (var b in oldBlocks)
        {
            b.blockGroup = newGroup;
        }
    }

    public static void ClearJointsFromBlock(Block block)
    {
        var joints = block.GetComponents<FixedJoint>();
        foreach(var j in joints)
        {
            Destroy(j);
        }
    }

    private void Glue3x3(List<Block> blocks)
    {
        if(blocks.Count != 9)
        {
            throw new System.Exception("Missing block from list");
        }

        //nw
        ClearJointsFromBlock(blocks[0]);

        var newJoint = blocks[0].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[1].gameObject.GetComponent<Rigidbody>();

        newJoint = blocks[0].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[3].gameObject.GetComponent<Rigidbody>();

        //ne
        ClearJointsFromBlock(blocks[2]);

        newJoint = blocks[2].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[1].gameObject.GetComponent<Rigidbody>();

        newJoint = blocks[2].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[5].gameObject.GetComponent<Rigidbody>();

        //se
        ClearJointsFromBlock(blocks[8]);

        newJoint = blocks[8].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[5].gameObject.GetComponent<Rigidbody>();

        newJoint = blocks[8].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[7].gameObject.GetComponent<Rigidbody>();

        //sw
        ClearJointsFromBlock(blocks[6]);

        newJoint = blocks[6].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[3].gameObject.GetComponent<Rigidbody>();

        newJoint = blocks[6].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[7].gameObject.GetComponent<Rigidbody>();

        //center
        ClearJointsFromBlock(blocks[4]);

        newJoint = blocks[4].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[1].gameObject.GetComponent<Rigidbody>();

        newJoint = blocks[4].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[5].gameObject.GetComponent<Rigidbody>();

        newJoint = blocks[4].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[7].gameObject.GetComponent<Rigidbody>();

        newJoint = blocks[4].gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = blocks[3].gameObject.GetComponent<Rigidbody>();
    }

    public void AddGlues(Block newBlock)
    {
        var x = newBlock.gridX;
        var y = newBlock.gridY;

        //c1
        var auxBlockList = CreateBlockList(x + 1, y - 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c1");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x + 1, y - 1, x, y, false));

            return;
        }

        //c2
        auxBlockList = CreateBlockList(x + 1, y, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c2");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x + 1, y, x, y, false));

            return;
        }

        //c3
        auxBlockList = CreateBlockList(x + 1, y + 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c3");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x + 1, y + 1, x, y, false));

            return;
        }

        //c4
        auxBlockList = CreateBlockList(x, y + 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c4");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x, y + 1, x, y, false));

            return;
        }

        //c5
        auxBlockList = CreateBlockList(x - 1, y + 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c5");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x - 1, y + 1, x, y, false));

            return;
        }

        //c6
        auxBlockList = CreateBlockList(x - 1, y, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c6");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x - 1, y, x, y, false));

            return;
        }

        //c7
        auxBlockList = CreateBlockList(x - 1, y - 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c7");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x - 1, y - 1, x, y, false));

            return;
        }

        //c8
        auxBlockList = CreateBlockList(x, y - 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c8");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x, y - 1, x, y, false));

            return;
        }

        //c9 - config not obtainable yet
        auxBlockList = CreateBlockList(x, y, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c9");
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x, y, x, y, false));

            return;
        }

        //c10
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y-2, x],
                this.glueBlockMatrix[y-1, x]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c10");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y - 1, x].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c11
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y, x+1],
                this.glueBlockMatrix[y, x+2]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c11");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x + 1].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c12
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y+1, x],
                this.glueBlockMatrix[y+2, x]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c12");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y + 1, x].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c13
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y, x-2],
                this.glueBlockMatrix[y, x-1]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c13");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x - 1].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c14
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y-1, x],
                this.glueBlockMatrix[y+1, x]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c14");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y - 1, x].gameObject.GetComponent<Rigidbody>();

            newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y + 1, x].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c15
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y, x-1],
                this.glueBlockMatrix[y, x+1]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c15");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x - 1].gameObject.GetComponent<Rigidbody>();

            newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x + 1].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c16
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y-1, x],
                this.glueBlockMatrix[y-1, x+1],
                this.glueBlockMatrix[y, x+1]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c16");
            ProcessGroupEntity(auxBlockList, newBlock);
            
            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y - 1, x].gameObject.GetComponent<Rigidbody>();

            newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x + 1].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c17
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y, x+1],
                this.glueBlockMatrix[y+1, x],
                this.glueBlockMatrix[y+1, x+1]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c17");
            ProcessGroupEntity(auxBlockList, newBlock);
            
            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x + 1].gameObject.GetComponent<Rigidbody>();

            newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y + 1, x].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c18
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y, x-1],
                this.glueBlockMatrix[y+1, x-1],
                this.glueBlockMatrix[y+1, x]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c18");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y + 1, x].gameObject.GetComponent<Rigidbody>();

            newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x - 1].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c19
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y-1, x-1],
                this.glueBlockMatrix[y-1, x],
                this.glueBlockMatrix[y, x-1]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c19");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x - 1].gameObject.GetComponent<Rigidbody>();

            newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y - 1, x].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c20
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y-1, x]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c20");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y - 1, x].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c21
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y, x+1]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c21");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x + 1].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c22
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y+1, x]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c22");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y + 1, x].gameObject.GetComponent<Rigidbody>();

            return;
        }

        //c23
        auxBlockList = new List<Block> {
                this.glueBlockMatrix[y, x-1]
            };
        if (CheckBlocks(auxBlockList, newBlock))
        {
            Debug.Log("c23");
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x - 1].gameObject.GetComponent<Rigidbody>();

            return;
        }

        Debug.Log("no glue");
    }

	public int GetBuilderScore() {
		int totalScore = 0;
		Debug.Log ("Reset total score");
		foreach (Block b in blocks) {
			if (!b.fellOffBase && b.locked == true) {
				totalScore += b.Score;
				Debug.Log (b.gameObject.name + " totalScore: " + totalScore + " b score: " + b.Score);
			}
		}
		return totalScore;
	}
}
