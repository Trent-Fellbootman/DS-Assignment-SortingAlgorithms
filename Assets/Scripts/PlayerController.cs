using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    private float baseVelocity;
    [SerializeField]
    private float mouseSensitivity;
    [SerializeField]
    private float sprintScale;
    [SerializeField]
    private float approachLimit;

    private float velocity;
    [HideInInspector]
    public bool controlled = false;
    private float INFINITY = 1e6f;

    private GameObject controller = null;

    // Update is called once per frame
    private void LateUpdate() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            velocity = sprintScale * baseVelocity;
        } else {
            velocity = baseVelocity;
        }

        updateTransform();

        if (Input.GetButton("Aim")) {
            GameObject sorter = null;
            GameObject tmp = null;
            float minDistance = 1e6f;

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Sorter")) {
                if (!obj.GetComponent<Sorter>().sortingDone && Vector3.Distance(transform.position, obj.transform.position) <= minDistance) {
                    tmp = obj;
                    minDistance = Vector3.Distance(transform.position, obj.transform.position);
                }
            }

            if (tmp && Vector3.Distance(transform.position, tmp.transform.position) <= approachLimit) {
                sorter = tmp;
            }

            if (sorter) {
                // sorter.GetComponent<Sorter>().aimmedIndicator.SetActive(true);
                sorter.GetComponent<Sorter>().player = gameObject;
                controller = sorter;
                controlled = true;
            }
        }

        if (Input.GetButtonDown("Quit Sorter View")) {
            controlled = false;
            if (controller) {
                controller.GetComponent<Sorter>().player = null;
            }
            controller = null;
            transform.parent = null;
        }
    }

    private void updateTransform() {
        if (!controlled) {
            transform.Translate(transform.forward * velocity * Time.deltaTime * Input.GetAxis("Vertical"), Space.World);
            transform.Translate(transform.right * velocity * Time.deltaTime * Input.GetAxis("Horizontal"), Space.World);
        }

        transform.Rotate(new Vector3(0, 1, 0), mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(1, 0, 0), -mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime, Space.Self);
    }
}
