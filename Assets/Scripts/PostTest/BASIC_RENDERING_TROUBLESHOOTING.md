# Basic Unity Rendering Troubleshooting Guide
## When "White Text on Black Background" Fails on Vision Pro

### 🚨 **Current Status: BASIC RENDERING FAILURE**
Your Vision Pro is showing white dots instead of the expected white text on black background. This indicates a fundamental Unity rendering issue.

---

## Step 1: Verify Unity and PolySpatial Setup

### 1.1 Check Unity Version
- **Required**: Unity 2022.3.19f1 or later
- **Check**: Go to `Unity > About Unity` in the menu bar
- **If outdated**: Update Unity to the latest LTS version

### 1.2 Verify PolySpatial Package
- **Open**: `Window > Package Manager`
- **Search**: "PolySpatial"
- **Required**: PolySpatial package should be installed and up to date
- **If missing**: Install from Package Manager

### 1.3 Check Build Settings
- **Open**: `File > Build Settings`
- **Platform**: Should be set to "visionOS"
- **Note**: Target Device and Architecture are set in Xcode after building

---

## Step 2: Create Ultra-Minimal Test Scene

### 2.1 Create New Scene
1. **File > New Scene**
2. **Choose**: "Basic (Built-in)" template
3. **Save**: As "UltraMinimalTest"

### 2.2 Add Basic Camera
1. **Create Empty GameObject**: Name it "MainCamera"
2. **Add Component**: Camera
3. **Set Tag**: "MainCamera"
4. **Position**: (0, 0, -10)
5. **Clear Flags**: Solid Color
6. **Background Color**: Black (0, 0, 0, 1)

### 2.3 Add Basic Light
1. **Create Empty GameObject**: Name it "DirectionalLight"
2. **Add Component**: Light
3. **Type**: Directional
4. **Intensity**: 1
5. **Rotation**: (50, -30, 0)

### 2.4 Add Simple Text
1. **Create Empty GameObject**: Name it "TextCanvas"
2. **Add Component**: Canvas
3. **Render Mode**: World Space
4. **World Camera**: Drag MainCamera here
5. **Add Component**: Canvas Scaler
6. **UI Scale Mode**: Scale With Screen Size
7. **Reference Resolution**: 1920 x 1080

### 2.5 Add Text Element
1. **Right-click TextCanvas**: UI > Text - TextMeshPro
2. **If prompted**: Import TMP Essentials
3. **Set Text**: "TEST TEXT"
4. **Font Size**: 48
5. **Color**: White
6. **Position**: Center of screen

---

## Step 3: Test Ultra-Minimal Scene

### 3.1 Build Settings Check
1. **File > Build Settings**
2. **Platform**: visionOS
3. **Player Settings**: Click "Player Settings"
4. **Other Settings > Rendering**:
   - **Color Space**: Linear
   - **Graphics APIs**: Look for Metal in the list (may be auto-selected)
5. **Note**: Target Device and Architecture settings are in Xcode after building
6. **Alternative**: If you don't see these options, Unity may auto-configure them for visionOS

### 3.2 Build and Test
1. **Build**: Click "Build"
2. **Choose folder**: Create "UltraMinimalBuild"
3. **Open in Xcode**: Let Unity open the Xcode project
4. **In Xcode**: 
   - Select your Vision Pro device
   - Click "Run" (play button)
5. **Expected**: Black screen with white "TEST TEXT"

---

## Step 4: If Ultra-Minimal Test Fails

### 4.1 Check Xcode Project Settings
1. **In Xcode**: Select your project in the navigator
2. **Target**: Select your app target
3. **General Tab**:
   - **Deployment Target**: visionOS 2.5 or later
   - **Frameworks**: Should include PolySpatial frameworks
4. **Build Settings**:
   - **Architectures**: ARM64
   - **Valid Architectures**: ARM64

### 4.2 Check PolySpatial Settings
1. **In Unity**: Go to `Assets > Resources`
2. **Look for**: `PolySpatialSettings.asset`
3. **If missing**: Create new PolySpatial settings
4. **Check settings**: Ensure visionOS is enabled

### 4.3 Check Render Pipeline
1. **In Unity**: Go to `Project Settings > Graphics`
2. **Scriptable Render Pipeline Settings**: Should be set to URP
3. **If not URP**: 
   - Go to `Window > Package Manager`
   - Install "Universal RP"
   - Create new URP Asset
   - Assign to Graphics settings

---

## Step 5: Advanced Troubleshooting

### 5.1 Check Console Logs
1. **In Xcode**: View > Debug Area > Activate Console
2. **Run app**: Look for any error messages
3. **Common errors**:
   - PolySpatial initialization failures
   - Metal shader compilation errors
   - Memory allocation issues

### 5.2 Check Device Compatibility
1. **Vision Pro**: Ensure it's running visionOS 2.5 or later
2. **Xcode**: Ensure it's the latest version
3. **macOS**: Ensure it's compatible with your Xcode version

### 5.3 Test with Different Render Settings
1. **In Unity**: Try different render pipeline settings
2. **Test**: Built-in Render Pipeline vs URP
3. **Check**: Which one works better

---

## Step 6: If Still Failing

### 6.1 Create Even Simpler Test
1. **New Scene**: Basic template
2. **Only Camera**: No lights, no UI
3. **Camera Background**: Bright red
4. **Test**: Should see red screen

### 6.2 Check for Hardware Issues
1. **Different Vision Pro**: Test on another device if available
2. **Different Mac**: Test on different development machine
3. **Clean Build**: Delete build folder, rebuild from scratch

### 6.3 Check Unity Forums
1. **Search**: "visionOS white dots rendering"
2. **Check**: Known issues with your Unity version
3. **Look for**: Workarounds or patches

---

## Step 7: Document Results

### 7.1 Record What You Tried
- [ ] Unity version tested
- [ ] PolySpatial version tested
- [ ] Render pipeline tested
- [ ] Xcode version used
- [ ] Vision Pro OS version
- [ ] What exactly you see (white dots, black screen, etc.)

### 7.2 Next Steps Based on Results
- **If ultra-minimal test works**: Gradually add complexity
- **If ultra-minimal test fails**: Fundamental setup issue
- **If red background test works**: UI/Canvas specific issue
- **If nothing works**: Hardware or compatibility issue

---

## Common Solutions

### Solution 1: Update Everything
```bash
# Update Unity to latest LTS
# Update PolySpatial package
# Update Xcode to latest version
# Update visionOS to latest version
```

### Solution 2: Clean Build
```bash
# Delete Library folder in Unity project
# Delete build folder
# Rebuild from scratch
```

### Solution 3: Check Render Pipeline
```csharp
// Ensure URP is properly configured
// Check shader compatibility
// Verify material settings
```

### Solution 4: PolySpatial Reset
```bash
# Remove PolySpatial package
# Reinstall PolySpatial package
# Reconfigure PolySpatial settings
```

---

## Contact Information

If you've tried all these steps and still have issues:
1. **Unity Version**: [Your version]
2. **PolySpatial Version**: [Your version]
3. **Xcode Version**: [Your version]
4. **Vision Pro OS**: [Your version]
5. **What you see**: [Describe exactly]
6. **Steps tried**: [List what you attempted]

This will help identify if it's a known issue or specific to your setup. 