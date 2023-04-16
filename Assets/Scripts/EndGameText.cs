using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameText : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField]
    SantaAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        UpdateText(agent.giftCollected);
    }


    void UpdateText(int giftsCollected)
    {
        text.text = "You delivered " + giftsCollected + " presents!";
    }
}
