using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevator : MonoBehaviour {
    [SerializeField]
    public Vector3 upFactor;

    [SerializeField]
    public GameObject leftdoor;
    public GameObject rightdoor;

    bool isOpen = false;


    protected bool isMoving = false;

    void Start()
    {
    }

    public void goUp()
    {
        if (!isMoving)
        {
            StartCoroutine(goUpImpl());
        }
    }

    IEnumerator goUpImpl()
    {
        if (isOpen)
        {
            closeDoor();
        }
        isMoving = true;

        yield return new WaitForSeconds(1.2f);
        float time = 0.0f;
        float total = 1.0f;

        Vector3 pos = transform.position + upFactor;
        Vector3 init = transform.position;
        while (time < total)
        {
            transform.position = Vector3.Lerp(init, pos, time / total);

            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        transform.position = pos;

        yield return new WaitForSeconds(1.5f);
        isMoving = false;
        openDoor();
    }

    public void goDown()
    {
        if (!isMoving)
        {
            StartCoroutine(goDownImpl());
        }
    }

    IEnumerator goDownImpl()
    {
        if (isOpen)
        {
            closeDoor();
        }
        isMoving = true;

        yield return new WaitForSeconds(1.2f);

        float time = 0.0f;
        float total = 1.0f;

        Vector3 pos = transform.position - upFactor;
        Vector3 init = transform.position;
        while (time < total)
        {
            transform.position = Vector3.Lerp(init, pos, time / total);

            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        transform.position = pos;

        yield return new WaitForSeconds(1.5f);
        isMoving = false;
        openDoor();
    }

    public void openDoor()
    {
        if (!isMoving)
        {
            leftdoor.GetComponent<openDoor>().openTheDoor();
            rightdoor.GetComponent<openDoor>().openTheDoor();
            isOpen = true;
        }
    }

    public void closeDoor()
    {
        if (!isMoving)
        {
            rightdoor.GetComponent<openDoor>().closeTheDoor();
            leftdoor.GetComponent<openDoor>().closeTheDoor();
            isOpen = false;
        }
    }
}
