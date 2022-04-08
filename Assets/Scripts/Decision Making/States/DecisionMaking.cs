using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//we cannot use it only use classes that inherit from it 
public abstract class State : MonoBehaviour
{
    public abstract State runCurrentState();
}
