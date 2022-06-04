using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECoinCollector : MonoBehaviour
{
    private EWaypointData[] waypoints;
    private EPathfinder pathfinder;
    private EnemyMovement enemyMovement;
    private EStartTargetWaypoints startTargetWaypoints;

    private int maxCoins = 0;
    private int pickedCoins = 0;
    private int requiredCoins = 500;

    private void Awake()
    {
        waypoints = FindObjectsOfType<EWaypointData>();
        enemyMovement = FindObjectOfType<EnemyMovement>();
        pathfinder = FindObjectOfType<EPathfinder>();
        startTargetWaypoints = FindObjectOfType<EStartTargetWaypoints>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(pickedCoins);

        if (pickedCoins <= requiredCoins)
            SelectTheGreatestPath();
        else
            startTargetWaypoints.ReadyToPickEndWaypoint = true;

        PickUpCoin(waypoints);
    }

    private void SelectTheGreatestPath()
    {
        if (pathfinder.AllPaths.Count == waypoints.Length && !enemyMovement.IsGoing)
        {
            foreach (List<EWaypointData> path in pathfinder.AllPaths)
            {
                int coins = 0;

                foreach (EWaypointData waypoint in path)
                {
                    if (waypoint.TextMesh.text != "")
                        coins += int.Parse(waypoint.TextMesh.text);
                }

                if (coins > maxCoins)
                    maxCoins = coins;
            }

            foreach (List<EWaypointData> path in pathfinder.AllPaths)
            {
                int coins = 0;

                foreach (EWaypointData waypoint in path)
                {
                    if (waypoint.TextMesh.text != "")
                        coins += int.Parse(waypoint.TextMesh.text);
                }

                if (coins == maxCoins && maxCoins != 0)
                {
                    enemyMovement.PathToFollow = path;
                    maxCoins = 0;

                    foreach (EWaypointData waypoint in path)
                        waypoint.GetComponent<MeshRenderer>().material.color = Color.green;

                    break;
                }
            }
        }
    }

    private void PickUpCoin(EWaypointData[] waypoints)
    {
        foreach (EWaypointData waypoint in waypoints)
        {
            if (Mathf.Approximately(waypoint.transform.position.x, enemyMovement.transform.position.x) && Mathf.Approximately(waypoint.transform.position.z, enemyMovement.transform.position.z))
            {
                if (waypoint.TextMesh.text != "")
                {
                    pickedCoins += int.Parse(waypoint.TextMesh.text);
                    waypoint.TextMesh.text = "";
                }
            }
        }
    }

    public int RequiredCoins { get { return requiredCoins; } }
    public int PickedCoins { get { return pickedCoins; } }
}
