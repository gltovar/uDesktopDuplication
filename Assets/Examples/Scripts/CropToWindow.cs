using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uDesktopDuplication;

public class CropToWindow : MonoBehaviour
{
    public TrackDesktopWindow testRenderTex;
    Material mat;

    int lastMonitorId = -1;

    // Start is called before the first frame update
    void Start()
    {
        if(testRenderTex != null)
        {
            testRenderTex.targetWName = "Calculator";
        }
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(testRenderTex && testRenderTex.currentMonitorId != -1 && mat)
        {
            if(lastMonitorId != testRenderTex.currentMonitorId)
            {
                lastMonitorId = testRenderTex.currentMonitorId;
                mat.SetTexture("_MainTex", Manager.GetMonitor(lastMonitorId).texture);
            }

            mat.SetTextureOffset("_MainTex", new Vector2(testRenderTex.normalizedWindowRectangle.x, testRenderTex.normalizedWindowRectangle.y));
            mat.SetTextureScale("_MainTex", new Vector2(testRenderTex.normalizedWindowRectangle.width, testRenderTex.normalizedWindowRectangle.height));
        }
    }
}
