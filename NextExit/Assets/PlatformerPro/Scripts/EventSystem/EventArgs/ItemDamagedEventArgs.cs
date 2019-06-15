using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
    /// <summary>
    /// Item event args with additioal args for a damaged event.s
    /// </summary>
    public class ItemDamagedEventArgs : ItemEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:PlatformerPro.ItemDamagedEventArgs"/> is destroyed.
        /// </summary>
        /// <value><c>true</c> if is destroyed; otherwise, <c>false</c>.</value>
        public bool IsDestroyed
        {
            get; protected set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PlatformerPro.ItemDamagedEventArgs"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="character">Character.</param>
        /// <param name="isDestroyed">If set to <c>true</c> is destroyed.</param>
        public ItemDamagedEventArgs(string type, Character character, bool isDestroyed) : base(type, character)
        {
            IsDestroyed = isDestroyed;
            Amount = 1;
        }
    }
}