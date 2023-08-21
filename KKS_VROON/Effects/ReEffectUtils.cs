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
            if (useGlobalFog) MainCamera.GetOrAddComponent<ReGlobalFog>().Source = gameMainCamera;
            if (useAmplifyOcclusionEffect) MainCamera.GetOrAddComponent<ReAmplifyOcclusionEffect>().Source = gameMainCamera;
            if (useBloomAndFlares) MainCamera.GetOrAddComponent<ReBloomAndFlares>().Source = gameMainCamera;
            if (useAmplifyColorEffect) MainCamera.GetOrAddComponent<ReAmplifyColorEffect>().Source = gameMainCamera;
            if (useSunShafts) MainCamera.GetOrAddComponent<ReSunShafts>().Source = gameMainCamera;
            if (useVignetteAndChromaticAberration) MainCamera.GetOrAddComponent<ReVignetteAndChromaticAberration>().Source = gameMainCamera;
            if (useDepthOfField) MainCamera.GetOrAddComponent<ReDepthOfField>().Source = gameMainCamera;
            if (useBlur) MainCamera.GetOrAddComponent<ReBlur>().Source = gameMainCamera;
            if (useCrossFade) MainCamera.GetOrAddComponent<ReCrossFade>().Source = gameMainCamera;
            if (useSepiaTone) MainCamera.GetOrAddComponent<ReSepiaTone>().Source = gameMainCamera;
        }
    }
}
