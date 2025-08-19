# Vision Pro White Dots Debugging Guide

## Problem Description
The app runs without errors on Mac but shows "flashing white dots and disabled buttons" on the Vision Pro.

## Debugging Process of Elimination

### Step 1: Minimal "Hello World" Test
1. **Create a new empty GameObject** in your scene
2. **Attach the `DebugSceneManager` script** to it
3. **Set `createMinimalScene = true`** in the inspector
4. **Build and test** on Vision Pro
5. **Expected result**: Should see basic text saying "Minimal Test Running"

### Step 2: Gradually Enable Features
Use the context menu options in the `DebugSceneManager`:

1. **Enable Basic Text** - Should show debug information
2. **Enable Background** - Should show a dark blue background
3. **Enable 3D Objects** - Should show simple cube and sphere
4. **Enable XR Components** - Should enable XR interaction

### Step 3: Identify the Breaking Point
- **If Step 1 fails**: Basic Unity rendering issue
- **If Step 2 fails**: UI/Canvas rendering issue
- **If Step 3 fails**: 3D rendering issue
- **If Step 4 fails**: XR/visionOS specific issue

## Common Causes of White Dots

### 1. Canvas/UI Issues
- **Problem**: Canvas not properly configured for visionOS
- **Solution**: Use `CanvasBackgroundFix` script
- **Check**: Canvas render mode, camera settings

### 2. Material/Shader Issues
- **Problem**: Materials using incompatible shaders
- **Solution**: Use URP (Universal Render Pipeline) shaders
- **Check**: All materials in scene

### 3. Camera Issues
- **Problem**: Camera not properly configured for visionOS
- **Solution**: Use simple camera setup
- **Check**: Camera clear flags, background color

### 4. XR/visionOS Specific Issues
- **Problem**: XR components not properly initialized
- **Solution**: Check XR settings and PolySpatial configuration
- **Check**: XR Interaction Toolkit setup

### 5. Performance Issues
- **Problem**: Too many objects or complex rendering
- **Solution**: Reduce scene complexity
- **Check**: Frame rate, object count

## Debugging Checklist

### Before Testing
- [ ] Unity version compatible with visionOS
- [ ] PolySpatial package installed and configured
- [ ] Build settings correct for visionOS
- [ ] Scene saved and all assets included

### During Testing
- [ ] Check Unity Console for errors
- [ ] Monitor frame rate
- [ ] Test with minimal scene first
- [ ] Gradually add complexity
- [ ] Document when issues appear

### After Testing
- [ ] Review console logs
- [ ] Check build size and performance
- [ ] Verify all required components present
- [ ] Test on different Vision Pro devices if possible

## Quick Fixes to Try

### 1. Canvas Fix
```csharp
// Add to any canvas GameObject
var canvas = GetComponent<Canvas>();
canvas.renderMode = RenderMode.WorldSpace;
canvas.worldCamera = Camera.main;
```

### 2. Camera Fix
```csharp
// Reset camera to basic settings
var camera = Camera.main;
camera.clearFlags = CameraClearFlags.SolidColor;
camera.backgroundColor = Color.black;
```

### 3. Material Fix
```csharp
// Use URP shader
var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
```

### 4. XR Fix
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

## Contact Information
If issues persist after following this guide, provide:
- Unity version
- PolySpatial version
- Console logs
- Step where issue occurs
- Device information 