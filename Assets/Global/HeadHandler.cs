using System;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class HeadHandler : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FreeTrackData
    {
        public int dataid;
        public int camwidth, camheight;
        public Single Yaw, Pitch, Roll, X, Y, Z;
        public Single RawYaw, RawPitch, RawRoll;
        public Single RawX, RawY, RawZ;
        public Single x1, y1, x2, y2, x3, y3, x4, y4;
    }

    // use `static extern` with DllImport to declare a method that is implemented externally.
    // (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/extern)
    // `ref` keyword is like inout in Swift 
    // (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/extern)
    // source code for FreeTrackClient: 
    // https://github.com/PeterN/freetrack/blob/master/FreetrackClient/FTClient.pas
    // source code for OpenTrackClient: 
    // https://github.com/opentrack/opentrack/blob/master/freetrackclient/freetrackclient.c
    [DllImport("FreeTrackClient64")]
    // there will be a function FTGetData in the source code
    public static extern bool FTGetData(ref FreeTrackData data);

    // FreeTrackData: https://github.com/opentrack/opentrack/blob/master/freetrackclient/fttypes.h
    [HideInInspector]
    public float Yaw = 0F;

    [HideInInspector]
    public float Pitch = 0F;

    [HideInInspector]
    public float Roll = 0F;

    [HideInInspector]
    public float X = 0F;

    [HideInInspector]
    public float Y = 0F;

    [HideInInspector]
    public float Z = 0F;

    [HideInInspector]
    public float RawYaw = 0F;

    [HideInInspector]
    public float RawPitch = 0F;

    [HideInInspector]
    public float RawRoll = 0F;

    [HideInInspector]
    public float RawX = 0F;

    [HideInInspector]
    public float RawY = 0F;

    [HideInInspector]
    public float RawZ = 0F;


    [HideInInspector]
    public float x1 = 0F;

    [HideInInspector]
    public float y1 = 0F;

    [HideInInspector]
    public float x2 = 0F;

    [HideInInspector]
    public float y2 = 0F;

    [HideInInspector]
    public float x3 = 0F;

    [HideInInspector]
    public float y3 = 0F;

    [HideInInspector]
    public float x4 = 0F;

    [HideInInspector]
    public float y4 = 0F;

    private HeadHandler.FreeTrackData trackData;

    void Start()
    {
        trackData = new HeadHandler.FreeTrackData();
    }

    // Update is called once per frame
    void Update()
    {
        if (!HeadHandler.FTGetData(ref trackData))
        {
            Debug.Log("FTGetData returned false. FreeTrack likely not working.");
            return;
        }
        HeadHandler.FTGetData(ref trackData);

        // try to use pitch to detect nods (positive is up) - page 6
        // https://link.springer.com/content/pdf/10.1007%2F978-3-319-07491-7_16.pdf
        Yaw = trackData.Yaw;
        Pitch = trackData.Pitch;
        Roll = trackData.Roll;
        X = trackData.X;
        Y = trackData.Y;
        Z = trackData.Z;

        RawYaw = trackData.RawYaw;
        RawPitch = trackData.RawPitch;
        RawRoll = trackData.RawRoll;

        RawX = trackData.RawX;
        RawY = trackData.RawY;
        RawZ = trackData.RawZ;

        x1 = trackData.x1;
        y1 = trackData.y1;
        x2 = trackData.x2;
        y2 = trackData.y2;
        x3 = trackData.x3;
        y3 = trackData.y3;
        x4 = trackData.x4;
        y4 = trackData.y4;

        //var info = string.Format("head X: {0}, head Y: {1}", X, Y);
        // Debug.Log(info);
        // Debug.Log(string.Format("Pitch: {0}", Pitch));
    }
}
