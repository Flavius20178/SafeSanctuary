using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
    public float rot_speed_x;
    public float rot_speed_y;
    public float rot_speed_z;
    public bool local;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (local)
            transform.RotateAroundLocal(transform.up, Time.fixedDeltaTime * rot_speed_x);
        else
            transform.Rotate(Time.fixedDeltaTime * new Vector3(rot_speed_x, rot_speed_y, rot_speed_z), Space.World);
    }
}