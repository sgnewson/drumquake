
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private int _totalHeight;
    public Spawner blockSpawner;

    public TowerGridSlot[,] gridCells;
    public Block[,] glueBlockMatrix;
    //public Block[,] blockMatrix;
    public List<Block> blocks;
    private Block lastBlock;

    public int gridHeight;
    public int gridWidth;

    public TowerGridSlot towerGridSlot;

    private void BuildGridCells()
    {
        gridCells = new TowerGridSlot[gridHeight, gridWidth];

        var buildDelta = new Vector3(0, 0, 0);
        for (var y = 2; y < gridHeight - 2; y++)
        {
            buildDelta.x = 0;
            for (var x = 2; x < gridWidth - 2; x++)
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

 	// Use this for initialization
	void Start ()
    {
        gridHeight = 15 + 4;
        gridWidth = 8 + 4;

        //blockMatrix = new Block[gridHeight, gridWidth];
        glueBlockMatrix = new Block[gridHeight, gridWidth];

        BuildGridCells();
        if (!blockSpawner) { return; }
        blockSpawner = Instantiate(blockSpawner, new Vector3(5, 5), Quaternion.identity);
        lastBlock = blockSpawner.SpawnBlock();
        glueBlockMatrix[lastBlock.gridY, lastBlock.gridX] = lastBlock;
        AddGlues(lastBlock);
        blocks.Add(lastBlock);
    }

    // Update is called once per frame
    void Update ()
    {
        if (blocks.Count != 0)
        {
            if (blocks[blocks.Count - 1].locked)
            {
                if (AddBlock(lastBlock))
                {
                    blockSpawner.hasBlock = false;
                    lastBlock = blockSpawner.SpawnBlock();
                    glueBlockMatrix[lastBlock.gridY, lastBlock.gridX] = lastBlock;
                    AddGlues(lastBlock);
                    blocks.Add(lastBlock);
                }
                else
                {
                    Block tempBlock = lastBlock;
                    blockSpawner.hasBlock = false;
                    lastBlock = blockSpawner.SpawnBlock();
                    glueBlockMatrix[lastBlock.gridY, lastBlock.gridX] = lastBlock;
                    AddGlues(lastBlock);
                    blocks.Add(lastBlock);
                    blocks.Remove(tempBlock);
                    DestroyImmediate(tempBlock.gameObject);
                }
            }
        }

        foreach (Block block in blocks)
        {
            if (block.transform.position.y + 2 < -1)
            {
                glueBlockMatrix[block.gridY, block.gridX] = null;
                Destroy(this);
            }
        }
    }


    public bool AddBlock(Block block)
    {
        //var x = block.gridX;
        //var y = block.gridY;

        //if (x < 0 || x >= gridWidth)
        //{
        //    return false;
        //}

        //if (y < 0 || y >= gridHeight)
        //{
        //    return false;
        //}

        //if (gridCells[x, y].isFilled)
        //{
        //    return false;
        //}

        //this.blockMatrix[y, x] = block;
        //gridCells[x, y].isFilled = true;
        //return true;

        var x = block.gridX;
        var y = block.gridY;

        if (x < 2 || x >= gridWidth - 2)
        {
            return false;
        }

        if (y < 2 || y >= gridHeight - 2)
        {
            return false;
        }

        if (gridCells[x, y] != null)
        {
            if (gridCells[x, y].isFilled)
            {
                return false;
            }
        }

        gridCells[x, y].isFilled = true;
        this.glueBlockMatrix[y, x] = block;
        this.AddGlues(block);

        return true;
    }

    //public bool AddTowerBlock(TowerBlock newBlock)
    //{
    //    var x = newBlock.gridX;
    //    var y = newBlock.gridY;

    //    if (x < 2 || x >= gridWidth - 2)
    //    {
    //        return false;
    //    }

    //    if (y < 2 || y >= gridHeight - 2)
    //    {
    //        return false;
    //    }

    //    if (gridCells[x, y].isFilled)
    //    {
    //        return false;
    //    }

    //    gridCells[x, y].isFilled = true;
    //    this.glueBlockMatrix[y, x] = newBlock;
    //    this.AddGlues(newBlock);

    //    return true;
    //}

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

        if(removeCenter)
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

    private void ClearJointsFromBlock(Block block)
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
        /*
        var b11 = this.glueBlockMatrix[y - 2, x - 2];
        var b12 = this.glueBlockMatrix[y - 2, x - 1];
        var b13 = this.glueBlockMatrix[y - 2, x];
        var b14 = this.glueBlockMatrix[y - 2, x + 1];
        var b15 = this.glueBlockMatrix[y - 2, x + 2];
        var b21 = this.glueBlockMatrix[y - 1, x - 2];
        var b22 = this.glueBlockMatrix[y - 1, x - 1];
        var b23 = this.glueBlockMatrix[y - 1, x];
        var b24 = this.glueBlockMatrix[y - 1, x + 1];
        var b25 = this.glueBlockMatrix[y - 1, x + 2];
        var b31 = this.glueBlockMatrix[y, x - 2];
        var b32 = this.glueBlockMatrix[y, x - 1];
        var b33 = this.glueBlockMatrix[y, x];
        var b34 = this.glueBlockMatrix[y, x + 1];
        var b35 = this.glueBlockMatrix[y, x + 2];
        var b41 = this.glueBlockMatrix[y + 1, x - 2];
        var b42 = this.glueBlockMatrix[y + 1, x - 1];
        var b43 = this.glueBlockMatrix[y + 1, x];
        var b44 = this.glueBlockMatrix[y + 1, x + 1];
        var b45 = this.glueBlockMatrix[y + 1, x + 2];
        var b51 = this.glueBlockMatrix[y + 2, x - 2];
        var b52 = this.glueBlockMatrix[y + 2, x - 1];
        var b53 = this.glueBlockMatrix[y + 2, x];
        var b54 = this.glueBlockMatrix[y + 2, x + 1];
        var b55 = this.glueBlockMatrix[y + 2, x + 2];
        */
        //c1
        var auxBlockList = CreateBlockList(x + 1, y - 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x + 1, y - 1, x, y, false));

            return;
        }

        //c2
        auxBlockList = CreateBlockList(x + 1, y, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x + 1, y, x, y, false));

            return;
        }

        //c3
        auxBlockList = CreateBlockList(x + 1, y + 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x + 1, y + 1, x, y, false));

            return;
        }

        //c4
        auxBlockList = CreateBlockList(x, y + 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x, y + 1, x, y, false));

            return;
        }

        //c5
        auxBlockList = CreateBlockList(x - 1, y + 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x - 1, y + 1, x, y, false));

            return;
        }

        //c6
        auxBlockList = CreateBlockList(x - 1, y, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x - 1, y, x, y, false));

            return;
        }

        //c7
        auxBlockList = CreateBlockList(x - 1, y - 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x - 1, y - 1, x, y, false));

            return;
        }

        //c8
        auxBlockList = CreateBlockList(x, y - 1, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
            ProcessGroupEntity(auxBlockList, newBlock);

            Glue3x3(CreateBlockList(x, y - 1, x, y, false));

            return;
        }

        //c9 - config not obtainable yet
        auxBlockList = CreateBlockList(x, y, x, y);
        if (CheckBlocks(auxBlockList, newBlock))
        {
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
            ProcessGroupEntity(auxBlockList, newBlock);

            var newJoint = newBlock.gameObject.AddComponent<FixedJoint>();
            newJoint.connectedBody = this.glueBlockMatrix[y, x - 1].gameObject.GetComponent<Rigidbody>();

            return;
        }
    }
}
