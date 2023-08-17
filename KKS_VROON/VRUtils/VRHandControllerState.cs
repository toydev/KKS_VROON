using UnityEngine;

namespace KKS_VROON.VRUtils
{
    public struct VRHandControllerState
    {
        #region Left
        public Vector3 LeftPosition { get; internal set; }
        public Vector3 LeftPositionDelta { get; internal set; }
        public Vector2 LeftJoystickAxis { get; internal set; }
        public Vector2 LeftJoystickAxisDelta { get; internal set; }
        public Vector3 LeftLaserPosition { get; internal set; }
        public Quaternion LeftLaserRotation { get; internal set; }
        public bool IsLeftTriggerOn { get; internal set; }
        public bool IsLeftTriggerDown { get; internal set; }
        public bool IsLeftTriggerUp { get; internal set; }
        public bool IsLeftGripOn { get; internal set; }
        public bool IsLeftGripDown { get; internal set; }
        public bool IsLeftGripUp { get; internal set; }
        public bool IsLeftJoystickClick { get; internal set; }
        public bool IsLeftJoystickDown { get; internal set; }
        public bool IsLeftJoystickUp { get; internal set; }
        public bool IsButtonXOn { get; internal set; }
        public bool IsButtonXDown { get; internal set; }
        public bool IsButtonXUp { get; internal set; }
        public bool IsButtonYOn { get; internal set; }
        public bool IsButtonYDown { get; internal set; }
        public bool IsButtonYUp { get; internal set; }
        #endregion

        #region Right
        public Vector3 RightPosition { get; internal set; }
        public Vector3 RightPositionDelta { get; internal set; }
        public Vector3 RightLaserPosition { get; internal set; }
        public Quaternion RightLaserRotation { get; internal set; }
        public Vector2 RightJoystickAxis { get; internal set; }
        public Vector2 RightJoystickAxisDelta { get; internal set; }
        public bool IsRightTriggerOn { get; internal set; }
        public bool IsRightTriggerDown { get; internal set; }
        public bool IsRightTriggerUp { get; internal set; }
        public bool IsRightGripOn { get; internal set; }
        public bool IsRightGripDown { get; internal set; }
        public bool IsRightGripUp { get; internal set; }
        public bool IsRightJoystickClick { get; internal set; }
        public bool IsRightJoystickDown { get; internal set; }
        public bool IsRightJoystickUp { get; internal set; }
        public bool IsButtonAOn { get; internal set; }
        public bool IsButtonADown { get; internal set; }
        public bool IsButtonAUp { get; internal set; }
        public bool IsButtonBOn { get; internal set; }
        public bool IsButtonBDown { get; internal set; }
        public bool IsButtonBUp { get; internal set; }
        #endregion

        #region Common
        public Vector3 PositionDelta => RightPositionDelta.magnitude < LeftPositionDelta.magnitude ? LeftPositionDelta : RightPositionDelta;
        public Vector2 JoystickAxis { get; internal set; }
        public bool IsTriggerOn => IsLeftTriggerOn || IsRightTriggerOn;
        public bool IsTriggerDown => IsLeftTriggerDown || IsRightTriggerDown;
        public bool IsTriggerUp => IsLeftTriggerUp || IsRightTriggerUp;
        public bool IsGripOn => IsLeftGripOn || IsRightGripOn;
        public bool IsGripDown => IsLeftGripDown || IsRightGripDown;
        public bool IsGripUp => IsLeftGripUp || IsRightGripUp;
        public bool IsJoystickClick => IsLeftJoystickClick || IsRightJoystickClick;
        public bool IsJoystickDown => IsLeftJoystickDown || IsRightJoystickDown;
        public bool IsJoystickUp => IsLeftJoystickUp || IsRightJoystickUp;

        public bool IsPositionChanging(float magnitubeThreshold = 0.0001f  /* 0.1mm */)
        {
            return magnitubeThreshold < LeftPositionDelta.magnitude
                || magnitubeThreshold < RightPositionDelta.magnitude;
        }
        #endregion
    }
}
