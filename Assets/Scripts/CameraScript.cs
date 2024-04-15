using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    int chunksLayer;
    Vector3 chunksCenter, vCenter;
    float distance;
    public float minDistance, maxDistance, sensitivity, scrollSensitivity, panSensitivity, rotateSensitivity;
    float horizontalAngle = Mathf.PI * 2 / 3;
    float verticalAngle = Mathf.PI / 6;
    bool firstUpdate;
    bool firstInput;
    Vector3 pan;
    bool revertPan;
    Vector3 vRevertPan;

    void Start() {
        chunksLayer = LayerMask.NameToLayer("Chunks");
    }

    void Update() {
        if (!firstUpdate) {
            chunksCenter = GetChunksCenter();
            firstUpdate = true;
        } else {
            chunksCenter = Vector3.SmoothDamp(chunksCenter, GetChunksCenter(), ref vCenter, .33f);
        }

        // Input.
        distance *= Mathf.Pow(scrollSensitivity, Input.mouseScrollDelta.y);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        if (Input.GetMouseButton(2)) {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            if (!firstInput) {
                firstInput = true;
            } else {
                horizontalAngle -= mx * sensitivity;
                verticalAngle -= my * sensitivity;
                verticalAngle = Mathf.Clamp(verticalAngle, Mathf.PI * .05f, Mathf.PI * .49f);
            }
        }
        int horizontalInput = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        int verticalInput = Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;
        float zoomedPanSensitivity = panSensitivity * Mathf.Sqrt(distance);
        pan += horizontalInput * transform.right * zoomedPanSensitivity * Time.deltaTime;
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        pan += verticalInput * flatForward * zoomedPanSensitivity * Time.deltaTime;
        pan = Vector3.ClampMagnitude(pan, 20);
        if (!revertPan && Input.GetKey(KeyCode.Space)) {
            revertPan = true;
            vRevertPan = Vector3.zero;
        } else if (horizontalInput != 0 || verticalInput != 0) {
            revertPan = false;
        }
        if (revertPan) {
            pan = Vector3.SmoothDamp(pan, Vector3.zero, ref vRevertPan, 0.1f);
            if (pan.sqrMagnitude < .1f) {
                revertPan = false;
            }
        }
        int rotInput = Input.GetKey(KeyCode.Q) ? -1 : Input.GetKey(KeyCode.E) ? 1 : 0;
        horizontalAngle += rotInput * rotateSensitivity * Time.deltaTime;

        // Set position.
        float xzDistance = distance * Mathf.Cos(verticalAngle);
        float x = Mathf.Cos(horizontalAngle) * xzDistance;
        float y = Mathf.Sin(verticalAngle) * distance;
        float z = Mathf.Sin(horizontalAngle) * xzDistance;
        transform.localPosition = chunksCenter + new Vector3(x, y, z);
        transform.LookAt(chunksCenter);
        transform.localPosition += pan;
    }

    Vector3 GetChunksCenter() {
        Bounds bounds = new Bounds();
        foreach (GameObject go in gameObject.scene.GetRootGameObjects()) {
            if (go.layer == chunksLayer) {
                Bounds chunkBounds = go.GetComponent<Collider>().bounds;
                if (bounds.size.x == 0) {
                    bounds = chunkBounds;
                } else {
                    bounds.Encapsulate(chunkBounds.min);
                    bounds.Encapsulate(chunkBounds.max);
                }
            }
        }
        return bounds.center;
    }
}
