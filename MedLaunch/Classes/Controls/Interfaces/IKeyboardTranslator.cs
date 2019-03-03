using System.Collections.Generic;

namespace MedLaunch.Classes.Controls
{
    public interface IKeyboardTranslator
    {
        List<DxKeys> dxKeys { get; set; }

        //void KeyboardTranslation(KeyboardType keyboardType);
    }


    public class DxKeys
    {
        public int SDLK { get; set; }
        public string DxUK { get; set; }
    }

    public enum KeyboardType
    {
        UK,
        US
    }
}
