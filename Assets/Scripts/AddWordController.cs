using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWordController : MonoBehaviour
{
    [SerializeField] private GameObject howManyPanel;
    [SerializeField] private GameObject addWordsPanel;
    [SerializeField] private GameObject wordTemplate;
    [SerializeField] private InputField amountToSpawn;
    [SerializeField] private Transform addWordLocation;

    private InputField[] iFields;

    public void EnableAddWords()
    {
        int spawnNumber;

        addWordsPanel.SetActive(true);
        EnableHowManyPanel();

        if(amountToSpawn.text == "")
        {
            return;
        }

        string amount = amountToSpawn.text;
        if (amount != null)
        {
            spawnNumber = Int32.Parse(amount);
        }
        else
        {
            spawnNumber = 0;
        }

        amountToSpawn.text = "";

        DateTime date = DateTime.Today;

        for (int i = 0; i<spawnNumber; i++)
        {
            GameObject temp = Instantiate(wordTemplate) as GameObject;
            iFields = temp.GetComponentsInChildren<InputField>();
            iFields[3].text = date.ToString("d");
            temp.transform.SetParent(addWordLocation, true);
        }
    }

    public void DisableAddPanel()
    {
        GameObject[] killWord = GameObject.FindGameObjectsWithTag("WordTemplate");

        foreach (GameObject wordTemplate in killWord)
        {
            Destroy(wordTemplate);
        }

        addWordsPanel.SetActive(false);
        ToggleButtons();
    }

    public void EnableHowManyPanel()
    {
        howManyPanel.SetActive(!howManyPanel.activeSelf);

        if (addWordsPanel.activeSelf == false)
        {
            ToggleButtons();
        }
    }

    void ToggleButtons()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = !button.GetComponent<Button>().interactable;
        }
    }
}
