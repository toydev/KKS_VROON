namespace KKS_VROON.Patches.InputPatches
{
    public interface IMouseEmulator
    {
        float? GetAxis(string axisName);
        void SendMouseEvent();
    }
}
