﻿using System.Collections.Generic;

namespace MedLaunch.Classes.Controls.VirtualDevices
{
    public class Snes_Legacy : VirtualDeviceBase
    {
        public static IDeviceDefinition GamePad(int VirtualPort)
        {
            IDeviceDefinition device = new DeviceDefinitionLegacy();
            device.DeviceName = "SNES GamePad";
            device.CommandStart = "snes.input.port" + VirtualPort;
            device.VirtualPort = VirtualPort;
            device.MapList = new List<Mapping>
            {
                new Mapping { Description = "D-Pad UP ↑", MednafenCommand = device.CommandStart +".gamepad.up" },
                new Mapping { Description = "D-Pad DOWN ↓", MednafenCommand = device.CommandStart +".gamepad.down" },
                new Mapping { Description = "D-Pad LEFT ←", MednafenCommand = device.CommandStart +".gamepad.left" },
                new Mapping { Description = "D-Pad RIGHT →", MednafenCommand = device.CommandStart +".gamepad.right" },
                new Mapping { Description = "START", MednafenCommand = device.CommandStart +".gamepad.start" },
                new Mapping { Description = "SELECT", MednafenCommand = device.CommandStart +".gamepad.select" },
                new Mapping { Description = "B (center, lower)", MednafenCommand = device.CommandStart +".gamepad.b" },
                new Mapping { Description = "A (right)", MednafenCommand = device.CommandStart +".gamepad.a" },
                new Mapping { Description = "Y (left)", MednafenCommand = device.CommandStart +".gamepad.y" },
                new Mapping { Description = "X (center, upper)", MednafenCommand = device.CommandStart +".gamepad.x" },
                new Mapping { Description = "Left Shoulder", MednafenCommand = device.CommandStart +".gamepad.l" },
                new Mapping { Description = "Right Shoulder", MednafenCommand = device.CommandStart +".gamepad.r" },
                new Mapping { Description = "Rapid B (center, lower)", MednafenCommand = device.CommandStart +".gamepad.rapid_b" },
                new Mapping { Description = "Rapid A (right)", MednafenCommand = device.CommandStart +".gamepad.rapid_a" },
                new Mapping { Description = "Rapid Y (left)", MednafenCommand = device.CommandStart +".gamepad.rapid_y" },
                new Mapping { Description = "Rapid X (center, upper)", MednafenCommand = device.CommandStart +".gamepad.rapid_x" },
            };
            DeviceDefinitionLegacy.PopulateConfig(device);
            return device;
        }

        public static IDeviceDefinition Superscope(int VirtualPort)
        {
            IDeviceDefinition device = new DeviceDefinitionLegacy();
            device.DeviceName = "SNES Super Scope";
            device.CommandStart = "snes.input.port" + VirtualPort;
            device.VirtualPort = VirtualPort;
            device.MapList = new List<Mapping>
            {
                new Mapping { Description = "Cursor", MednafenCommand = device.CommandStart +".superscope.cursor" },
                new Mapping { Description = "Offscreen Shot(Simulated)", MednafenCommand = device.CommandStart +".superscope.offscreen_shot" },
                new Mapping { Description = "Pause", MednafenCommand = device.CommandStart +".superscope.pause" },
                new Mapping { Description = "Trigger", MednafenCommand = device.CommandStart +".superscope.trigger" },
                new Mapping { Description = "Turbo", MednafenCommand = device.CommandStart +".superscope.turbo" },
                new Mapping { Description = "X Axis", MednafenCommand = device.CommandStart +".superscope.x_axis" },
                new Mapping { Description = "Y Axis", MednafenCommand = device.CommandStart +".superscope.y_axis" },
            };
            DeviceDefinitionLegacy.PopulateConfig(device);
            return device;
        }

        public static IDeviceDefinition Mouse(int VirtualPort)
        {
            IDeviceDefinition device = new DeviceDefinitionLegacy();
            device.DeviceName = "SNES Mouse";
            device.CommandStart = "snes.input.port" + VirtualPort;
            device.VirtualPort = VirtualPort;
            device.MapList = new List<Mapping>
            {
                new Mapping { Description = "Left Button", MednafenCommand = device.CommandStart +".mouse.left" },
                new Mapping { Description = "Right Button", MednafenCommand = device.CommandStart +".mouse.right" },
            };
            DeviceDefinitionLegacy.PopulateConfig(device);
            return device;
        }
    }
}
