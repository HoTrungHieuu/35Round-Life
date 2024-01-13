using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentForGameplay
{
    public string Name { get; set; }
    public int team { get; set; }
    public int position { get; set; }
    public List<int> PassiveActiveTime { get; set; }
}
