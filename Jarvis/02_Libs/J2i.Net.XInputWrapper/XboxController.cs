using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace J2i.Net.XInputWrapper
{
    public class XboxController
    {
        public const int FIRST_CONTROLLER_INDEX = 0;
        public const int LAST_CONTROLLER_INDEX = MAX_CONTROLLER_COUNT - 1;
        public const int MAX_CONTROLLER_COUNT = 4;
        private static readonly XboxController[] Controllers;
        private static bool _isRunning;
        private static bool _keepRunning;
        private static Thread _pollingThread;
        private static readonly object SyncLock;
        private static int _updateFrequency;
        private static int _waitTime;
        private readonly int _playerIndex;
        private DateTime _stopMotorTime;
        private bool _stopMotorTimerActive;
        private XInputState _gamepadStateCurrent;
        private XINPUT_GAMEPAD_SECRET _struc;

        private XInputState _gamepadStatePrev = new XInputState();

        static XboxController()
        {
            Controllers = new XboxController[MAX_CONTROLLER_COUNT];
            SyncLock = new object();
            for (var i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; ++i)
            {
                Controllers[i] = new XboxController(i);
            }

            UpdateFrequency = 25;
        }

        private XboxController(int playerIndex)
        {
            _playerIndex = playerIndex;
            _gamepadStatePrev.Copy(_gamepadStateCurrent);
        }

        public XInputBatteryInformation BatteryInformationGamepad { get; internal set; }

        public XInputBatteryInformation BatteryInformationHeadset { get; internal set; }

        public bool IsConnected { get; internal set; }

        public static int UpdateFrequency
        {
            get => _updateFrequency;
            set
            {
                _updateFrequency = value;
                _waitTime = 1000 / _updateFrequency;
            }
        }

        public XInputCapabilities GetCapabilities()
        {
            var capabilities = new XInputCapabilities();
            XInput.XInputGetCapabilities(_playerIndex, XInputConstants.XINPUT_FLAG_GAMEPAD, ref capabilities);
            return capabilities;
        }

        public static XboxController RetrieveController(int index)
        {
            return Controllers[index];
        }

        public event EventHandler<XboxControllerStateChangedEventArgs> StateChanged;

        public override string ToString()
        {
            return _playerIndex.ToString();
        }

        public void UpdateBatteryState()
        {
            XInputBatteryInformation headset = new XInputBatteryInformation(),
                gamepad = new XInputBatteryInformation();

            XInput.XInputGetBatteryInformation(_playerIndex, (byte)BatteryDeviceType.BATTERY_DEVTYPE_GAMEPAD,
                ref gamepad);
            XInput.XInputGetBatteryInformation(_playerIndex, (byte)BatteryDeviceType.BATTERY_DEVTYPE_HEADSET,
                ref headset);

            BatteryInformationHeadset = headset;
            BatteryInformationGamepad = gamepad;
        }

        protected void OnStateChanged()
        {
            StateChanged?.Invoke(this, new XboxControllerStateChangedEventArgs
            {
                CurrentInputState = _gamepadStateCurrent,
                PreviousInputState = _gamepadStatePrev
            });
        }

        #region Digital Button States

        public bool IsDPadUpPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_UP);

        public bool IsDPadDownPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN);

        public bool IsDPadLeftPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT);

        public bool IsDPadRightPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT);

        public bool IsAPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_A);

        public bool IsBPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_B);

        public bool IsXPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_X);

        public bool IsYPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_Y);

        public bool IsBackPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_BACK);

        public bool IsStartPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_START);

        public bool IsLeftShoulderPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER);

        public bool IsRightShoulderPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER);

        public bool IsLeftStickPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_THUMB);

        public bool IsRightStickPressed => _gamepadStateCurrent.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_THUMB);

        #endregion

        #region Analogue Input States

        public int LeftTrigger => _gamepadStateCurrent.Gamepad.bLeftTrigger;

        public int RightTrigger => _gamepadStateCurrent.Gamepad.bRightTrigger;

        public Point LeftThumbStick
        {
            get
            {
                var p = new Point
                {
                    X = _gamepadStateCurrent.Gamepad.sThumbLX,
                    Y = _gamepadStateCurrent.Gamepad.sThumbLY
                };
                return p;
            }
        }

        public Point RightThumbStick
        {
            get
            {
                var p = new Point
                {
                    X = _gamepadStateCurrent.Gamepad.sThumbRX,
                    Y = _gamepadStateCurrent.Gamepad.sThumbRY
                };
                return p;
            }
        }

        #endregion

        #region Polling

        public static void StartPolling()
        {
            if (!_isRunning)
            {
                lock (SyncLock)
                {
                    if (!_isRunning)
                    {
                        _pollingThread = new Thread(PollerLoop);
                        _pollingThread.Start();
                    }
                }
            }
        }

        public static void StopPolling()
        {
            if (_isRunning)
            {
                _keepRunning = false;
            }
        }

        private static void PollerLoop()
        {
            lock (SyncLock)
            {
                if (_isRunning)
                {
                    return;
                }

                _isRunning = true;
            }

            _keepRunning = true;
            while (_keepRunning)
            {
                for (var i = FIRST_CONTROLLER_INDEX; i <= LAST_CONTROLLER_INDEX; ++i)
                {
                    Controllers[i].UpdateState();
                }

                Thread.Sleep(_updateFrequency);
            }

            lock (SyncLock)
            {
                _isRunning = false;
            }
        }

        public void UpdateState()
        {
            var result = XInput.XInputGetState(_playerIndex, ref _gamepadStateCurrent);
            IsConnected = result == 0;

            ////var result2 = XInput.XInputGetKeystroke(_playerIndex, 0, ref _gamepadKeystrokeCurrent);

            var stat = secret_get_gamepad(_playerIndex, out _struc);
            var value = (xgs.wButtons & 0x0400) != 0;

            UpdateBatteryState();
            if (_gamepadStateCurrent.PacketNumber != _gamepadStatePrev.PacketNumber)
            {
                Console.WriteLine("State changed");
                OnStateChanged();
            }

            _gamepadStatePrev.Copy(_gamepadStateCurrent);

            if (_stopMotorTimerActive && (DateTime.Now >= _stopMotorTime))
            {
                var stopStrength = new XInputVibration { LeftMotorSpeed = 0, RightMotorSpeed = 0 };
                XInput.XInputSetState(_playerIndex, ref stopStrength);
            }
        }

        #endregion

        #region Motor Functions

        public void Vibrate(double leftMotor, double rightMotor)
        {
            Vibrate(leftMotor, rightMotor, TimeSpan.MinValue);
        }

        public void Vibrate(double leftMotor, double rightMotor, TimeSpan length)
        {
            leftMotor = Math.Max(0d, Math.Min(1d, leftMotor));
            rightMotor = Math.Max(0d, Math.Min(1d, rightMotor));

            var vibration = new XInputVibration
            {
                LeftMotorSpeed = (ushort)(65535d * leftMotor),
                RightMotorSpeed = (ushort)(65535d * rightMotor)
            };
            Vibrate(vibration, length);
        }

        public void Vibrate(XInputVibration strength)
        {
            _stopMotorTimerActive = false;
            XInput.XInputSetState(_playerIndex, ref strength);
        }

        public void Vibrate(XInputVibration strength, TimeSpan length)
        {
            XInput.XInputSetState(_playerIndex, ref strength);
            if (length != TimeSpan.MinValue)
            {
                _stopMotorTime = DateTime.Now.Add(length);
                _stopMotorTimerActive = true;
            }
        }

        #endregion

        [DllImport("xinput1_3.dll", EntryPoint = "#100")]
        private static extern int secret_get_gamepad(int playerIndex, out XINPUT_GAMEPAD_SECRET struc);

        public struct XINPUT_GAMEPAD_SECRET
        {
            public uint eventCount;
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        public XINPUT_GAMEPAD_SECRET xgs;

        private bool testHomeButton()
        {
            int stat;
            bool value;

            for (var i = 0; i < 4; i++)
            {
                stat = secret_get_gamepad(0, out xgs);

                if (stat != 0)
                {
                    continue;
                }

                value = (xgs.wButtons & 0x0400) != 0;

                if (value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}