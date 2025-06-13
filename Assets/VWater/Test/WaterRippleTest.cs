using System.Collections;
using UnityEngine;
using Vertigo2;

public class WaterTestCam : MonoBehaviour
{
    public float moveSpeed = 5;
    public float turnSpeed = 1;
    public Camera cam;

    public float freq = 4;

    private RaycastHit hit;


    private bool holdingRigidbody;

    private void Update()
    {
        if (Input.GetMouseButton(0) && !holdingRigidbody) Interact();

        if (Input.GetMouseButton(1))
        {
            cam.transform.position += cam.transform.rotation *
                                      new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) *
                                      moveSpeed * Time.deltaTime;
            var euler = cam.transform.eulerAngles;
            euler.x += Input.GetAxis("Mouse Y") * -turnSpeed;
            euler.y += Input.GetAxis("Mouse X") * turnSpeed;
            if (euler.x > 180) euler.x -= 360;
            euler.x = Mathf.Clamp(euler.x, -90, 90);
            cam.transform.eulerAngles = euler;
        }
    }

    private void Interact()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                StartCoroutine(PickUpRigidbody(hit.point, hit.rigidbody));
            else
                VWaterDynamicsManager.instance.AddFlow(hit.point, 0.1f, Mathf.Sin(Time.time * 3 * freq), 10);
        }
    }

    private IEnumerator PickUpRigidbody(Vector3 pos, Rigidbody rigidbody)
    {
        var depth = cam.WorldToScreenPoint(pos).z;
        var grabPoint = rigidbody.transform.InverseTransformPoint(pos);
        holdingRigidbody = true;
        while (Input.GetMouseButton(0) && rigidbody != null)
        {
            var worldGrabPt = rigidbody.transform.TransformPoint(grabPoint);
            var target = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
            var offset = target - worldGrabPt;
            rigidbody.AddForceAtPosition(offset * 50, worldGrabPt, ForceMode.Acceleration);
            rigidbody.AddForceAtPosition(-rigidbody.GetPointVelocity(worldGrabPt) * 10, worldGrabPt,
                ForceMode.Acceleration);

            yield return new WaitForFixedUpdate();
        }

        holdingRigidbody = false;
    }
}