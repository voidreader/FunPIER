using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SA.Android.App.View
{

    /// <summary>
    /// Standard constants and tools for placing an object within a potentially larger container.
    /// </summary>
    public class AN_Gravity  {


        /// <summary>
        /// Raw bit controlling whether the right/bottom edge is clipped to its container, 
        /// based on the gravity direction being applied.
        /// </summary>
        public const int AXIS_CLIP = 8;

        /// <summary>
        /// Raw bit controlling how the right/bottom edge is placed.
        /// </summary>
        public const int AXIS_PULL_AFTER = 4;

        /// <summary>
        /// Raw bit controlling how the left/top edge is placed.
        /// </summary>
        public const int AXIS_PULL_BEFORE = 2;

        /// <summary>
        /// Raw bit indicating the gravity for an axis has been specified.
        /// </summary>
        public const int AXIS_SPECIFIED = 1;

        /// <summary>
        /// Bits defining the horizontal axis.
        /// </summary>
        public const int AXIS_X_SHIFT = 0;

        /// <summary>
        /// Bits defining the vertical axis.
        /// </summary>
        public const int AXIS_Y_SHIFT = 4;

        /// <summary>
        /// Push object to the bottom of its container, not changing its size.
        /// </summary>
        public const int BOTTOM = 80;

        /// <summary>
        /// Place the object in the center of its container in both the vertical and horizontal axis, not changing its size.
        /// </summary>
        public const int CENTER = 17;

        /// <summary>
        /// Place object in the horizontal center of its container, not changing its size.
        /// </summary>
        public const int CENTER_HORIZONTAL = 1;

        /// <summary>
        /// Place object in the vertical center of its container, not changing its size.
        /// </summary>
        public const int CENTER_VERTICAL = 16;

        /// <summary>
        /// Flag to clip the edges of the object to its container along the horizontal axis.
        /// </summary>
        public const int CLIP_HORIZONTAL = 8;

        /// <summary>
        /// Flag to clip the edges of the object to its container along the vertical axis.
        /// </summary>
        public const int CLIP_VERTICAL = 128;

        /// <summary>
        /// Push object to x-axis position at the end of its container, not changing its size.
        /// </summary>
        public const int END = 8388613;

        /// <summary>
        /// Grow the horizontal and vertical size of the object if needed so it completely fills its container.
        /// </summary>
        public const int FILL = 119;

        /// <summary>
        /// Grow the horizontal size of the object if needed so it completely fills its container.
        /// </summary>
        public const int FILL_HORIZONTAL = 7;

        /// <summary>
        /// Grow the vertical size of the object if needed so it completely fills its container.
        /// </summary>
        public const int FILL_VERTICAL = 112;

        /// <summary>
        /// Binary mask to get the absolute horizontal gravity of a gravity.
        /// </summary>
        public const int HORIZONTAL_GRAVITY_MASK = 7;

        /// <summary>
        /// Push object to the left of its container, not changing its size.
        /// </summary>
        public const int LEFT = 3;

        /// <summary>
        /// Constant indicating that no gravity has been set
        /// </summary>
        public const int NO_GRAVITY = 0;

        /// <summary>
        /// Binary mask for the horizontal gravity and script specific direction bit.
        /// </summary>
        public const int RELATIVE_HORIZONTAL_GRAVITY_MASK = 8388615;

        /// <summary>
        /// Raw bit controlling whether the layout direction is relative or not (START/END instead of absolute LEFT/RIGHT).
        /// </summary>
        public const int RELATIVE_LAYOUT_DIRECTION = 8388608;

        /// <summary>
        /// Push object to the right of its container, not changing its size.
        /// </summary>
        public const int RIGHT = 5;

        /// <summary>
        /// Push object to x-axis position at the start of its container, not changing its size.
        /// </summary>
        public const int START = 8388611;

        /// <summary>
        /// Push object to the top of its container, not changing its size.
        /// </summary>
        public const int TOP = 48;

        /// <summary>
        /// Binary mask to get the vertical gravity of a gravity.
        /// </summary>
        public const int VERTICAL_GRAVITY_MASK = 112;
    }
}