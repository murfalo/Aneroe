using System;
using System.Collections.Generic;

[Serializable]
public class StatInfo
{
    public static Dictionary<string, float> defaultLevels = new Dictionary<string, float>
    {
        {"health", 3},
        {"speed", 1},
        {"attack", 1},
        {"defense", 1}
    };

    private readonly Dictionary<string, float> statLevels;

    public StatInfo()
    {
        statLevels = new Dictionary<string, float>();
        foreach (var pair in defaultLevels) statLevels.Add(pair.Key, pair.Value);
    }

    public StatInfo(Dictionary<string, float> stats)
    {
        statLevels = stats;
    }

    public void ModifyStatsByFactor(float factor)
    {
        var keys = new List<string>(statLevels.Keys);
        foreach (var key in keys) statLevels[key] *= factor;
    }

    // Raises or lowers stat by amount
    public void ChangeStat(string stat, float amount)
    {
        statLevels[stat] += amount;
    }

    public void SetStat(string stat, float amount)
    {
        statLevels[stat] = amount;
    }

    public float GetStat(string stat)
    {
        return statLevels[stat];
    }

    public Dictionary<string, float> GetStats()
    {
        return statLevels;
    }
}
