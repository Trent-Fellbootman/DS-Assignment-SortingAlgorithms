using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float baseVelocity;
    public float mouseSensitivity;
    public float sprintScale;

    private float velocity;
    private bool controlled = false;

    // Update is called once per frame
    private void Update() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            velocity = sprintScale * baseVelocity;
        } else {
            velocity = baseVelocity;
        }

        if (!controlled) {
            updateTransform();
        }
    }

    private void updateTransform() {
        transform.Translate(transform.forward * velocity * Time.deltaTime * Input.GetAxis("Vertical"), Space.World);
        transform.Translate(transform.right * velocity * Time.deltaTime * Input.GetAxis("Vertical"), Space.World);

        transform.Rotate(new Vector3(0, 1, 0), mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(1, 0, 0), -mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime, Space.Self);
    }
}
