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
        foreach (var i in tower.blocks)
        {
            var rb = i.GetComponent<Rigidbody>();
            rb.AddForce(100, 0, 0);
        }
    }
}
