namespace MiLight.Lib.Models
{
    public static class MiLightCommands
    {
        public static readonly byte[] AllFull = { 0xB5, 0x0, 0x55 };
        public static readonly byte[] AllNight = { 0xB9, 0x0, 0x55 };
        public static readonly byte[] AllOff = { 0x39, 0x0, 0x55 };

        // White LEDs
        public static readonly byte[] AllOn = { 0x35, 0x0, 0x55 };
        public static readonly byte[] BrightnessDown = { 0x34, 0x0, 0x55 };
        public static readonly byte[] BrightnessUp = { 0x3C, 0x0, 0x55 };
        public static readonly byte[] ColorTempDown = { 0x3F, 0x0, 0x55 };
        public static readonly byte[] ColorTempUp = { 0x3E, 0x0, 0x55 };
        public static readonly byte[] Group1Full = { 0xB8, 0x0, 0x55 };
        public static readonly byte[] Group1Night = { 0xBB, 0x0, 0x55 };
        public static readonly byte[] Group1Off = { 0x3B, 0x0, 0x55 };
        public static readonly byte[] Group1On = { 0x38, 0x0, 0x55 };
        public static readonly byte[] Group2Full = { 0xBD, 0x0, 0x55 };
        public static readonly byte[] Group2Night = { 0xB3, 0x0, 0x55 };
        public static readonly byte[] Group2Off = { 0x33, 0x0, 0x55 };
        public static readonly byte[] Group2On = { 0x3D, 0x0, 0x55 };
        public static readonly byte[] Group3Full = { 0xB7, 0x0, 0x55 };
        public static readonly byte[] Group3Night = { 0xBA, 0x0, 0x55 };
        public static readonly byte[] Group3Off = { 0x3A, 0x0, 0x55 };
        public static readonly byte[] Group3On = { 0x37, 0x0, 0x55 };
        public static readonly byte[] Group4Full = { 0xB2, 0x0, 0x55 };
        public static readonly byte[] Group4Night = { 0xB6, 0x0, 0x55 };
        public static readonly byte[] Group4Off = { 0x36, 0x0, 0x55 };
        public static readonly byte[] Group4On = { 0x32, 0x0, 0x55 };

        // RGB LEDs
        public static readonly byte[] RGBBrightnessDown = { 0x24, 0x0, 0x55 };
        public static readonly byte[] RGBBrightnessUp = { 0x23, 0x0, 0x55 };
        public static readonly byte[] RGBColour = { 0x20, 0x0, 0x55 };
        public static readonly byte[] RGBDiscoLast = { 0x28, 0x0, 0x55 };
        public static readonly byte[] RGBDiscoNext = { 0x27, 0x0, 0x55 };
        public static readonly byte[] RGBOff = { 0x21, 0x0, 0x55 };
        public static readonly byte[] RGBOn = { 0x22, 0x0, 0x55 };
        public static readonly byte[] RGBSpeedDown = { 0x26, 0x0, 0x55 };
        public static readonly byte[] RGBSpeedUp = { 0x25, 0x0, 0x55 };

        //RGBW LEDs   
        public static readonly byte[] RGBWBrightness = { 0x4E, 0x0, 0x55 };
        public static readonly byte[] RGBWColor = { 0x40, 0x0, 0x55 };
        public static readonly byte[] RGBWDiscoMode = { 0x4D, 0x0, 0x55 };
        public static readonly byte[] RGBWDiscoSpeedFaster = { 0x44, 0x0, 0x55 };
        public static readonly byte[] RGBWDiscoSpeedSlower = { 0x43, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup1AllNight = { 0xC6, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup1AllOff = { 0x46, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup1AllOn = { 0xC6, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup2AllNight = { 0xC8, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup2AllOff = { 0x48, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup2AllOn = { 0x47, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup3AllNight = { 0xCA, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup3AllOff = { 0x4A, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup3AllOn = { 0x49, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup4AllNight = { 0xCC, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup4AllOff = { 0x4C, 0x0, 0x55 };
        public static readonly byte[] RGBWGroup4AllOn = { 0x4B, 0x0, 0x55 };
        public static readonly byte[] RGBWOff = { 0x41, 0x0, 0x55 };
        public static readonly byte[] RGBWOn = { 0x42, 0x0, 0x55 };
        public static readonly byte[] SetColorToWhite = { 0xC2, 0x0, 0x55 };
        public static readonly byte[] SetColorToWhiteGroup1 = { 0xC5, 0x0, 0x55 };
        public static readonly byte[] SetColorToWhiteGroup2 = { 0xC7, 0x0, 0x55 };
        public static readonly byte[] SetColorToWhiteGroup3 = { 0xC9, 0x0, 0x55 };
        public static readonly byte[] SetColorToWhiteGroup4 = { 0xCB, 0x0, 0x55 };
    }
}