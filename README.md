# Trayscout
This lightweight Nightscout client for Windows is written in C# and will display your latest blood glucose value and trend in the system tray.

![Preview](Preview.png)

A simple INI file is used for configuration:
- BaseUrl = URL of your Nightscout instance
- APISecret = API secret of your Nightscout instance
- UpdateInterval = Blood glucose update interval in minutes
- High = Upper limit of your normal range
- Low = Lower limit of your normal range
- UseColor = Whether to use different colors (red = high, low = blue, normal = green) instead of white
- UseAlarm = Whether to play an alarm sound when out of normal range
- AlarmInterval = Alarm interval in minutes

How to use:
- Extract the Trayscout.zip into a folder of your choice
- Open Config.ini in Notepad > Set at least BaseUrl and APISecret > Save and close
- Run Trayscout.exe, now you should see your blood glucose value in the system tray
  - If not, an error window should pop up and tell you what's wrong

Always display tray icon:
- Right click on taskbar > Taskbar settings > Notification area > Select which icons appear on the taskbar
- Enable Trayscout

Run on startup:
- Right click on Trayscout.exe > Create shortcut > Cut (CTRL+X)
- Windows + R > shell:startup > Enter > Paste (CTRL+V)

Customization:
- If you like, you can change the font/color and the alarm sound by editing or replacing the corresponding files
