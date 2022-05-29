using Jarvis.Features.Agents.GameControllerAgent.Contracts;
using Jarvis.Features.Agents.GameControllerAgent.Models;
using Jarvis.Features.Agents.GameControllerAgent.Services.CECService.Contracts;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Lib.Core;
using Lib.Win32;
using SharpDX.XInput;
using WindowsInput;
using WindowsInput.Native;

namespace Jarvis.Features.Agents.GameControllerAgent;

public class XboxControllerAgent : IGameControllerClientAgent
{
    private readonly AppSettingsModel _appSettings;
    private readonly ILogger<XboxControllerAgent> _logger;
    private readonly IDisplayManager _displayManager;
    private readonly IProcessManager _processManager;
    private readonly ISoundManager _soundManager;
    private readonly IWindowManager _windowManager;
    private readonly IInputSimulator _inputSimulator;
    private readonly ICECService _cecService;
    private readonly IEnumerable<Tuple<string, string, string>> _fullscreenApps;
    private readonly IEnumerable<Controller> _gameControllers = new List<Controller>()
    {
        new Controller(UserIndex.One),
        new Controller(UserIndex.Two),
        new Controller(UserIndex.Three),
        new Controller(UserIndex.Four)
    };
    private CancellationTokenSource _cancellationTokenSource;

    private Controller _selectedGameController;

    public GameControllerClientStateModel CurrentState { get; set; }

    public event EventHandler StateChanged;

    public XboxControllerAgent(
        IOptions<AppSettingsModel> appSettings,
        ILogger<XboxControllerAgent> logger,
        IDisplayManager displayManager,
        IProcessManager processManager,
        ISoundManager soundManager,
        IWindowManager windowManager,
        IInputSimulator inputSimulator,
        ICECService cecService)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _displayManager = displayManager;
        _processManager = processManager;
        _soundManager = soundManager;
        _windowManager = windowManager;
        _inputSimulator = inputSimulator;
        _cecService = cecService;
        _fullscreenApps = new List<Tuple<string, string, string>>
        {
            Tuple.Create("steam", $"{Path.Combine(_appSettings.steamDirectory, "steam.exe")}", "\"steam://open/bigpicture\"")
        };
    }

    public void StartBackgroundLoopForControllerDetection()
    {
        Task.Run(async () =>
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            do
            {
                try
                {
                    var selectedGameController = _gameControllers.FirstOrDefault(obj => obj.IsConnected);
                    if (_selectedGameController != selectedGameController)
                    {
                        _selectedGameController = selectedGameController;
                        if (_selectedGameController == null)
                        {
                            _logger.LogInformation("Controller disconnected");
                        }
                        else
                        {
                            _logger.LogInformation("Controller connected");
                        }
                    }

                    var resourceManager = new System.Resources.ResourceManager(
                        "Jarvis.Resources.BackgroundAgents.GameControllerBackgroundAgent.XboxControllerBackgroundAgent",
                        Assembly.GetExecutingAssembly());

                    var currentStateTemp = new GameControllerClientStateModel(
                        title: resourceManager.GetString("Title"),
                        subtitle: resourceManager.GetString("Subtitle"),
                        isActive: _selectedGameController != null);
                    if (currentStateTemp.IsActive != CurrentState?.IsActive)
                    {
                        StateChanged?.Invoke(currentStateTemp, EventArgs.Empty);
                    }

                    CurrentState = currentStateTemp;
                    if (!CurrentState.IsActive)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "XXXXX");
                }
            } while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token));
        });
    }

    public void StartBackgroundLoopForCommands()
    {
        Task.Run(async () =>
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            do
            {
                try
                {
                    var currentState = string.Empty;
                    var previousState = string.Empty;
                    var isPowerActive = false;
                    DateTime? isPowerActiveStartDate = null;

                    while (CurrentState?.IsActive == true)
                    {
                        var state = _selectedGameController.GetState();
                        var isBackActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back);
                        var isStartActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start);
                        var isLeftTriggerActive = state.Gamepad.LeftTrigger > 0;
                        var isRightTriggerActive = state.Gamepad.RightTrigger > 0;
                        var isLeftShoulderActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder);
                        var isRightShoulderActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder);
                        var isLeftThumbActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
                        var isRightThumbActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb);
                        var isDPadUpActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
                        var isDPadDownActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown);
                        var isDPadLeftActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft);
                        var isDPadRightActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight);
                        var isAActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A);
                        var isXActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X);
                        var isYActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y);
                        var isBActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B);

                        string getState()
                        {
                            return $"{isBackActive}{isStartActive}{isLeftTriggerActive}{isRightTriggerActive}{isLeftShoulderActive}{isRightShoulderActive}{isLeftThumbActive}{isRightThumbActive}{isDPadUpActive}{isDPadDownActive}{isDPadLeftActive}{isDPadRightActive}{isAActive}{isXActive}{isYActive}{isBActive}";
                        }

                        currentState = getState();
                        if (string.IsNullOrWhiteSpace(previousState))
                        {
                            previousState = currentState;
                        }

                        if (currentState != previousState)
                        {
                            previousState = getState();

                            if (isLeftThumbActive && isRightThumbActive)
                            {
                                if (isXActive)
                                {
                                    OnXboxServiceProcessKillRequested();
                                }
                            }

                            if (isLeftShoulderActive && isRightShoulderActive)
                            {
                                _inputSimulator.Keyboard.KeyDown(VirtualKeyCode.MENU);
                                _inputSimulator.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
                                _inputSimulator.Keyboard.KeyDown(VirtualKeyCode.TAB);
                                _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.MENU);
                                _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
                                _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.TAB);
                            }

                            if (isPowerActive)
                            {
                                isPowerActive = false;

                                if (isPowerActiveStartDate == null)
                                {
                                    throw new InvalidOperationException("isPowerActiveStartDate should not be null");
                                }
                                else if ((DateTime.Now - isPowerActiveStartDate.Value).TotalSeconds >= 3)
                                {
                                    isPowerActiveStartDate = null;
                                    _logger.LogInformation("Power Off");
                                    PowerOff();
                                }
                                else
                                {
                                    isPowerActiveStartDate = null;
                                    _logger.LogInformation("Power On");
                                    PowerOn();
                                }
                            }
                            else
                            {
                                isPowerActive = isBackActive && isStartActive;
                                isPowerActiveStartDate = DateTime.Now;
                            }
                        }

                        await Task.Delay(100);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "XXXXX");
                }
            } while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token));
        });
    }

    public void StartBackgroundLoopForSoundAndMouseManagement()
    {
        Task.Run(async () =>
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            do
            {
                try
                {
                    while (CurrentState?.IsActive == true)
                    {
                        var foregroundProcess = _processManager.GetForegroundProcess();
                        var isForegroundWindowInFullscreenMode = _windowManager.IsForegroundWindowInFullscreenMode() && foregroundProcess.Name != "explorer";
                        if (isForegroundWindowInFullscreenMode)
                        {
                            await Task.Delay(1000);
                        }
                        else
                        {
                            var state = _selectedGameController.GetState();
                            var isLeftThumbActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
                            ////var isRightThumbActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb);
                            var volumeUpRequested = isLeftThumbActive && (state.Gamepad.LeftThumbX > 20000 || state.Gamepad.LeftThumbY > 20000);
                            var volumeDownRequested = isLeftThumbActive && (state.Gamepad.LeftThumbX < -20000 || state.Gamepad.LeftThumbY < -20000);
                            var mouseMoveRequested = !isLeftThumbActive && (state.Gamepad.LeftThumbX > 20000 || state.Gamepad.LeftThumbY > 20000 || state.Gamepad.LeftThumbX < -20000 || state.Gamepad.LeftThumbY < -20000);
                            var isAActive = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A);

                            if (volumeUpRequested)
                            {
                                _logger.LogInformation("Volume up");
                                _soundManager.IncreaseVolume(2);
                            }

                            if (volumeDownRequested)
                            {
                                _logger.LogInformation("Volume down");
                                _soundManager.DecreaseVolume(2);
                            }

                            if (mouseMoveRequested)
                            {
                                if (state.Gamepad.LeftThumbX > 20000)
                                {
                                    _inputSimulator.Mouse.MoveMouseBy(20, 0);
                                }

                                if (state.Gamepad.LeftThumbY > 20000)
                                {
                                    _inputSimulator.Mouse.MoveMouseBy(0, -20);
                                }

                                if (state.Gamepad.LeftThumbX < -20000)
                                {
                                    _inputSimulator.Mouse.MoveMouseBy(-20, 0);
                                }

                                if (state.Gamepad.LeftThumbY < -20000)
                                {
                                    _inputSimulator.Mouse.MoveMouseBy(0, 20);
                                }
                            }

                            if (isAActive)
                            {
                                _inputSimulator.Mouse.LeftButtonClick();
                            }

                            await Task.Delay(100);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "XXXXX");
                }
            } while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token));
        });
    }

    #region Private use

    private void PowerOn()
    {
        foreach (var fullscreenApp in _fullscreenApps)
        {
            _processManager.Start(
                filePath: fullscreenApp.Item2,
                arguments: fullscreenApp.Item3,
                waitForExit: false);
        }

        _cecService.SwitchToComputerHDMISourceAsync(_appSettings.cecConfig.computerHDMISource);
    }

    private void PowerOff()
    {
        _cecService.SwitchToComputerHDMISourceAsync(_appSettings.cecConfig.defaultHDMISource);
    }

    private void OnXboxServiceProcessKillRequested()
    {
        _logger.LogInformation("Kill game process");

        var allProcesses = _processManager.GetAll();
        var hasKilledAtLeastOneProcess = false;

        foreach (var gameProcess in _appSettings.gameProcesses)
        {
            if (allProcesses.Any(obj => obj.Name == gameProcess))
            {
                try
                {
                    _processManager.Stop(gameProcess);
                    _logger.LogInformation($"{gameProcess} killed");
                    hasKilledAtLeastOneProcess = true;

                    Thread.Sleep(1000);

                    var oldDisplay = _displayManager.GetCurrent();
                    var newDisplay = _displayManager.GetHigher();

                    if (oldDisplay.Width < newDisplay.Width || oldDisplay.Height < newDisplay.Height)
                    {
                        _logger.LogInformation($"Old display : {oldDisplay.Width} x {oldDisplay.Height}");
                        _displayManager.SetCurrent(newDisplay);
                        _logger.LogInformation($"New display : {newDisplay.Width} x {newDisplay.Height}");
                    }
                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }

        if (!hasKilledAtLeastOneProcess)
        {
            var foregroundProcess = _processManager.GetForegroundProcess();

            try
            {
                _processManager.Stop(foregroundProcess.Name);
                _logger.LogInformation($"{foregroundProcess.Name} killed");
            }
            catch (Exception)
            {
                // Ignore
            }
        }
    }

    #endregion
}
