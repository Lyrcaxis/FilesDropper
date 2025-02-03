[![GitHub Release](https://img.shields.io/github/release/Lyrcaxis/FilesDropper.svg)](https://github.com/Lyrcaxis/FilesDropper/releases)

# FilesDropper
*A magical drag-and-drop clipboard wizard for developers*

![FilesDropper_Modes](https://github.com/user-attachments/assets/c8a8c051-b182-401e-b467-a391910b0b0e)


With the absurd rise of AI popularity, being able to easily share a list or visualization of files can come handy. FilesDropper addresses those needs by providing a minimal, lightweight, and easily accessible multi-purpose UI.

![image](https://github.com/user-attachments/assets/853f70d2-c54e-47d2-b94c-866d70549d9f)

**Become a power user too with the click of one button!** *(I hope I'm doing that marketing thing right)*

## Features
- Transparent overlay UI (always on top)
- Copies info from files you drop onto it into your clipboard, then exits seamlessly
- Auto-detects and ignores non-text files (prevents junk in clipboard)
- Drag & Drop files and folders from within Visual Studio!
- Provides THREE different ways to visualize your files
- Right-click or press escape to exit instantly
- For absolute transparency, releases are automated by github workflow
- Single and portable EXE - no dependencies needed (-.NET runtime)

---------------------

## Three different modes:

### Plain Text Mode
When you drop files into the window, you get the content back as text, directly in your clipboard.

https://github.com/user-attachments/assets/eef69c80-fe96-4d3b-ad3e-f0457b39fb81

###### This can/will include whole directories (after your confirmation). Allows easy paste of whole projects.
---------------------

### File hierarchy mode (Ctrl+Drop)
If you hold `Ctrl` while dropping the files, you get back a visualization of the hierarchy instead of file contents.

https://github.com/user-attachments/assets/04ccc65f-bbb8-4fdf-af65-9abf47069905

###### Allows textual visualization of project structures for easy debugging. Check how pretty it looks on the ^ video!
---------------------

### Folder Info mode (Ctrl+Alt+Drop)
With `Ctrl+Alt+Drop`, you retrieve a hierarchical visualization of directories, including their size and file counts.

https://github.com/user-attachments/assets/4476fa12-674d-4b24-9e0c-48019f4932f5

###### The files are not rendered as icons in this mode, but as numbers instead. Useful to analyze and optimize large projects at a glance.
---------------------

## Usage
1. Download the latest version from the [Github Releases Page](https://github.com/Lyrcaxis/FilesDropper/releases).
2. (Optional, but highly recommended) Pin to Taskbar for easy access.
3. Drop some files in! Hold `Ctrl` or `Ctrl+Alt` while dropping for different modes.
4. The text will be copied onto your clipboard directly, and the app will automatically exit.

![image](https://github.com/user-attachments/assets/d0c3fb67-5ace-40e7-a7aa-d4eebb19ee38)
<----- FilesDropper's rightful place

**Tips for Safe Usage**
- Clipboard contents can potentially be accessed by other applications on your system. While FilesDropper is completely safe, it's always a good idea to ensure your system is malware-free when working with sensitive data.
- Make sure you're not using a heavily outdated browser. Modern browsers protect your clipboard from websites.
- Only download applications from their official source. For FilesDropper, that's the [Github Releases page](https://github.com/Lyrcaxis/FilesDropper/releases).
- For full transparency, this app's code is open source, and the build/upload are automated by Github from source. This section was not made to scare people, but rather to promote safe software usage and prevent mishaps.
---------------------

## Notes
- FilesDropper should be able to handle symbolic links properly.
- Color shifts indicate active mode (blue/purple/dark blue).
- Files that can't be accessed are skipped, and an info dialog will notify you.
- Sadly, I couldn't make it support VSCode. Open to contributions as it would be awesome!
- Also, I'm not the **best** UI/UX designer. Your UI/UX improvements and ideas are welcome!

## License
- This project is licensed under the [MIT License](https://github.com/Lyrcaxis/FilesDropper/blob/main/LICENSE).
