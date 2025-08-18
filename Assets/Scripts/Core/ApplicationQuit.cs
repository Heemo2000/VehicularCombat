using UnityEngine;

namespace Game.Core
{
    public class ApplicationQuit : MonoBehaviour
    {
        public void QuitApplication()
        {
            Debug.Log("Quitting Application");
            Application.Quit();
        }
    }
}
