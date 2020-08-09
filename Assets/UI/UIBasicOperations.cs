using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBasicOperations : MonoBehaviour
{
    public void OpenAndClose(GameObject UIelement)
    {
        bool state = UIelement.activeInHierarchy;
        UIelement.SetActive(!state);
    }
}
