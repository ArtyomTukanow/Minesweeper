using Controller.Map;
using UnityEngine;

namespace AllUtils
{
    public class DebugTool : MonoBehaviour
    {
        void Update()
        {
#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z)) 
                    MapController.Instance.UserMap.CommandSystem.Undo();
#endif
        }
    }
}