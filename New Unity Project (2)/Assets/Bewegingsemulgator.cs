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
    //Eigenschap van walkDirection, checkt of er iets staat waar hij niet mag komen, bijvoorbeeld muren of voorwerpen.

    public LayerMask SceneShifter;
    public LayerMask trainers;
    private door closestDoor;
    private trainerInMap closestTrainer;

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

            if (Vector3.Distance(transform.position, walkDirection.position) <= 0.1f)
            {

                if ((Input.GetAxisRaw("Horizontal")) == 1f || (Input.GetAxisRaw("Horizontal") == -1f))
                {
                    if (!Physics2D.OverlapCircle(walkDirection.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 0.05f, blockDetector))
                    {
                        walkDirection.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    }

                    //set up the sprite
                    if (Input.GetAxisRaw("Horizontal") > 0)
                    {
                        //make the sprite move right
                    }
                    else if (Input.GetAxisRaw("Horizontal") < 0)
                    {
                        //make the sprite move left
                    }
                }

                else if ((Input.GetAxisRaw("Vertical")) == 1f || (Input.GetAxisRaw("Vertical") == -1f))
                {
                    if (!Physics2D.OverlapCircle(walkDirection.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 0.05f, blockDetector))
                    {
                        walkDirection.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    }
                    if (Input.GetAxisRaw("Vertical") > 0)
                    {
                        //make the sprite move up
                    }
                    else if (Input.GetAxisRaw("Vertical") < 0)
                    {
                        //make the sprite move down
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
        float getDistance(GameObject target)
        {
            // ((x1-x2)^2+(y1-y2)^2)^0.5 <-- pythagoras formula for the distance 
            return Mathf.Pow(Mathf.Pow(gameObject.transform.position.x - target.transform.position.x, 2) + Mathf.Pow(gameObject.transform.position.y - target.transform.position.y, 2), 0.5f);
        }
    }
}

