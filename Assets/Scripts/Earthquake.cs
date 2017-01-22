using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : MonoBehaviour
{
    public Tower tower;
    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shake_decay;
    public float shake_intensity;

    public float speed = 0.1f;

    void Update()
    {
        UpdateCameraPosition();
        UpdateCameraShake();
    }

    void UpdateCameraPosition()
    {
        var y = Input.GetAxis("Vertical");
        var x = Input.GetAxis("Horizontal");
        var delta = new Vector3(x, y, 0);
        this.transform.Translate(delta * speed);
    }

    void UpdateCameraShake()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Block block in tower.blockMatrix)
            {
                if (block != null)
                {
                    block.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    block.GetComponent<Rigidbody>().useGravity = true;
                }
            }

            Shake(0.3f);
        }
        if (shake_intensity > 0)
        {
            gameObject.transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            gameObject.transform.rotation = new Quaternion(originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f, originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f, originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f, originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f);
            shake_intensity -= shake_decay;
        }
    }

    public void Shake(float shakeIntensity)
    {
        Debug.Log("Quake Magnitude: " + shakeIntensity);
        originPosition = transform.position;

        foreach (Block block in tower.blockMatrix)
        {
            if (block != null)
            {
                block.transform.position = transform.position;
            }
        }

        originRotation = transform.rotation;
        shake_intensity = shakeIntensity;
        shake_decay = 0.002f;
    }
}
