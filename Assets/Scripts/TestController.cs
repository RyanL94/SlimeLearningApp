using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestController : MonoBehaviour
{
    [Header("Test Setup")]
    [SerializeField]
    protected GameObject testSetup;
    [SerializeField]
    protected GameObject testPanel;
    [SerializeField]
    protected InputField numberOfQuestions;
    [SerializeField]
    protected Dropdown testType;
    [SerializeField]
    protected Toggle furiganaToggle;

    private string testing;
    private int questions = 0;
    private bool includeFurigana;

    [Header("Test Ongoing")]
    [SerializeField]
    protected Text questionText;
    [SerializeField]
    protected Text prompt;
    [SerializeField]
    protected Text furigana;
    [SerializeField]
    protected Text answer;
    [SerializeField]
    protected Text questionNum;
    [SerializeField]
    protected Text questionTotal;

    private List<GameObject> testingWords = new List<GameObject>();
    private int questionNumber = 0;

    public void StartTest()
    {
        GetTestWords();

        if (testType.value == 0)
        {
            testing = "fromKanji";
        }
        else if (testType.value == 1)
        {
            testing = "fromEnglish";
        }
        else if (testType.value == 2)
        {
            testing = "writeKanji";
        }

        if (numberOfQuestions.text == "")
        {
            questions = 0;
        }
        else
        {
            questions = Int32.Parse(numberOfQuestions.text);
            numberOfQuestions.text = "";
        }
        
        if (furiganaToggle.isOn == true)
        {
            includeFurigana = true;
        }
        else
        {
            includeFurigana = false;
        }

        if (questions == 0 || questions > testingWords.Count)
        {
            return;
        }

         testType.value = 0;
         numberOfQuestions.text = "";
         furiganaToggle.isOn = true;
         testSetup.SetActive(false);
         testPanel.SetActive(true);

        for (int i = 0; i < testingWords.Count; i++)
        {
            GameObject temp = testingWords[i];
            int randomIndex = UnityEngine.Random.Range(i, testingWords.Count);
            testingWords[i] = testingWords[randomIndex];
            testingWords[randomIndex] = temp;
        }

        answer.gameObject.SetActive(false);
        questionNumber = 0;
        questionNum.text = questionNumber.ToString();
        questionTotal.text = questions.ToString();
        NextQuestion();
    }

    public void NextQuestion()
    {
        if (questionNumber == questions)
        {
            prompt.text = "DONE";
            furigana.text = "";
            answer.text = "";
            return;
        }

        Text[] wordStats = testingWords[questionNumber].GetComponentsInChildren<Text>();
        if (testing == "fromKanji")
        {
            questionText.text = "Translate the following to English:";
            prompt.text = wordStats[2].text;
            if(includeFurigana == true)
            {
                furigana.text = wordStats[1].text;
            }
            else
            {
                furigana.text = "";
            }
            answer.text = wordStats[0].text;
        }
        else if (testing == "fromEnglish")
        {
            questionText.text = "Translate the following to Japanese:";
            prompt.text = wordStats[0].text;
            furigana.text = "";
            if (includeFurigana == true)
            {
                answer.text = wordStats[2].text + " (" + wordStats[1].text + ")";
            }
            else
            {
                answer.text = wordStats[2].text;
            }
        }
        else if (testing == "writeKanji")
        {
            questionText.text = "Write the following as kanji:";
            prompt.text = wordStats[0].text;
            if (includeFurigana == true)
            {
                furigana.text = wordStats[1].text;
            }
            else
            {
                furigana.text = "";
            }
            answer.text = wordStats[2].text;
        }

        answer.gameObject.SetActive(false);
        questionNumber++;
        questionNum.text = questionNumber.ToString();
    }

    public void RevealAnswer()
    {
        answer.gameObject.SetActive(true);
    }

    public void EndTest()
    {
        testPanel.SetActive(false);
        ToggleButtons();
    }

    private void GetTestWords()
    {
        GameObject[] allWords = GameObject.FindGameObjectsWithTag("Word");

        for (int i = 0; i<allWords.Length; i++)
        {
            Toggle testing = allWords[i].GetComponentInChildren<Toggle>();
            if (testing.isOn == true)
            {
                testingWords.Add(allWords[i]);
            }
        }
    }

    public void OpenTestSetup()
    {
        testSetup.SetActive(true);
        ToggleButtons();
    }

    public void CloseTestSetup()
    {
        testSetup.SetActive(false);
        ToggleButtons();
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
