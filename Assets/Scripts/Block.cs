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

    void OnMouseDown()
    {
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    private void OnMouseUp()
    {
        gameObject.GetComponent<Rigidbody>().detectCollisions = true;
        gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;    
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
                        for (int iii = 0; iii < canMove.Length; iii++)
                        {
                            canMove[iii] = true;
                        }
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

                if (IsCornerCollision())
                {
                    for (int kkk = 0; kkk < canMove.Length; kkk++)
                    {
                        canMove[kkk] = true;
                    }
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

            if (!locked && !IsCornerCollision())
            {
                locked = true;
            }
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

    private void Start()
    {
        locked = false;
    }
}
