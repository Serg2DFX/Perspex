﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;

namespace Perspex.Platform
{
    public interface IPlatformSettings
    {
        Size DoubleClickSize { get; }

        TimeSpan DoubleClickTime { get; }

        double RenderScalingFactor { get; }

        double LayoutScalingFactor { get; }
    }
}
