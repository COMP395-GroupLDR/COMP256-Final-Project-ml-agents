using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;

public class SantaAgent : Agent
{
    public GiftIconGroup giftIconGroup;
    public SleighContainer sleighContainer;
    public Rigidbody rb;
    public float collectGiftReward = 0.5f;
    public float carryGiftToSleighReward = 1f;
    public float agentRunSpeed = 1.5f;
    public float agentRotationSpeed = 150f;
    public GameObject giftPrefab;
    public GameObject sleigh;
    public Transform area;
    public Transform giftSpawnArea;
    public Transform sleighSpawnArea;
    public GameObject giftOnBag;
    public bool useVectorObs;
    public TextMeshProUGUI giftCountText;
    public int giftCollected = 0;



    [Header("Display")]
    public TextMeshProUGUI rewardText;

    [SerializeField]
    private bool isGameMode;
    private string giftTag;
    private string sleighTag;
    private List<GameObject> giftList;
    private bool hasGift;
    private int giftCount = 3;

    public int giftsOnGround = 0;

    private PlayerController playerController;

    // Start is called before the first frame update
    public override void Initialize()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        GetTags();
        giftList = new List<GameObject>();
        
    }

    private void GetRandomGiftCount()
    {
        giftCount = Random.Range(1, sleighContainer.GetGiftCapacity() + 1);
    }

    private void GetTags()
    {
        giftTag = giftPrefab.tag;
        sleighTag = sleighContainer.tag;
    }

    public void ResetArea()
    {
        EnableGiftOnBag(false);
        //GetRandomGiftCount(); // vector observation size will produce warning
        ResetSleighContainer();
        RemoveAllGifts();
        PlaceBuggy();
        PlaceAgent();
        PlaceGifts(giftCount);
        //giftIconGroup.Init(giftCount);
    }

    private void ResetSleighContainer()
    {
        sleighContainer.UpdateGiftCount(0);
        sleighContainer.EnableIndicator(false);
    }

    void Update()
    {
        rewardText.text = GetCumulativeReward().ToString("0.00");
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

    private void RemoveAllGifts()
    {
        if (giftList != null)
        {
            for (int i = 0; i < giftList.Count; i++)
            {
                if (giftList[i] != null)
                {
                    Destroy(giftList[i]);
                }
            }
            giftList.Clear();
        }
    }

    private void PlaceBuggy()
    {
        //will need to add randomness to this later
        sleigh.transform.position = sleighSpawnArea.transform.position;
    }

    private void PlaceAgent()
    {
        transform.position = ChooseRandomPosition(area.position, 0f, 360f, 0f, 5f) + Vector3.up * 0.5f;
        transform.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    private void PlaceGifts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject gift = Instantiate(giftPrefab);
            gift.transform.position = ChooseRandomPosition(giftSpawnArea.position, 0f, 360f, 0.5f, 4f);
            gift.transform.parent = area.transform;
            giftList.Add(gift);
            giftsOnGround++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(giftTag) && hasGift == false)
        {
            EnableGiftOnBag(true);
            hasGift = true;            
            giftsOnGround--;
            DisableGift(other.gameObject);
            //sleighContainer.EnableIndicator(true);
            AddReward(collectGiftReward);
        }

        if (other.gameObject.CompareTag(sleighTag) && hasGift == true)
        {
            giftCollected++;
            UpdateGiftCollectedDisplay();
            sleighContainer.UpdateGiftCount(giftCollected);
            //sleighContainer.EnableIndicator(false);
            EnableGiftOnBag(false);
            hasGift = false;
            AddReward(carryGiftToSleighReward);
        }
    }

    private void DisableGift(GameObject gift)
    {
        gift.SetActive(false);
    }

    private void EnableGiftOnBag(bool enable)
    {
        giftOnBag.SetActive(enable);
    }

    public override void OnEpisodeBegin()
    {
        ResetArea();
        hasGift = false;
        playerController.hasGift = false;
        giftCollected = 0;
    }

    private void UpdateGiftCollectedDisplay()
    {
        //giftIconGroup.UpdateFilledCount(giftCollected);
        giftCountText.text = giftCollected.ToString();

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions);
        AddReward(-1f / MaxStep);

        if (IsGameOver())
        {
            if(isGameMode)
            {
                PlaceGifts(giftCount);
            }
            else
            {
                EndEpisode();
            }
            
        }
    }

    private void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * agentRotationSpeed);
        rb.AddForce(dirToGo * agentRunSpeed, ForceMode.VelocityChange);
    }

    private bool IsGameOver()
    {
        bool isGameOver = false;
        if (giftsOnGround == 0 && hasGift == false)
        {
            isGameOver = true;
        }

        return isGameOver;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(StepCount / (float)MaxStep);
        }
        else
        {
            sensor.AddObservation(transform);
            sensor.AddObservation(sleigh.transform);
            foreach (var gift in giftList)
            {
                sensor.AddObservation(gift.transform);
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }
}
