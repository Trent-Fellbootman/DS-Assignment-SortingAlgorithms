using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float baseVelocity;
    public float mouseSensitivity;
    public float sprintScale;

    private float velocity;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            velocity = sprintScale * baseVelocity;
        } else {
            velocity = baseVelocity;
        }

        updateTransform();
    }

    private void updateTransform() {
        transform.Translate(transform.forward * velocity * Input.GetAxis("Vertical") * Time.deltaTime, Space.World);
        transform.Translate(transform.right * velocity * Input.GetAxis("Horizontal") * Time.deltaTime, Space.World);

        transform.Rotate(new Vector3(0, 1, 0), mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(1, 0, 0), -mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime, Space.Self);
    }
}
