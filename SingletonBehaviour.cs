using UnityEngine;

namespace UpgradeFramework
{
    internal class SingletonBehaviour : MonoBehaviour
    {
        private static SingletonBehaviour _instance;
        internal static SingletonBehaviour instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("SingletonBehaviour");
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<SingletonBehaviour>();
                }
                return _instance;
            }
        }
    }
}
