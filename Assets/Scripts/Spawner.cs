using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool hasBlock = false;
    public Block towerBlock;

    public Block SpawnBlock()
    {
        if (!hasBlock)
        {
            var newCube = Instantiate(towerBlock, new Vector3(5, 5), Quaternion.identity);
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
