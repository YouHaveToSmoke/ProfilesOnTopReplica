using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OnTopReplica.WindowSeekers;

namespace OnTopReplica {
    
    /// <summary>
    /// Contains profile-related functionality of MainForm.
    /// </summary>
    partial class MainForm {
        
        /// <summary>
        /// Creates a profile region configuration from the current window state.
        /// </summary>
        private ProfileRegionConfiguration CreateRegionConfigurationFromCurrentState(string configName, RegionAnchor regionAnchor, bool scaleWithSource) {
            var config = new ProfileRegionConfiguration(configName);
            config.ScaleWithSourceWindow = scaleWithSource;

            // Region settings
            if (SelectedThumbnailRegion != null) {
                config.HasRegion = true;
                config.RegionIsRelative = SelectedThumbnailRegion.Relative;
                config.RegionAnchor = regionAnchor;

                var originalWindowSize = Size.Empty;
                if (_thumbnailPanel.IsShowingThumbnail) {
                    originalWindowSize = _thumbnailPanel.ThumbnailOriginalSize;
                }

                // Get the internal bounds based on the mode
                if (SelectedThumbnailRegion.Relative) {
                    // In relative mode, store as padding (Left, Top, Right, Bottom)
                    var padding = SelectedThumbnailRegion.BoundsAsPadding;
                    config.RegionBounds = new Rectangle(
                        padding.Left,
                        padding.Top,
                        padding.Right,
                        padding.Bottom
                    );
                }
                else {
                    // In absolute mode, store absolute pixel distance from anchor
                    var bounds = SelectedThumbnailRegion.Bounds;

                    // Store absolute bounds
                    config.RegionBounds = bounds;

                    // Calculate absolute pixel distance from the selected anchor
                    int distX, distY;

                    switch (regionAnchor) {
                        case RegionAnchor.TopLeft:
                            distX = bounds.X;
                            distY = bounds.Y;
                            break;

                        case RegionAnchor.TopRight:
                            distX = originalWindowSize.Width - bounds.Right;
                            distY = bounds.Y;
                            break;

                        case RegionAnchor.BottomLeft:
                            distX = bounds.X;
                            distY = originalWindowSize.Height - bounds.Bottom;
                            break;

                        case RegionAnchor.BottomRight:
                            distX = originalWindowSize.Width - bounds.Right;
                            distY = originalWindowSize.Height - bounds.Bottom;
                            break;

                        default:
                            distX = bounds.X;
                            distY = bounds.Y;
                            break;
                    }

                    config.AbsoluteDistanceFromAnchor = new Point(distX, distY);

                    Log.Write($"Saved region with {regionAnchor} anchor: distance({distX}px, {distY}px), size({bounds.Width}px, {bounds.Height}px)");
                }
            }
            else {
                config.HasRegion = false;
            }

            // Window position and size
            config.WindowLocation = this.Location;
            config.WindowSize = this.ClientSize; // Store ClientSize to avoid DWM border issues

            // Calculate relative offset to target window (if available)
            if (CurrentThumbnailWindowHandle != null) {
                try {
                    var targetWindowRect = Native.WindowMethods.GetWindowRectangle(CurrentThumbnailWindowHandle.Handle);
                    var targetSize = new Size(targetWindowRect.Width, targetWindowRect.Height);

                    // Absolute offset in pixels
                    var offsetX = this.Location.X - targetWindowRect.Left;
                    var offsetY = this.Location.Y - targetWindowRect.Top;

                    config.RelativeOffsetToTargetWindow = new Point(offsetX, offsetY);
                    config.TargetWindowSize = targetSize;

                    // Relative offset as percentage of target window size
                    if (targetSize.Width > 0 && targetSize.Height > 0) {
                        config.RelativeOffsetPercent = new PointF(
                            (float)offsetX / targetSize.Width,
                            (float)offsetY / targetSize.Height
                        );
                        Log.Write($"Saved relative offset: pixels({offsetX}, {offsetY}), percent({config.RelativeOffsetPercent.Value.X:P2}, {config.RelativeOffsetPercent.Value.Y:P2}), target size({targetSize.Width}x{targetSize.Height})");
                    }
                }
                catch (Exception ex) {
                    Log.Write($"Warning: Could not calculate relative offset to target window: {ex.Message}");
                    config.RelativeOffsetToTargetWindow = null;
                    config.RelativeOffsetPercent = null;
                    config.TargetWindowSize = null;
                }
            }

            // Visual settings
            config.Opacity = this.Opacity;
            config.ClickThrough = this.ClickThroughEnabled;
            config.ClickForwarding = this.ClickForwardingEnabled;
            config.ChromeVisible = this.IsChromeVisible;
            config.PositionLock = this.PositionLock;
            config.TopMost = this.TopMost;

            return config;
        }
        
        /// <summary>
        /// Creates a profile from the current window state.
        /// </summary>
        public Profile CreateProfileFromCurrentState(string profileName, string regionName, RegionAnchor regionAnchor, bool scaleWithSource) {
            var profile = new Profile(profileName);

            // Window information
            if (CurrentThumbnailWindowHandle != null) {
                profile.WindowHandle = (long)CurrentThumbnailWindowHandle.Handle;
                profile.WindowTitle = CurrentThumbnailWindowHandle.Title;
                profile.WindowClass = CurrentThumbnailWindowHandle.Class;

                // Store original window size for region transformation
                if (_thumbnailPanel.IsShowingThumbnail) {
                    profile.OriginalWindowSize = _thumbnailPanel.ThumbnailOriginalSize;
                }

                // Try to get process name
                try {
                    var process = Process.GetProcessById(
                        Native.WindowMethods.GetWindowProcessId(CurrentThumbnailWindowHandle.Handle));
                    profile.ProcessName = process.ProcessName;
                }
                catch {
                    profile.ProcessName = null;
                }
            }

            // Create and add the region configuration
            var config = CreateRegionConfigurationFromCurrentState(regionName, regionAnchor, scaleWithSource);
            profile.RegionConfigurations.Add(config);

            return profile;
        }
        
        /// <summary>
        /// Adds a new region configuration to an existing profile.
        /// </summary>
        public void AddRegionToProfile(Profile profile, string regionName, RegionAnchor regionAnchor, bool scaleWithSource) {
            if (profile == null) {
                throw new ArgumentNullException(nameof(profile));
            }

            // Update profile metadata
            profile.LastModified = DateTime.Now;

            // Store original window size if not already set
            if (profile.OriginalWindowSize == Size.Empty && _thumbnailPanel.IsShowingThumbnail) {
                profile.OriginalWindowSize = _thumbnailPanel.ThumbnailOriginalSize;
            }

            // Create and add the new configuration
            var config = CreateRegionConfigurationFromCurrentState(regionName, regionAnchor, scaleWithSource);
            profile.RegionConfigurations.Add(config);

            // Save the updated profile
            ProfileManager.SaveProfile(profile);
        }
        
        /// <summary>
        /// Applies a profile configuration to the current window.
        /// </summary>
        public void ApplyProfileConfiguration(Profile profile, ProfileRegionConfiguration config) {
            if (profile == null) {
                throw new ArgumentNullException(nameof(profile));
            }
            if (config == null) {
                throw new ArgumentNullException(nameof(config));
            }
            
            Log.Write($"Applying profile configuration: {profile.Name} - {config.Name}");
            
            // First, try to find and set the window
            WindowHandle handle = FindWindowForProfile(profile);
            
            if (handle == null) {
                Log.Write("Warning: Could not find window for profile " + profile.Name);
                MessageBox.Show(
                    $"Das Fenster für Profil '{profile.Name}' konnte nicht gefunden werden.\n\n" +
                    $"Prozess: {profile.ProcessName ?? "N/A"}\n" +
                    $"Klasse: {profile.WindowClass ?? "N/A"}\n" +
                    $"Titel: {profile.WindowTitle ?? "N/A"}",
                    "Fenster nicht gefunden",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            
            // Apply region if specified
            ThumbnailRegion region = null;
            if (config.HasRegion) {
                var currentWindowSize = Native.WindowMethods.GetWindowSize(handle.Handle);

                // Use absolute distance if available
                if (config.AbsoluteDistanceFromAnchor.HasValue && currentWindowSize != Size.Empty && currentWindowSize.Width > 0 && currentWindowSize.Height > 0) {
                    var absDist = config.AbsoluteDistanceFromAnchor.Value;

                    if (config.RegionIsRelative) {
                        // For relative mode, use absolute bounds
                        var bounds = config.RegionBounds;
                        region = new ThumbnailRegion(bounds, true);
                        Log.Write($"Using relative region padding: {bounds}");
                    }
                    else {
                        // For absolute mode: FIXED size, position from ABSOLUTE pixel distance to anchor
                        int width = config.RegionBounds.Width;
                        int height = config.RegionBounds.Height;

                        // Clamp size
                        width = Math.Min(width, currentWindowSize.Width);
                        height = Math.Min(height, currentWindowSize.Height);

                        int x, y;

                        switch (config.RegionAnchor) {
                            case RegionAnchor.TopLeft:
                                // Position = distance from top-left
                                x = absDist.X;
                                y = absDist.Y;
                                break;

                            case RegionAnchor.TopRight:
                                // Position = windowWidth - distanceFromRight - width
                                x = currentWindowSize.Width - absDist.X - width;
                                y = absDist.Y;
                                break;

                            case RegionAnchor.BottomLeft:
                                // Position = windowHeight - distanceFromBottom - height
                                x = absDist.X;
                                y = currentWindowSize.Height - absDist.Y - height;
                                break;

                            case RegionAnchor.BottomRight:
                                // Position from both edges
                                x = currentWindowSize.Width - absDist.X - width;
                                y = currentWindowSize.Height - absDist.Y - height;
                                break;

                            default:
                                x = absDist.X;
                                y = absDist.Y;
                                break;
                        }

                        // Clamp
                        x = Math.Max(0, Math.Min(x, currentWindowSize.Width - width));
                        y = Math.Max(0, Math.Min(y, currentWindowSize.Height - height));

                        var bounds = new Rectangle(x, y, width, height);
                        region = new ThumbnailRegion(bounds, false);

                        Log.Write($"Applied {config.RegionAnchor} anchor: absDistance({absDist.X}px,{absDist.Y}px) -> pos({x},{y}) size({width}x{height})");
                    }
                }
                else {
                    // Fallback: use absolute bounds
                    var bounds = config.RegionBounds;

                    if (currentWindowSize != Size.Empty && !config.RegionIsRelative) {
                        var clampedX = Math.Max(0, Math.Min(bounds.X, currentWindowSize.Width - 1));
                        var clampedY = Math.Max(0, Math.Min(bounds.Y, currentWindowSize.Height - 1));
                        var clampedWidth = Math.Min(bounds.Width, currentWindowSize.Width - clampedX);
                        var clampedHeight = Math.Min(bounds.Height, currentWindowSize.Height - clampedY);

                        bounds = new Rectangle(clampedX, clampedY, clampedWidth, clampedHeight);
                        Log.Write($"Using clamped absolute bounds: {bounds}");
                    }

                    region = new ThumbnailRegion(bounds, config.RegionIsRelative);
                }
            }
            
            // Set the thumbnail
            SetThumbnail(handle, region);

            // Apply window position and size
            this.StartPosition = FormStartPosition.Manual;

            // Use relative offset if available, otherwise fall back to absolute position
            if (config.RelativeOffsetPercent.HasValue && config.TargetWindowSize.HasValue && handle != null) {
                try {
                    var targetWindowRect = Native.WindowMethods.GetWindowRectangle(handle.Handle);
                    var currentTargetSize = new Size(targetWindowRect.Width, targetWindowRect.Height);

                    // Calculate new offset based on current target window size
                    // Use percentage to scale with resolution changes
                    int offsetX = (int)(config.RelativeOffsetPercent.Value.X * currentTargetSize.Width);
                    int offsetY = (int)(config.RelativeOffsetPercent.Value.Y * currentTargetSize.Height);

                    var newLocation = new Point(
                        targetWindowRect.Left + offsetX,
                        targetWindowRect.Top + offsetY
                    );

                    this.Location = newLocation;
                    Log.Write($"Positioned replica relative to target (scaled): target@({targetWindowRect.Left},{targetWindowRect.Top}) size({currentTargetSize.Width}x{currentTargetSize.Height}), offset({offsetX}px,{offsetY}px) -> replica@({newLocation.X},{newLocation.Y})");

                    if (config.TargetWindowSize.Value != currentTargetSize) {
                        Log.Write($"Target window size changed: {config.TargetWindowSize.Value.Width}x{config.TargetWindowSize.Value.Height} -> {currentTargetSize.Width}x{currentTargetSize.Height}");
                    }
                }
                catch (Exception ex) {
                    Log.Write($"Warning: Could not position relative to target window, using absolute position: {ex.Message}");
                    this.Location = config.WindowLocation;
                }
            }
            else if (config.RelativeOffsetToTargetWindow.HasValue && handle != null) {
                // Fallback: use pixel offset (for old profiles without percentage)
                try {
                    var targetWindowRect = Native.WindowMethods.GetWindowRectangle(handle.Handle);
                    var newLocation = new Point(
                        targetWindowRect.Left + config.RelativeOffsetToTargetWindow.Value.X,
                        targetWindowRect.Top + config.RelativeOffsetToTargetWindow.Value.Y
                    );
                    this.Location = newLocation;
                    Log.Write($"Positioned replica relative to target window: ({newLocation.X}, {newLocation.Y})");
                }
                catch (Exception ex) {
                    Log.Write($"Warning: Could not position relative to target window, using absolute position: {ex.Message}");
                    this.Location = config.WindowLocation;
                }
            }
            else {
                this.Location = config.WindowLocation;
                Log.Write($"Using absolute position: ({config.WindowLocation.X}, {config.WindowLocation.Y})");
            }

            // Apply visual settings FIRST (especially chrome, which affects size calculation)
            this.Opacity = config.Opacity;
            this.ClickThroughEnabled = config.ClickThrough;
            this.ClickForwardingEnabled = config.ClickForwarding;
            this.IsChromeVisible = config.ChromeVisible;
            this.PositionLock = config.PositionLock;
            this.TopMost = config.TopMost;

            // Set ClientSize AFTER chrome state to avoid DWM border calculation issues
            this.ClientSize = config.WindowSize;

            Log.Write($"Applied visual settings: Opacity={config.Opacity:P0}, Chrome={config.ChromeVisible}, ClickThrough={config.ClickThrough}, ClickForward={config.ClickForwarding}, TopMost={config.TopMost}");
            Log.Write("Profile configuration applied successfully: " + config.Name);
        }
        
        /// <summary>
        /// Finds the window handle for a given profile.
        /// </summary>
        private WindowHandle FindWindowForProfile(Profile profile) {
            WindowHandle handle = null;
            
            // Try by process name first (most reliable across restarts)
            if (!string.IsNullOrEmpty(profile.ProcessName)) {
                var processes = Process.GetProcessesByName(profile.ProcessName);
                if (processes.Length > 0) {
                    foreach (var proc in processes) {
                        if (proc.MainWindowHandle != IntPtr.Zero) {
                            handle = new WindowHandle(proc.MainWindowHandle);
                            
                            if (!string.IsNullOrEmpty(profile.WindowClass) && 
                                handle.Class != profile.WindowClass) {
                                continue;
                            }
                            
                            break;
                        }
                    }
                }
            }
            
            // Try by window class if process name didn't work
            if (handle == null && !string.IsNullOrEmpty(profile.WindowClass)) {
                var seeker = new ByClassWindowSeeker(profile.WindowClass) {
                    OwnerHandle = this.Handle,
                    SkipNotVisibleWindows = true
                };
                seeker.Refresh();
                handle = seeker.Windows.FirstOrDefault();
            }
            
            // Try by window title if we still don't have a match
            if (handle == null && !string.IsNullOrEmpty(profile.WindowTitle)) {
                var seeker = new ByTitleWindowSeeker(profile.WindowTitle) {
                    OwnerHandle = this.Handle,
                    SkipNotVisibleWindows = true
                };
                seeker.Refresh();
                handle = seeker.Windows.FirstOrDefault();
            }
            
            return handle;
        }
        
        /// <summary>
        /// Shows a dialog to save the current state as a profile.
        /// </summary>
        public void ShowSaveProfileDialog() {
            using (var dialog = new Forms.ProfileNameDialog()) {
                dialog.Text = "Profil speichern";
                dialog.PromptText = "Geben Sie einen Namen für das Profil ein:";
                
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    var profileName = dialog.ProfileName;
                    var regionAnchor = dialog.RegionAnchor;
                    
                    if (ProfileManager.ProfileExists(profileName)) {
                        var result = MessageBox.Show(
                            $"Ein Profil mit dem Namen '{profileName}' existiert bereits.\nMöchten Sie eine neue Region zu diesem Profil hinzufügen?",
                            "Profil existiert bereits",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question
                        );
                        
                        if (result == DialogResult.Cancel) {
                            return;
                        }
                        else if (result == DialogResult.Yes) {
                            // Add to existing profile
                            try {
                                var existingProfile = ProfileManager.LoadProfile(profileName);

                                using (var regionDialog = new Forms.AddRegionToProfileDialog()) {
                                    if (regionDialog.ShowDialog(this) == DialogResult.OK) {
                                        AddRegionToProfile(existingProfile, regionDialog.RegionName, regionDialog.RegionAnchor, regionDialog.ScaleWithSourceWindow);
                                        MessageBox.Show(
                                            $"Region '{regionDialog.RegionName}' wurde erfolgreich zum Profil '{profileName}' hinzugefügt.",
                                            "Region hinzugefügt",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information
                                        );
                                    }
                                }
                            }
                            catch (Exception ex) {
                                MessageBox.Show(
                                    $"Fehler beim Hinzufügen der Region:\n{ex.Message}",
                                    "Fehler",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                                Log.WriteException("Error adding region to profile", ex);
                            }
                            return;
                        }
                        // If No, continue to overwrite
                    }

                    // Create new profile or overwrite
                    try {
                        using (var regionDialog = new Forms.AddRegionToProfileDialog()) {
                            if (regionDialog.ShowDialog(this) == DialogResult.OK) {
                                var profile = CreateProfileFromCurrentState(profileName, regionDialog.RegionName, regionDialog.RegionAnchor, regionDialog.ScaleWithSourceWindow);
                                ProfileManager.SaveProfile(profile);
                                
                                MessageBox.Show(
                                    $"Profil '{profileName}' wurde erfolgreich gespeichert.",
                                    "Profil gespeichert",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }
                        }
                    }
                    catch (Exception ex) {
                        MessageBox.Show(
                            $"Fehler beim Speichern des Profils:\n{ex.Message}",
                            "Fehler",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        Log.WriteException("Error saving profile", ex);
                    }
                }
            }
        }
        
        /// <summary>
        /// Shows a dialog to load a profile.
        /// </summary>
        public void ShowLoadProfileDialog() {
            using (var dialog = new Forms.ProfileSelectionDialog()) {
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    try {
                        var profile = ProfileManager.LoadProfile(dialog.SelectedProfileName);
                        
                        // Convert legacy format if needed
                        if (profile.IsLegacyFormat()) {
                            profile.ConvertFromLegacy();
                            ProfileManager.SaveProfile(profile);
                        }
                        
                        if (profile.RegionConfigurations.Count == 0) {
                            MessageBox.Show(
                                "Dieses Profil enthält keine Regionen.",
                                "Keine Regionen",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        
                        if (profile.RegionConfigurations.Count == 1) {
                            // Only one configuration, apply directly
                            ApplyProfileConfiguration(profile, profile.RegionConfigurations[0]);
                        }
                        else {
                            // Multiple configurations, show selection dialog
                            using (var regionDialog = new Forms.ProfileRegionSelectionDialog(profile)) {
                                if (regionDialog.ShowDialog(this) == DialogResult.OK) {
                                    if (regionDialog.LoadAll) {
                                        // Load all configurations (start new instances)
                                        LoadAllProfileConfigurations(profile);
                                    }
                                    else {
                                        // Load selected configuration
                                        ApplyProfileConfiguration(profile, regionDialog.SelectedConfiguration);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) {
                        MessageBox.Show(
                            $"Fehler beim Laden des Profils:\n{ex.Message}",
                            "Fehler",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        Log.WriteException("Error loading profile", ex);
                    }
                }
            }
        }
        
        /// <summary>
        /// Loads all configurations from a profile by opening new MainForm windows.
        /// </summary>
        private void LoadAllProfileConfigurations(Profile profile) {
            // Apply first configuration to current instance
            ApplyProfileConfiguration(profile, profile.RegionConfigurations[0]);

            // Open new MainForm windows for remaining configurations
            for (int i = 1; i < profile.RegionConfigurations.Count; i++) {
                try {
                    var config = profile.RegionConfigurations[i];
                    var configToApply = config; // Capture config in closure correctly

                    // Create new MainForm window with default startup options
                    var newForm = new MainForm(StartupOptions.Factory.CreateOptions(new string[0]));

                    // Set startup position
                    newForm.StartPosition = FormStartPosition.Manual;

                    // Temporarily position off-screen to avoid flickering during setup
                    newForm.Location = new Point(-10000, -10000);

                    // Apply the configuration after form is shown
                    newForm.Shown += (sender, e) => {
                        try {
                            var form = (MainForm)sender;
                            form.ApplyProfileConfiguration(profile, configToApply);
                        }
                        catch (Exception ex) {
                            Log.WriteException($"Error applying configuration {configToApply.Name}", ex);
                        }
                    };

                    // Show the new form
                    newForm.Show();
                }
                catch (Exception ex) {
                    Log.WriteException($"Error creating window for configuration {i}", ex);
                }
            }

            MessageBox.Show(
                $"Profil '{profile.Name}' wurde geladen.\n" +
                $"{profile.RegionConfigurations.Count} Fenster wurden geöffnet.",
                "Profil geladen",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        
        /// <summary>
        /// Shows a dialog to manage profiles.
        /// </summary>
        public void ShowManageProfilesDialog() {
            using (var dialog = new Forms.ProfileManagerDialog()) {
                dialog.ShowDialog(this);
            }
        }

        /// <summary>
        /// Shows a dialog to add the current region to an existing profile.
        /// </summary>
        public void ShowAddRegionToExistingProfileDialog() {
            using (var dialog = new Forms.AddRegionToExistingProfileDialog()) {
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    try {
                        var profile = ProfileManager.LoadProfile(dialog.SelectedProfileName);
                        AddRegionToProfile(profile, dialog.RegionName, dialog.RegionAnchor, dialog.ScaleWithSourceWindow);

                        MessageBox.Show(
                            $"Region '{dialog.RegionName}' wurde erfolgreich zum Profil '{dialog.SelectedProfileName}' hinzugefügt.",
                            "Region hinzugefügt",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    catch (Exception ex) {
                        MessageBox.Show(
                            $"Fehler beim Hinzufügen der Region:\n{ex.Message}",
                            "Fehler",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        Log.WriteException("Error adding region to existing profile", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Shows a dialog to save all open instances as a single profile with multiple regions.
        /// </summary>
        public void ShowSaveAllInstancesDialog() {
            // Collect all open MainForm instances
            var instances = Application.OpenForms.OfType<MainForm>().Where(f => f.ThumbnailPanel.IsShowingThumbnail).ToList();

            if (instances.Count == 0) {
                MessageBox.Show(
                    "Es sind keine Fenster mit Thumbnails geöffnet.",
                    "Keine Fenster",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            using (var dialog = new Forms.SaveAllInstancesDialog(instances)) {
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    try {
                        var profileName = dialog.ProfileName;

                        // Create the profile based on the FIRST instance
                        var firstInstance = instances[0];
                        var profile = new Profile(profileName);

                        if (firstInstance.CurrentThumbnailWindowHandle != null) {
                            profile.WindowHandle = (long)firstInstance.CurrentThumbnailWindowHandle.Handle;
                            profile.WindowTitle = firstInstance.CurrentThumbnailWindowHandle.Title;
                            profile.WindowClass = firstInstance.CurrentThumbnailWindowHandle.Class;

                            if (firstInstance._thumbnailPanel.IsShowingThumbnail) {
                                profile.OriginalWindowSize = firstInstance._thumbnailPanel.ThumbnailOriginalSize;
                            }

                            try {
                                var process = Process.GetProcessById(
                                    Native.WindowMethods.GetWindowProcessId(firstInstance.CurrentThumbnailWindowHandle.Handle));
                                profile.ProcessName = process.ProcessName;
                            }
                            catch {
                                profile.ProcessName = null;
                            }
                        }

                        // Add all instances as region configurations
                        foreach (var instanceConfig in dialog.InstanceConfigurations) {
                            var config = CreateRegionConfigurationFromInstance(
                                instanceConfig.Instance,
                                instanceConfig.RegionName,
                                instanceConfig.RegionAnchor,
                                instanceConfig.ScaleWithSourceWindow
                            );
                            profile.RegionConfigurations.Add(config);
                        }

                        ProfileManager.SaveProfile(profile);

                        MessageBox.Show(
                            $"Profil '{profileName}' mit {profile.RegionConfigurations.Count} Regionen wurde gespeichert.",
                            "Profil gespeichert",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    catch (Exception ex) {
                        MessageBox.Show(
                            $"Fehler beim Speichern des Profils:\n{ex.Message}",
                            "Fehler",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        Log.WriteException("Error saving all instances as profile", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a profile region configuration from a MainForm instance.
        /// </summary>
        private ProfileRegionConfiguration CreateRegionConfigurationFromInstance(MainForm instance, string configName, RegionAnchor regionAnchor, bool scaleWithSource) {
            var config = new ProfileRegionConfiguration(configName);
            config.ScaleWithSourceWindow = scaleWithSource;

            // Region settings
            if (instance.SelectedThumbnailRegion != null) {
                config.HasRegion = true;
                config.RegionIsRelative = instance.SelectedThumbnailRegion.Relative;
                config.RegionAnchor = regionAnchor;

                var originalWindowSize = Size.Empty;
                if (instance._thumbnailPanel.IsShowingThumbnail) {
                    originalWindowSize = instance._thumbnailPanel.ThumbnailOriginalSize;
                }

                // Get the internal bounds based on the mode
                if (instance.SelectedThumbnailRegion.Relative) {
                    // In relative mode, store as padding (Left, Top, Right, Bottom)
                    var padding = instance.SelectedThumbnailRegion.BoundsAsPadding;
                    config.RegionBounds = new Rectangle(
                        padding.Left,
                        padding.Top,
                        padding.Right,
                        padding.Bottom
                    );
                }
                else {
                    // In absolute mode, store absolute pixel distance from anchor
                    var bounds = instance.SelectedThumbnailRegion.Bounds;

                    // Store absolute bounds
                    config.RegionBounds = bounds;

                    // Calculate absolute pixel distance from the selected anchor
                    int distX, distY;

                    switch (regionAnchor) {
                        case RegionAnchor.TopLeft:
                            distX = bounds.X;
                            distY = bounds.Y;
                            break;

                        case RegionAnchor.TopRight:
                            distX = originalWindowSize.Width - bounds.Right;
                            distY = bounds.Y;
                            break;

                        case RegionAnchor.BottomLeft:
                            distX = bounds.X;
                            distY = originalWindowSize.Height - bounds.Bottom;
                            break;

                        case RegionAnchor.BottomRight:
                            distX = originalWindowSize.Width - bounds.Right;
                            distY = originalWindowSize.Height - bounds.Bottom;
                            break;

                        default:
                            distX = bounds.X;
                            distY = bounds.Y;
                            break;
                    }

                    config.AbsoluteDistanceFromAnchor = new Point(distX, distY);
                }
            }
            else {
                config.HasRegion = false;
            }

            // Window position and size
            config.WindowLocation = instance.Location;
            config.WindowSize = instance.ClientSize; // Store ClientSize to avoid DWM border issues

            // Calculate relative offset to target window (if available)
            if (instance.CurrentThumbnailWindowHandle != null) {
                try {
                    var targetWindowRect = Native.WindowMethods.GetWindowRectangle(instance.CurrentThumbnailWindowHandle.Handle);
                    var targetSize = new Size(targetWindowRect.Width, targetWindowRect.Height);

                    // Absolute offset in pixels
                    var offsetX = instance.Location.X - targetWindowRect.Left;
                    var offsetY = instance.Location.Y - targetWindowRect.Top;

                    config.RelativeOffsetToTargetWindow = new Point(offsetX, offsetY);
                    config.TargetWindowSize = targetSize;

                    // Relative offset as percentage of target window size
                    if (targetSize.Width > 0 && targetSize.Height > 0) {
                        config.RelativeOffsetPercent = new PointF(
                            (float)offsetX / targetSize.Width,
                            (float)offsetY / targetSize.Height
                        );
                        Log.Write($"Saved relative offset for instance: pixels({offsetX}, {offsetY}), percent({config.RelativeOffsetPercent.Value.X:P2}, {config.RelativeOffsetPercent.Value.Y:P2}), target size({targetSize.Width}x{targetSize.Height})");
                    }
                }
                catch (Exception ex) {
                    Log.Write($"Warning: Could not calculate relative offset for instance: {ex.Message}");
                    config.RelativeOffsetToTargetWindow = null;
                    config.RelativeOffsetPercent = null;
                    config.TargetWindowSize = null;
                }
            }

            // Visual settings
            config.Opacity = instance.Opacity;
            config.ClickThrough = instance.ClickThroughEnabled;
            config.ClickForwarding = instance.ClickForwardingEnabled;
            config.ChromeVisible = instance.IsChromeVisible;
            config.PositionLock = instance.PositionLock;
            config.TopMost = instance.TopMost;

            return config;
        }
    }
}
