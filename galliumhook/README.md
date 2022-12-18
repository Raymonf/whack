# galliumhook

Loads `mercuryhook.dll` at startup and then forces created windows to be 1080x1920 regardless of screen resolution (or original size).

If you want to configure galliumhook without recompiling it, make a file named `galliumhook.toml` and put this in:

```toml
[window]
width = 1080
height = 1920
wndproc = 0x72AE80
```

Note that sane defaults (for 3.07.01 ONLY) will be applied when this config file isn't found. Set wndproc to 0 to disable the WndProc hook.