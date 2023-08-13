using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using KKS_VROON.Logging;
using KKS_VROON.ScenePlugins.ActiveScene;
using KKS_VROON.ScenePlugins.SimpleScreenScene;

namespace KKS_VROON.ScenePlugins
{
    public class ScenePluginManager : MonoBehaviour
    {
        #region Control scene
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "Logo":
                case "Title":
                    CreateSceneControllerGameObject(typeof(SimpleScreenScenePlugin));
                    break;
                case "Action":
                case "FreeH":
                case "OpeningScene":
                    CreateSceneControllerGameObject(typeof(ActiveScenePlugin));
                    break;
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endregion

        #region Initialize
        public static void Initialize()
        {
            var gameObject = new GameObject(nameof(ScenePluginManager));
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<ScenePluginManager>();
        }
        #endregion

        #region CurrentScene Control
        private GameObject CurrentSceneControllerGameObject { get; set; }

        private GameObject CreateSceneControllerGameObject(Type type)
        {
            PluginLog.Info($"CreateSceneGameObject: {type.Name}");

            if (CurrentSceneControllerGameObject != null) Destroy(CurrentSceneControllerGameObject);

            CurrentSceneControllerGameObject = new GameObject(type.Name);
            CurrentSceneControllerGameObject.AddComponent(type);
            return CurrentSceneControllerGameObject;
        }
        #endregion
    }
}
