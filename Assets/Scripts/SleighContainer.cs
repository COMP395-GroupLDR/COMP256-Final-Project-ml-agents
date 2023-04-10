using UnityEngine;

public class SleighContainer : MonoBehaviour
{
    [SerializeField] private GameObject[] m_Gifts;
    [SerializeField] private GameObject indicator;

    void OnEnable()
    {
        for (int i = 0; i < m_Gifts.Length; i++)
        {
            m_Gifts[i].SetActive(false);
        }

        EnableIndicator(false);
    }

    public int GetGiftCapacity()
    {
        return m_Gifts.Length;
    }

    public void EnableIndicator(bool active)
    {
        indicator.SetActive(active);
    }

    public void UpdateGiftCount(int count)
    {
        for (int i = 0; i < m_Gifts.Length; i++)
        {
            m_Gifts[i].SetActive(count > i);
        }
    }
}
