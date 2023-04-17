using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float moveSpeed;
    public float rotationSpeed = 150f;
    public float collectGiftReward = 0.5f;
    public float carryGiftToSleighReward = 1f;

    public Rigidbody rb;
    public GameObject giftOnBag;
    public GameObject giftPrefab;
    public SleighContainer sleighContainer;

    [SerializeField]
    private string giftTag;
    private string sleighTag;
    public bool hasGift = false;

    private SantaAgent santaAgent;

    // Start is called before the first frame update
    void Start()
    {
        GetTags();
        santaAgent = GameObject.FindGameObjectWithTag("SantaAgent").GetComponent<SantaAgent>();
        moveSpeed = 0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        if (Input.GetKey(KeyCode.D))
        {
            rotateDir = transform.up * 1f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            dirToGo = transform.forward * 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotateDir = transform.up * -1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dirToGo = transform.forward * -1f;
        }

        transform.Rotate(rotateDir, Time.deltaTime * rotationSpeed);
        rb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
    }

    private void EnableGiftOnBag(bool enable)
    {
        giftOnBag.SetActive(enable);
    }

    private void GetTags()
    {
        giftTag = giftPrefab.tag;
        sleighTag = sleighContainer.tag;
    }

    private void DisableGift(GameObject gift)
    {
        gift.SetActive(false);
    }

    private void UpdateGiftCollectedDisplay()
    {
        //giftIconGroup.UpdateFilledCount(giftCollected);
        santaAgent.giftCountText.text = santaAgent.giftCollected.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(giftTag) && hasGift == false)
        {
            EnableGiftOnBag(true);
            hasGift = true;            
            santaAgent.giftsOnGround--;
            DisableGift(other.gameObject);
            sleighContainer.EnableIndicator(true);
            santaAgent.AddReward(collectGiftReward);
        }

        if (other.gameObject.CompareTag(sleighTag) && hasGift == true)
        {
            hasGift = false;
            santaAgent.giftCollected++;
            UpdateGiftCollectedDisplay();
            sleighContainer.UpdateGiftCount(santaAgent.giftCollected);
            sleighContainer.EnableIndicator(false);
            EnableGiftOnBag(false);            
            santaAgent.AddReward(carryGiftToSleighReward);
        }
    }
}
