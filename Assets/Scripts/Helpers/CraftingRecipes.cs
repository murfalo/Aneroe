using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CraftingRecipes {

    public static Dictionary<string[], string> recipes = new Dictionary<string[], string>(new RecipeEqualityComparer())
    {
        {new[] {"Bone", "IceBerry", "StoneFlask"}, "IcePotion"},
		{new[] {"FirePotion","",""}, "FireElement"},
		{new[] {"Bone","Bone","BloodVial"}, "RefinedBloodVial"},
		{new[] {"IceBerry","FirePotion","StoneFlask"}, "WaterVial"}
    };

    public static GameObject CraftItem(string item1, string item2, string item3)
    {
        var key = new[] {item1, item2, item3};
        if (!recipes.ContainsKey(key)) return null;
        var newItem = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Items/" + recipes[key]));
        newItem.GetComponent<Item>().Setup();
        return newItem;
    }

	public static bool CanCraft(string item1, string item2, string item3) 
	{
		var key = new[] {item1, item2, item3};
		return recipes.ContainsKey (key);
	}
}

public class RecipeEqualityComparer : IEqualityComparer<string[]>
{
    public bool Equals(string[] x, string[] y)
    {
        if (x.Length != 3 || y.Length != 3) return false;
        Array.Sort(x, StringComparer.InvariantCulture);
        Array.Sort(y, StringComparer.InvariantCulture);
        return !x.Where((t, i) => t != y[i]).Any();
    }

    public int GetHashCode(string[] recipes)
    {
        return recipes.Aggregate(0, (current, r) => unchecked (current + EqualityComparer<string>.Default.GetHashCode(r)));
    }
}
