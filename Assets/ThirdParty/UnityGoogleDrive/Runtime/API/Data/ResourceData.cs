﻿// Copyright 2017 Elringus (Artyom Sovetnikov). All Rights Reserved.

namespace UnityGoogleDrive
{
    namespace Data
    {
        /// <summary>
        /// Base class for a Google Drive resource data representation.
        /// </summary>
        public abstract class ResourceData
        {
            /// <summary>
            /// Identifies what kind of resource this is.
            /// </summary>
            public abstract string Kind { get; }
    
        }
    }
    
}
