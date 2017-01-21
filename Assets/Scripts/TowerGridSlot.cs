using UnityEngine;

public class TowerGridSlot : MonoBehaviour {

    public bool isSelected = false;
    
    public Tower tower;

    public int gridX { get; set; }
    public int gridY { get; set; }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseEnter()
    {
        isSelected = true;
    }

    private void OnMouseExit()
    {
        isSelected = false;
    }

    private void OnMouseUp()
    {
        //var newCube = Instantiate(towerBlock, this.transform.position, Quaternion.identity);
        //newCube.gameObject.SetActive(true);
        
        //tower.blocks.Add(newCube);
    }
}
