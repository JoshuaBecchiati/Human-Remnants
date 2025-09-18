using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // la barra guarda sempre la camera
        transform.LookAt(transform.position + _mainCam.transform.forward);
    }
}