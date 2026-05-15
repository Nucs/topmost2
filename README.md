<p align="center">
	<a href="https://github.com/Nucs/topmost2"><img src="https://i.imgur.com/r7PW6a2.png" alt="IntroIcon" width="100"></a>
</p>
<h3 align="center">TopMost2</h3>
<p align="center">This tool allows you to make any windows always on top.</p>
<p align="center">A modernized .NET 10 fork of <a href="https://github.com/Jerrylum/topmost2">Jerrylum/topmost2</a>.</p>

<h4 align="center"><a href="#build-from-source">Build from Source</a> · <a href="#fork-changes">Fork Changes</a> · <a href="#license">License</a></h4>

---

### Double Click

Double Click the tray icon, make the current selected window stay on top.

<h5 align="left">
<img src="https://i.imgur.com/kuBflkz.gif">
</h5>

<br>

### Global Hotkey

Use the default hotkey `Ctrl + Alt + Space` to make the current selected window stay on top.

<h5 align="left">
<img src="https://i.imgur.com/NokjMLd.gif">
</h5>
<br>

### Change The Hotkey

Right click the tray icon and go to `options` page to change the hotkey to your own favorite combination.

<h5 align="left">
<img src="https://i.imgur.com/LfNdpHR.gif">
</h5>

<br>

### Window List

Click on a menu item from the `Window List` to pin or unpin any windows.

<h5 align="left">
<img src="https://i.imgur.com/6KIfi3d.gif">
</h5>

<br>

### Other Features

- Dynamic icon
- Clear all function
- Automatically startup option
- Enable/Disable keyboard shortcut option
- Freely customizable hotkey
- Global hotkey
- Command-line support
- PowerToys-style on/off sound cues
- High compatibility with other programs  
- Negligible system resources usage



---

### Why do I need this?

`Topmost` or `Always On Top` is a property of every window you see on your computer. A window whose Topmost property is set to `true` appears above all windows whose Topmost properties are set to `false`.  <br>

Many windows applications don’t offer an option to make itself topmost. When you are browsing multiple windows at the same time, this may make you annoyed by frequent switching to different windows. With TopMost2,  you can add this feature to any applications and solve the above problem.



### Details

- **Tray Icon**  
  The color of the icon represents the top-most state of the current selected window.  
  🟥RED = Normal, 🟩GREEN = Top-most
  
- **Clear All Function**  
  This function will set all windows to normal states.
  
- **Elevated Privileges**  
  If you are trying to set an elevated window, TopMost2 will ask you to elevate the privileges in order to have higher permission to finish the action. Obviously, the reason is that they are protected by the operating system. You can also start TopMost2 as administrator to avoid the above problem.
  
- **Hotkey**  
  You can freely set any hotkey combinations. By clicking the `Edit` button, you can then press a new combination. After that, click `Done` to finish. If you leave or close the option form. The hotkey setting will be auto-saved by the system.  
  ![Hot Key Demo](https://i.imgur.com/jGFi1tC.gif)  
  If TopMost2 starts with normal permission, it may not be able to read the input of the keyboard in the elevated window.

- **Sound Cues**
  TopMost2 plays the same Windows media cues used by Microsoft PowerToys Always On Top: `Speech On.wav` when a window is pinned and `Speech Sleep.wav` when it is unpinned. If those files are unavailable, it falls back to built-in system sounds.

- **Exit**  
  This function will set all windows to normal state and shut down the program.


### Command Line

Usage:

```powershell
.\topmost2 [--autostart] {action hWnd}
```

**action:**

- Set top-most: `--set` or `-S` or `/S`
- Remove top-most: `--remove` or `-R` or `/R`

**hWnd:**

The handle pointer in hexadecimal. HWND is a "handle to a window" and is part of the Win32 API.

<br>

For example:

```powershell
.\topmost2 -S 0x311A0 -S 0x190D4E
.\topmost2 -R 0x311A0
```


---

### Fork Changes

This fork keeps the original tray-based workflow and command-line interface, while updating the project for current Windows and .NET tooling.

- Migrated the project from .NET Framework 4.7.2 to SDK-style `net10.0-windows`.
- Added self-contained single-file `win-x64` publishing support.
- Fixed executable path detection for .NET single-file builds.
- Updated the global keyboard hook interop for modern 64-bit .NET.
- Made the options window reliably come to the front from the tray menu.
- Added PowerToys-style sound cues for pinning and unpinning windows.


### Build from Source

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), then publish a portable single-file build:

```powershell
dotnet publish TopMost\TopMost2.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:EnableCompressionInSingleFile=true `
  -p:DebugType=none `
  -p:DebugSymbols=false
```

The executable will be generated under:

```powershell
TopMost\bin\Release\net10.0-windows\win-x64\publish\TopMost2.exe
```


### Install at Startup

TopMost2 can still be configured from the options window to start with Windows. For a portable install, copy the published `TopMost2.exe` somewhere stable and create a shortcut to it in `shell:startup`.


### Security Notes

TopMost2 does not use network access or third-party runtime packages. The app uses expected Windows APIs for this kind of utility: a low-level keyboard hook for the hotkey, `SetWindowPos` for top-most state, HKCU registry values for settings, and optional UAC elevation when changing elevated windows.


### Contributing

Issues and pull requests are welcome. Please keep changes focused, describe the Windows version and .NET SDK used for testing, and include screenshots or short reproduction steps for tray, hotkey, and window-focus behavior.



---

### Other Software

There are similar software like [DeskPins](https://efotinis.neocities.org/deskpins/) and [Window TopMost Control](https://www.sordum.org/9182/window-topmost-control-v1-2/). I am trying to compare them in several ways. Keep in mind, everyone has different opinion so this comparison is for reference only.



|                                                 | TopMost2          | DeskPins                 | Window TopMost Control |
| ----------------------------------------------- | ----------------- | ------------------------ | ---------------------- |
| Set Elevated application's Window <sup>#0</sup> | ✔️                 | ✔️ <sup>#1</sup>          | ✔️                      |
| Command Line Support                            | ✔️                 | ❌                        | ✔️                      |
| Portable                                        | ✔️                 | ❌                        | ✔️                      |
| Auto Start                                      | ✔️                 | ❌                        | ✔️                      |
| Auto Pin                                        | ❌                 | ✔️                        | ❌                      |
| Open Source                                     | ✔️                 | ✔️                        | ❌                      |
| State visibility                                | 🟡Good             | 🟢Excellent <sup>#2</sup> | 🟠Limited <sup>#3</sup> |
| CPU Usage                                       | 🟢Least            | 🟠Highest                 | 🟡Medium                |
| Customize                                       | 🟡Good             | 🟢Excellent               | 🟡Good                  |
| Compatibility With Programs                     | 🟢Excellent        | 🟡Good <sup>#4</sup>      | 🟢Excellent             |
| Hotkey                                          | More Combinations | More shortcuts           | Limited                |
| Size                                            | 47KB              | 103KB                    | 680KB                  |

#0 Able to change a window that belongs to a process with elevated privileges (run as the administrator).  
#1 Only if the application starts as administrator. Otherwise, trying to do that will cause unknown behavior.  
#2 Pin icon at the top-right corner of the top-most window.  
#3 Only provide the "Window List" feature.  
#4 Not Compatible with windows which also have top-most setting.  

<br> 

---

### Download

The original .NET Framework release is available from [Jerrylum/topmost2 releases](https://github.com/Jerrylum/topmost2/releases).
This fork is intended to be built from source with .NET 10 and published as a self-contained Windows executable.

<br>

### License

TopMost2 is licensed under the [MIT License](LICENSE). This fork keeps the original license and attribution.

### Special Thanks

Thanks to [Jerrylum](https://github.com/Jerrylum) for the original TopMost2 project.
Thanks to [SamNg](https://github.com/ngkachunhlp) and [COMMANDER.WONG](https://github.com/COMMANDERWONG) for their suggestions and software testing.
