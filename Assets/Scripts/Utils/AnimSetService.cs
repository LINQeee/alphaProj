using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimSetService : MonoBehaviour
{
    public static void AnimSetTypeUniqueCheck(List<AnimSet> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            for (var j = 0; j < list.Count; j++)
            {
                if (i == j || list[i].animSetType != list[j].animSetType) continue;
                Debug.LogError("animSetType field must be unique");
                return;
            }
        }
    }

    public static AnimSet GetAnimSetByType(IEnumerable<AnimSet> list, AnimSet.AnimSetTypes type)
    {
        return list.FirstOrDefault(animSet => animSet.animSetType == type);
    }
}