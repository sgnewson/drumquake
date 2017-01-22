using UnityEngine;

public class TowerGridSlot : MonoBehaviour {

    public bool isSelected = false;
    public bool isFilled = false;
    
    public Tower tower;
    public Block block;

    public int gridX { get; set; }
    public int gridY { get; set; }

    private bool mouseDownHere = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseEnter()
    {
        isSelected = true;
        //this.GetComponent<Renderer>().material.color = Color.green;

        if(tower.currentBlock)
        {
            tower.currentBlock.isOverGridSlot = true;
            tower.currentBlock.gridX = this.gridX;
            tower.currentBlock.gridY = this.gridY;
            tower.currentBlock.transform.position = transform.position;
        }
    }

    private void OnMouseExit()
    {
        isSelected = false;

        if (tower.currentBlock)
        {
            tower.currentBlock.isOverGridSlot = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //if (collision.collider.bounds.min == gameObject.transform.position || collision.collider.bounds.max == gameObject.transform.position)
        //{
        //    print("In grid.");
        //}
    }

    private void OnMouseDown()
    {
        //mouseDownHere = true;
    }

    private void OnMouseUp()
    {
        //if(!mouseDownHere)
        //{
        //    return;
        //}

        //var newCube = Instantiate(block, this.transform.position, Quaternion.identity);
        //Material material = new Material(Shader.Find("Standard"));
        //newCube.GetComponent<Renderer>().material = material;

        //newCube.gridX = this.gridX;
        //newCube.gridY = this.gridY;

        //newCube.gameObject.SetActive(true);

        //this.tower.glueBlockMatrix[this.gridY, this.gridX] = newCube;

        //tower.AddGlues(newCube);
    }
}
