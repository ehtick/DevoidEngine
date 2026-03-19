namespace DevoidEngine.Engine.InputSystem
{
    public class InputBinding
    {
        public InputDeviceType DeviceType;
        public ushort Control;

        public float Scale = 1f;
        public List<IInputProcessor> Processors = new();
    }
}