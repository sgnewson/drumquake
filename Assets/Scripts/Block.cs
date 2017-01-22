using UnityEngine;
using System;
using System.Collections.Generic;

public class Block : MonoBehaviour
{
    public class GroupEntity
    {
        public List<Block> memberBlocks;
    }

	public Vector3 InitialPosition;
    private Vector3 screenPoint;
	public int Score = 0;
    private Vector3 offset;
    private bool isColliding = false;
    private ContactPoint[] contacts;
    private enum Faces { TOP, BOTTOM, LEFT, RIGHT };
    private bool[] canMove = { true, true, true, true };
    public bool locked = false;
    public bool collapsed = false;
    private float zVal;
    int row;
    int col;

	public bool fellOffBase;

    public int gridX { get; set; }
    public int gridY { get; set; }
    public int type { get; set; }

    public GroupEntity blockGroup { get; set; }

	SoundEffectManager soundEffectManager;

    private void Start()
    {
		soundEffectManager = GameObject.Find ("SoundEffectManager").GetComponent<SoundEffectManager> ();
		fellOffBase = false;
        locked = false;
        isColliding = false;
        ResetCanMove();
        collapsed = false;
        zVal = gameObject.transform.position.z;
    }

    void Update()
    {
		// enforce an integer position when you're building
		if (!GameManager.PlayOn) {
			return;
		}
		gameObject.transform.position = new Vector3 (Mathf.RoundToInt (gameObject.transform.position.x), Mathf.RoundToInt (gameObject.transform.position.y), Mathf.RoundToInt (gameObject.transform.position.z));
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
		if (!GameManager.PlayOn) {
			return;
		}

        if (!locked)
        {
            if (!isColliding)
            {
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
//				Debug.Log ("curPosition y: " + curPosition.y);

				if (curPosition.y < 0) {
					return;
				}

                // Reassigns curPosition to rounded values for grid-snapping.
                curPosition = new Vector3(Mathf.Round(curPosition.x), Mathf.Round(curPosition.y), Mathf.Round(zVal));
                transform.position = curPosition;
				//Debug.Log ("Drag pos x: " + curPosition.x + " y: " + curPosition.y);
            }
            else
            {
                foreach (ContactPoint contact in contacts)
                {
                    if (!IsCornerCollision())
                    {
                        if (contact.normal.x == 1)
                        {
                            canMove[(int)Faces.LEFT] = false;
                        }
                        else if (contact.normal.x == -1)
                        {
                            canMove[(int)Faces.RIGHT] = false;
                        }
                        else if (contact.normal.y == 1)
                        {
                            canMove[(int)Faces.BOTTOM] = false;
                        }
                        else if (contact.normal.y == -1)
                        {
                            canMove[(int)Faces.TOP] = false;
                        }
                    }
                    else
                    {
                        ResetCanMove();
                    }
                }

                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

                // Reassigns curPosition to rounded values for grid-snapping.
                curPosition = new Vector3(Mathf.Round(curPosition.x), Mathf.Round(curPosition.y), Mathf.Round(zVal));

                if (!canMove[(int)Faces.LEFT] && curPosition.x < transform.position.x)
                {
                    curPosition.x = transform.position.x;
                }

                if (!canMove[(int)Faces.RIGHT] && curPosition.x > transform.position.x)
                {
                    curPosition.x = transform.position.x;
                }

                if (!canMove[(int)Faces.TOP] && curPosition.y > transform.position.y + 2)
                {
                    curPosition.y = transform.position.y + 2;
                }

                if (!canMove[(int)Faces.BOTTOM] && curPosition.y < transform.position.y + 2)
                {
                    curPosition.y = transform.position.y + 2;
                }

                transform.position = curPosition;

                if (IsCornerCollision())
                {
                    ResetCanMove();
                }
            }
        }
    }

    private bool IsCornerCollision()
    {
        for (int iii = 0; iii < contacts.Length - 1; iii++)
        {
            for (int jjj = iii + 1; jjj < contacts.Length; jjj++)
            {
                if (contacts[iii].point == contacts[jjj].point)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!locked)
		{
            contacts = collision.contacts;
            isColliding = true;

			bool cornerTest = IsCornerCollision ();

			Debug.Log ("Corner test: " + cornerTest);

			if (!locked && !cornerTest)
            {
				soundEffectManager.PlayPlaceBlockSound ();
                locked = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                gridX = (int)Mathf.Round(transform.position.x + 2);
                gridY = (int)Mathf.Round(transform.position.y + 2);
            }
        }
    }

	private void OnCollisionEnter(Collision collision) {
		if (collision.collider.name == "Terrain") {
			Debug.Log ("Kill block: " + this.gameObject.name);
			soundEffectManager.PlayTowerFallSound ();
			fellOffBase = true;
		}
	}

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
        ResetCanMove();
    }

    private void ResetCanMove()
    {
        for (int iii = 0; iii < canMove.Length; iii++)
        {
            canMove[iii] = true;
        }
    }
}
