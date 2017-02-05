namespace VRTK.Keyboard
{
    /// <summary>
    /// Keyboard key type
    /// </summary>
    /// <param name="Character">A key with character that should be typed.</param>
    /// <param name="KeysetModifier">A key that switches keysets.</param>
    /// <param name="Backspace">A backspace/delete key.</param>
    /// <param name="Enter">An enter/return key.</param>
    /// <param name="Done">A done key.</param>
    public enum KeyClass
    {
        /// <summary>
        /// A key with character that should be typed.
        /// </summary>
        Character,
        /// <summary>
        /// A key that switches keysets
        /// </summary>
        KeysetModifier,
        /// <summary>
        /// A backspace/delete key.
        /// </summary>
        Backspace,
        /// <summary>
        /// An enter/return key.
        /// </summary>
        Enter,
        /// <summary>
        /// A done key.
        /// </summary>
        Done
    }
}
