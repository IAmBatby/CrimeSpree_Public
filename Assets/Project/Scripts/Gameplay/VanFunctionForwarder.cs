using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanFunctionForwarder : MonoBehaviour
{
    public VanManager vanManager;
    public void VanForwader()
    {
        vanManager.RefreshText();
    }
}
