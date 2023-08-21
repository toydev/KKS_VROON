using UnityEngine;

using KKS_VROON.VRUtils;

namespace KKS_VROON.Effects
{
    public class ReEffectUtils
    {
        public static void AddEffects(
            Camera gameMainCamera,
            VRCamera MainCamera,
            bool useGlobalFog = true,
            bool useAmplifyOcclusionEffect = true,
            bool useBloomAndFlares = true,
            bool useAmplifyColorEffect = true,
            bool useSunShafts = true,
            bool useVignetteAndChromaticAberration = true,
            bool useDepthOfField = true,
            bool useBlur = true,
            bool useCrossFade = true,
            bool useSepiaTone = true
        )
        {
            if (useGlobalFog) MainCamera.Normal.GetOrAddComponent<ReGlobalFog>().Source = gameMainCamera;
            if (useAmplifyOcclusionEffect) MainCamera.Normal.GetOrAddComponent<ReAmplifyOcclusionEffect>().Source = gameMainCamera;
            if (useBloomAndFlares) MainCamera.Normal.GetOrAddComponent<ReBloomAndFlares>().Source = gameMainCamera;
            if (useAmplifyColorEffect) MainCamera.Normal.GetOrAddComponent<ReAmplifyColorEffect>().Source = gameMainCamera;
            if (useSunShafts) MainCamera.Normal.GetOrAddComponent<ReSunShafts>().Source = gameMainCamera;
            if (useVignetteAndChromaticAberration) MainCamera.Normal.GetOrAddComponent<ReVignetteAndChromaticAberration>().Source = gameMainCamera;
            if (useDepthOfField) MainCamera.Normal.GetOrAddComponent<ReDepthOfField>().Source = gameMainCamera;
            if (useBlur) MainCamera.Normal.GetOrAddComponent<ReBlur>().Source = gameMainCamera;
            if (useCrossFade) MainCamera.Normal.GetOrAddComponent<ReCrossFade>().Source = gameMainCamera;
            if (useSepiaTone) MainCamera.Normal.GetOrAddComponent<ReSepiaTone>().Source = gameMainCamera;
        }
    }
}
