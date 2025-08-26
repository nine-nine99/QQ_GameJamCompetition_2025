using UnityEngine;

public class PartManager : MonoBehaviour
{
  public static PartManager Instance;

  private IPart currentPart;
  [SerializeField] private Part_Real startWorld;

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    // 默认进入现实世界
    if (startWorld != null)
    {
      startWorld.OnEnter();
      currentPart = startWorld;
      Debug.Log($"[PartManager] 初始进入 {startWorld.name}");
    }
  }

  public void SwitchTo(IPart newPart)
  {
    if (currentPart != null)
    {
      Debug.Log(currentPart);
      Debug.Log("trigger");
      currentPart.OnExit();
    }

    newPart.OnEnter();
    currentPart = newPart;
  }

  public IPart GetCurrentPart() => currentPart;

  
}
