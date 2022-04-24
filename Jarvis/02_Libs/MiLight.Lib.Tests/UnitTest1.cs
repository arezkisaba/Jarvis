using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiLight.Lib.Enums;
using MiLight.Lib.Models;

namespace MiLight.Lib.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly MilightAdminService _miLightAdminClient = new MilightAdminService("192.168.1.19");
        private readonly MilightService _miLightClient = new MilightService("192.168.1.19");

        [TestMethod]
        public void RGBWSetColorAsync()
        {
            _miLightClient.RGBWSetColorAsync(MiLightGroups.All, 0xa0).Wait();
        }

        [TestMethod]
        public void RGBWSetDiscoModeAsync()
        {
            _miLightClient.RGBWSetDiscoModeAsync(MiLightGroups.All).Wait();
        }

        [TestMethod]
        public void RGBWSetNightModeAsync()
        {
            _miLightClient.RGBWSetNightModeAsync(MiLightGroups.All).Wait();
        }

        [TestMethod]
        public void RGBWSetWhiteModeAsync()
        {
            _miLightClient.RGBWSetWhiteModeAsync(MiLightGroups.All).Wait();
        }

        [TestMethod]
        public void RGBWSwitchOffAsync()
        {
            _miLightClient.RGBWSwitchOffAsync(MiLightGroups.All).Wait();
        }

        [TestMethod]
        public void RGBWSwitchOnAsync()
        {
            _miLightClient.RGBWSwitchOnAsync(MiLightGroups.All).Wait();
        }

        [TestMethod]
        public void Setup()
        {
            var bridges = _miLightAdminClient.FindBridgesAsync().GetAwaiter().GetResult();
            if (!bridges.Any())
            {
                return;
            }

            var bridgeIp = bridges.First().Value;
            var hotspots = _miLightAdminClient.FindWifiHostspotsAsync(bridgeIp).GetAwaiter().GetResult();
            var version = _miLightAdminClient.FindVersionAsync(bridgeIp).GetAwaiter().GetResult();
            var isStaSetupOk = _miLightAdminClient.SetupHotspotAsync(bridgeIp, hotspots.FirstOrDefault(obj => obj.Ssid == "Bbox-00F68F1D").Ssid, "ilovejessicaalba").GetAwaiter().GetResult();
            if (isStaSetupOk)
            {
            }
        }

        [TestMethod]
        public void SyncArea2()
        {
        }

        [TestMethod]
        public void UnsyncArea2()
        {
        }
    }
}