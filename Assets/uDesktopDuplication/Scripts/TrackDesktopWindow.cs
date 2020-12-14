using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using uDesktopDuplication;
using UnityEngine.Rendering;


public class TrackDesktopWindow : MonoBehaviour
{
    private const int MONITOR_ID_INVALID = -1;
    private const int MONITOR_DEFAULTTONEAREST = 2;

    public enum ShaderType
    {
        UNSET,
        LEGACY,
        URP,
        HDRP
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct MONITORINFOEX
    {
        public int Size;
        public RECT Monitor;
        public RECT WorkArea;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow(string strClassName, string strWindowName);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

    [DllImport("user32.dll")]
    static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    static string MonitorEnumProc(IntPtr hMonitor/*, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData*/)
    {
        MONITORINFOEX mi = new MONITORINFOEX();
        mi.Size = Marshal.SizeOf(typeof(MONITORINFOEX));
        if (GetMonitorInfo(hMonitor, ref mi))
            return mi.DeviceName;

        return "";
    }

    static Regex normalizeMonitorNamePattern = new Regex(@"[^a-zA-Z0-9 -]");
    static string GetNormalizedMonitorName(string monitorName)
    {
        return normalizeMonitorNamePattern.Replace(monitorName, "");
    }
    static Rect GetNormalizedRectRelativeToMonitor(RECT globalPos, Monitor targetMonitor)
    {
        Rect result = new Rect();

        result.yMin = (float)(globalPos.Top - targetMonitor.top) / targetMonitor.height;
        result.yMax = (float)(globalPos.Bottom - targetMonitor.top) / targetMonitor.height;
        result.xMin = (float)(globalPos.Left - targetMonitor.left) / targetMonitor.width;
        result.xMax = (float)(globalPos.Right - targetMonitor.left) / targetMonitor.width;

        return result;
    }

    // Material is optional
    public Material renderTextureMaterial;
    // Render texture is optional
    public RenderTexture renderTexture;
    
    public string targetWName = "";
    // test names: Calculator, Rocket League (64-bit, DX11, Cooked)
    public Rect normalizedWindowRectangle;

    public int requestMonitorId = MONITOR_ID_INVALID;
    public ShaderType shaderType;
    
    public int currentMonitorId { get => _currentMonitorId; }

    private const string legacyTexturePropertyName = "_MainTex";
    private const string urpTexturePropertyName = "_BaseMap";
    private const string hdrpTexturePropertyName = "_BaseMap";

    private string targetTexturePropertyName;

    Monitor monitor;

    private int _currentMonitorId = MONITOR_ID_INVALID;


    void OnEnable()
    {
        // Determine the graphics type to target the texture property name
        if (shaderType == ShaderType.UNSET)
        {
            if (GraphicsSettings.currentRenderPipeline)
            {
                if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
                {
                    shaderType = ShaderType.HDRP;
                }
                else // assuming here we only have HDRP or URP options here
                {
                    shaderType = ShaderType.URP;
                }
            }
            else
            {
                shaderType = ShaderType.LEGACY;
            }
        }

        Manager.onReinitialized += Reinitialize;
    }

    void OnDisable()
    {
        Manager.onReinitialized -= Reinitialize;
    }

    void Update()
    {
        KeepMonitor();
        RequireUpdate();
        UpdateMaterial();
    }

    void KeepMonitor()
    {
        int monitorId = MONITOR_ID_INVALID;
        RECT output = new RECT();
        bool trackWindow = targetWName != null && targetWName.Length > 0;

        // if specific window tracking is requested
        if (trackWindow)
        {
            

            IntPtr hwnd = FindWindow(null, targetWName);
            if (hwnd == IntPtr.Zero) return;
            GetWindowRect(hwnd, ref output);

            string targetMonitorName = GetNormalizedMonitorName(MonitorEnumProc(MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST)));


            foreach (var target in Manager.monitors)
            {
                if (targetMonitorName == GetNormalizedMonitorName(target.name))
                {
                    monitorId = target.id;
                }
            }
        }
        // if just the whole monitor is requested
        else if(requestMonitorId != -1)
        {
            monitorId = requestMonitorId;
        }

        if(monitorId != MONITOR_ID_INVALID && monitorId != currentMonitorId)
        {
            _currentMonitorId = monitorId;
            Reinitialize();
        }

        // update window position
        if (monitor != null)
        {
            if (trackWindow)
            {
                normalizedWindowRectangle = GetNormalizedRectRelativeToMonitor(output, monitor);
            }
            else
            {
                normalizedWindowRectangle = new Rect(0, 0, 1, 1); // whole window
            }
            
        }
    }

    void RequireUpdate()
    {
        if (monitor != null)
        {
            monitor.shouldBeUpdated = true;
        }
    }

    void Reinitialize()
    {
        // Monitor instance is released here when initialized.
        if(currentMonitorId != MONITOR_ID_INVALID)
        {
            monitor = Manager.GetMonitor(currentMonitorId);
        }
        else
        {
            monitor = null;
        }
    }

    void UpdateMaterial()
    {
        if(renderTexture != null && monitor != null)
        {
            Graphics.Blit(monitor.texture, renderTexture);
        }

        if(renderTextureMaterial != null)
        {
            renderTextureMaterial.SetTextureOffset(GetTexturePropertyName(), new Vector2(normalizedWindowRectangle.x, normalizedWindowRectangle.y));
            renderTextureMaterial.SetTextureScale(GetTexturePropertyName(), new Vector2(normalizedWindowRectangle.width, normalizedWindowRectangle.height));
        }
    }

    private string GetTexturePropertyName()
    {
        switch (shaderType)
        {
            case ShaderType.LEGACY:
                return legacyTexturePropertyName;
            case ShaderType.URP:
                return urpTexturePropertyName;
            case ShaderType.HDRP:
                return hdrpTexturePropertyName;
        }

        return legacyTexturePropertyName;
    }
}
