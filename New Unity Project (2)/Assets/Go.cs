using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Go : MonoBehaviour
    
{
    bool checker = false;
    float speed = 6f;
    bool isnottriggered = true;
    public LayerMask Challanger;
    public Transform stand;
    private Vector2 position;
    
    // Start is called before the first frame update
    void Start()
    {
       
        position = gameObject.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        

        stand.position = new Vector3(23.85f, 71f, 0f);

        if (Physics2D.OverlapCircle(new Vector3(24f, 73f, 0f), 4.04f, Challanger) && isnottriggered == true)
        {
            checker = true;
        }
        if(checker == true)
        { 
            Debug.Log("Hij heeft hem hoor");
            isnottriggered = false;
            transform.position = Vector3.MoveTowards(transform.position, stand.position, speed * Time.deltaTime);
            if(transform.position != stand.position) 
            {
                GameObject player = GameObject.Find("player");
                Bewegingsemulgator bewegingsemulgator = player.GetComponent<Bewegingsemulgator>();
                bewegingsemulgator.freeze = true;
            }
            else if(transform.position == stand.position)
            {
                GameObject player = GameObject.Find("player");
                Bewegingsemulgator bewegingsemulgator = player.GetComponent<Bewegingsemulgator>();
                bewegingsemulgator.freeze = false;
            }
        }
    }
}