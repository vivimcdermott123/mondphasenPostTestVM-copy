# Step-by-Step Debugging Instructions for Vision Pro White Dots Issue

## Quick Start (5 minutes)

### Step 1: Create Minimal Test Scene
1. Open your Unity project
2. **Wait for Unity to finish compiling** (check console for any red errors)
3. In the current scene, create an empty GameObject
4. Name it "DebugHelper"
5. Attach the `TestSceneSetup` script to it (or `SimpleTestSetup` if TestSceneSetup still has issues)
6. Right-click on the script component and select "Create Hello World Scene"
7. Save the scene

### Step 2: Test on Vision Pro
1. Build the project for visionOS
2. Install on Vision Pro
3. **Expected Result**: Should see white text saying "Minimal Test Running (v2.5)" with debug info
4. **If you see white dots**: Basic Unity rendering issue - check Unity version and PolySpatial setup

## Detailed Process of Elimination

### Phase 1: Basic Rendering Test
**Goal**: Verify basic Unity rendering works on Vision Pro

1. **Use Hello World Scene** (created above)
2. **Expected**: White text on black background
3. **If fails**: 
   - Check Unity version (should be 2022.3.19f1 or later)
   - Verify PolySpatial package is installed
   - Check build settings for visionOS
   - **Ensure platform version is set to 2.5** in build settings

### Phase 2: UI/Canvas Test
**Goal**: Verify UI rendering works

1. In the Hello World scene, find the `MinimalTest` component
2. Right-click and select "Enable Background"
3. **Expected**: Dark blue background appears
4. **If fails**: Canvas rendering issue - check canvas settings

### Phase 3: 3D Objects Test
**Goal**: Verify 3D rendering works

1. Right-click on `MinimalTest` and select "Enable 3D Objects"
2. **Expected**: Simple cube and sphere appear
3. **If fails**: 3D rendering issue - check materials and shaders

### Phase 4: XR Components Test
**Goal**: Verify XR functionality works

1. Right-click on `MinimalTest` and select "Enable XR Components"
2. **Expected**: XR interaction becomes available
3. **If fails**: XR/visionOS specific issue

## Diagnostic Tools

### VisionProDiagnostics Script
1. Attach `VisionProDiagnostics` to any GameObject
2. Right-click and select "Run Full Diagnostics"
3. Check console for detailed information
4. Look for warnings or errors

### DebugSceneManager Script
1. Attach `DebugSceneManager` to any GameObject
2. Set `createMinimalScene = true`
3. This will automatically disable complex scripts and create a minimal setup

## Common Issues and Solutions

### Issue: Platform version linking error (2.5 vs 1.0)
**Solution**: 
- Check Unity build settings for visionOS
- Ensure platform version is set to 2.5
- Clean and rebuild the project
- Check Xcode project settings for deployment target

### Issue: Script compilation errors (can't add TestSceneSetup)
**Solution**: 
- Check Unity console for red compilation errors
- Fix any errors in `VisionProDiagnostics.cs` or other scripts
- Wait for Unity to finish recompiling
- Use `SimpleTestSetup` as an alternative if issues persist

### Issue: White dots appear immediately
**Solution**: 
- Check Unity version compatibility
- Verify PolySpatial package installation
- Check build settings
- **Verify platform version 2.5 compatibility**

### Issue: White dots after enabling background
**Solution**:
- Check canvas render mode (should be World Space)
- Verify camera assignment to canvas
- Use `CanvasBackgroundFix` script

### Issue: White dots after enabling 3D objects
**Solution**:
- Check material shaders (use URP shaders)
- Verify lighting setup
- Check for incompatible materials

### Issue: White dots after enabling XR
**Solution**:
- Check XR Interaction Toolkit setup
- Verify PolySpatial configuration
- Check XR settings in project

## Testing Checklist

### Before Each Test
- [ ] Scene is saved
- [ ] **No compilation errors (check console for red errors)**
- [ ] Build settings correct
- [ ] Console is clear of errors
- [ ] **Scripts compile successfully**
- [ ] **Platform version set to 2.5**

### During Test
- [ ] Monitor console for errors
- [ ] Check frame rate
- [ ] Test with minimal scene first
- [ ] Gradually add complexity
- [ ] Document when issues appear

### After Test
- [ ] Review console logs
- [ ] Check build size and performance
- [ ] Verify all required components present
- [ ] Test on different Vision Pro devices if possible

## Quick Fixes to Try

### 1. Platform Version Fix
```bash
# In Xcode, check deployment target
# Should be set to visionOS 2.5 or later
```

### 2. Canvas Fix
```csharp
// Add to any canvas GameObject
var canvas = GetComponent<Canvas>();
canvas.renderMode = RenderMode.WorldSpace;
canvas.worldCamera = Camera.main;
```

### 3. Camera Fix
```csharp
// Reset camera to basic settings
var camera = Camera.main;
camera.clearFlags = CameraClearFlags.SolidColor;
camera.backgroundColor = Color.black;
```

### 4. Material Fix
```csharp
// Use URP shader
var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
```

### 5. XR Fix
```csharp
// Check XR availability
if (UnityEngine.XR.XRSettings.isDeviceActive)
{
    Debug.Log("XR device is active");
}
```

## Next Steps

1. **Start with minimal test** using `DebugSceneManager`
2. **Document results** at each step
3. **Identify breaking point** when white dots appear
4. **Apply specific fixes** based on the issue
5. **Test incrementally** to ensure fixes work

## Getting Help

If you need additional help:
1. Document which step fails
2. Include console logs
3. Note Unity and PolySpatial versions
4. Describe exactly what you see on Vision Pro
5. Include diagnostic output
6. **Specify platform version (2.5)**

## Files Created for Debugging

- `MinimalTest.cs` - Basic test with gradual feature enablement (v2.5)
- `DebugSceneManager.cs` - Manages minimal scene creation (v2.5)
- `VisionProDiagnostics.cs` - Comprehensive diagnostic tool (v2.5)
- `TestSceneSetup.cs` - Quick scene setup helper (v2.5)
- `SimpleTestSetup.cs` - Alternative setup script (v2.5)
- `DebuggingGuide.md` - Detailed debugging guide
- `DEBUGGING_INSTRUCTIONS.md` - This file

## Troubleshooting Compilation Issues

If you encounter "script class cannot be found" errors:
1. **Check Unity Console** for red compilation errors
2. **Fix any errors** in the scripts (especially XR-related issues)
3. **Wait for Unity to recompile** automatically
4. **Use `SimpleTestSetup`** as a fallback if `TestSceneSetup` has issues
5. **Ensure all dependencies** are properly installed (XR Interaction Toolkit, PolySpatial)
6. **Verify platform version 2.5** compatibility

## Platform Version 2.5 Specific Notes

- All debugging scripts have been updated for visionOS platform version 2.5
- Scripts now include version indicators in debug messages
- Enhanced compatibility with newer XR APIs
- Improved error handling for platform-specific issues

Use these tools systematically to identify and fix the white dots issue. 