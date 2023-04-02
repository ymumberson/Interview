using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaCameraScript : MonoBehaviour
{
    private float move_speed;
    private float rotate_speed;

    private void Awake()
    {
        move_speed = 50f;
        rotate_speed = 150f;
    }

    private void Update()
    {
        Vector3 raw_move_direction = new Vector3(0,0,0);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0) raw_move_direction.x = horizontal;
        if (vertical != 0) raw_move_direction.z = vertical;
        if (Input.GetKey(KeyCode.LeftControl)) raw_move_direction.y = -1f;
        if (Input.GetKey(KeyCode.LeftShift)) raw_move_direction.y = 1f;

        Vector3 move_direction = transform.forward * raw_move_direction.z + transform.right * raw_move_direction.x + transform.up * raw_move_direction.y;
        transform.position += move_speed * move_direction * Time.deltaTime;

        float rotate_amount = 0f;
        if (Input.GetKey(KeyCode.Q)) rotate_amount += +1f;
        if (Input.GetKey(KeyCode.E)) rotate_amount += -1f;

        transform.eulerAngles += new Vector3(0, rotate_amount * rotate_speed * Time.deltaTime, 0);
    }
}
