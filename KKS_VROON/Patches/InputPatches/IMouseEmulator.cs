namespace KKS_VROON.Patches.InputPatches
{
    public interface IMouseEmulator
    {
        float? GetAxis(string axisName);
        bool? GetMouseButton(int button);
        bool? GetMouseButtonDown(int button);
        bool? GetMouseButtonUp(int button);

        void SendMouseEvent();
    }
}
