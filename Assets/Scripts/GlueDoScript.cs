using UnityEngine;

public class GlueDoScript : MonoBehaviour {

    public Tower tower;

    private void OnMouseUp()
    {
        for(var y = 0; y < tower.gridHeight; y++)
        {
            for (var x = 0; x < tower.gridWidth; x++)
            {
                if(!tower.glueBlockMatrix[y, x])
                {
                    continue;
                }

                var rb = tower.glueBlockMatrix[y, x].GetComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.AddForce(200, 200, 0);
            }
        }
    }
}
