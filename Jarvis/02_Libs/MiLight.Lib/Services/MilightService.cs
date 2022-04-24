using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MiLight.Lib.Enums;

namespace MiLight.Lib.Models
{
    public class MilightService
    {
        private static readonly object Lock = new object();
        private readonly string _remoteHost;
        private readonly int _remotePort;
        private StringBuilder ReceiveBuffer = new StringBuilder();
        private UdpClient UdpClient;
        private IPEndPoint _outIp;
        private IPEndPoint _inIp;
        protected bool _isAdmin = false;

        public MilightService(string remoteHost = "10.10.100.254", int remotePort = 8899)
        {
            _remoteHost = remoteHost;
            _remotePort = remotePort;
        }

        public async Task<string> ReceiveDataAsync()
        {
            for (var i = 0; i < 3; i++)
            {
                if (UdpClient.Available > 0)
                {
                    ReceiveBuffer.AppendLine(Encoding.UTF8.GetString(UdpClient.Receive(ref _inIp)));
                    break;
                }

                await Task.Delay(500);
            }

            var s = ReceiveBuffer.ToString();
            ReceiveBuffer.Clear();
            await Task.Run(() => { });
            return s;
        }

        public async Task SendDataAsync(string data, int delay = 100)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            await SendDataAsync(dataBytes, delay);
        }

        public async Task SendDataAsync(byte[] data, int delay = 100)
        {
            if (UdpClient == null)
            {
                lock (Lock)
                {
                    if (UdpClient == null)
                    {
                        UdpClient = new UdpClient();
                    }
                }

                _outIp = new IPEndPoint(IPAddress.Parse(_remoteHost), _remotePort);
                UdpClient.Client.MulticastLoopback = false;
                UdpClient.EnableBroadcast = true;

                if (_isAdmin)
                {
                    _inIp = new IPEndPoint(IPAddress.Parse("0.0.0.0"), _remotePort);
                    UdpClient.Client.ReceiveTimeout = 3000;
                    UdpClient.Client.Bind(_inIp);
                }
            }

            UdpClient.Send(data, data.Length, _outIp);
            await Task.Delay(delay);
        }

        public void Close()
        {
            if (UdpClient != null)
            {
                UdpClient.Close();
            }
        }

        private async Task DelayAsync(int millisecondsTimeout = 101)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        #region LED Commands

        public async Task RGBWSetBrightnessAsync(MiLightGroups group, byte level)
        {
            if (level < 0x02)
            {
                level = 0x02;
            }
            else if (level > 0x27)
            {
                level = 0x27;
            }

            var cmd = (byte[]) MiLightCommands.RGBWBrightness.Clone();
            cmd[1] = level;

            await RGBWSwitchOnAsync(group);
            await DelayAsync();
            await SendCommandAsync(cmd);
        }

        public async Task RGBWSetColorAsync(MiLightGroups group, byte color)
        {
            var cmd = (byte[]) MiLightCommands.RGBWColor.Clone();
            cmd[1] = color;

            await RGBWSwitchOnAsync(group);
            await DelayAsync();
            await SendCommandAsync(cmd);
        }

        public async Task RGBWSetDiscoModeAsync(MiLightGroups group)
        {
            await RGBWSwitchOnAsync(group);
            await DelayAsync();
            await SendCommandAsync(MiLightCommands.RGBWDiscoMode);
        }

        public async Task RGBWSetNightModeAsync(MiLightGroups group)
        {
            byte[] cmd;
            switch (group)
            {
                case MiLightGroups.One:
                    cmd = MiLightCommands.RGBWGroup1AllNight;
                    break;
                case MiLightGroups.Two:
                    cmd = MiLightCommands.RGBWGroup2AllNight;
                    break;
                case MiLightGroups.Three:
                    cmd = MiLightCommands.RGBWGroup3AllNight;
                    break;
                case MiLightGroups.Four:
                    cmd = MiLightCommands.RGBWGroup4AllNight;
                    break;
                default:
                    cmd = MiLightCommands.RGBWOff;
                    break;
            }

            await RGBWSwitchOffAsync(group);
            await DelayAsync();
            await SendCommandAsync(cmd);
        }

        public async Task RGBWSetWhiteModeAsync(MiLightGroups group)
        {
            byte[] cmd;
            switch (group)
            {
                case MiLightGroups.One:
                    cmd = MiLightCommands.SetColorToWhiteGroup1;
                    break;
                case MiLightGroups.Two:
                    cmd = MiLightCommands.SetColorToWhiteGroup2;
                    break;
                case MiLightGroups.Three:
                    cmd = MiLightCommands.SetColorToWhiteGroup3;
                    break;
                case MiLightGroups.Four:
                    cmd = MiLightCommands.SetColorToWhiteGroup4;
                    break;
                default:
                    cmd = MiLightCommands.SetColorToWhite;
                    break;
            }

            await RGBWSwitchOnAsync(group);
            await DelayAsync();
            await SendCommandAsync(cmd);
        }

        public async Task RGBWSwitchOffAsync(MiLightGroups group)
        {
            byte[] cmd;
            switch (group)
            {
                case MiLightGroups.One:
                    cmd = MiLightCommands.RGBWGroup1AllOff;
                    break;
                case MiLightGroups.Two:
                    cmd = MiLightCommands.RGBWGroup2AllOff;
                    break;
                case MiLightGroups.Three:
                    cmd = MiLightCommands.RGBWGroup3AllOff;
                    break;
                case MiLightGroups.Four:
                    cmd = MiLightCommands.RGBWGroup4AllOff;
                    break;
                default:
                    cmd = MiLightCommands.RGBWOff;
                    break;
            }
            await SendCommandAsync(cmd);
        }

        public async Task RGBWSwitchOnAsync(MiLightGroups group)
        {
            byte[] cmd;
            switch (group)
            {
                case MiLightGroups.One:
                    cmd = MiLightCommands.RGBWGroup1AllOn;
                    break;
                case MiLightGroups.Two:
                    cmd = MiLightCommands.RGBWGroup2AllOn;
                    break;
                case MiLightGroups.Three:
                    cmd = MiLightCommands.RGBWGroup3AllOn;
                    break;
                case MiLightGroups.Four:
                    cmd = MiLightCommands.RGBWGroup4AllOn;
                    break;
                default:
                    cmd = MiLightCommands.RGBWOn;
                    break;
            }

            await SendCommandAsync(cmd);
        }

        private async Task SendCommandAsync(byte[] buffer)
        {
            await SendDataAsync(buffer, buffer.Length);
        }

        #endregion
    }
}