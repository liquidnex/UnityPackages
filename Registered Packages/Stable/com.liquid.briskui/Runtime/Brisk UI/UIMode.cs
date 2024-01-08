using System;

namespace Liquid.BriskUI
{
    /// <summary>
    /// Enumeration of UI modes.
    /// Attributes of a UI form can be expressed using the bitwise OR operator concatenation.
    /// </summary>
    [Flags]
    public enum UIMode
    {
        NONE = 1,
        NORMAL = 1 << 1,
        POPUP = 1 << 2,
        UNIQUE = 1 << 3,
        KEEP_TOP = 1 << 4,
        KEEP_BOTTOM = 1 << 5
    }
}