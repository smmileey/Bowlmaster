using UnityEngine;

public class PinSetterShredder : MonoBehaviour
{
    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Pin>() != null) Destroy(collider.gameObject);
    }
}
