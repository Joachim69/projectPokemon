using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class confirmMoves : MonoBehaviour
{

    public GameObject moveListPrefab;
    public GameObject moveListParent;
    public Button confirmButton;
    

    private List<GameObject> moveLists = new List<GameObject> { };
    private GameObject prefabToAdd;
    private List<List<move>> movesOfPokemons = new List<List<move>> { };
    

    //for debugging
    public List<Pokemon> testParty = new List<Pokemon> { };
    public bool useTestParty;
    public bool enableDebug;
    private string debugPackage;
    private int count = 0;

    /*
     * this script is responsible for the creation of all the move lists. This includes making sure that they all have the right
     * pokemon selected. there is a big part of the script that is just there for debugging.
    */

    void Start()
    {
        //for debugging only
        if (useTestParty)
        {
            count = 0;
            foreach (Pokemon pokemon in testParty)
            {
                prefabToAdd = Instantiate(moveListPrefab, moveListParent.transform);
                (prefabToAdd.GetComponent("moveCollector") as moveCollector).setPokemon(pokemon, count);
                moveLists.Add(prefabToAdd);
                count += 1;
            }
        }
        //for the real deal
        else
        {
            count = 0;
            foreach (Pokemon pokemon in (GameObject.FindGameObjectWithTag("party").GetComponent("party") as party).pokemons) //get all the chosen pokemon and loop through it
            {
                prefabToAdd = Instantiate(moveListPrefab, moveListParent.transform); //build all the list objects and set the parent to movelistParent
                (prefabToAdd.GetComponent("moveCollector") as moveCollector).setPokemon(pokemon, count); //make sure that list has the right pokemon
                moveLists.Add(prefabToAdd); 
                count += 1;
            }
        }
        confirmButton.onClick.AddListener(collectMoves); //makes sure that the next comment is made reality

        foreach (ScrollRect scrollRect in FindObjectsOfType<ScrollRect>())
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    //this method is called once the confirmbutton is pressed
    void collectMoves()
    {
        movesOfPokemons.Clear();
        foreach (GameObject moveList in moveLists)
        {
            movesOfPokemons.Add((moveList.GetComponent("moveCollector") as moveCollector).getMoves()); //get the moves from all the move slots and put it in to a list
        }
        movesOfPokemons[0] = moveLists[0].GetComponent<moveCollector>().getMoves();

        //for debugging only
        if (enableDebug)
        {
            count = 0;
            foreach (List<move> moveSet in movesOfPokemons)
            {
                debugPackage = "Pokemon: "+count.ToString()+" moveSet: ";
                foreach (move Move in moveSet)
                {
                    debugPackage += Move.name+ ", ";
                }
                Debug.Log(debugPackage);
                count += 1;
            }
        }
        GameObject.FindGameObjectWithTag("party").GetComponent<party>().setMoves(movesOfPokemons);
    }
}
