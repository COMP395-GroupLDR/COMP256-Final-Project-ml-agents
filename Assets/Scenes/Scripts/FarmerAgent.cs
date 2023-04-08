using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using TMPro;

public class FarmerAgent : Agent
{
    Rigidbody rb;
    public GameObject waterPrefab;
    public GameObject plot;
    public Transform area;
    public Transform waterSpawnArea;
    public Transform plotSpawnArea;
    public TextMeshProUGUI rewardText;
    public float cutOffDistance = Mathf.Sqrt(2);

    private List<GameObject> waterList;

    private RayPerceptionSensorComponent3D rayPerception;

    private bool hasWater;

    private int wateredPlant = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ResetArea()
    {
        RemoveAllWater();
        PlacePlot();
        PlaceAgent();
        PlaceWater(3);
    }

    public void RemoveWaterNode(GameObject waterObj)
    {
        waterList.Remove(waterObj);
        Destroy(waterObj);
    }

    // will need to update this to choose random position within rectangular area
    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minDist, float maxDist)
    {
        float distance = minDist;

        if (maxDist > minDist)
        {
            distance = UnityEngine.Random.Range(minDist, maxDist);
        }

        return center + Quaternion.Euler(0f, UnityEngine.Random.Range(minAngle, maxAngle), 0f) * Vector3.forward * distance;
    }

    private void RemoveAllWater()
    {
        if(waterList != null)
        {
            for (int i = 0; i < waterList.Count; i++)
            {
                if (waterList[i] != null )
                {
                    Destroy(waterList[i]);
                }
            }
        }

        waterList = new List<GameObject>();
    }

    private void PlacePlot()
    {
        //will need to add randomness to this later
        plot.transform.position = plotSpawnArea.transform.position;
    }

    private void PlaceAgent()
    {
        transform.position = ChooseRandomPosition(area.position, 0f, 360f, 0f, 5f) + Vector3.up * 0.5f;
        transform.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

    }

    private void PlaceWater(int count)
    {
       for (int i = 0; i < count; i++)
        {
            GameObject waternode = Instantiate(waterPrefab);
            waternode.transform.position = ChooseRandomPosition(waterSpawnArea.position, 0f, 360f, 0.5f, 4f);
            waternode.transform.parent = area.transform;
            waterList.Add( waternode );
        }
    }

    private void Update()
    {
        rewardText.text = GetCumulativeReward().ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("water") && hasWater == false)
        {
            hasWater = true;
            wateredPlant++;
            waterList.Remove(other.gameObject );
            Destroy(other.gameObject);
            AddReward(0.5f);
        }

        if (other.gameObject.CompareTag("plot") && hasWater == true)
        {
            hasWater = false;
            AddReward(1f);
        }
        
    }

    public override void OnEpisodeBegin()
    {
        ResetArea();
        hasWater = false;
        wateredPlant = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //actions
        Vector3 controls = Vector3.zero;

        controls.x = actions.ContinuousActions[0];
        controls.z = actions.ContinuousActions[1];
        rb.AddForce(controls.x, 0, controls.z);

        AddReward(-0.001f);

        if (IsGameOver())
        {
            EndEpisode();
        }
      
    }

    private bool IsGameOver()
    {
        bool isGameOver = false;
        if (waterList.Count == 0 && hasWater == false)
        {
            isGameOver = true;
        }

        return isGameOver;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform);
        sensor.AddObservation(plot.transform);
        foreach(var waterNode in waterList)
        {
            sensor.AddObservation(waterNode.transform);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");        
        continuousActionsOut[1] = Input.GetAxis("Horizontal") * -1;
    }







}
