using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private bool isColliding = false;
    private ContactPoint[] contacts;
    private enum Faces { TOP, BOTTOM, LEFT, RIGHT };
    private bool[] canMove = { true, true, true, true };
    private bool locked = false;

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (!locked)
        {
            if (!isColliding)
            {
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
                // Reassigns curPosition to rounded values for grid-snapping.
                curPosition = new Vector3(Mathf.Round(curPosition.x), Mathf.Round(curPosition.y), Mathf.Round(curPosition.z));
                transform.position = curPosition;
            }
            else
            {
                foreach (ContactPoint contact in contacts)
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

                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
                
                // Reassigns curPosition to rounded values for grid-snapping.
                curPosition = new Vector3(Mathf.Round(curPosition.x), Mathf.Round(curPosition.y), Mathf.Round(curPosition.z));

                if (!canMove[(int)Faces.LEFT] && curPosition.x < transform.position.x)
                {
                    curPosition.x = transform.position.x;
                }

                if (!canMove[(int)Faces.RIGHT] && curPosition.x > transform.position.x)
                {
                    curPosition.x = transform.position.x;
                }

                if (!canMove[(int)Faces.TOP] && curPosition.y > transform.position.y)
                {
                    curPosition.y = transform.position.y;
                }

                if (!canMove[(int)Faces.BOTTOM] && curPosition.y < transform.position.y)
                {
                    curPosition.y = transform.position.y;
                }

                transform.position = curPosition;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        contacts = collision.contacts;
        isColliding = true;

        if (collision.collider.gameObject.GetComponent<Block>() != null)
        {
            locked = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
        
        for (int iii = 0; iii < canMove.Length; iii++)
        {
            canMove[iii] = true;
        }
    }


    int row;
    int col;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
