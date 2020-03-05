using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Battle_Script : MonoBehaviour
{


    [Header("Sprites")]
    public Image Ourpoksprite;
    public Image Enemypoksprite;

    [Header("voor later")]
    public displayBar dbOurpok;
    public displayBar dbEnemypok;

    [Header("FBPR")]
    public Button FightButton;
    public Button BagButton;
    public Button PokemonButton;
    public Button RunButton;

    [Header("Moves")]
    public Button move0b;
    public Button move1b;
    public Button move2b;
    public Button move3b;
    public Button backbutton;
    public Text move1;
    public Text move2;
    public Text move3;
    public Text move4;


    [Header("pokUI")]
    public Text Ourpokname;
    public Text Enemypokname;
    public Text OurPokMaxHp;
    public Text ourPokHp;

    // pp toevoegen
    [Header("misc")]
    public Text Eventtekst;
    public party pokemonparty;

    public battlesceneHider longmoves;
    public battlesceneHider fbpr;
    public move incapacitatedMove;

    [Header("swapPokemon")]
    public showHide pokSlcShowHide;
    public Button pokSlcBack;

    [Header("not for editor")]
    public Pokemon ourpok;
    public int ourpokIndex;
    public Pokemon enemypok;
    public int enemyPokIndex;
    public List<int> enemyHps = new List<int> { };

    //results for damage calculation functions
    private float effectiveness;
    private float stabResult;
    private float atDefResult;
    private float damage;

    //variables for storing the attacks in order
    private bool playerIsFirst;
    private move enemyMoveWhenItem;
    private bool playerHasFreeAction = false;
    private bool playerWon = false;
    private bool denyFreeAction;

    /*
     * dit is verreweg het grootste script uit project pokemon. Het handelt de battlescene, de meest complexe scene bijna helemaal
     * alleen. Hij is verantwoordelijk voor alle UI (daarom staan hierboven heel veel texts en images), hij is verantwoordelijk voor
     * het berekenen van de damage en het kijken of de pokemon al dood is, en het is verantwoordelijk voor het bijhouden van de
     * tegenstanders variabelen.
     */



    void Start()
    {
        //toevoegen van de pokemonparty
        pokemonparty = (GameObject.FindWithTag("party").GetComponent<party>() as party);

        // als of fight, dan fight - hide en long moves - show.

        //fightbutton - als je klikt gaat figntbuttonfunction aan.
        FightButton.onClick.AddListener(Fightbuttonfunction);
        //longmoves (panel) staat uit. 
        longmoves.changeVisibility(false);
        //backbutton - als je klikt gaat backbutotnfunction aan.

        //zorg dat de goede functies worden uitgevoerd op een buttonpress
        backbutton.onClick.AddListener(Backbuttonfunction);
        BagButton.onClick.AddListener(bag);
        PokemonButton.onClick.AddListener(pokemonButtonFunction);
        pokSlcBack.onClick.AddListener(pokSlcBackBtnFunction);
        initiateMoveButtons();

        //als je naar inventory gaat worden alle variabelen hierin verwijderd, in party niet.
        //dus daarom kijken wij of we het gevecht niet opnieuw moeten oppakken
        if (pokemonparty.inBattle)
        {
            //resuming the battle
            enemyHps = pokemonparty.EnemyHps;
            enemyPokIndex = pokemonparty.enemyPokemonIndex;
            enemypok = pokemonparty.trainer.pokemons[enemyPokIndex];


            ourpokIndex = pokemonparty.currentPokemonIndex;
            ourpok = pokemonparty.pokemons[ourpokIndex];
        }
        else 
        {
            //making a new battle
            pokemonparty.inBattle = true; //zorg dat party weet dat wij in battle zijn
            ourpok = pokemonparty.pokemons[0];
            ourpokIndex = 0; 
            enemypok = pokemonparty.trainer.pokemons[0];
            enemyPokIndex = 0;

            //zorg dat wij de hp lijst van de tegenstander volle hps heeft
            foreach (Pokemon pokemon in pokemonparty.trainer.pokemons)
            {
                enemyHps.Add(pokemon.HP);
            }
            pokemonparty.resetMusic(); //update de muziek
        }

        Eventtekst.text = "What will " + ourpok.name + " do?";
        //aanroepen van updateUI
        updateUI();
        checkForFaint();
    }

    //deze functie word geroepen door een knop
    void Fightbuttonfunction()
    {
        longmoves.changeVisibility(true); //moves worden zichtbaar
        fbpr.changeVisibility(false); //fight button run en bag onzichtbaar
    }

    //deze functie word gebruikt om terug te gaan als je de fight al hebt ingedrukt
    void Backbuttonfunction()
    {
        longmoves.changeVisibility(false); 
        fbpr.changeVisibility(true);
    }
    void pokemonButtonFunction()
    {
        pokSlcShowHide.show(); //laat de keuze van andere pokemons zien
    }
    void pokSlcBackBtnFunction()
    {
        pokSlcShowHide.hide(); //hide de keuze van andere pokemons pokemon
    }

    void updateUI()
    {
        //de goede sprite in de Image component zetten
        Ourpoksprite.sprite = ourpok.sprite;

        //de goede sprite in de Image component zetten
        Enemypoksprite.sprite = enemypok.sprite;

        //pokemonnamen toevoegen
        Ourpokname.text = ourpok.name;
        Enemypokname.text = enemypok.name;

        //veranderen 'move #' naar *movenaam*
        move1.text = pokemonparty.moveSets[ourpokIndex][0].name;
        move2.text = pokemonparty.moveSets[ourpokIndex][1].name; ;
        move3.text = pokemonparty.moveSets[ourpokIndex][2].name; ;
        move4.text = pokemonparty.moveSets[ourpokIndex][3].name; ;

        // ourpok hp
        OurPokMaxHp.text = ourpok.HP.ToString();
        ourPokHp.text = pokemonparty.HPs[ourpokIndex].ToString();
        dbOurpok.value = (float)pokemonparty.HPs[ourpokIndex] / (float)ourpok.HP;
        try //this needs a try statement because it will throw an error once the enemypokindex is higher then the list. which
        {   //happens when the player wins.
            dbEnemypok.value = (float)enemyHps[enemyPokIndex] / (float)enemypok.HP;
        }
        catch (System.Exception)
        {

        }


    }

    //coroutines duren langer dan 1, frame dit is een coroutine. deze coroutine word geroepen door een van de move knoppen
    IEnumerator Startbattleround(move attackOur, move attackEnemy)
    {
        longmoves.changeVisibility(false); //alles invisible
        fbpr.changeVisibility(false);
        Debug.Log("enemyPok used: " + attackEnemy.name); //eigenlijk python print functie
        Debug.Log("ourPok used: " + attackOur.name);

        //de pokemon met de hoogste snelheid gaat eerst, als gelijk dan wint de speler
        if (ourpok.Speed >= enemypok.Speed)
        {
            playerIsFirst = true; 
        }
        else
        {
            playerIsFirst = false;
        }

        yield return new WaitForSeconds(2); //hier wachten wij 1 seconden voordat het script word hervat
        //move 1
        displayAttack(attackEnemy, attackOur, true); //laat de aanval op het schermkomen

        yield return new WaitForSeconds(2);
        useMove(attackOur, attackEnemy, true); //berekent de damage van de move en trekt het van het hp af.
        updateUI();

        yield return new WaitForSeconds(2);
        displayAttack(attackEnemy, attackOur, false); //de andere move

        yield return new WaitForSeconds(2);
        //battleselection
        useMove(attackOur, attackEnemy, false);
        updateUI();
        yield return new WaitForSeconds(2);


        //eventtekst
        //hit. wacht 1 tel, 2e move
        longmoves.changeVisibility(false); //alles weer klaarmaken voor de speler om opnieuw te gebruiken
        fbpr.changeVisibility(true);
        Eventtekst.text = "What will " + ourpok.name + " do?";
        updateUI();
        checkForFaint(); //kijkt naar de pokemons hp of ze dood zijn
    }

    //word geroepen als je op bag clickt
    void bag()
    {
        pokemonparty.accesInventory(); //zie party.cs voor meer info
    }
    void endBattle() //de speler heeft gewonnen
    {
        pokemonparty.inBattle = false; //laat de party dat weten
        pokemonparty.trainersDefeated.Add(pokemonparty.trainer); 
        pokemonparty.endBattle();
    }
    void initiateMoveButtons()
    {
        move0b.onClick.AddListener(() => chooseMove(0)); //dit zorgt ervoor dat de button weet welke index hij aan zijn functie mee
        move1b.onClick.AddListener(() => chooseMove(1)); //moet geven
        move2b.onClick.AddListener(() => chooseMove(2));
        move3b.onClick.AddListener(() => chooseMove(3));
    }

    //word geroepen door een move button
    void chooseMove(int moveIndex)
    {
        if (playerHasFreeAction&&denyFreeAction == false) //als de speler een vrije move heeft
        {
            StartCoroutine(Startbattleround(pokemonparty.moveSets[ourpokIndex][moveIndex], incapacitatedMove));
        }
        else
        {
            StartCoroutine(Startbattleround(pokemonparty.moveSets[ourpokIndex][moveIndex], enemypok.moves[(int)Mathf.Round(Random.Range(0f, 3.4f))]));
        }
        playerHasFreeAction = false; //reset de vrije move
        denyFreeAction = false;
    }

    //kijkt hoe effectief de move is, in het kader van, water is goed tegen vuur etc.
    float checkEffectiveness(move usedMove, Pokemon Defender)
    {
        effectiveness = 1f;
        foreach (Type type in Defender.types)
        {
            foreach (Type resistance in type.resistances)
            {
                if (resistance == usedMove.type)
                {
                    //move is not very effective
                    effectiveness = effectiveness * 0.5f;
                }
            }
            foreach (Type weakness in type.vunerabilities)
            {
                if (weakness == usedMove.type)
                {
                    //move is super effective
                    effectiveness = effectiveness * 2f;
                }
            }
            foreach (Type immunity in type.immunities)
            {
                if (immunity == usedMove.type)
                {
                    //move has no effect
                    effectiveness = 0f;
                }
            }
        }
        DisplayEffectiveness(effectiveness); //zorg dat de speler dit te zien krijgt
        return effectiveness;
    }

    
    void DisplayEffectiveness(float effectiveness)
    {
        if (effectiveness > 1f)
        {
            Eventtekst.text = "It's super effective!";
        }
        else if (effectiveness < 1f)
        {
            Eventtekst.text = "It's not very effective...";
        }
        else if (effectiveness == 1f)
        {
            Eventtekst.text = "It hit!";
        }
    }

    //stab is een term voor pokemonspelers het staat voor same type attack bonus en geeft een extra boost aan de kracht van de aanval als
    //er bepaalde eisen worden voldaan.
    float stab(Pokemon defender, Pokemon attacker, move Move)
    {
        stabResult = 1f;

        if (attacker.Attack >= attacker.SpAttack || Move.isSpecial == false || attacker.types.Contains(Move.type))
        {
            stabResult = 1.5f;
        }
        else if (attacker.SpAttack >= attacker.Attack || Move.isSpecial == true || attacker.types.Contains(Move.type))
        {
            stabResult = 1.5f;
        }
        return stabResult;
    }

    //atDef is een term voor pokemon fans om uit een bepaalde game mechanic uit te drukken. Er komt namelijk een bonus als de situatie
    //aan bepaalde eisen worden voldaan. 
    float atDef(Pokemon defender, Pokemon attacker, move Move)
    {
        if (Move.isSpecial == false)
        {
            atDefResult = (float)attacker.Attack / (float)defender.Defense;
        }
        else
        {
            atDefResult = (float)attacker.SpAttack / (float)defender.SpDefense;
        }
        return atDefResult;
    }

    //voegt alle damage bij elkaar en returnt die waarde
    float CalcDamage(Pokemon defender, Pokemon attacker, move Move)
    {
        damage = atDef(defender, attacker, Move) * stab(defender, attacker, Move) * checkEffectiveness(Move, defender) * Move.power * 0.25f;
        return damage;
    }

    
    void useMove(move attackOur, move attackEnemy, bool isFirstMove)
    { //all the if statements are for checking which pokemon goes first
        if (isFirstMove) 
        {
            if (playerIsFirst)
            {
                //use player move
                //hp = hp - damage and damage is rounded to an int, because pokemon can only have an rounded number in hp.
                enemyHps[enemyPokIndex] = enemyHps[enemyPokIndex] - (int)Mathf.Round(CalcDamage(enemypok, ourpok, attackOur));
            }
            else
            {
                //use enemy move. just the same as the one a few lines above. But reversed, so the player is defending and the enemy attacking.
                pokemonparty.HPs[ourpokIndex] = pokemonparty.HPs[ourpokIndex] - (int)Mathf.Round(CalcDamage(ourpok, enemypok, attackEnemy));
            }
        }
        else 
        {
            if (playerIsFirst == false)
            {
                enemyHps[enemyPokIndex] = enemyHps[enemyPokIndex] - (int)Mathf.Round(CalcDamage(enemypok, ourpok, attackOur));
            }
            else
            {
                pokemonparty.HPs[ourpokIndex] = pokemonparty.HPs[ourpokIndex] - (int)Mathf.Round(CalcDamage(ourpok, enemypok, attackEnemy));
            }
        }
        checkForFaint();
    }

    //laat de aanval op het scherm komen
    void displayAttack(move attackEnemy, move attackOur, bool isFirstMove)
    {
        if (playerIsFirst == isFirstMove)
        {
            Eventtekst.text = ourpok.name + " used " + attackOur.name;
        }
        else
        {
            Eventtekst.text = enemypok.name + " used " + attackEnemy.name;
        }
    }

    //word aangeroepen als er bijvoorbeeld een pokemon dood is gegaan
    void interruptBattleRound()
    {
        longmoves.changeVisibility(false);
        fbpr.changeVisibility(true);
        Eventtekst.text = "What will " + ourpok.name + " do?";
        updateUI();
        StopAllCoroutines(); //zorgt dat de normale battlecycle onderbroken word
    }

    //word geroepen door een knop in het swap pokemon menuutje
    public void switchPokemon(int newIndex)
    {
        pokSlcBack.interactable = true; //zorgt dat het terugknop weer werkt nadat hij is uitgeschakelt doordat de pokemon dood is ofzo
        ourpokIndex = newIndex;
        ourpok = pokemonparty.pokemons[newIndex];
        pokSlcShowHide.hide();
        updateUI();
        longmoves.changeVisibility(false); 
        fbpr.changeVisibility(false);

        Eventtekst.text = ourpok.name + " I choose you!";
        if (denyFreeAction == false) //als jij geforceerd pokemon wisselt dan krijgt de tegenstander geen vrije move, anders wel
        {
            StartCoroutine(enemyFreeAction());
        }
        else
        {
            fbpr.changeVisibility(true); 
        }
    }

    //zorgt dat de tegenstander een vrije move krijgt als jij een item gebruikt en zet het op het scherm
    public void spendItem(Item item)
    {
        longmoves.changeVisibility(false);
        fbpr.changeVisibility(false);

        Eventtekst.text = "you used a " + item.name;
        StartCoroutine(enemyFreeAction());
    }


    public IEnumerator enemyFreeAction()
    {
        yield return new WaitForEndOfFrame(); //zorgt dat alles goed is ingeladen, een bug blijkbaar als dit niet is
        if (playerHasFreeAction && denyFreeAction == false) //als de speler een vrije move krijgt, word de tegenstander incapacitated
        {
            enemyMoveWhenItem = incapacitatedMove;
            playerHasFreeAction = false;
        }
        else
        {
            enemyMoveWhenItem = enemypok.moves[(int)Mathf.Round(Random.Range(0f, 3.4f))];
        }
        playerHasFreeAction = false;
        
        if (denyFreeAction) //checkt of de tegenstander wel een vrije move heeft
        {
            interruptBattleRound();
            checkForFaint();
            StopCoroutine(enemyFreeAction());
        }
        denyFreeAction = false;
        Eventtekst.text = enemypok.name + " used " + enemyMoveWhenItem.name;
        yield return new WaitForSeconds(2);
        DisplayEffectiveness(checkEffectiveness(enemyMoveWhenItem, ourpok));
        pokemonparty.HPs[ourpokIndex] = pokemonparty.HPs[ourpokIndex] - (int)Mathf.Round(CalcDamage(ourpok, enemypok, enemyMoveWhenItem));
        checkForFaint();
        yield return new WaitForSeconds(2);
        interruptBattleRound();
        checkForFaint();
    }


    void checkForFaint() //kijkt naar dode pokemons
    {
        if (enemyHps[enemyPokIndex] <= 0)
        { //enemy fainted
            Debug.Log("enemy pokemon fainted");
            interruptBattleRound();
            enemyHps[enemyPokIndex] = 0; //make sure that the hp is not negative

            enemyPokIndex += 1; //make sure that the enemy swaps to the next pokemon

            if (enemyPokIndex > pokemonparty.trainer.pokemons.Count - 1) //checks if the enemy ran out of pokemon
            {
                pokemonparty.endBattle(); //player wins
                return;
            }

            enemypok = pokemonparty.trainer.pokemons[enemyPokIndex]; //swaps to the new pokemon
            updateUI();
            denyFreeAction = true;
        }
        else if (pokemonparty.HPs[ourpokIndex] <= 0)
        { //onze pokemon faint
            Debug.Log("our pokemon fainted");
            interruptBattleRound(); 
            pokemonparty.HPs[ourpokIndex] = 0; 
            pokemonparty.fainted[ourpokIndex] = false; //laat de party weten dat hij dood is
            pokemonButtonFunction();
            pokSlcBack.interactable = false; 
            fbpr.changeVisibility(false);
            denyFreeAction = true; //zorg dat de tegenstander geen vrije move krijgt omdat dit een geforceerde swap is
        }
        checkforWinnings(); 
    }
    
    //kijkt of iemand gewonnen heeft
    void checkforWinnings()
    {
        if (pokemonparty.fainted.Contains(true) == false) //als er nog een levende pokemon is
        {
            pokemonparty.lose(); //game over
        }
        else
        {
            playerWon = true; //bewijst het tegendeel hierna
            foreach (int hp in enemyHps)
            {
                if (hp > 0)
                {
                    playerWon = false; //kijkt dus of er nog een levende pokemon is
                }
            }
            if (playerWon)
            {
                pokemonparty.endBattle(); //laat de player winnen
            }
        }
    }
}

