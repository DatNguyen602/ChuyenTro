using UnityEngine;

public static class UtilsInput
{
    public static bool IsValidMousePosition(Vector3 mousePos)
    {
        return !float.IsInfinity(mousePos.x) && !float.IsInfinity(mousePos.y)
            && !float.IsNaN(mousePos.x) && !float.IsNaN(mousePos.y);
    }
}
