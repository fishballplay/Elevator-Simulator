using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public int floor,specified_floor,n;
    bool in_elevator;
    ElevatorCenter elevator_center;
    GameObject elevator;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime * elevator_center.speed;
        timer += dt;
        if (in_elevator)
        {
            n = elevator.GetComponent<Elevator>().Users.IndexOf(gameObject);
            transform.localPosition = new Vector3(n * 15-15, 0 , 0) + elevator.transform.localPosition;
            if (timer > 3f)
            {
                elevator.GetComponent<Elevator>().current_button(specified_floor);
                timer -= 3f;
            }
            if (exit_elevator())
                Destroy(gameObject);
        }
        else
        {
            n = elevator_center.Users[floor-1].IndexOf(gameObject);
            transform.localPosition = new Vector3(n*15-15, floor * 50 - 285, 0);
            if (!enter_elevator(elevator_center.elevator1))
                if (!enter_elevator(elevator_center.elevator2))
                {
                    if (timer > 3f)
                    {
                        if (floor < specified_floor)
                        {
                            if (!elevator_center.floor_button_up[floor - 1])
                                elevator_center.floor_button(floor, floor < specified_floor);
                        }
                        else
                        {
                            if (!elevator_center.floor_button_down[floor - 1])
                                elevator_center.floor_button(floor, floor < specified_floor);
                        }
                        timer -= 3f;
                    }
                }
        }
    }
    public bool enter_elevator(GameObject elevator)
    {
        if (elevator.GetComponent<Elevator>().display_floor() == floor)
        {
            if (elevator.GetComponent<Elevator>().door == 2)
            {
                if ((floor < specified_floor && elevator.GetComponent<Elevator>().display_direction() == 1)
                    || (floor > specified_floor && elevator.GetComponent<Elevator>().display_direction() == 2))
                {
                    if (elevator.GetComponent<Elevator>().Users.Count < 3 && n < elevator.GetComponent<Elevator>().timer)
                    {
                        
                        this.elevator = elevator;
                        elevator_center.Users[floor-1].Remove(gameObject);
                        elevator.GetComponent<Elevator>().Users.Add(gameObject);
                        elevator.GetComponent<Elevator>().current_button(specified_floor);
                        in_elevator = true;
                        return true;
                    }
                    timer = 0;
                }
            }
        }
        return false;
    }
    public bool exit_elevator()
    {
        if (elevator.GetComponent<Elevator>().display_floor() == specified_floor)
        {
            if (elevator.GetComponent<Elevator>().door == 2 && n < elevator.GetComponent<Elevator>().timer)
            {
                //Debug.Log(specified_floor);
                elevator.GetComponent<Elevator>().Users.Remove(gameObject);
                return true;
            }
        }
        return false;
    }
    public void setup(int floor, int specified_floor)
    {
        this.floor = floor;
        this.specified_floor = specified_floor;
        elevator_center = transform.parent.GetComponent<ElevatorCenter>();
        if (floor < specified_floor)
        {
            if (!elevator_center.floor_button_up[floor - 1])
                elevator_center.floor_button(floor, floor < specified_floor);
        }
        else
        {
            if (!elevator_center.floor_button_down[floor - 1])
                elevator_center.floor_button(floor, floor < specified_floor);
        }
    }
}
