using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Core
{
    public class ServiceLocator : MonoBehaviour
    {
        private const string GLOBAL_SERVICE_LOCATOR_NAME = "Service Locator(Global)";
        private const string SCENE_SERVICE_LOCATOR_NAME = "Service Locator(Scene)";

        private static ServiceLocator global;
        private static Dictionary<Scene, ServiceLocator> sceneServiceLocators;
        private static List<GameObject> tempSceneGameObjects;

        private readonly ServiceManager serviceManager = new ServiceManager();
        
        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if(global == this)
            {
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal already configure as global", this);
            }
            else if(global != null)
            {
                Debug.LogError("ServiceLocator.ConfigureAsGlobal has other instance already configured", this);
            }
            else
            {
                global = this;
                if(dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }

        }

        internal void ConfigureForScene()
        {
            Scene scene = gameObject.scene;

            if(sceneServiceLocators.ContainsKey(scene))
            {
                Debug.LogError("ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene.", this);
                return;
            }

            sceneServiceLocators.Add(scene, this);
        }

        public static ServiceLocator Global
        {
            get
            {
                //If global exists, then return it.
                if(global != null)
                {
                    return global;
                }

                //If global bootstrapper is found, then return it.
                if(FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return global;
                }

                //Otherwise, make the global instance as necessary.
                var container = new GameObject(GLOBAL_SERVICE_LOCATOR_NAME, 
                                               typeof(ServiceLocator));

                container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();


                return global;
            }
        }
    
        public static ServiceLocator For(MonoBehaviour monoBehaviour)
        {
            var serviceLocator = monoBehaviour.GetComponentInParent<ServiceLocator>();
            if(serviceLocator != null)
            {
                return serviceLocator;
            }

            serviceLocator = ForSceneOf(monoBehaviour);
            if(serviceLocator != null)
            {
                return serviceLocator;
            }

            return serviceLocator;
        }
        
        public static ServiceLocator ForSceneOf(MonoBehaviour monoBehaviour)
        {
            Scene scene = monoBehaviour.gameObject.scene;

            if(sceneServiceLocators.TryGetValue(scene, out var container) && container != monoBehaviour)
            {
                Debug.Log("Found service locator scene: " + container.transform.name);
                return container;
            }

            tempSceneGameObjects.Clear();
            scene.GetRootGameObjects(tempSceneGameObjects);

            foreach (GameObject go in tempSceneGameObjects.Where(go => go.GetComponent<ServiceLocatorSceneBootstrapper>() != null))
            {
                if(go.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) && bootstrapper.Container != monoBehaviour)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;
        }
    
        public ServiceLocator Register<T>(T service)
        {
            serviceManager.Register(service);
            return this;
        }

        public ServiceLocator Register(Type type, object service)
        {
            serviceManager.Register(type, service);
            return this;
        }

        public ServiceLocator Get<T>(out T service) where T : class
        {
            if(TryGetService(out service))
            {
                return this;
            }

            if(TryGetNextInHierarchy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException($"ServiceLocator.Get : Service of type {typeof(T).FullName} not registered.");
        }

        public bool TryGetService<T>(out T service) where T : class
        {
            return serviceManager.TryGet(out service);
        }

        private bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if(this == global)
            {
                container = null;
                return false;
            }

            if (transform.parent == null)
            {
                container = null;
                return false;
            }

            container = transform.parent.GetComponentInParent<ServiceLocator>();

            if (container != null)
            {
                return true;
            }

            container = ForSceneOf(this);

            return container != null;
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatistics()
        {
            global = null;
            sceneServiceLocators = new Dictionary<Scene, ServiceLocator>();
            tempSceneGameObjects = new List<GameObject>();
        }

        #if UNITY_EDITOR

        [MenuItem("GameObject/ServiceLocator/Add Global")]
        private static void AddGlobalInstance()
        {
            var go = new GameObject(GLOBAL_SERVICE_LOCATOR_NAME, typeof(ServiceLocatorGlobalBootstrapper));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        private static void AddSceneInstance()
        {
            var go = new GameObject(SCENE_SERVICE_LOCATOR_NAME, typeof(ServiceLocatorSceneBootstrapper));
        }

        #endif

        private void OnDestroy()
        {
            if(global)
            {
                global = null;
            }
            else if(sceneServiceLocators.ContainsValue(this))
            {
                sceneServiceLocators.Remove(gameObject.scene);
            }
        }
    }
}
