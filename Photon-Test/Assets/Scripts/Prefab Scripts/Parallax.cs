using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    public bool parallax = true;
    Transform cam;
    Vector3 previousCamPos;
    Vector3 previousLocalCamPos;
    // Start is called before the first frame update
    void Awake()
    {
        
        if (this.gameObject.GetComponent<SpriteRenderer>())
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = Mathf.FloorToInt(-this.gameObject.transform.position.z * 100);
        else
            foreach (SpriteRenderer sprite in this.gameObject.GetComponentsInChildren<SpriteRenderer>()){

            sprite.sortingOrder = Mathf.FloorToInt(-this.gameObject.transform.position.z * 100);
            }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      
        if (cam == null)
            cam = Camera.main.transform;
            
        if (parallax)
        {
            float parallaxX = (cam.position.x - previousCamPos.x) * Mathf.Clamp(this.gameObject.transform.position.z, -1000, 12) / 12;
            Vector3 backgroundTargetPosX = new Vector3(transform.position.x + parallaxX,
               transform.position.y,
               transform.position.z);

            this.gameObject.transform.position = Vector3.Lerp(transform.position, backgroundTargetPosX, 1.0f);

            previousCamPos = cam.position;
            previousLocalCamPos = cam.localPosition;
        }
    }
}
