﻿using System.Collections.Generic;

namespace MedLaunch.Classes.Controls.VirtualDevices
{
    public class Gg : VirtualDeviceBase
    {
        public static DeviceDefinition GamePad(int VirtualPort)
        {
            DeviceDefinition device = new DeviceDefinition();
            device.DeviceName = "GG GamePad";
            device.CommandStart = "gg.input.builtin";
            device.ControllerName = "gamepad";
            device.VirtualPort = 0;
            device.MapList = new List<Mapping>
            {
                /* MapList is now autogenerated from mednafen.cfg */
            };
            DeviceDefinition.ParseOptionsFromConfig(device);
            DeviceDefinition.PopulateConfig(device);
            return device;
        }
    }
}
