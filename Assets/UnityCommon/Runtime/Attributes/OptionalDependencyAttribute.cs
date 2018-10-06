﻿using System;
using System.Diagnostics;

[assembly: OptionalDependency("UnityGoogleDrive.GoogleDriveRequest", "UNITY_GOOGLE_DRIVE_AVAILABLE")]

/// <summary>
/// Adds a define based on presence of specified type in the project.
/// </summary>
/// <remarks>
/// Unity's conditional compilation utility (<see cref="https://github.com/Unity-Technologies/ConditionalCompilationUtility"/>)
/// uses this attribute to manage the project defines.
/// </remarks>
[Conditional("UNITY_CCU")]
public class OptionalDependencyAttribute : Attribute
{
    public string dependentClass;
    public string define;

    public OptionalDependencyAttribute (string dependentClass, string define)
    {
        this.dependentClass = dependentClass;
        this.define = define;
    }
}