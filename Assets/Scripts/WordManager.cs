using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;

public class WordManager : MonoBehaviour
{
    private List<string> fileLines;

    [Header("Word List")]
    [SerializeField] private Text wordCount = default;
    [SerializeField] private GameObject wordCountPanel = default;

    [Header("Add Words")]
    [SerializeField] private GameObject word = default;
    [SerializeField] private GameObject addWordPanel = default;
    [SerializeField] private Transform wordList = default;

    [Header("Select words")]
    [SerializeField] private GameObject selectWordPanel = default;
    [SerializeField] private InputField newestSelect = default;
    [SerializeField] private InputField oldestSelect = default;
    [SerializeField] private InputField startDateSelect = default;
    [SerializeField] private InputField endDateSelect = default;

    [Header("Search")]
    [SerializeField] private GameObject searchWordPanel = default;
    [SerializeField] private InputField searchField = default;

    private Text[] tFields;
    private List<GameObject> cachedList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateFile();
        RefreshAllWords();
    }

    void CreateFile()
    {
        string path = Application.dataPath + "/WordList.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }
    }

    public void RefreshAllWords()
    {
        DestroyAllWords();

        string readFrom = Application.dataPath + "/WordList.txt";

        fileLines = File.ReadAllLines(readFrom).ToList();

        for (int i = 0; i < fileLines.Count; i++)
        {
            string[] wordData = fileLines[i].Split('/');
            GameObject temp = Instantiate(word, wordList);
            cachedList.Add(temp);
            tFields = temp.GetComponentsInChildren<Text>();
            tFields[0].text = wordData[0];
            temp.GetComponent<WordInfo>().engWord = wordData[0];
            tFields[1].text = wordData[1];
            temp.GetComponent<WordInfo>().furiWord = wordData[1];
            tFields[2].text = wordData[2];
            temp.GetComponent<WordInfo>().kanjiWord = wordData[2];
            tFields[3].text = wordData[3];
            temp.GetComponent<WordInfo>().date = wordData[3];
            tFields[4].text = wordData[4];
            temp.GetComponent<WordInfo>().diff = wordData[4];
        }
    }

    public void AddWord()
    {
        string path = Application.dataPath + "/WordList.txt";
        string wordToAdd;
        string diff;
        bool error = false;
        bool currentError = false;

        GameObject[] makeWord = GameObject.FindGameObjectsWithTag("WordTemplate");

        for (int i = 0; i < makeWord.Length; i++)
        {
            currentError = false;

            InputField[] iFields = makeWord[i].GetComponentsInChildren<InputField>();

            if (iFields[0].text == "" || iFields[1].text == "" || iFields[2].text == "" || iFields[3].text == "")
            {
                currentError = true;
                error = true;
            }

            string eng = iFields[0].text;
            string furi = iFields[1].text;
            string kanji = iFields[2].text;
            string date = iFields[3].text;
            Dropdown dropdown = makeWord[i].GetComponentInChildren<Dropdown>();

            if(dropdown.value == 0)
            {
                diff = "E";
            }
            else if (dropdown.value == 1)
            {
                diff = "M";
            }
            else if (dropdown.value == 2)
            {
                diff = "H";
            }
            else
            {
                diff = "?";
            }

            wordToAdd = eng + "/" + furi + "/" + kanji + "/" + date + "/" + diff + "\n";

            if (currentError == false)
            {
                File.AppendAllText(path, wordToAdd);
                Destroy(makeWord[i]);
            }
        }

        if (error == false)
        {
            addWordPanel.SetActive(false);

            ToggleButtons();
        }

        string readFrom = Application.dataPath + "/WordList.txt";

        fileLines = File.ReadAllLines(readFrom).ToList();
        string[] wordData = fileLines[fileLines.Count-1].Split('/');
        GameObject temp = Instantiate(word, wordList);
        cachedList.Add(temp);
        tFields = temp.GetComponentsInChildren<Text>();
        tFields[0].text = wordData[0];
        temp.GetComponent<WordInfo>().engWord = wordData[0];
        tFields[1].text = wordData[1];
        temp.GetComponent<WordInfo>().furiWord = wordData[1];
        tFields[2].text = wordData[2];
        temp.GetComponent<WordInfo>().kanjiWord = wordData[2];
        tFields[3].text = wordData[3];
        temp.GetComponent<WordInfo>().date = wordData[3];
        tFields[4].text = wordData[4];
        temp.GetComponent<WordInfo>().diff = wordData[4];
    }

    public void SortByEnglish()
    {
        List<GameObject> wordListSort = new List<GameObject>();

        foreach (GameObject word in cachedList)
        {
            wordListSort.Add(word);
        }

        if(wordListSort.Count > 0)
        {
            wordListSort = wordListSort.OrderBy(x => x.GetComponent<WordInfo>().engWord).ToList();
        }

        foreach (GameObject word in wordListSort)
        {
            word.transform.SetAsLastSibling();
        }
    }

    public void GetWordCount()
    {
        ToggleButtons();

        wordCountPanel.SetActive(true);

        wordCount.text = cachedList.Count.ToString();
    }

    public void CloseWordCount()
    {
        wordCountPanel.SetActive(false);

        ToggleButtons();
    }

    public void ToggleSelectBy()
    {
        ToggleButtons();

        selectWordPanel.SetActive(!selectWordPanel.activeSelf);
    }

    public void SelectAll()
    {
        foreach (GameObject word in cachedList)
        {
            word.GetComponentInChildren<Toggle>().isOn = true;
        }
    }

    public void UnselectAll()
    {
        foreach (GameObject word in cachedList)
        {
            word.GetComponentInChildren<Toggle>().isOn = false;
        }
    }

    public void SelectConditions()
    {
        ToggleButtons();
        selectWordPanel.SetActive(!selectWordPanel.activeSelf);

        if((startDateSelect.text != "" && endDateSelect.text != "") || newestSelect.text != "" || oldestSelect.text != "")
        {
            UnselectAll();
        }

        if(startDateSelect.text != "" && endDateSelect.text != "")
        {
            System.DateTime startDate = System.DateTime.Parse(startDateSelect.text);
            System.DateTime endDate = System.DateTime.Parse(endDateSelect.text);

            for (int i = 0; i<cachedList.Count; i++)
            {
                tFields = cachedList[i].GetComponentsInChildren<Text>();
                String dateText = tFields[3].text;
                System.DateTime date = System.DateTime.Parse(dateText);

                if(DateTime.Compare(date,startDate) >= 0  && DateTime.Compare(date,endDate) <= 0)
                {
                    cachedList[i].GetComponentInChildren<Toggle>().isOn = true;
                }
            }
        }
        startDateSelect.text = "";
        endDateSelect.text = "";

        int selectAmount;
        if (newestSelect.text != "")
        {
            selectAmount = cachedList.Count - Int32.Parse(newestSelect.text);
        }
        else
        {
            selectAmount = cachedList.Count;
        }
        newestSelect.text = "";

        for (int i = cachedList.Count; i > selectAmount; i--)
        {
            cachedList[i-1].GetComponentInChildren<Toggle>().isOn = true;
        }

        if (oldestSelect.text != "")
        {
            selectAmount = Int32.Parse(oldestSelect.text);
        }
        else
        {
            selectAmount = 0;
        }
        oldestSelect.text = "";

        for (int i= 0; i< selectAmount; i++)
        {
            cachedList[i].GetComponentInChildren<Toggle>().isOn = true;
        }
    }

    public void ToggleSearchWords()
    {
        ToggleButtons();

        searchWordPanel.SetActive(!searchWordPanel.activeSelf);
    }

    public void SearchWords()
    {
        if (searchField.text == "")
        {
            return;
        }

        for (int i = 0; i < cachedList.Count; i++)
        {
            tFields = cachedList[i].GetComponentsInChildren<Text>();
            String comparisonText = tFields[0].text.ToLower();
            if (!(comparisonText.Contains(searchField.text.ToLower()) || tFields[1].text.Contains(searchField.text) || tFields[2].text.Contains(searchField.text)))
            {
                cachedList[i].SetActive(false);
            }
        }

        searchField.text = "";
        ToggleSearchWords();
    }

    public void ResetWords()
    {
        foreach(GameObject word in cachedList)
        {
            word.SetActive(true);
            word.transform.SetAsLastSibling();
            word.GetComponentInChildren<Toggle>().isOn = true;
        }
    }

    void DestroyAllWords()
    {
        foreach (GameObject killWord in cachedList)
        {
            Destroy(killWord);
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
