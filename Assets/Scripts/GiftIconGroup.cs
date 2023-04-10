using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GiftIconGroup : MonoBehaviour
{
    [SerializeField] private GiftIcon giftIconPrefab;
    [SerializeField] private int filled;

    private List<GiftIcon> giftIcons;

    public void Init(int totalCount)
    {
        ResetIcons();
        giftIcons = new List<GiftIcon>();

        for (int i = 0; i < totalCount; i++)
        {
            GiftIcon giftIcon = Instantiate(giftIconPrefab, transform);
            giftIcons.Add(giftIcon);
        }
    }
    private void ResetIcons()
    {
        foreach (Transform transform in transform)
        {
            if (transform.GetComponent<GiftIcon>() != null)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    public void UpdateFilledCount(int filledCount)
    {
        filled = filledCount;
        UpdateIcons();
    }

    [Button]
    private void UpdateIcons()
    {
        for (int i = 0; i < giftIcons.Count; i++)
        {
            giftIcons[i].Show(i < filled);
        }
    }

    [Button]
    private void UpdateGiftListByChild()
    {
        giftIcons = transform.GetComponentsInChildren<GiftIcon>().ToList();
    }
}
