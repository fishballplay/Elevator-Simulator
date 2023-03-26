using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Elevator : MonoBehaviour
{
    int start_floor, current_floor, specified_floor;//電梯開始樓層，目前樓層，目標樓層
    int direction,next_direction;//不動為0,往上為1,往下為2,next_direction為到達後的方向
    public int door;//0是關閉 1是打開中 2是打開 3是關閉中
    bool[] current_buttons;//電梯內的樓層按鈕 true為亮 false為按
    GameObject[] button;
    public List<GameObject> Users;
    int n;//第幾個電梯
    public float position,speed,timer;
    float dt;
    ElevatorCenter elevator_center;
    // Start is called before the first frame update
    void Start()
    {
        elevator_center = transform.parent.GetComponent<ElevatorCenter>();
        current_buttons = new bool[10];
        button = new GameObject[10];
        Users = new List<GameObject>();
        current_floor = 1;
        position = 1f;
        start_floor = current_floor;
        transform.localPosition = new Vector3(n * 100 - 150, position * 50 - 275, 0);
        for (int i = 1; i < 11; i++)
        {
            GameObject newbtn = Instantiate(Resources.Load<GameObject>("GameObject/Btn"), new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
            newbtn.transform.localPosition = new Vector3(n*500-750, i * 30 - 150, 0);
            int floor = i;
            newbtn.GetComponent<Button>().onClick.AddListener(delegate () { current_button(floor); });
            button[i - 1] = newbtn;
            newbtn.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == 0)
            return;
        dt = Time.deltaTime*elevator_center.speed;
        if (door == 0)
            move();
        else
            open();

    }
    void open()
    {
        timer += dt;
        if (door == 1)
        {
            GetComponent<Image>().color = new Color(1, 1, (3f - timer)/2f, 1);
            if (timer > 2f)
            {
                door = 2;
                timer -= 2f;
            }
        }
        else if (door == 2)
        {
            GetComponent<Image>().color = new Color(1, 1, 0, 1);
            if (timer > 5f)
            {
                door = 3;
                timer -= 5f;
            }
        }
        else if (door == 3)
        {
            GetComponent<Image>().color = new Color(1, 1, timer/2f, 1);
            if (timer > 3f)
            {
                timer =0f;
                bool t = true;
                if (direction == 1)
                {
                    for (int i = current_floor; i <= 10; i++)
                    {
                        if (current_buttons[i - 1])
                        {
                            direction = 0;
                            set_specified_floor(i, i==10 ? 2 : 1);
                            elevator_center.next_specified_floor(gameObject, current_floor, direction, i);
                            t = false;
                            break;
                        }

                    }
                }
                else
                {
                    for (int i = current_floor; i > 0; i--)
                    {
                        if (current_buttons[i - 1])
                        {
                            direction = 0;
                            set_specified_floor(i, i==1 ? 1 : 2);
                            elevator_center.next_specified_floor(gameObject, current_floor, direction, i);
                            t = false;
                            break;
                        }
                    }
                }
                if (t)
                {
                    specified_floor = direction == 1 ? 11 : 0;
                    if (elevator_center.next_specified_floor(gameObject, current_floor, direction, direction == 1 ? 10 : 1))
                    {
                        door = 0;
                        return;
                    }
                    direction = 3 - direction;
                    next_direction = direction;
                    specified_floor = direction == 1 ? 11 : 0;
                    if (elevator_center.next_specified_floor(gameObject, current_floor, direction, direction == 1 ? 10 : 1))
                    {
                        door = 0;
                        return;
                    }
                    direction = 0;
                }
                door = 0;
            }
        }
    }

    public int display_floor()
    {
        return current_floor;
    }
    public int display_direction()
    {
        return direction;
    }
    void move()
    {
        float t = direction == 1 ? Mathf.Min(specified_floor - position, position - start_floor) : Mathf.Max(specified_floor - position, position - start_floor);
        if (Mathf.Abs(t) < 1)
        {
            if (Mathf.Abs(t) < 0.01f)
                speed = direction == 1 ? 0.03f : -0.03f;
            else
                speed = t / Mathf.Sqrt(Mathf.Abs(t));
            if (Mathf.Abs(specified_floor - position) < 0.01f)
            {
                speed = 0;
                position = specified_floor;
                door = 1;
                if (direction == 1)
                {
                    for (int i = specified_floor; i >= 1; i--)
                    {
                        current_buttons[i - 1] = false;
                        button[i - 1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                }
                else
                {
                    for (int i = specified_floor; i <= 10; i++)
                    {
                        current_buttons[i - 1] = false;
                        button[i - 1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                }
                direction = next_direction;
                if (specified_floor == 10)
                    direction = 2;
                if (specified_floor == 1)
                    direction = 1;
                start_floor = current_floor;
                elevator_center.elevator_arrive(specified_floor, direction);
            }
        }
        else if (current_floor < specified_floor)
            speed = 1;
        else if (current_floor > specified_floor)
            speed = -1;
        else
            speed = 0;
        position += speed * dt;
        current_floor = (int)(position + 0.5f);
        transform.localPosition = new Vector3(n * 100 - 150, position * 50 - 275, 0);
    }
    public float ask_specified_floor(int floor, int direct)// 方向1往上 2往下
    {
        if (direction == 0)//電梯未動
        {
            return Mathf.Abs(floor - position)*0.1f;
        }
        else if (direction == 1)//電梯正向上
        {
            if (direct == 1 || floor == 10)//輸入為往上
            {
                if (floor < specified_floor || next_direction == 2)//樓層小於目標樓層時或電梯原本要向下才可能改變目標
                {
                    if (position < floor - 1 || (speed < 0.7f && position <= floor))//判定是否來得及停下
                    {
                        return floor - position;
                    }
                }
            }
            else if (direct == 2)//輸入為往下
            {
                if (next_direction == 2)//電梯將往下
                {
                    if (floor >= specified_floor)//樓層大於目標樓層時才可能改變目標
                    {
                        if (speed > 0.9f)//判定是否已經開始減速了
                        {
                            return floor - position;
                        }
                    }
                }
            }
        }
        else if (direction == 2 || floor == 1)//電梯正向下
        {
            if (direct == 2)//輸入為往下
            {
                if (floor > specified_floor || next_direction == 1)//樓層大於目標樓層時或電梯原本要向上才可能改變目標
                {
                    if (position > floor + 1 || (speed > -0.7f && position >= floor))//判定是否來得及停下
                    {
                        return position - floor;
                    }
                }
            }
            else if (direct == 1)//輸入為往上
            {
                if (next_direction == 1)//電梯將往上
                {
                    if (floor <= specified_floor)//樓層小於目標樓層時才可能改變目標
                    {
                        if (speed < -0.9f)//判定是否已經開始減速了
                        {
                            return position - floor;
                        }
                    }
                }
            }
        }
        return 10;
    }
    public bool set_specified_floor(int floor,int direct)//往上為1,往下為2
    {
        if (direction == 0)//電梯未動
        {
            specified_floor = floor;
            next_direction = direct;
            start_floor = current_floor;
            if (floor > current_floor)
                direction = 1;
            else
                direction = 2;
            return true;
        }
        else if (direction == 1)//電梯正向上
        {
            if (direct == 1 || floor == 10)//輸入為往上
            {
                if (floor < specified_floor || next_direction == 2)//樓層小於目標樓層時或電梯原本要向下才可能改變目標
                {
                    if (position < floor - 1 || (speed < 0.7f && position <= floor))//判定是否來得及停下
                    {
                        specified_floor = floor;
                        next_direction = direct;
                        return true;
                    }
                }
            }
            else if (direct == 2)//輸入為往下
            {
                if (next_direction == 2 || speed == 0)//電梯將往下
                {
                    if (floor >= specified_floor)//樓層大於目標樓層時才可能改變目標
                    {
                        if (speed > 0.9f || speed == 0)//判定是否已經開始減速了
                        {
                            specified_floor = floor;
                            next_direction = direct;
                            return true;
                        }
                    }
                }
            }
        }
        else if (direction == 2)//電梯正向下
        {
            if (direct == 2 || floor == 1)//輸入為往下
            {
                if (floor > specified_floor || next_direction == 1)//樓層大於目標樓層時或電梯原本要向上才可能改變目標
                {
                    if (position > floor + 1 || (speed > -0.7f && position >= floor))//判定是否來得及停下
                    {
                        specified_floor = floor;
                        next_direction = direct;
                        return true;
                    }
                }
            }
            else if (direct == 1)//輸入為往上
            {
                if (next_direction == 1 || speed == 0)//電梯將往上
                {
                    if (floor <= specified_floor)//樓層小於目標樓層時才可能改變目標
                    {
                        if (speed < -0.9f || speed == 0)//判定是否已經開始減速了
                        {
                            specified_floor = floor;
                            next_direction = direct;
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public int get_direction()//沒動為0 往上為1 往下為2
    {
        return direction;
    }
    public void setup(int number)
    {
        n = number;
    }
    public bool get_stop_floor(int floor , int direct)
    {
        if (door > 0)
        {
            if(floor == specified_floor && direct == next_direction)
            return true;
        }
        return false;
    }
    public bool check_specified(int floor, int direct)
    {
        if (floor == specified_floor && direct == next_direction)
            return true;
        return false;
    }
    public void current_button(int floor)
    {
        if (floor != start_floor)
        {
            current_buttons[floor - 1] = true;
            button[floor - 1].GetComponent<Image>().color = new Color(1, 1, 0, 1);
            set_specified_floor(floor, floor > start_floor ? 1 : 2);
        }
    }
}
