using UnityEngine;
using System.Collections;

public class CustomScript : MonoBehaviour 
{
    public string myString = "";

    public float timeToWait = 5f;

    public void MyFunction()
    {
        Debug.Log("Called my function");
    }

    public IEnumerator MyCoroutine()
    {
        Debug.Log("Called my coroutine");

        yield return new WaitForSeconds(timeToWait);

        Debug.Log("Coroutine finished");
    }

}
