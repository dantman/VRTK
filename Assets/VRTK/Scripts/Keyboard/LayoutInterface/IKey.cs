namespace VRTK.Keyboard
{
    /// <summary>
    /// The core interface defining metadata a key requirires for `VRTK_Keyboard` to
    /// handle key presses for it.
    /// </summary>
    public interface IKey
    {
        /// <summary>
        /// Get the class of this key
        /// </summary>
        /// <returns></returns>
        KeyClass GetKeyClass();
        /// <summary>
        /// Get the character to type if key class is KeyClass.Character
        /// </summary>
        /// <returns>The character to type</returns>
        char GetCharacter();
        /// <summary>
        /// Get the keyset to switch to if key class is KeyClass.Keyset
        /// </summary>
        /// <returns>The keyset to switch to</returns>
        int GetKeyset();
    }
}