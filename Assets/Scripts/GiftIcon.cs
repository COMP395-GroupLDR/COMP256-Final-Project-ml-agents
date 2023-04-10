using UnityEngine;

public class GiftIcon : MonoBehaviour
{
    [SerializeField] private GameObject icon;

    public void Show(bool filled)
    {
        icon.SetActive(filled);
    }

    void OnValidate()
    {
        icon.SetActive(false);
    }
}
