using UnityEngine;

[CreateAssetMenu(fileName = "NewMiniGame", menuName = "MiniGame Data")]
public class MiniGameData : ScriptableObject
{
    public string gameName;
    public Sprite icon;
    public int costToPlay;
    public int rewardOnWin;
    public string sceneName;
}
