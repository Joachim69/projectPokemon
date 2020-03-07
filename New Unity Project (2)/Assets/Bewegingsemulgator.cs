using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bewegingsemulgator : MonoBehaviour

{
    public float speed = 5.0f;
    //Snelheid van de speler.

    public Transform walkDirection;
    //Geeft het punt aan waar de player naar gaat lopen.
    public bool freeze = false;
    public LayerMask blockDetector;
    public Sprite moveRight;
    public Sprite forwards;
    public Sprite backwards;
    public SpriteRenderer sr;

    public bool touchOnlyDeviceBuild; //this is a bolean for checking if we are creating the game for a touch only device
                                      //the touch only device code has been added after the deadline (for personal use mostly)
    public Joystick joystick;   //the joystick for touch-only devices downloaded from here:https://assetstore.unity.com/packages/tools/input-management/joystick-pack-107631?aid=1100l355n&gclid=Cj0KCQiAhojzBRC3ARIsAGtNtHXTxBNhcUm1ULeqGJn7ldcK-ohA62wsdeWNLcMUdQKaq5V851UzJ1waAkbEEALw_wcB&pubref=UnityAssets%2ADyn03%2A1723478829%2A67594162255%2A336302044055%2Ag%2A%2A%2Ab%2Ac%2Agclid%3DCj0KCQiAhojzBRC3ARIsAGtNtHXTxBNhcUm1ULeqGJn7ldcK-ohA62wsdeWNLcMUdQKaq5V851UzJ1waAkbEEALw_wcB&utm_source=aff

    [Range(0.0f, 1.0f)] //makes a slider in the editor
    public float joystickThreshold; //a threshold which the joystick has to be pushed past for it to work and the axis to become 1

    //Eigenschap van walkDirection, checkt of er iets staat waar hij niet mag komen, bijvoorbeeld muren of voorwerpen.
    public LayerMask SceneShifter;
    public LayerMask trainers;
    private door closestDoor;
    private trainerInMap closestTrainer;

    private float horizontalInput;
    private float verticalInput;

    void Start()
    //Wat de computer bij het opstarten moet laden / uitvoeren.
    {
        GameObject.FindObjectOfType<party>().resetMusic();
    }


    private void FixedUpdate()
    // Update het programma een aantal keer per seconde

    {
        if (freeze != true)
        {

            transform.position = Vector3.MoveTowards(transform.position, walkDirection.position, speed * Time.deltaTime);
            //Geeft directie en snelheid aan waarmee hij naar het punt gaat waar hij toe gaat lopen, met de snelheid. Transform.position is de positie van de speler, walkDirection.position is het punt waar hij naartoe gaat, en speed * Time.deltaTime zorgt voor de snelheid.

            if (touchOnlyDeviceBuild)
            {
                if (joystick.Horizontal > joystickThreshold)
                {
                    horizontalInput = 1f;
                }
                else if (joystick.Horizontal < -1*joystickThreshold)
                {
                    horizontalInput = -1f;
                }
                else
                {
                    horizontalInput = 0f;
                }

                if (joystick.Vertical > joystickThreshold)
                {
                    verticalInput = 1f;
                }
                else if (joystick.Vertical < -1 * joystickThreshold)
                {
                    Debug.Log("move down");
                    verticalInput = -1f;
                }
                else
                {
                    verticalInput = 0f;
                }
            }
            else
            {
                horizontalInput = Input.GetAxisRaw("Horizontal");
                verticalInput = Input.GetAxisRaw("Vertical");
            }

            if (Vector3.Distance(transform.position, walkDirection.position) <= 0.1f)
            {

                if (horizontalInput == 1f || horizontalInput == -1f)
                {
                    if (!Physics2D.OverlapCircle(walkDirection.position + new Vector3(horizontalInput, 0f, 0f), 0.05f, blockDetector))
                    {
                        walkDirection.position += new Vector3(horizontalInput, 0f, 0f);
                    }
                    else
                    {
                        Debug.Log("Detecting a block");
                    }

                    //set up the sprite
                    if (horizontalInput > 0)
                    {
                        //make the sprite move right
                        sr.sprite = moveRight;
                        sr.flipX = false;
                    }
                    else if (horizontalInput < 0)
                    {
                        //make the sprite move left
                        sr.sprite = moveRight;
                        sr.flipX = true;
                    }
                }

                else if (verticalInput == 1f || verticalInput == -1f)
                {
                    if (!Physics2D.OverlapCircle(walkDirection.position + new Vector3(0f, verticalInput, 0f), 0.05f, blockDetector))
                    {
                        walkDirection.position += new Vector3(0f, verticalInput, 0f);
                    }
                    else
                    {
                        Debug.Log("Detecting a block");
                    }
                    if (verticalInput > 0)
                    {
                        //make the sprite move up
                        sr.sprite = forwards;
                    }
                    else if (verticalInput < 0)
                    {
                        //make the sprite move down
                        sr.sprite = backwards;
                    }
                }

                //check For Doors
                if (Physics2D.OverlapCircle(transform.position, 0.05f, SceneShifter))
                {
                    //omdat wij geen gameobject van de deur kunnen krijgen uit deze if statement, vinden wij hier de dichstbijzijnde deur
                    //en gebruiken die
                    closestDoor = null;
                    foreach (door door in FindObjectsOfType<door>())
                    {
                        //zorgt dat de closestdoor niet null is en er geen exceptions worden gegooid.
                        if (closestDoor == null)
                        {
                            closestDoor = door;
                        }

                        //als de door dichterbij is dan de tot nu closest door, verplaats de closest door dan
                        else if (getDistance(door.gameObject) < getDistance(closestDoor.gameObject))
                        {
                            closestDoor = door;
                        }
                    }
                    if (closestDoor.isOpen())
                    {
                        walkDirection.position = closestDoor.otherSide.position;
                        transform.position = closestDoor.otherSide.position;
                    }
                }

                //checkfortrainers
                if (Physics2D.OverlapCircle(transform.position, 0.05f, trainers))
                {
                    foreach (trainerInMap trainer in FindObjectsOfType<trainerInMap>())
                    {
                        //zorgt dat de closest trainer niet null is en er geen exceptions worden gegooid.
                        if (closestTrainer == null)
                        {
                            closestTrainer = trainer;
                        }

                        else if (getDistance(trainer.gameObject) < getDistance(closestTrainer.gameObject))
                        {
                            closestTrainer = trainer;
                        }
                    }
                    if (closestTrainer.hasBeenBeaten == false)
                    {
                        closestTrainer.battle();
                    }
                    else
                    {
                        Debug.Log("blocking...");
                        //makes sure that the door behaves as a blocker when closed
                        walkDirection.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    }
                }
            }
        }
    }
    float getDistance(GameObject target)
    {
        // ((x1-x2)^2+(y1-y2)^2)^0.5 <-- pythagoras formula for the distance 
        return Mathf.Pow(Mathf.Pow(gameObject.transform.position.x - target.transform.position.x, 2) + Mathf.Pow(gameObject.transform.position.y - target.transform.position.y, 2), 0.5f);
    }
}

