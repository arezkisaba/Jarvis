using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiLight.Lib.Models
{
    public class MilightAdminService : MilightService
    {
        private static readonly Regex HotspotRegEx = new Regex(@"^(?<ch>\d{1,2}),(?<ssid>.*),(?<bssid>.*),(?<security>.*),(?<signal>\d{1,3}),(?<extch>.*),(?<nt>.*),(?<wps>.*),(?<dpid>.*),$");
        private static readonly Regex IpIdRegEx = new Regex(@"^(?<ip>\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b),.*(?<id>[0-9a-zA-Z]{12}).*$");
        private static readonly Regex VersionRegEx = new Regex(@"^(?<ok>[+ok=]{4})(?<ver>[0-9a-zA-Z\.\-]*).*$");

        public MilightAdminService(string outHost = "10.10.100.254", int outPort = 48899) : base(outHost, outPort)
        {
            _isAdmin = true;
        }

        public async Task<Dictionary<string, string>> FindBridgesAsync()
        {
            var bridges = new Dictionary<string, string>();
            for (var i = 0; i < 10; i++)
            {
                await SendDataAsync("Link_Wi-Fi");
            }

            string data;
            while (!string.IsNullOrEmpty(data = await ReceiveDataAsync()))
            {
                var ma = IpIdRegEx.Match(data);
                if (ma.Success)
                {
                    var id = ma.Groups["id"].Value;
                    if (!bridges.ContainsKey(id))
                    {
                        bridges.Add(id, ma.Groups["ip"].Value);
                    }
                }
            }

            return bridges;
        }

        public async Task<List<MiLightHotspot>> FindWifiHostspotsAsync()
        {
            var hostspots = new List<MiLightHotspot>();

            var isOk = await HandshakeAsync();
            if (!isOk)
            {
                return hostspots;
            }

            await SendDataAsync("AT+WSCAN\r\n");
            await Task.Delay(1000);
            string data;

            while (!string.IsNullOrEmpty(data = await ReceiveDataAsync()))
            {
                foreach (var s in data.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var ma = HotspotRegEx.Match(s);
                    if (ma.Success)
                    {
                        var id = ma.Groups["ssid"].Value;
                        var hotspot = hostspots.Find(h => h.Bssid == id);
                        if (hotspot == null)
                        {
                            hotspot = new MiLightHotspot {Bssid = ma.Groups["ssid"].Value, Channel = byte.Parse(ma.Groups["ch"].Value), Dpid = ma.Groups["dpid"].Value, ExtCh = ma.Groups["extch"].Value, Nt = ma.Groups["nt"].Value, Security = ma.Groups["security"].Value, Signal = byte.Parse(ma.Groups["signal"].Value), Ssid = ma.Groups["ssid"].Value, Wps = ma.Groups["wps"].Value};
                            hostspots.Add(hotspot);
                        }
                    }
                }
            }

            return hostspots;
        }

        public async Task<string> FindVersionAsync(string bridgeIp)
        {
            var isOk = await HandshakeAsync();
            if (!isOk)
            {
                return string.Empty;
            }

            await SendDataAsync("AT+VER\r\n");
            string data;

            while (!string.IsNullOrEmpty(data = await ReceiveDataAsync()))
            {
                var ma = VersionRegEx.Match(data.Replace("\r\n", ""));
                if (ma.Success)
                {
                    return ma.Groups["ver"].Value;
                }
            }

            return string.Empty;
        }

        public async Task<bool> SetupHotspotAsync(string ssid, string password)
        {
            var isOk = await HandshakeAsync();
            if (!isOk)
            {
                return false;
            }

            await SendDataAsync(string.Format("AT+WSSSID={0}\r", ssid));
            isOk = await ReceiveOkAsync();
            isOk = !isOk;
            if (!isOk)
            {
                return false;
            }

            await SendDataAsync(string.Format("AT+WSKEY={0},{1},{2}\r", "WPA2PSK", "AES", password));
            isOk = await ReceiveOkAsync();
            if (!isOk)
            {
                return false;
            }

            await SendDataAsync("AT+WMODE=STA\r");
            isOk = await ReceiveOkAsync();
            if (!isOk)
            {
                return false;
            }

            await SendDataAsync("AT+Z\r");
            isOk = await ReceiveOkAsync();
            if (!isOk)
            {
                return false;
            }

            await SendDataAsync("AT+Q\r");
            isOk = await ReceiveOkAsync();
            if (!isOk)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> HandshakeAsync()
        {
            await SendDataAsync("Link_Wi-Fi");
            var isOk = false;
            string data;

            while (!string.IsNullOrEmpty(data = await ReceiveDataAsync()))
            {
                var ma = IpIdRegEx.Match(data);
                if (ma.Success)
                {
                    isOk = true;
                    break;
                }
            }

            if (!isOk)
            {
                return false;
            }

            await SendDataAsync("+ok");
            return true;
        }

        private async Task<bool> ReceiveOkAsync()
        {
            var s = await ReceiveDataAsync();
            return s.Contains("+ok");
        }
    }
}