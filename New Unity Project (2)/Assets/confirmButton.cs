﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class confirmButton : MonoBehaviour
{
    public Button btn;
    public List<GameObject> pokemonSelectors = new List<GameObject> {};
    private List<Pokemon> newParty = new List<Pokemon> { };
    public GameObject party;

    /*
     * Dit script is verantwoordelijk om alle moves uit de moveselectors in een lijst te stoppen en aan party te geven.
     */

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(taskOnClick);//make sure the task on click is called once the button is pressed
        party = GameObject.FindObjectOfType<party>().gameObject; //finds the party object
    }

    //go past all the pokemon selectors and record the pokemon selected.
    void taskOnClick()
    {
        newParty.Clear();
        foreach (GameObject pokemonSelector in pokemonSelectors) 
        {
            newParty.Add((pokemonSelector.GetComponent("PokemonSelector") as PokemonSelector).pokemon);//get the pokemon and put it in the list
        }
        party.SendMessage("IchooseThese", newParty); //send the list to the party object
    }
}
