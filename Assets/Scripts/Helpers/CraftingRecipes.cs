using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CraftingRecipes {

    public static Dictionary<string[], string> recipes = new Dictionary<string[], string>(new RecipeEqualityComparer())
    {
        {new[] {"Bone", "IceBerry", "StoneFlask"}, "IcePotion"},
    };

    public static GameObject CraftItem(string item1, string item2, string item3)
    {
        var key = new[] {item1, item2, item3};
        return recipes.ContainsKey(key) ? Resources.Load<GameObject>("Prefabs/Items/" + recipes[key]) : null;
    }
}

public class RecipeEqualityComparer : IEqualityComparer<string[]>
{
    public bool Equals(string[] x, string[] y)
    {
        if (x.Length != 3 || y.Length != 3) return false;
        return !x.Where((t, i) => t != y[i]).Any();
    }

    public int GetHashCode(string[] recipes)
    {
        return recipes.Aggregate(0, (current, r) => unchecked (current + EqualityComparer<string>.Default.GetHashCode(r)));
    }
}
