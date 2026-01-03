using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(1)) {
            transform.Rotate(-Input.GetAxis("Mouse Y") * 2, Input.GetAxis("Mouse X") * 2, 0);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
        float speed = 10f * Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) transform.Translate(0, 0, speed);
        if (Input.GetKey(KeyCode.S)) transform.Translate(0, 0, -speed);
        if (Input.GetKey(KeyCode.A)) transform.Translate(-speed, 0, 0);
        if (Input.GetKey(KeyCode.D)) transform.Translate(speed, 0, 0);
    }
}