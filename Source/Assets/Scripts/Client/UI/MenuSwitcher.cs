using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private int startPage;

    private void Start()
    {
        OpenPage(startPage);
    }

    public void OpenPage(int index) 
    {
        if(index < 0 || index >= pages.Length)
            throw new System.ArgumentOutOfRangeException();

        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == index);
    }
}
