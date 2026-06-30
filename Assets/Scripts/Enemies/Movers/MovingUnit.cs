using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Unit", menuName = "Unit", order = 0)]
public class MovingUnit : ScriptableObject
{
    
    
    public float baseSpeed = 5;
    public float attackSpeed = 2, firstHitCooldown = 0.5f;
    public int damage = 1;
    public int hp = 2;
    public int scoreValue = 5;

    [Header("")]
    public bool flies = false;
    public bool pushable = true;

    public Dictionary<string, float> CostPerType = new Dictionary<string, float>{
     ["S"] = 5.5f,
     ["G"] = 5f,
     ["D"] = 5.5f,
     ["L"] = 10f,
     ["P"] = 3f,
     ["B"] = -1f,
     ["X"] = -1f,
     ["H"] = 7f,
     ["TS"] = 5.2f,
     ["TD"] = 5.2f
    };
    public Dictionary<string, float> CostDecos = new Dictionary<string, float>();



    


    [SerializeField]
    public InspectorItem[] costPerTypeIns = new InspectorItem[] {
        new InspectorItem("S", 5.5f), new InspectorItem("G",5f),
        new InspectorItem("D", 5.5f), new InspectorItem("L",10f),
        new InspectorItem("P",   3f), new InspectorItem("B",-1f),
        new InspectorItem("X",  -1f), new InspectorItem("H",7f),
        new InspectorItem("TS",5.2f), new InspectorItem("TD",5.2f)
    };

    [SerializeField]
    public InspectorItem[] additiveDecos = new InspectorItem[] { new InspectorItem("STUMP", float.MinValue),
                                                                 new InspectorItem("BUSH", 2.5f),
                                                                 new InspectorItem("SUMP", 1f)
                                                               };



    private void OnValidate()
    {
        CostPerType.Clear();
        for(int i = 0; i < costPerTypeIns.Length; i++)
        {
            CostPerType.Add(costPerTypeIns[i].key, costPerTypeIns[i].item);
        }

        CostDecos.Clear();
        for (int i = 0; i < additiveDecos.Length; i++)
        {
            CostDecos.Add(additiveDecos[i].key, additiveDecos[i].item);
        }
    }
}
[Serializable]
public class InspectorItem
{
    [SerializeField]
    public string key;
    [SerializeField]
    public float item;

    public InspectorItem(string k, float i)
    {
        key = k;
        item = i;
    }
}
