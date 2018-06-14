using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openDoor : MonoBehaviour
{
    [SerializeField]
    public Vector3 dPos;

    private Vector3 initPos;

    private Vector3 endPos;

    bool isInteractive = false;

    bool isOpen = false;

    bool stillOpen = false;

    public void notifyInteractive()
    {
        isInteractive = true;
    }

    void Start()
    {
        initPos = transform.localPosition;
        endPos = transform.localPosition - dPos;
    }

    void Update()
    {
        if (isInteractive)
        {
            if (!stillOpen)
            {
                stillOpen = true;
                if (isOpen)
                    StartCoroutine(opendoor());
                else
                    StartCoroutine(closedoor());
            }
        }
    }

    IEnumerator opendoor()
    {
        float time = 0.0f;
        float total = 0.8f;
        while (time < total)
        {
            transform.localPosition = Vector3.Lerp(initPos, endPos, time / total);
            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        transform.localPosition = endPos;
        isOpen = !isOpen;
        stillOpen = false;
        isInteractive = false;
    }

    IEnumerator closedoor()
    {
        float time = 0.0f;
        float total = 0.8f;
        while (time < total)
        {
            transform.localPosition = Vector3.Lerp(endPos, initPos, time / total);
            time += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        transform.localPosition = initPos;
        isOpen = !isOpen;
        stillOpen = false;
        isInteractive = false;
    }


    public void openTheDoor()
    {
        if (!stillOpen)
        {
            stillOpen = true;
            StartCoroutine(opendoor());
        }
    }

    public void closeTheDoor()
    {
        if (!stillOpen)
        {
            stillOpen = true;
            StartCoroutine(closedoor());
        }
    }
}
