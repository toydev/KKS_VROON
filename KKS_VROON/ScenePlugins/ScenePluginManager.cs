using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using KKS_VROON.Logging;
using KKS_VROON.ScenePlugins.ActionScene;
using KKS_VROON.ScenePlugins.HScene;
using KKS_VROON.ScenePlugins.OpeningScene;
using KKS_VROON.ScenePlugins.SimpleScreenScene;
using KKS_VROON.VRUtils;

namespace KKS_VROON.ScenePlugins
{
    public class ScenePluginManager : MonoBehaviour
    {
        #region Control scene
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            new GameObject(nameof(VRMirrorManager) + scene.name).AddComponent<VRMirrorManager>();

            if (mode == LoadSceneMode.Single)
            {
                switch (scene.name)
                {
                    // for MainGame
                    case SceneNames.LOGO:
                    case SceneNames.TITLE:
                        CreateSceneControllerGameObject(typeof(SimpleScreenScenePlugin));
                        break;
                    case SceneNames.OPENING_SCENE:
                        CreateSceneControllerGameObject(typeof(OpeningScenePlugin));
                        break;
                    case SceneNames.ACTION:
                        CreateSceneControllerGameObject(typeof(ActionScenePlugin));
                        break;
                    case SceneNames.FREE_H:
                        CreateSceneControllerGameObject(typeof(SimpleScreenScenePlugin));
                        break;
                    case SceneNames.H:
                        CreateSceneControllerGameObject(typeof(HScenePlugin));
                        break;
                }
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
