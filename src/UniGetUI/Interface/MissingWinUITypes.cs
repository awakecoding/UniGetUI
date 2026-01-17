using Avalonia.Controls;
using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Navigation event args for Frame navigation
    /// </summary>
    public class NavigationEventArgs : EventArgs
    {
        public object? Content { get; set; }
        public Type? SourcePageType { get; set; }
        public object? Parameter { get; set; }
    }
}

namespace Avalonia.Controls
{
    using System;
    using System.Collections.Generic;
    using UniGetUI.Interface;

    /// <summary>
    /// Stub implementation of Frame navigation for ContentControl
    /// TODO: Implement proper Avalonia navigation
    /// This uses attached events to provide Navigated/Navigating on ContentControl
    /// </summary>
    public static class FrameExtensions
    {
        // Attached property to track navigation state
        private static readonly Dictionary<ContentControl, FrameState> _frameStates = new();

        private class FrameState
        {
            public Stack<(Type pageType, object? parameter)> BackStack { get; } = new();
            public event EventHandler<object>? NavigatedEvent;
            public event EventHandler<object>? NavigatingEvent;

            public void RaiseNavigated(object sender, object args) => NavigatedEvent?.Invoke(sender, args);
            public void RaiseNavigating(object sender, object args) => NavigatingEvent?.Invoke(sender, args);
            
            public void AddNavigated(EventHandler<object> handler) => NavigatedEvent += handler;
            public void RemoveNavigated(EventHandler<object> handler) => NavigatedEvent -= handler;
            public void AddNavigating(EventHandler<object> handler) => NavigatingEvent += handler;
            public void RemoveNavigating(EventHandler<object> handler) => NavigatingEvent -= handler;
        }

        private static FrameState GetOrCreateState(ContentControl frame)
        {
            if (!_frameStates.TryGetValue(frame, out var state))
            {
                state = new FrameState();
                _frameStates[frame] = state;
            }
            return state;
        }

        public static void Navigate(this ContentControl frame, Type pageType, object? parameter, object transitionInfo)
        {
            var state = GetOrCreateState(frame);
            
            // Store current page in back stack if exists
            if (frame.Content != null && frame.Content.GetType() != pageType)
            {
                state.BackStack.Push((frame.Content.GetType(), parameter));
            }

            // Raise Navigating event
            state.RaiseNavigating(frame, parameter ?? new object());

            // Create new page instance
            try
            {
                var page = Activator.CreateInstance(pageType);
                frame.Content = page;

                // Raise Navigated event
                var navArgs = new NavigationEventArgs
                {
                    Content = page,
                    SourcePageType = pageType,
                    Parameter = parameter
                };
                state.RaiseNavigated(frame, navArgs);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        public static bool CanGoBack(this ContentControl frame)
        {
            var state = GetOrCreateState(frame);
            return state.BackStack.Count > 0;
        }

        public static void GoBack(this ContentControl frame)
        {
            var state = GetOrCreateState(frame);
            if (state.BackStack.Count > 0)
            {
                var (pageType, parameter) = state.BackStack.Pop();
                
                // Raise Navigating event
                state.RaiseNavigating(frame, parameter ?? new object());

                // Navigate back
                try
                {
                    var page = Activator.CreateInstance(pageType);
                    frame.Content = page;

                    // Raise Navigated event
                    var navArgs = new NavigationEventArgs
                    {
                        Content = page,
                        SourcePageType = pageType,
                        Parameter = parameter
                    };
                    state.RaiseNavigated(frame, navArgs);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Back navigation failed: {ex.Message}");
                }
            }
        }

        // Event subscription methods (to work around C# event += limitation with extension methods)
        public static void SubscribeNavigated(this ContentControl frame, EventHandler<object> handler)
        {
            var state = GetOrCreateState(frame);
            state.AddNavigated(handler);
        }

        public static void UnsubscribeNavigated(this ContentControl frame, EventHandler<object> handler)
        {
            var state = GetOrCreateState(frame);
            state.RemoveNavigated(handler);
        }

        public static void SubscribeNavigating(this ContentControl frame, EventHandler<object> handler)
        {
            var state = GetOrCreateState(frame);
            state.AddNavigating(handler);
        }

        public static void UnsubscribeNavigating(this ContentControl frame, EventHandler<object> handler)
        {
            var state = GetOrCreateState(frame);
            state.RemoveNavigating(handler);
        }
    }
}
