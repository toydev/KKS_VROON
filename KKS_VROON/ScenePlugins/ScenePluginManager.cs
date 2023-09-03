using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using KKS_VROON.Logging;
using KKS_VROON.ScenePlugins.ActionScene;
using KKS_VROON.ScenePlugins.CustomScene;
using KKS_VROON.ScenePlugins.HScene;
using KKS_VROON.ScenePlugins.OpeningScene;
using KKS_VROON.ScenePlugins.SimpleScreenScene;
using KKS_VROON.ScenePlugins.StudioScene;
using KKS_VROON.VRUtils;

namespace KKS_VROON.ScenePlugins
{
    public class ScenePluginManager : MonoBehaviour
    {
        #region Control scene
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!PluginConfig.EnableMirror.Value)
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
                    case SceneNames.CUSTOM_SCENE:
                        CreateSceneControllerGameObject(typeof(CustomScenePlugin));
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

                    // for CharaStudio
                    case SceneNames.STUDIO_START:
                        // Not supported: because the flickers.
                        break;
                    case SceneNames.STUDIO:
                        CreateSceneControllerGameObject(typeof(StudioScenePlugin));
                        break;
                }
            }
            else
            {
                switch (scene.name)
                {
                    // for MainGame
                    case SceneNames.CUSTOM_SCENE:
                        CreateSceneControllerGameObject(typeof(CustomScenePlugin));
                        break;
                }
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            switch (scene.name)
            {
                // for MainGame
                case SceneNames.CUSTOM_SCENE:
                    if (Manager.Scene.NowSceneNames.Contains(SceneNames.OPENING_SCENE))
                        CreateSceneControllerGameObject(typeof(OpeningScenePlugin));
                    if (Manager.Scene.NowSceneNames.Contains(SceneNames.ACTION))
                        CreateSceneControllerGameObject(typeof(ActionScenePlugin));
                    break;
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
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
            PluginLog.Debug($"CreateSceneGameObject: {type.Name}");

            if (CurrentSceneControllerGameObject != null) Destroy(CurrentSceneControllerGameObject);

            CurrentSceneControllerGameObject = new GameObject(type.Name);
            DontDestroyOnLoad(CurrentSceneControllerGameObject);
            CurrentSceneControllerGameObject.AddComponent(type);
            return CurrentSceneControllerGameObject;
        }
        #endregion
    }
}
