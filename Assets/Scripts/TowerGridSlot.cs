using UnityEngine;

public class TowerGridSlot : MonoBehaviour {

    public bool isSelected = false;

    public TowerBlock towerBlock;
    public Tower tower;

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
        /*
        Material material = new Material(Shader.Find("Transparent/Diffuse"));
        material.color = new Color(0, 1, 0, 0.5f);
        GetComponent<Renderer>().material = material;
        */
        var newCube = Instantiate(towerBlock, this.transform.position, Quaternion.identity);
        newCube.gameObject.SetActive(true);
        
        tower.blocks.Add(newCube);
    }
}
