using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorCenter : MonoBehaviour
{
    public GameObject elevator1, elevator2;
    public bool[] floor_button_up;//電梯外的樓層按鈕 true為亮 false為按
    GameObject[] button_up,button_down;
    public List<List<GameObject>> Users;
    public bool[] floor_button_down;//電梯外的樓層按鈕 true為亮 false為按
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        Users = new List<List<GameObject>>();
        for (int i = 0; i < 10; i++)
        {
            Users.Add(new List<GameObject>());
        }
        floor_button_up = new bool[10];
        floor_button_down = new bool[10];
        button_up = new GameObject[10];
        button_down = new GameObject[10];

        elevator1 = Instantiate(Resources.Load<GameObject>("GameObject/Elevator"), new Vector3(0, 0, 0), Quaternion.identity,transform);
        elevator1.GetComponent<Elevator>().setup(1);
        elevator2 = Instantiate(Resources.Load<GameObject>("GameObject/Elevator"), new Vector3(0, 0, 0), Quaternion.identity, transform);
        elevator2.GetComponent<Elevator>().setup(2);
        for (int i = 2; i < 11; i++)
        {
            GameObject newbtn=Instantiate(Resources.Load<GameObject>("GameObject/Btn"), new Vector3(0, 0, 0), Quaternion.identity, transform);
            newbtn.transform.localPosition = new Vector3(0, i * 50 - 285, 0);
            int floor = i;
            button_down[i-1] = newbtn;
            newbtn.GetComponent<Button>().onClick.AddListener(delegate () { floor_button(floor, false); });
        }
        for (int i = 1; i < 10; i++)
        {
            GameObject newbtn = Instantiate(Resources.Load<GameObject>("GameObject/Btn"), new Vector3(0, 0, 0), Quaternion.identity, transform);
            newbtn.transform.localPosition = new Vector3(0, i * 50 - 265, 0);
            int floor = i;
            button_up[i - 1] = newbtn;
            newbtn.GetComponent<Button>().onClick.AddListener(delegate () { floor_button(floor, true); });
            newbtn.transform.GetChild(0).GetComponent<Text>().text = "^";
        }
        speed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void new_user(int floor, int specified_floor)
    {
        GameObject newuser = Instantiate(Resources.Load<GameObject>("GameObject/User"), new Vector3(0, 0, 0), Quaternion.identity, transform);
        newuser.GetComponent<User>().setup(floor, specified_floor);
        newuser.GetComponent<Image>().color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1);
        newuser.transform.GetChild(0).GetComponent<Image>().color = new Color(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f), Random.Range(0f, 0.5f), 1);
        Users[floor-1].Add(newuser);
    }
    public void random_user()
    {
        int floor = Random.Range(1, 11);
        int specified_floor = Random.Range(1, 10);
        if (specified_floor >= floor)
            specified_floor++;
        /*Debug.Log(floor);
        Debug.Log(specified_floor);*/
        new_user(floor, specified_floor);
    }
    public void floor_button(int floor, bool direction)//true為向上 false為向下
    {
        if (elevator1.GetComponent<Elevator>().get_stop_floor(floor, direction ? 1 : 2))
            return;
        if (elevator2.GetComponent<Elevator>().get_stop_floor(floor, direction ? 1 : 2))
            return;
        if (direction)
        {
            floor_button_up[floor - 1] = true;
            button_up[floor - 1].GetComponent<Image>().color = new Color(1, 1, 0, 1);
        }
        else
        {
            floor_button_down[floor - 1] = true;
            button_down[floor - 1].GetComponent<Image>().color = new Color(1, 1, 0, 1);
        }
        float ask_elevator1 = elevator1.GetComponent<Elevator>().ask_specified_floor(floor, direction ? 1 : 2);
        float ask_elevator2 = elevator2.GetComponent<Elevator>().ask_specified_floor(floor, direction ? 1 : 2);
        if (ask_elevator1 < ask_elevator2)
        {
            elevator1.GetComponent<Elevator>().set_specified_floor(floor, direction ? 1 : 2);
        }
        else if (ask_elevator1 > ask_elevator2)
        {
            elevator2.GetComponent<Elevator>().set_specified_floor(floor, direction ? 1 : 2);
        }
        else if(ask_elevator1 < 10)
        {
            if(Random.Range(0,100)<50)
                elevator1.GetComponent<Elevator>().set_specified_floor(floor, direction ? 1 : 2);
            else
                elevator2.GetComponent<Elevator>().set_specified_floor(floor, direction ? 1 : 2);
        }
    }
    public void elevator_arrive(int floor, int direction)
    {
        if (direction == 1)
        {
            floor_button_up[floor - 1] = false;
            Debug.Log(floor);
            button_up[floor - 1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            floor_button_down[floor - 1] = false;
            Debug.Log(floor);
            button_down[floor - 1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
    
    public bool next_specified_floor(GameObject elevator,int current_floor, int direction, int floor)
    {
        GameObject other_elevator;
        if (elevator == elevator1)
            other_elevator = elevator2;
        else
            other_elevator = elevator1;
        if (direction == 1)//電梯向上
        {
            for (int i = current_floor; i <= floor; i++)
            {
                if (floor_button_up[i - 1])
                {
                    if (!other_elevator.GetComponent<Elevator>().check_specified(i, 1))
                    {
                        if (elevator.GetComponent<Elevator>().set_specified_floor(i, 1))
                        {
                            return true;
                        }
                    }
                }
            }
            for (int i = current_floor; i <= floor; i++)
            {
                if (floor_button_down[i - 1])
                {
                    if (!other_elevator.GetComponent<Elevator>().check_specified(i, 2))
                    {
                        if(elevator.GetComponent<Elevator>().set_specified_floor(i, 2))
                            return true;
                    }
                }
            }
        }
        else //電梯向下
        {
            for (int i = current_floor; i >= floor; i--)
            {
                if (floor_button_down[i - 1])
                {
                    if (!other_elevator.GetComponent<Elevator>().check_specified(i, 2))
                    {
                        if(elevator.GetComponent<Elevator>().set_specified_floor(i, 2))
                            return true;
                    }
                }
            }
            for (int i = current_floor; i >= floor; i--)
            {
                if (floor_button_up[i - 1])
                {
                    if (!other_elevator.GetComponent<Elevator>().check_specified(i, 1))
                    {
                        if(elevator.GetComponent<Elevator>().set_specified_floor(i, 1))
                            return true;
                    }
                }
            }
        }
        return false;
    }
    public void speedup(bool b)
    {
        if (b)
        {
            speed += 1f;
            if (speed > 10)
                speed = 10;
        }
        else
        {
            speed -= 1f;
            if (speed < 1)
                speed = 1;
        }
        GameObject.FindGameObjectWithTag("Speed").GetComponent<Text>().text = "Speed*"+speed.ToString();
    }
}
