uDesktopDuplication
===================

![MIT License](http://img.shields.io/badge/license-MIT-blue.svg?style=flat)

**uDesktopDuplication** is an Unity asset to use the realtime screen capture as `Texture2D` using Desktop Duplication API.


ScreenShot
----------
![uDesktopDuplication](https://raw.githubusercontent.com/wiki/hecomi/uDesktopDuplication/animation.gif)


Environment
-----------
- Windows 8 / 10
- Unity 5


Installation
------------

This fork allows for installation through UPM
Just add`https://github.com/gltovar/uDesktopDuplication.git#upm` to the package manager

Alternatively the following still works, but only contains changes till 1.7.0

Please download the latest *uDesktopDuplication.unitypackage* from the [release page](https://github.com/hecomi/uDesktopDuplication/releases).


Usage
-----
Attach `uDesktopDuplication/Texture` component to the target object, then its main texture will be replaced with the captured screen. Please see example scenes for more details.

Changelog
-----
1.7.2
- Added new script `TrackDesktopWindow` that allows a WName of a window to track the monitor and generate a normalized rectangle for cropping
- `TrackDesktopWindow` also supports getting entire monitor if WName is empty using `requestMonitorId`
- New example scene `WindowTracking` contains a sample of using window tracking with script or with creating a material and render texture

1.7.1
- Initial commit with changes to support UPM


License
-------
The MIT License (MIT)

Copyright (c) 2016 hecomi

Copyright (c) 2020 gltovar

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
