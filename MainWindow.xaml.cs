namespace FilesDropper;

using System.Text;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;

public partial class MainWindow : Window, INotifyPropertyChanged {
    MessageBoxResult mbResult = MessageBoxResult.None;
    bool canceled => mbResult == MessageBoxResult.Cancel;
    bool consumed; // Handle for the background worker.
    HashSet<string> visitedPaths = []; // Handles symbolic link edge-case.

    public bool isControlHeld { get; set; }
    public bool isAltHeld { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;


    public MainWindow() {
        InitializeComponent();
        PreviewKeyDown += (_, e) => { if (e.Key == Key.Escape) { Close(); } };
        _ = CheckForKeyModifierState();

        // Background worker that checks and updates the keyboard modifiers every now and then.
        async Task CheckForKeyModifierState() {
            while (!consumed) {
                var (wasControlHeld, wasAltHeld) = (isControlHeld, isAltHeld);
                isAltHeld     = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);
                isControlHeld = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                if (wasControlHeld != isControlHeld) { PropertyChanged?.Invoke(this, new(nameof(isControlHeld))); }
                if (wasAltHeld != isAltHeld) { PropertyChanged?.Invoke(this, new(nameof(isAltHeld))); }
                await Task.Delay(100);
            }
        }
    }


    void OnDrop(object sender, DragEventArgs e) {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) { return; }
        consumed = true; // Mark the app as consumed so the task exits.

        // When drag & dropping content from some apps (e.g. VSCode), the FileDrop format is not recognized.
        string[] filePaths;
        try { filePaths = (string[]) e.Data.GetData(DataFormats.FileDrop); }
        catch { MessageBox.Show(this, $"Invalid filedrop format.\nCross-app filedrop is not supported.", "Not supported types"); Close(); return; }

        // Reaching here means we got some juicy files to process.
        var sb = new StringBuilder(); // Find the appropriate mode and let's go!
        var (altMode1, altMode2) = (isControlHeld, isControlHeld && isAltHeld);
        foreach (var filePath in filePaths) {
            if (altMode2) { AppendFolderSizesRecursively(filePath, sb); }    // Alter.2 mode -- Copy the folder sizes to clipboard.
            else if (altMode1) { AppendStructureRecursively(filePath, sb); } // Alter.1 mode -- Copy the structure to clipboard.
            else { AppendFileTextContentsRecursively(filePath, sb); }        // Content mode -- Copy all file contents to clipboard.
        }

        // Set the clipboard to the text we worked so hard on forming.
        if (!canceled) { Clipboard.SetText(sb.Replace("\r\n", "\n").ToString().Trim()); }
        Exit(this, null); // Job done. We can now exit the app.
    }

    void DragWindow(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); } }
    void Exit(object sender, MouseButtonEventArgs e) => Close();

    /// <summary> When called for files, it appends the filename and contents of that file into the sb. </summary>
    /// <remarks> For directories, it recursively calls this method again on their content -- after confirmation from user. </remarks>
    void AppendFileTextContentsRecursively(string path, StringBuilder sb) {
        if (canceled || !visitedPaths.Add(path)) { return; }

        // The first time a directory is met (if any), pop up a MessageBox that allows confirming/denying access to those files.
        if (Directory.Exists(path)) {
            if (mbResult == MessageBoxResult.None) {
                mbResult = MessageBox.Show(this, "Do you want to include files inside directories?\nIf you press \"No\", only top-level files will be copied.\n\nPressing \"Escape\" or \"Cancel\" will abort completely.", "Include directory contents?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (mbResult == MessageBoxResult.Cancel) { Close(); }
            }
            if (mbResult == MessageBoxResult.Yes) {
                var subFiles = Directory.EnumerateFileSystemEntries(path);
                foreach (var subFilePath in subFiles) { AppendFileTextContentsRecursively(subFilePath, sb); }
            }
            return;
        }

        // Make sure non-text files are not copied, we're not interested in them.
        try { // This also acts as a test run to check permissions.
            if (File.ReadAllBytes(path).Contains((byte) 0)) { return; }
        }
        catch (Exception e) {
            var result = MessageBox.Show(this, $"{path}:\n\n{e.Message}\n\n\nContinue anyway? The rest of the files may still be accessible.", "A file cannot be read. Do you wish to continue anyway?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) { mbResult =  MessageBoxResult.Cancel; Close(); }
            return;
        }

        // If all went well, append the header and content of the file to the sb.
        var fileExt = Path.GetExtension(path).Trim('.');
        sb.AppendLine($"### `{Path.GetFileName(path)}`:");
        sb.AppendLine($"\n```{fileExt}\n{File.ReadAllText(path).Trim()}\n```\n");
    }

    /// <summary> Recursively lists files & subfiles of the given path with proper indentation. Directories as well. </summary>
    /// <remarks> Files are listed directly, while directories are traversed to include their contents hierarchically. </remarks>
    void AppendStructureRecursively(string path, StringBuilder sb, string indent = "", bool isLast = true) {
        if (!visitedPaths.Add(path)) { return; }

        if (File.Exists(path)) { sb.AppendLine($"{indent}{(isLast ? "└─" : "├─")} 📄 {Path.GetFileName(path)}"); }
        else if (Directory.Exists(path)) {
            sb.AppendLine($"{indent}{(isLast ? "└─" : "├─")} 📁 {Path.GetFileName(path)}");

            var subPaths = Directory.EnumerateFileSystemEntries(path).OrderBy(p => Directory.Exists(p) ? 0 : 1).ThenBy(p => Path.GetFileName(p)).ToList();
            for (int i = 0; i < subPaths.Count; i++) { AppendStructureRecursively(subPaths[i], sb, indent + (isLast ? "    " : "│   "), i == subPaths.Count - 1); }
        }
    }

    /// <summary> Recursively lists directories with proper indentation and displays info about its contents. Files are not rendered. </summary>
    /// <remarks> Each directory is appended by the summed size of ALL included files (in MB), and total (recursive) file count. </remarks>
    void AppendFolderSizesRecursively(string path, StringBuilder sb, string indent = "", bool isLast = true) {
        if (!visitedPaths.Add(path) || !Directory.Exists(path)) { return; }

        var (totalSize, fileCount) = (0L, 0);
        foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)) { totalSize += new FileInfo(file).Length; fileCount++; }

        var mb = Math.Round(totalSize / (1024.0 * 1024));
        var fileInfo = mb == 0 ? $"({fileCount} files)" : $"({mb} MB, {fileCount} files)";
        sb.AppendLine($"{indent}{(isLast ? "└─" : "├─")} 📁 {Path.GetFileName(path)} {fileInfo}");

        var subDirs = Directory.EnumerateDirectories(path).OrderBy(Path.GetFileName).ToList();
        for (int i = 0; i < subDirs.Count; i++) { AppendFolderSizesRecursively(subDirs[i], sb, indent + (isLast ? "    " : "│   "), i == subDirs.Count - 1); }
    }
}
