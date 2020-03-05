using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class credit : MonoBehaviour
{
    public List<string> textLines = new List<string> { };
    public ScrollRect sr;
    public float speed = 50f;
    public GameObject textLinePrefab;
    public Transform content;

    private GameObject textToEdit;

    /*
     * Dit script laat een scrollrect loopen. Het zet ook alle lines in de content van die scrollrect. Het word gebruikt in de eindscene
     * voor de credits
     */

    private void Start()
    {
        foreach (string line in textLines) 
        {
            textToEdit = Instantiate(textLinePrefab,content); //maak een nieuw object aan
            textToEdit.GetComponent<Text>().text = line; //benader het textcomponent van dat object
        }
    }

    private void FixedUpdate()
    {
        sr.velocity = new Vector2(0f,speed); //zet de snelheid van de scrollrect
        if (sr.verticalNormalizedPosition < 0) //als de scrollrect beneden is , teleporteer hem weer omhoog
        {
            sr.verticalNormalizedPosition = 1.1f;
        }
    }
}
