using UnityEngine;
using System;
using System.Collections.Generic;

public class CreditBlock : MonoBehaviour
{
    public class GroupEntity
    {
        public List<Block> memberBlocks;
    }

    private Vector3 screenPoint;
    private Vector3 offset;

    public GroupEntity blockGroup { get; set; }

	SoundEffectManager soundEffectManager;

    private void Start()
    {
		soundEffectManager = GameObject.Find ("SoundEffectManager").GetComponent<SoundEffectManager> ();
    }

    void Update()
    {
		//gameObject.transform.position = new Vector3 (Mathf.RoundToInt (gameObject.transform.position.x), Mathf.RoundToInt (gameObject.transform.position.y), Mathf.RoundToInt (gameObject.transform.position.z));
    }

    void OnMouseDown()
    {
		if (!GameManager.PlayOn) {
			return;
		}
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		soundEffectManager.PlayPickupBlockSound ();
    }

    private void OnMouseUp()
    {
        gameObject.GetComponent<Rigidbody>().detectCollisions = true;
    }

    void OnMouseDrag()
    {
        if (!GameManager.PlayOn)
        {
            return;
        }

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        //				Debug.Log ("curPosition y: " + curPosition.y);

        if (curPosition.y < 0)
        {
            return;
        }

        transform.position = curPosition;
        //Debug.Log ("Drag pos x: " + curPosition.x + " y: " + curPosition.y);
    }
}
