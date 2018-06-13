using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openDoor : MonoBehaviour {
    [SerializeField]
    public Vector3 dPos;

    bool isInteractive = false;

    bool isOpen = false;

    bool stillOpen = false;

    public void notifyInteractive()
    {
        isInteractive = true;
    }

    void Update()
    {
        if (isInteractive)
        {
            if (!stillOpen)
            {
                StartCoroutine(opendoor());
            }
            isInteractive = false;
        }
    }

    IEnumerator opendoor()
    {
        stillOpen = true;
        Vector3 pos = dPos;
        pos.Scale(new Vector3(0.2f, 0.2f, 0.2f));
        float time = Time.deltaTime * 5;
        while (time > 0)
        {
            if (isOpen)
                transform.position -= pos;
            else
                transform.position += pos;

            time -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        isOpen = !isOpen;
        stillOpen = false;
    }
}
