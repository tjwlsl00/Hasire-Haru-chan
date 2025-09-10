using UnityEngine;

public class FaceCamera : MonoBehaviour
{

    public Transform target;
    public Camera faceCamera;
    public Vector3 localOffset = new Vector3(0f, 0.12f, 0.25f);
    public Vector3 lookOffset = new Vector3(0f, 0.08f, 0f);

    void LateUpdate()
    {
        if (target == null || faceCamera == null) return;
        faceCamera.transform.position = target.TransformPoint(localOffset);
        faceCamera.transform.LookAt(target.position + lookOffset);
    }
}
