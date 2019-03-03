﻿using System.Collections.Generic;

namespace MedLaunch.Classes.Controls.VirtualDevices
{
    public class Gg_Legacy : VirtualDeviceBase
    {
        public static IDeviceDefinition GamePad(int VirtualPort)
        {
            IDeviceDefinition device = new DeviceDefinitionLegacy();
            device.DeviceName = "GG GamePad";
            device.CommandStart = "gg.input.builtin";
            device.VirtualPort = 0;
            device.MapList = new List<Mapping>
            {
                new Mapping { Description = "UP ↑", MednafenCommand = "gg.input.builtin.gamepad.up" },
                new Mapping { Description = "DOWN ↓", MednafenCommand = "gg.input.builtin.gamepad.down" },
                new Mapping { Description = "LEFT ←", MednafenCommand = "gg.input.builtin.gamepad.left" },
                new Mapping { Description = "RIGHT →", MednafenCommand = "gg.input.builtin.gamepad.right" },
                new Mapping { Description = "START", MednafenCommand = "gg.input.builtin.gamepad.start" },
                new Mapping { Description = "Button 1", MednafenCommand = "gg.input.builtin.gamepad.button1" },
                new Mapping { Description = "Button 2", MednafenCommand = "gg.input.builtin.gamepad.button2" },
                new Mapping { Description = "Rapid Button 1", MednafenCommand = "gg.input.builtin.gamepad.rapid_button1" },
                new Mapping { Description = "Rapid Button 2", MednafenCommand = "gg.input.builtin.gamepad.rapid_button2" }
            };
            DeviceDefinitionLegacy.PopulateConfig(device);
            return device;
        }
    }
}
