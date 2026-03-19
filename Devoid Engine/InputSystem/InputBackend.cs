using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.InputSystem
{
    public class InputBackend
    {
        public InputDeviceRegistry InputDeviceRegistry { get; set; }
        List<InputDeviceHandler> _inputDeviceHandlers;
        List<InputEvent> _events;

        public InputBackend()
        {
            _inputDeviceHandlers = new List<InputDeviceHandler>();
            _events = new List<InputEvent>();
        }

        public void AddInputDevice(InputDeviceHandler deviceHandler)
        {
            _inputDeviceHandlers.Add(deviceHandler);
        }
        public void Emit(InputEvent e)
        {
            _events.Add(e);
        }

        public List<InputEvent> GetEvents()
        {
            return _events;
        }

        public void UpdateInput()
        {
            _events.Clear();
            for (int i = 0; i < _inputDeviceHandlers.Count; i++)
            {
                InputDeviceHandler handler = _inputDeviceHandlers[i];
                handler.Update(this);
            }
        }
    }
}
