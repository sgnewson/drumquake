using UnityEngine;
using System;

public class Block : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private bool isColliding = false;
    private ContactPoint[] contacts;
    private enum Faces { TOP, BOTTOM, LEFT, RIGHT };
    private bool[] canMove = { true, true, true, true };
    public bool locked = false;
    public bool collapsed = false;
    int row;
    int col;

    public int gridX { get; set; }
    public int gridY { get; set; }

    public Tower tower;

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
        bool cornerCollision = false;

        if (!locked)
        {
            // Checks for corner collisions.
            for (int iii = 0; iii < contacts.Length - 1; iii++)
            {
                for (int jjj = iii + 1; jjj < contacts.Length; jjj++)
                {
                    if (contacts[iii].point == contacts[jjj].point)
                    {
                        cornerCollision = true;
                        break;
                    }
                }

                if (cornerCollision)
                {
                    break;
                }
            }
        }

        if (!cornerCollision)
        {
            locked = true;
        }

        this.gridX = (int)Math.Round( this.transform.position.x );
        this.gridY = (int)Math.Round( this.transform.position.y );
        this.tower.AddBlock(this);
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;

        for (int iii = 0; iii < canMove.Length; iii++)
        {
            canMove[iii] = true;
        }
    }

    private void Start()
    {
        locked = false;
    }
}
