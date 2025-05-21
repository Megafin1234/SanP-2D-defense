using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SKP_ITEM
{
    public int id;
    public string name;
    public string desc;
    public List<string> attributes;
    public int maxStack;
    public SKP_ITEM(string name, string desc, int maxStack = 64)
    {
        this.name = name;
        this.desc = desc;
        this.attributes = new List<string>();
        this.maxStack = maxStack;
    }
    public override bool Equals(object obj)
    {
        if (obj is not SKP_ITEM other) return false;
        return id == other.id &&
               name == other.name &&
               desc == other.desc &&
               maxStack == other.maxStack &&
               attributes.SequenceEqual(other.attributes);
    }
}

public class SKP_HUD : MonoBehaviour
{

    void Start()
    {

    }
    void Update()
    {

    }

    void OnInv()
    {
        Debug.Log("HIHIHIHIHIHI");
    }
}
