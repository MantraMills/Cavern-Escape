using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlat : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWaypointIndex = 0;

    [SerializeField] private float speed = 2f;

    public bool plat9 = false;

    // Start is called before the first frame update
    void Start()
    {
        plat9 = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(name != "Mov.Plat 9")
        {
            if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < 0.1f)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = 0;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed);
        }

        if (name == "Mov.Plat 9" && plat9 == true)
        {
            if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < 0.1f)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = 0;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed);
        }

        //if(Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < 0.1f)
        //{
        //    currentWaypointIndex++;

        //    if(currentWaypointIndex >= waypoints.Length)
        //    {
        //        currentWaypointIndex = 0;
        //    }
        //}

        //transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed);
    }
}
