# Avalonia Migration Completion Plan - Incremental Approach

## Current Status
- **Starting Errors**: 1,086 compilation errors in main UniGetUI project
- **All Dependencies**: Building successfully (0 errors)
- **Completion**: ~55%

## Incremental Execution Plan - 10 Phases

### Phase 1: Quick Wins - Task.Avalonia & Import Fixes (Est: 30 min, ~40 errors)
**Goal**: Fix systematic errors that can be addressed with search/replace

1. **Fix remaining Task.Avalonia references** (~10 errors)
   - Search: `Task.Avalonia.Controls.Documents.Run`
   - Replace: `Task.Run`
   - Files: Any remaining in PackageBundlesPage.cs, other dialog files

2. **Add missing using statements** (~30 errors)
   - Add `using Avalonia.Layout;` for HorizontalAlignment/VerticalAlignment
   - Add `using Avalonia.Controls.Primitives;` for TemplatedControl
   - Add `using Avalonia.Media;` for Brushes

**Success Criteria**: Errors reduced to ~1,046

---

### Phase 2: Alignment Property Static Access (Est: 1 hour, ~150 errors)
**Goal**: Fix HorizontalAlignment/VerticalAlignment to use static enum access

**Pattern to Fix**:
```csharp
// Wrong (WinUI):
element.HorizontalAlignment = element.HorizontalAlignment.Center;

// Correct (Avalonia):
element.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
```

**Files to Fix** (based on common patterns):
- MainView.xaml.cs
- AbstractPackagesPage.xaml.cs  
- All Settings pages (12 files)
- All Dialog pages (9 files)

**Approach**: Systematic search/replace with validation

**Success Criteria**: Errors reduced to ~896

---

### Phase 3: Window Dialog Properties Removal (Est: 30 min, ~30 errors)
**Goal**: Remove WinUI-specific Window properties

**Properties to Remove/Comment**:
- `Window.PrimaryButtonText`
- `Window.SecondaryButtonText`
- `Window.DefaultButton`
- `Window.XamlRoot`

**Files to Fix**:
- DialogHelper_Packages.cs
- Any dialog initialization code

**Approach**: Comment out with TODO markers for future Avalonia dialog implementation

**Success Criteria**: Errors reduced to ~866

---

### Phase 4: Visibility Enum Completion (Est: 45 min, ~30 errors)
**Goal**: Complete remaining Visibility → IsVisible conversions

**Pattern to Fix**:
```csharp
// Wrong:
element.Visibility = Visibility.Visible;
element.Visibility = Visibility.Collapsed;

// Correct:
element.IsVisible = true;
element.IsVisible = false;
```

**Files to Fix** (scan for remaining):
- Any files still using Visibility enum
- Check all .cs files in Interface directory

**Success Criteria**: Errors reduced to ~836

---

### Phase 5: Windows-Specific API Guards (Est: 1 hour, ~70 errors)
**Goal**: Add #if WINDOWS guards for platform-specific APIs

**APIs to Guard**:
- `AppNotificationManager`
- `AppNotificationScenario`
- `WinRT.Interop.WindowNative.GetWindowHandle`
- Any Windows.* namespace calls

**Files to Fix**:
- NotificationManager.cs
- MainWindow.xaml.cs  
- Any file with Windows-specific interop

**Pattern**:
```csharp
#if WINDOWS
    // Windows-specific code
#else
    // Cross-platform alternative or throw NotSupportedException
#endif
```

**Success Criteria**: Errors reduced to ~766

---

### Phase 6: WinUI Control Property Replacements (Est: 2 hours, ~150 errors)
**Goal**: Replace WinUI-specific control properties with Avalonia equivalents

**Property Mappings**:
1. `TextBlock.Glyph` → Use `Text` property with icon font character
2. `Run.Foreground` → Remove (not supported in Avalonia Run)
3. `ListBoxItem.Icon` → Use custom property or content template
4. `BetterMenuItem.KeyboardAcceleratorTextOverride` → Custom property or remove
5. `MenuFlyoutSubItem.Text` → Change to `Header`

**Files to Fix**:
- MainView.xaml.cs
- AbstractPackagesPage.xaml.cs
- BetterMenuItem.cs (custom control)
- All menu-related code

**Success Criteria**: Errors reduced to ~616

---

### Phase 7: Navigation & Event Handler Fixes (Est: 1.5 hours, ~50 errors)
**Goal**: Fix navigation system and event handler mismatches

**Issues to Address**:
1. `UserControl.OnNavigatedTo` → Remove or use Avalonia navigation events
2. `SlideNavigationTransitionEffect` → Comment out (not in Avalonia)
3. Missing namespace imports for `Orientation`, `DecodePixelType`

**Files to Fix**:
- SettingsBasePage.cs
- All page navigation code
- MainView.xaml.cs navigation system

**Approach**: Comment out WinUI navigation, add TODO for Avalonia equivalent

**Success Criteria**: Errors reduced to ~566

---

### Phase 8: XAML Control Generation Issues (Est: 2-3 hours, ~300 errors)
**Goal**: Fix x:Name bindings not generating properly

**Root Cause Investigation**:
1. Check .csproj for proper AvaloniaResource configuration
2. Verify all .axaml files have correct namespace declarations
3. Ensure x:Name attributes are present and match code-behind references

**Files to Check**:
- UniGetUI.csproj (ItemGroup with AvaloniaResource)
- All .axaml files for x:Name correctness
- Generated files in obj/Debug for clues

**Specific Controls Missing**:
- LoadingIndicator, RightPanel, LeftPanel
- KillProcessesBox, CommandLineViewBox, AbortInsFailedCheck
- CloseButton, SettingsTabBar, ScreenshotsPanel

**Approach**:
1. Verify .csproj has proper Avalonia XAML compilation
2. Check each .axaml file for x:Name declarations
3. Rebuild with clean obj directory
4. Add missing controls or fix naming mismatches

**Success Criteria**: Errors reduced to ~266

---

### Phase 9: Remaining Type/Property Issues (Est: 2 hours, ~186 errors)
**Goal**: Fix miscellaneous type conversions and property access

**Issues to Address**:
1. `FlyoutShowOptions` type conversion
2. `ToolTipService` accessibility (different API in Avalonia)
3. Method signature mismatches (Flyout.ShowAt, etc.)
4. Property type mismatches

**Approach**: Case-by-case fixes based on compiler errors

**Success Criteria**: Errors reduced to ~80

---

### Phase 10: Generated Code & Final Fixes (Est: 1-2 hours, ~80 errors)
**Goal**: Fix remaining InitializeComponent and generated code issues

**Approach**:
1. Clean build (delete obj/bin directories)
2. Rebuild and address any remaining errors
3. Fix any lingering WinUI→Avalonia API mismatches
4. Verify all projects build successfully

**Success Criteria**: **ZERO COMPILATION ERRORS** ✅

---

## Post-Build Testing Phase (After 0 Errors)

### Phase 11: Runtime Initialization (Est: 2 hours)
1. Launch application on Linux
2. Verify Avalonia window opens
3. Fix any runtime initialization errors
4. Test basic UI rendering

### Phase 12: Navigation Testing (Est: 2 hours)
1. Test main window navigation
2. Verify page switching works
3. Test settings navigation
4. Fix navigation issues

### Phase 13: Feature Validation (Est: 4 hours)
1. Test package discovery
2. Test package operations (mock)
3. Test all dialogs
4. Test settings functionality

### Phase 14: Cross-Platform Build (Est: 2 hours)
1. Test build on Windows
2. Test build on Linux  
3. Verify platform-specific code works
4. Test on both platforms

### Phase 15: Polish & Documentation (Est: 2 hours)
1. Address remaining warnings
2. Update documentation
3. Create migration summary
4. Mark PR as ready for review

---

## Total Estimated Time
- **Phases 1-10 (Build Success)**: 12-16 hours
- **Phases 11-15 (Runtime & Testing)**: 12-14 hours
- **Total to Production-Ready**: 24-30 hours

## Execution Strategy
- Execute phases sequentially
- Commit after each phase with error count verification
- Test build after each phase
- Document any blocking issues encountered
- Adapt plan if unexpected issues arise

## Success Metrics
- Phase 1-10: Zero compilation errors
- Phase 11: Application launches
- Phase 12: Navigation works
- Phase 13: Core features functional
- Phase 14: Cross-platform builds successful
- Phase 15: Ready for production use

---

**Plan Created**: 2026-01-17
**Status**: Ready for execution
