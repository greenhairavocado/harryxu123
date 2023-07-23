using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guidance_arrow : MonoBehaviour
{
    public Transform target;
    public Transform pointer;
    RectTransform arrow;
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        arrow = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        pointer.LookAt(target);
        // Vector3 direction = target.position - Camera.main.transform.position;
        // direction.Normalize();

        Vector3 targetViewPosition = mainCamera.WorldToViewportPoint(target.position);
        Vector3 rocketViewPosition = mainCamera.WorldToViewportPoint(transform.position);

        Vector3 viewportDirection = targetViewPosition - rocketViewPosition;

        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angle = Vector3.SignedAngle(Vector3.up, viewportDirection, Vector3.forward);
        Debug.Log(angle);
        arrow.localEulerAngles = new Vector3(0, 0, -angle);
    }
}
