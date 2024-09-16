using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Target>() != null)
        {
            other.gameObject.SetActive(false);
            other.transform.localPosition = Vector3.zero;
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}