# UniGetUI Avalonia Migration Status

**Last Updated:** January 18, 2026  
**Migration Status:** C# Backend Complete ✅ | XAML UI Layer In Progress ⚠️

## Executive Summary

The UniGetUI application is undergoing migration from **WinUI 3** to **Avalonia 11.2.2** for cross-platform support. The C# backend migration is **complete** with zero compilation errors. The XAML UI layer requires extensive conversion work.

### Current Build Status
- ✅ **C# Compilation:** 0 errors
- ⚠️ **XAML Compilation:** 442 errors (blocking executable creation)
- ⚠️ **Warnings:** 471 (non-blocking)
- 🎯 **Target:** Executable build → Runtime testing → Feature completion

---

## Completed Work ✅

### Infrastructure & Configuration
- ✅ Target Framework: `net8.0-windows10.0.19041.0` (Windows 10 19041 minimum)
- ✅ RuntimeIdentifiers: `win-x64;win-arm64` (removed Linux/macOS targets)
- ✅ Avalonia 11.2.2 packages installed and configured
- ✅ Microsoft.Windows.SDK.NET.Ref: 10.0.26100.48 for Windows APIs

### C# Backend Migration
- ✅ **File Pickers:** Migrated from WinRT `FileSavePicker` to Avalonia `SaveFileDialog`
  - Files: `AppOperationHelper.cs`, `LogPage.xaml.cs`
  - Uses `string filePath` instead of `StorageFile`
  
- ✅ **Window Handles:** Created `WindowExtensions.GetWindowHandle()` 
  - Uses Avalonia's `TryGetPlatformHandle()?.Handle`
  - Used by 4 files for Windows interop
  
- ✅ **Notifications:** Temporarily disabled (Windows App SDK incompatible with Avalonia)
  - Files: `SoftwareUpdatesPage.cs`, `OperationControl.cs`, `AutoUpdater.cs`, `DialogHelper_Generic.cs`
  - Commented out with TODO markers for future implementation
  
- ✅ **Text Selection:** Fixed `Select()` → `SelectionStart`/`SelectionEnd` in `LogPage.xaml.cs`

- ✅ **Build Configuration:**
  - Removed incompatible Microsoft.WindowsAppSDK package
  - All C# syntax and semantic errors resolved

### XAML Quick Fixes
- ✅ Fixed `Announcer.axaml`: RichTextBlock → TextBlock
- ✅ Fixed `SourceManager.axaml`: Removed ColumnSpacing, fixed XML structure
- ✅ Added `DocumentStubs.cs` with Inlines collection for Paragraph

---

## Remaining Work - XAML UI Migration ⚠️

**Critical Path:** Fix 442 XAML errors to enable executable build

### Category 1: Missing Controls (High Priority)
Replace WinUI-only controls with Avalonia equivalents:

| WinUI Control | Avalonia Equivalent | Occurrences | Complexity |
|---------------|---------------------|-------------|------------|
| `RichTextBlock` | `TextBlock` or `SelectableTextBlock` | ~50 | Medium |
| `ListView` | `ListBox` | ~15 | Low |
| `ProgressRing` | `ProgressBar` (IsIndeterminate=true) | ~20 | Low |
| `InfoBar` | Custom control or `Border` | ~15 | High |
| `PasswordBox` | `TextBox` (PasswordChar='•') | ~10 | Low |
| `SymbolIcon` | `PathIcon` or custom | ~12 | Medium |
| `ThemeResource` | `StaticResource`/`DynamicResource` | ~8 | Low |
| `BitmapImage` | `Bitmap` | ~5 | Low |

**Estimated Effort:** 6-8 hours

### Category 2: Missing Properties (Medium Priority)
Remove or replace unsupported properties:

| Property | Affected Controls | Solution | Occurrences |
|----------|------------------|----------|-------------|
| `ColumnSpacing`/`RowSpacing` | `Grid` | Use `Margin` on children | ~30 |
| `Padding` | `StackPanel`, `Grid` | Use `Margin` or `Border` wrapper | ~25 |
| `Click` | Non-Button controls | Use `Tapped` event or `Command` | ~15 |
| `ContentAlignment` | `SettingsCard` | Custom layout | ~10 |
| `ActionIcon` | `SettingsCard` | Custom property | ~5 |
| `HorizontalTextAlignment` | Custom controls | `TextAlignment` | ~8 |
| Various WinUI attached properties | Multiple | Find Avalonia equivalents | ~20 |

**Estimated Effort:** 4-6 hours

### Category 3: Binding & Type Errors (Medium Priority)
Fix XAML binding mismatches:

| Error Type | Description | Occurrences | Complexity |
|------------|-------------|-------------|------------|
| Icon property type | String → `IconType` enum | ~25 | Low |
| Event handler signatures | WinUI → Avalonia events | ~15 | Low |
| Items property | Binding → Object | ~5 | Medium |
| DoubleTapped | Binding syntax | ~10 | Low |
| Custom property access | `_checkbox` field access | ~20 | Medium |
| Converter issues | WinUI → Avalonia converters | ~5 | Medium |

**Estimated Effort:** 3-4 hours

### Category 4: Custom Controls (Low Priority - Post-Build)
Fix custom control properties and features:

- `CustomNavViewItem`: GlyphIcon, InfoBadge, Icon properties (~10 files)
- `SettingsCard`: Description, Header, Click properties (~25 files)
- `CheckboxCard`: _checkbox field access (~15 files)
- `TranslatedTextBlock`: TextWrapping, WrappingMode (~20 files)
- `PackageItemContainer`: PreviewKeyDown, RightTapped (~10 files)

**Estimated Effort:** 3-5 hours (can defer until after first build)

---

## Migration Strategy

### Phase 1: Critical Path to First Build (Priority: CRITICAL)
**Goal:** Fix minimum XAML errors to produce executable

1. **Replace Top 5 Missing Controls** (4-5 hours)
   - RichTextBlock → TextBlock (50 instances)
   - ListView → ListBox (15 instances)
   - ProgressRing → ProgressBar (20 instances)
   - PasswordBox → TextBox with PasswordChar (10 instances)
   - Remove or stub InfoBar (15 instances)

2. **Fix Critical Properties** (2-3 hours)
   - Remove ColumnSpacing/RowSpacing from Grid (30 instances)
   - Remove Padding from StackPanel (25 instances)
   - Change Click → Tapped where needed (15 instances)

3. **Fix Type Mismatches** (1-2 hours)
   - Icon property string → IconType conversions
   - Event handler signature fixes

**Success Criteria:** 
- Executable builds successfully
- Application launches (crashes expected)
- Can begin runtime testing

**Estimated Total:** 7-10 hours

### Phase 2: Runtime Stability (Priority: HIGH)
**Goal:** Application starts and displays main window

1. **Fix Application Startup**
   - Test App.axaml initialization
   - Verify MainWindow loads
   - Fix blocking exceptions

2. **Fix Main Navigation**
   - Test page navigation
   - Fix navigation-related crashes
   - Verify basic UI rendering

**Estimated Total:** 3-5 hours

### Phase 3: Feature Completeness (Priority: MEDIUM)
**Goal:** Restore all functionality

1. **Custom Controls**
   - Fix all custom control properties
   - Test control rendering
   - Fix layout issues

2. **Advanced Features**
   - Implement Avalonia-compatible notifications
   - Fix remaining bindings
   - Polish UI appearance

**Estimated Total:** 5-8 hours

### Phase 4: Testing & Refinement (Priority: LOW)
**Goal:** Production-ready application

1. **Comprehensive Testing**
   - Test all pages and dialogs
   - Test package operations
   - Test settings and preferences

2. **Bug Fixes & Polish**
   - Fix discovered issues
   - Performance optimization
   - Visual refinement

**Estimated Total:** 8-12 hours

---

## File-by-File Priority List

### Tier 1: Critical for Launch (Must Fix First)
1. `Pages/MainView.axaml` - Main navigation shell (21 errors)
2. `Pages/SoftwarePages/AbstractPackagesPage.axaml` - Package list base (73 errors)
3. `App.axaml` - Application resources (2 errors)
4. `Themes/Generic.axaml` - Control templates (1 error)

### Tier 2: High-Traffic Pages (Fix Second)
5. `Pages/SoftwarePages/*Page.axaml` - Discovered/Installed/Updates (similar patterns)
6. `Pages/DialogPages/PackageDetailsPage.axaml` - Package info (9 errors)
7. `Pages/LogPages/LogPage.axaml` - Log viewer (28 errors)
8. `Pages/HelpPage.axaml` - Help content (3 errors)

### Tier 3: Settings & Configuration (Fix Third)
9-25. `Pages/SettingsPages/**/*.axaml` - All settings pages (~150 errors total)

### Tier 4: Secondary Dialogs (Fix Last)
26-36. `Pages/DialogPages/*.axaml` - Various dialogs (~80 errors total)

---

## Known Issues & Blockers

### Current Blockers
1. ❌ **No Executable:** XAML errors prevent build completion
2. ❌ **Can't Test Runtime:** Need executable to begin runtime testing
3. ❌ **Unknown Runtime Issues:** Many issues won't surface until runtime

### Resolved Issues
- ✅ C# compilation errors (was 292 → now 0)
- ✅ TFM configuration (net8.0-windows10.0.19041.0)
- ✅ Windows App SDK incompatibility (removed)
- ✅ File picker migration (Avalonia SaveFileDialog)
- ✅ Window handle access (WindowExtensions)

### Deferred Work
- 🔄 Notification system (requires Avalonia-compatible solution)
- 🔄 Custom control full polish (post-launch)
- 🔄 Advanced XAML features (post-launch)
- 🔄 Performance optimization (post-launch)

---

## Technical Decisions & Rationale

### Why Windows-Only?
- UniGetUI depends heavily on Windows package managers (WinGet, Chocolatey, etc.)
- Windows-specific APIs required for package manager integration
- Cross-platform support would require significant architectural changes
- RuntimeIdentifiers set to win-x64 and win-arm64 only

### Why Avalonia Over WinUI 3?
- Better performance and flexibility
- Modern architecture and active development
- Not specified in this codebase - appears to be project direction

### Notification Strategy
- Windows App SDK incompatible with Avalonia applications
- Temporarily disabled to unblock development
- Future options:
  - Custom toast notification implementation
  - Third-party Avalonia notification library
  - System tray notifications
  - Or accept as non-critical feature

---

## Quick Reference

### Build Commands
```powershell
# Build from src directory
cd src
dotnet build UniGetUI/UniGetUI.csproj --configuration Debug

# Run (after XAML errors fixed)
cd UniGetUI
dotnet run --configuration Debug
```

### Key Files
- **Project:** `src/UniGetUI/UniGetUI.csproj`
- **Build Config:** `src/Directory.Build.props`
- **Main App:** `src/UniGetUI/App.axaml.cs`
- **Main Window:** `src/UniGetUI/Pages/MainView.axaml`

### Error Categories
- **AVLN2000:** Cannot resolve type or property
- **AVLN3000:** Cannot find suitable setter (type mismatch)
- **AVLN2200:** Cannot convert property value
- **AVLN2203:** Duplicate property setter (warning)

---

## Next Steps

**Immediate Actions (This Session):**
1. Begin Phase 1: Fix RichTextBlock → TextBlock (highest occurrence)
2. Fix ProgressRing → ProgressBar (quick wins)
3. Remove Grid ColumnSpacing/RowSpacing properties
4. Target: Reduce errors from 442 to <200

**Short Term (Next Session):**
1. Continue Phase 1 until executable builds
2. Begin Phase 2 runtime testing
3. Document runtime issues discovered

**Long Term:**
1. Complete all XAML migrations
2. Implement notification system
3. Polish and test all features
4. Production release

---

## Resources

### Documentation
- Avalonia Docs: https://docs.avaloniaui.net/
- WinUI → Avalonia Migration: https://docs.avaloniaui.net/guides/platforms/desktop/wpf-migration
- Avalonia GitHub: https://github.com/AvaloniaUI/Avalonia

### Team Communication
- Document all breaking changes
- Test thoroughly before commits
- Keep build errors at 0 for C# code
- XAML errors tracked in this document

---

**Document Status:** Living document - update after each migration session
