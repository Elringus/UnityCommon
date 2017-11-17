﻿// Copyright 2017 Elringus (Artyom Sovetnikov). All Rights Reserved.

namespace UnityGoogleDrive
{
    using System;
    
    /// <summary>
    /// Implementation is able to retrieve access token.
    /// </summary>
    public interface IAccessTokenProvider
    {
        event Action<IAccessTokenProvider> OnDone;
    
        bool IsDone { get; }
        bool IsError { get; }
        string AccessToken { get; }
    
        void ProvideAccessToken ();
    }
    
}
