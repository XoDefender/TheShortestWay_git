using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEndWaypoints : MonoBehaviour
{
    [SerializeField] private LayerMask waypointLayerMask;
    [SerializeField] private WaypointData endWaypoint;

    private WaypointData startWaypoint;
    private WaypointData targetWaypoint;
    private WaypointData[] waypoints;
    private GameObject player;
    private GameObject enemy;
    private Pathfinder pathfinder;
    private ChestController chestController;
    private ScoreManager scoreManager;

    private const string playerName = "Player";
    private const string enemyName = "Enemy";

    private bool hasPickedTargetWaypoint = false;
    public bool readyToPickEndWaypoint = true;

    private void Awake()
    {
        player = GameObject.Find(playerName);
        enemy = GameObject.Find(enemyName);
        waypoints = FindObjectsOfType<WaypointData>();
        pathfinder = FindObjectOfType<Pathfinder>();
        chestController = FindObjectOfType<ChestController>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void Update()
    {
        if (player)
        {
            PickStartWaypoint(waypoints);
            PickTargetWaypoint();

            endWaypoint.GetComponent<MeshRenderer>().material.color = Color.black;

            if(chestController.HasOpened && scoreManager.Score >= chestController.RequiredAmountOfPoints)
            {
                if (Mathf.Approximately(endWaypoint.transform.position.x, player.transform.position.x) && Mathf.Approximately(endWaypoint.transform.position.z, player.transform.position.z))
                {
                    Destroy(pathfinder);
                    Destroy(enemy);
                }
            }
        }
    }

    private void PickStartWaypoint(WaypointData[] waypoints)
    {
        if (startWaypoint == null)
        {
            foreach (WaypointData waypoint in waypoints)
            {
                if (Mathf.Approximately(waypoint.transform.position.x, player.transform.position.x) && Mathf.Approximately(waypoint.transform.position.z, player.transform.position.z))
                {
                    startWaypoint = waypoint;

                    break;
                }
            }
        }
    }

    private void PickTargetWaypoint()
    {
        if (!hasPickedTargetWaypoint && readyToPickEndWaypoint)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, waypointLayerMask))
            {
                targetWaypoint = hit.collider.gameObject.GetComponent<WaypointData>();

                readyToPickEndWaypoint = false;
                pathfinder.IsObserved = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit, 1000, waypointLayerMask))
                    hasPickedTargetWaypoint = true;
            }
        }
    }

    public WaypointData StartWaypoint { get { return startWaypoint; } set { startWaypoint = null; } }

    public WaypointData EndWaypoint { get { return endWaypoint; } }

    public WaypointData TargetWaypoint { get { return targetWaypoint; } set { targetWaypoint = null; } }

    public bool HasPickedTargetWaypoint { get { return hasPickedTargetWaypoint; } set { hasPickedTargetWaypoint = value; } }
}
