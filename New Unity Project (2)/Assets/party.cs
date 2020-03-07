using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class party : MonoBehaviour
{
    public Trainer trainer;

    [Header("pokemon lists")]
    public List<Pokemon> pokemons = new List<Pokemon> { };
    public List<int> HPs = new List<int> { };
    public List<List<move>> moveSets = new List<List<move>> { };
    public List<bool> fainted = new List<bool> { };
    public List<Item> items = new List<Item> { };
    public List<int> itemAmounts = new List<int> { };

    [Header("debug settings and misc")]
    public bool isTestParty;
    public bool inBattle = false;
    public bool automateMoveSelection = false;

    [Header("Misc")]
    public Trainer endBoss;

    [Header("audio")]
    public AudioSource musicSource;
    public AudioClip mapMusic;

    [Header("not to be assigned in the editor")]
    public int firsPokemonIndex;
    public int currentPokemonIndex;
    public int enemyPokemonIndex;
    public List<int> EnemyHps;
    public List<Trainer> trainersDefeated;
    public Vector3 playerPosInMap;
    public bool usePlayerPosOnLoad = false;
    /*
     * this script is designed to keep track of variables across scenes. the data in this object can be accessed by all scripts in a scene.
     * in start we make sure that this object is immortal, which means that it is not scene bound.
     * this script also acts as a scene manager, this is in charge of loading new scenes.
     * 
     * WARNING: the fader parts of this script are developed after the deadline.
     */

    private Battle_Script battle_Script;
    private List<move> moveSetToAdd = new List<move> { };
    private Item lastUsedItem;



    void Start()
    {
        //sets up the fainted list
        fainted = new List<bool> { };
        for (int i = 0; i < 6; i++)
        {
            fainted.Add(true);
        }

        //if there is another party in the scene, suicide
        if (GameObject.FindGameObjectsWithTag("party").Length > 1)
        {
            Destroy(gameObject);
        }

        if (isTestParty == false) //this is normally the case
        {
            DontDestroyOnLoad(gameObject); //makes sure that this object is immortal
        }

        if (automateMoveSelection) //this is only for debugging
        {
            //make test movesets by just taking the first 4
            foreach (Pokemon pokemon in pokemons)
            {
                moveSetToAdd = new List<move> { };
                for (int i = 0; i < 4; i++)
                {
                    moveSetToAdd.Add(pokemon.moves[i]);
                }
                moveSets.Add(moveSetToAdd);
            }
        }
        resetMusic(); //updates the music
    }
    //this is called by an object in the pokemonselector screen once all the pokemons are chosen
    void IchooseThese(List<Pokemon> newParty)
    {
        pokemons.Clear(); //clears the list
        foreach (Pokemon newPokemon in newParty)
        {
            pokemons.Add(newPokemon); 
            HPs.Add(newPokemon.HP);
        }
         //fader starts
        SceneManager.LoadScene("Scenes/MoveSelector"); //move on to the moveselectorscreen
    }

    //this is called by an object in the moveselector screen once all the moves are chosen.
    public void setMoves(List<List<move>> newMoves)
    {
        moveSets = newMoves;
        
        SceneManager.LoadScene("Scenes/Places/map");
    }

    //this is called once an item is used in the inventory scene
    public void applyItem(Item item, int index)
    {
        HPs[index] = HPs[index] + item.healing;
        if (HPs[index] > pokemons[index].HP)
        {
            HPs[index] = pokemons[index].HP;
        }
        itemAmounts[items.IndexOf(item)] -= 1; //makes the item have 1 less amount

        if (itemAmounts[items.IndexOf(item)] == 0) //if you are out of items
        {
            items.Remove(item);
        }
        exitInventory(item);
    }

    //for exiting the inventory
    public void exitInventory(Item item)
    {
        if (inBattle) //if we opened the inventory out of a battle, we go back to the battle.
        {
            
            SceneManager.LoadScene("Scenes/battle");
            if (item != null) //if the player used an item in the inventory, make sure that the player spends a turn doing that
            {
                lastUsedItem = item;
                SceneManager.sceneLoaded += onSceneLoad;
                //we are adding an action to an event here. The event is triggered once
                //the new scene is loaded, because the objects are not instantiated yet.
                //when using scenemanager.loadscene the scene is loaded the next frame, so we cant do GameObject.Find() which we need.
            }
        }
        else
        {
            SceneManager.LoadScene("Scenes/Places/map");
        }
    }
    //delegete 
    //with help from: https://answers.unity.com/questions/1279397/onscenewasloaded-deprecated-cannot-find-work-aroun.html
    void onSceneLoad(Scene scene, LoadSceneMode mode)
    {
        FindObjectOfType<Battle_Script>().spendItem(lastUsedItem);
        SceneManager.sceneLoaded -= onSceneLoad;
    }

    //for accesing the inventory
    public void accesInventory()
    {
        if (inBattle)
        {
            battle_Script = FindObjectOfType<Battle_Script>().GetComponent<Battle_Script>();
            //get the index of the pokemon now in battle
            currentPokemonIndex = pokemons.IndexOf(battle_Script.ourpok);

            //get the index of the enemies pokemon now in battle
            enemyPokemonIndex = battle_Script.enemyPokIndex;

            //store the hps of the enemy
            EnemyHps = battle_Script.enemyHps;
        }
        
        SceneManager.LoadScene("Scenes/inventory");
    }
    public void startFight(Trainer newTrainer) //moves on to the battle scene and stores the trainer
    {
        trainer = newTrainer;
        
        SceneManager.LoadScene("Scenes/battle");

    }
    public void endBattle() //the player won
    {
        inBattle = false;
        
        SceneManager.LoadScene("Scenes/Places/map"); //go to the mapscene
        trainersDefeated.Add(trainer); //store the trainer defeated
        if (trainer == endBoss) 
        {
            winGame();
        }
    }
    public void lose() //called when all pokemons died
    {
        
        SceneManager.LoadScene("Scenes/titleScreen");
    }
    public void resetMusic() //is called on start of certain scenes
    {
        musicSource.Stop();
        if (inBattle)
        {
            musicSource.clip = trainer.trainerTheme;
        }
        else
        {
            musicSource.clip = mapMusic;
        }
        musicSource.Play();
    }
    void winGame()
    {
        //displays the winscreen
        
        SceneManager.LoadScene("Scenes/winScreen");
        Destroy(gameObject); //destroys object because we dont need it anymore.
    }


}



