using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour
{
    public Image image;
    public float fadingTime;
    public bool iswhite;
    private float rate;
    private float i = 0f;

    /*
     * this is a class which has been made AFTER the deadline no technical documentation available.
     * this clas is used to fade in and out of a scene. it uses image components alpha with a black image to fade.
     */

    public IEnumerator Fade(float time)
    {
        
        rate = 1 / time;
        if (iswhite) //if the screen is clear, we make it black, the current alpha is 0
        { 
            iswhite = false;
            i = 0f;
        }
        else //if its black, we fade it to white by subtracting the rate in each step, and the current alpha is 1
        {
            iswhite = true;
            rate = rate * -1f;
            i = 1f;
        }

        while (0 <= i && i <= 1) //it may never go above or below 1
        {
            i += rate * Time.deltaTime; 
            image.color = new Color(0f, 0f, 0f, i);
            yield return new WaitForEndOfFrame(); //waits until the end of the frame
        }
    }
    public void startFading(bool towhite) //an easy way to use this script from other scripts
    {
        StartCoroutine(Fade(2f));
    }
    private void Start() //make sure it fades in on start
    {
        iswhite = false;
        image.color = new Color(0f, 0f, 0f, 255f);
        startFading(true);
    }
    public IEnumerator fadeAndLoadScene(string scenePath) //load scene after the fade
    {
        yield return Fade(fadingTime);
        SceneManager.LoadScene(scenePath);
    }
}

