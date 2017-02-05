namespace VRTK.Keyboard
{
    /// <summary>
    /// A key area containing keys spanning several rows
    /// </summary>
    /// <remarks>
    /// Key areas are used in place of rows when a layout calculator has calculated the placement
    /// of keys in the keyboard layout.
    /// </remarks>
    public interface IKeyArea<Key> where Key : IKey
    {
        Key[] GetKeys();
    }
}
