using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
    public Transform player;
    public Transform movePoint;

    private party Party;
    private Scene currentScene;
    private Vector3 playerPos;

    /*
     * this script is responsible for the opening of the inventory on the click of the inventory button in mobile mode,
     * or the TAB button in desktop / macOS mode. It is also used to store the position before switching scenes to 
     * make sure that the player doesnt teleport after the fight.
     */

    private void Start()
    {
        Party = GameObject.FindObjectOfType<party>().GetComponent<party>();
        if (Party.usePlayerPosOnLoad)
        {
            player.position = Party.playerPosInMap;
            movePoint.position = Party.playerPosInMap;
        }
    }
    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            openInventory();
        }
    }
    public void openInventory()
    {
        playerPos = movePoint.position;
        Party.playerPosInMap = playerPos;
        Party.usePlayerPosOnLoad = true;
        Party.accesInventory();
    }
}
