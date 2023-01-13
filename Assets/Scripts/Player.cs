using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player
{
    private BlockBehaviour blockSelected;
    public BlockBehaviour BlockSelected { get { return blockSelected; } set { blockSelected = value; } }
}