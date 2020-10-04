using UnityEngine.InputSystem;

namespace Data
{
    public class PlayerInfo
    {
        public InputDevice Device { get; private set; }
        public ControlType Type { get; private set; }
        
        public bool IsPlayer { get; private set; }

        public PlayerInfo(InputDevice device, ControlType type, bool isPlayer = true)
        {
            Device = device;
            Type = type;
            IsPlayer = isPlayer;
        }
    }
}