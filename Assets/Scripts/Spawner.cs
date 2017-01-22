using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool hasBlock = false;
    public Block towerBlock;
    public int currentSelectedBlockType { get; set; }
	public Tower Tower;

    private void Start()
    {
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
            var newCube = Instantiate(towerBlock, transform.position, Quaternion.identity);
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

            newCube.gameObject.SetActive(true);

            newCube.gameObject.SetActive(true);
            towerBlock = newCube;
            hasBlock = true;
            return towerBlock;
        }
        else
        {
            return null;
        }
    }
}
