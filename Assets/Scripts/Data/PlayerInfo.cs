using UnityEngine.InputSystem;

namespace Data
{
    public class PlayerInfo
    {
        public InputDevice Device { get; private set; }
        public ControlType Type { get; private set; }

        public PlayerInfo(InputDevice device, ControlType type)
        {
            Device = device;
            Type = type;
        }
    }
}