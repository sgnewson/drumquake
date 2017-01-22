using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Spawner : MonoBehaviour
{
    public bool hasBlock = false;
    public Block[] towerBlock;
    public int currentSelectedBlockType { get; set; }
	public Tower Tower;
	private Dictionary<XboxButton, int> dict;

    private void Start()
    {
		dict = new Dictionary<XboxButton, int>();
		dict.Add (XboxButton.A, 0);
		dict.Add (XboxButton.X, 1);
		dict.Add (XboxButton.Y, 2);
		dict.Add (XboxButton.B, 3);

        currentSelectedBlockType = 0;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            currentSelectedBlockType = 0;
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            currentSelectedBlockType = 1;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            currentSelectedBlockType = 2;
        }

        Material material = new Material(Shader.Find("Standard"));
        material.color = currentSelectedBlockType == 0
            ? Color.red
            : currentSelectedBlockType == 1
                ? Color.green
                : Color.blue;
        //towerBlock.GetComponent<Renderer>().material = material;
    }

    public Block SpawnBlock()
    {
        if (!hasBlock)
        {
			int towerBlockIndex = dict[GameManager.LastHitType];
			var newCube = Instantiate(towerBlock[towerBlockIndex], transform.position, Quaternion.identity);
			newCube.gameObject.name = "Block " + Tower.blocks.Count;

            Material material = new Material(Shader.Find("Standard"));

            switch (currentSelectedBlockType)
            {
                case 0:
                    {
                        material.color = Color.red;
                        break;
                    }
                case 1:
                    {
                        material.color = Color.green;
                        break;
                    }
                case 2:
                    {
                        material.color = Color.blue;
                        break;
                    }
            }

            //newCube.GetComponent<Renderer>().material = material;

            newCube.gridX = (int)transform.position.x;
            newCube.gridY = (int)transform.position.y;
            newCube.type = currentSelectedBlockType;

            Tower.ClearJointsFromBlock(newCube);
            newCube.gameObject.SetActive(true);
            
            towerBlock[towerBlockIndex] = newCube;
            hasBlock = true;
			return towerBlock[towerBlockIndex];
        }
        else
        {
            return null;
        }
    }
}
