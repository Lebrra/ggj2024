using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private void Start()
    {
        SetLock(CursorLockMode.Locked);
    }

    private void OnDestroy()
    {
        SetLock(CursorLockMode.None);
    }

    public static void SetLock(CursorLockMode mode)
    {
        Cursor.lockState = mode;
    }
}
