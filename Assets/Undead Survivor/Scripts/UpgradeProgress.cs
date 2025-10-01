using System.Collections.Generic;

public static class UpgradeProgress
{
    // key = itemId, value = current level
    private static readonly Dictionary<int, int> levels = new Dictionary<int, int>();

    public static int GetLevel(int itemId)
    {
        if (itemId < 0) return 0;
        return levels.TryGetValue(itemId, out var lv) ? lv : 0;
    }

    public static void SetLevel(int itemId, int level)
    {
        if (itemId < 0) return;
        levels[itemId] = level;
    }

    public static int IncLevel(int itemId)
    {
        var next = GetLevel(itemId) + 1;
        SetLevel(itemId, next);
        return next;
    }
}
