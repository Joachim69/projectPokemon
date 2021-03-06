﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moveCollector : MonoBehaviour
{
    public List<snappable> chosenSlotList = new List<snappable> { };
    public List<move> moves = new List<move> { };
    public GameObject moveAdder;
    public Image pokemonIMG;
    public int key;
    public string errorMsgIfNoMoveAssigned = "please choose all the moves for your pokemons, all slots must be filled";
    public GameObject statDisplayerPrefab;
    public GameObject statDisplayerParent;
    public List<string> stats = new List<string> { };
    public Pokemon pokemonOfObject;
    
    /*
     * this script is responsible for collecting the moves once the confirm button is pressed and setting the pokemon at the start of the
     * scene runtime. This means setting the stats to the right value and the picture to the right sprite.
     */

    private move moveToAdd;
    private List<GameObject> statDisplays = new List<GameObject> { };
    private int count = 0;

    public List<move> getMoves()
    {
        moves.Clear();
        foreach (snappable slot in chosenSlotList)
        {
            try
            { // getting move, in a try statement because it gives an error if there is no move in the slot.
                moveToAdd = (slot.objectInSlot.GetComponent("selectMove") as selectMove).Move;
            }
            catch
            { //if there is no move in the slot, notify the player that not all moves are assigned.
                GameObject.FindGameObjectWithTag("popUpWindow").SendMessage("notification", errorMsgIfNoMoveAssigned);
            }
            moves.Add(moveToAdd);
            Debug.Log(moveToAdd);
        }
        return moves;
    }

    //this is called when this object is created
    public void setPokemon(Pokemon pokemon, int newKey)
    {
        pokemonOfObject = pokemon;
        key = newKey;
        moveAdder.SendMessage("addKey", key); //give the moveAdder the key
        moveAdder.SendMessage("makeMoves", pokemon); //give the move adder the pokemon
        pokemonIMG.sprite = pokemon.sprite;

        //give the keys to the slots which are not in the scrollable list
        foreach (snappable chosenSlot in chosenSlotList)
        {
            chosenSlot.addKey( key);
        }

        //update the stat displayers
        statDisplays.Clear();
        foreach (string stat in stats)
        {
            statDisplays.Add(Instantiate(statDisplayerPrefab, statDisplayerParent.transform));
            statDisplays[statDisplays.Count-1].SendMessage("setStat", stat); //set the text
            statDisplays[statDisplays.Count - 1].SendMessage("setValue", (int)pokemon.GetType().GetField(stat).GetValue(pokemon)); //set the value
            //https://forum.unity.com/threads/access-variable-by-string-name.42487/ the last variable in the send message is from this forum.
        }
        
    }
    public void defaultMoves()
    {
        count = 0;
        foreach (snappable slot in chosenSlotList)
        {
            slot.occupyWith(pokemonOfObject.moves[count]);
            count += 1;
        }
    }
}
