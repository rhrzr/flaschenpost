﻿using Android;
using Android.App;
using Android.Runtime;

[assembly: UsesPermission(Manifest.Permission.AccessNetworkState)]

namespace FlaschenpostToDo;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}