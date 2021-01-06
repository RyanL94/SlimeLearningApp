﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class WordManager : MonoBehaviour
{
    private List<string> fileLines;

    [SerializeField]
    protected Text wordCount;
    [SerializeField]
    protected GameObject wordCountPanel;

    [SerializeField]
    protected GameObject word;
    [SerializeField]
    protected GameObject addWordPanel;
    [SerializeField]
    protected Transform wordList;

    Text[] tFields;

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
            GameObject temp = Instantiate(word) as GameObject;
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
            temp.transform.SetParent(wordList, true);
        }
    }

    public void AddWord()
    {
        string path = Application.dataPath + "/WordList.txt";
        string word;
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

            word = eng + "/" + furi + "/" + kanji + "/" + date + "/" + diff + "\n";

            if (currentError == false)
            {
                File.AppendAllText(path, word);
                Destroy(makeWord[i]);
            }
        }

        if (error == false)
        {
            addWordPanel.SetActive(false);

            ToggleButtons();
        }

        RefreshAllWords();
    }

    public void SortByEnglish()
    {
        List<GameObject> wordListSort = new List<GameObject>();
        GameObject[] allWords = GameObject.FindGameObjectsWithTag("Word");

        foreach (GameObject word in allWords)
        {
            wordListSort.Add(word);
        }

        if(wordListSort.Count > 0)
        {
            wordListSort = wordListSort.OrderBy(x => x.GetComponent<WordInfo>().engWord).ToList();
        }

        foreach (GameObject word in wordListSort)
        {
            GameObject temp = Instantiate(word) as GameObject;
            temp.transform.SetParent(wordList, true);
        }

        foreach (GameObject killThisStupidWord in allWords)
        {
            Destroy(killThisStupidWord);
        }
    }

    public void GetWordCount()
    {
        ToggleButtons();

        wordCountPanel.SetActive(true);

        GameObject[] allWords = GameObject.FindGameObjectsWithTag("Word");

        wordCount.text = allWords.Length.ToString();
    }

    public void CloseWordCount()
    {
        wordCountPanel.SetActive(false);

        ToggleButtons();
    }

    void DestroyAllWords()
    {
        GameObject[] allWords = GameObject.FindGameObjectsWithTag("Word");

        foreach (GameObject killWord in allWords)
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