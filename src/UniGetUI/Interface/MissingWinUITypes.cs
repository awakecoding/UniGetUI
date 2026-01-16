using Avalonia.Controls;

namespace UniGetUI.Interface
{
    // Missing WinUI 3 types - stub implementations for Avalonia migration
    
    /// <summary>
    /// Represents the result of a content dialog
    /// </summary>
    public enum ContentDialogResult
    {
        None = 0,
        Primary = 1,
        Secondary = 2
    }

    /// <summary>
    /// Defines constants that specify which button on a ContentDialog is the default action
    /// </summary>
    public enum ContentDialogButton
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Close = 3
    }

    /// <summary>
    /// Defines constants that specify the preferred location for positioning a flyout relative to a target element
    /// </summary>
    public enum FlyoutPlacementMode
    {
        Top,
        Bottom,
        Left,
        Right,
        Full,
        TopEdgeAlignedLeft,
        TopEdgeAlignedRight,
        BottomEdgeAlignedLeft,
        BottomEdgeAlignedRight,
        LeftEdgeAlignedTop,
        LeftEdgeAlignedBottom,
        RightEdgeAlignedTop,
        RightEdgeAlignedBottom,
        Auto
    }

    /// <summary>
    /// Stub for FlyoutShowOptions
    /// </summary>
    public class FlyoutShowOptions
    {
        public FlyoutPlacementMode Placement { get; set; }
        public bool ShowMode { get; set; }
    }

    /// <summary>
    /// Navigation transition info stubs
    /// </summary>
    public class DrillInNavigationTransitionInfo
    {
    }

    public class SlideNavigationTransitionInfo
    {
        public SlideNavigationTransitionInfo() { }
        public SlideNavigationTransitionEffect Effect { get; set; }
    }

    public enum SlideNavigationTransitionEffect
    {
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
    }

    /// <summary>
    /// Menu flyout items
    /// </summary>
    public class MenuFlyoutSubItem : MenuItem
    {
        public MenuFlyoutSubItem()
        {
        }
    }

    public class MenuFlyoutSeparator : Separator
    {
        public MenuFlyoutSeparator()
        {
        }
    }

    public class MenuFlyoutItem : MenuItem
    {
        public MenuFlyoutItem()
        {
        }
    }

    /// <summary>
    /// Picker location enum
    /// </summary>
    public enum PickerLocationId
    {
        DocumentsLibrary,
        ComputerFolder,
        Desktop,
        Downloads,
        HomeGroup,
        MusicLibrary,
        PicturesLibrary,
        VideosLibrary,
        Objects3D,
        Unspecified
    }

    /// <summary>
    /// Element theme enum
    /// </summary>
    public enum ElementTheme
    {
        Default,
        Light,
        Dark
    }
}
