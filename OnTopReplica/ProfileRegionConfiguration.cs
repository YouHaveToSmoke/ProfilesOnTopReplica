using System;
using System.Drawing;

namespace OnTopReplica {
    
    /// <summary>
    /// Represents a single region configuration within a profile.
    /// </summary>
    [Serializable]
    public class ProfileRegionConfiguration {
        
        public string Name { get; set; }
        
        public bool HasRegion { get; set; }
        
        public Rectangle RegionBounds { get; set; }

        /// <summary>
        /// Absolute pixel distance from the anchor point.
        /// </summary>
        public Point? AbsoluteDistanceFromAnchor { get; set; }

        public bool RegionIsRelative { get; set; }
        
        public RegionAnchor RegionAnchor { get; set; }

        public bool ScaleWithSourceWindow { get; set; }

        /// <summary>
        /// Offset of the replica window relative to the target window's top-left corner.
        /// Used to position replica relative to target window when loading profile.
        /// </summary>
        public Point? RelativeOffsetToTargetWindow { get; set; }

        /// <summary>
        /// Relative offset as percentage of target window size (X: 0.0-1.0, Y: 0.0-1.0).
        /// Allows positioning to scale with different window resolutions.
        /// </summary>
        public PointF? RelativeOffsetPercent { get; set; }

        /// <summary>
        /// Target window size when the profile was saved.
        /// Used to scale relative positions when target window resolution changes.
        /// </summary>
        public Size? TargetWindowSize { get; set; }

        public Point WindowLocation { get; set; }
        
        public Size WindowSize { get; set; }
        
        public double Opacity { get; set; }
        
        public bool ClickThrough { get; set; }
        
        public bool ClickForwarding { get; set; }
        
        public bool ChromeVisible { get; set; }
        
        public ScreenPosition? PositionLock { get; set; }
        
        public bool TopMost { get; set; }
        
        public ProfileRegionConfiguration() {
            Opacity = 1.0;
            ChromeVisible = true;
            TopMost = true;
            RegionAnchor = RegionAnchor.TopLeft;
        }
        
        public ProfileRegionConfiguration(string name) : this() {
            Name = name;
        }
        
        public override string ToString() {
            return Name ?? "Unnamed Configuration";
        }
    }
}
