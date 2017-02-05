namespace VRTK.Keyboard
{
    /// <summary>
    /// An abstract key layout, containing a number of named keysets
    /// </summary>
    /// <remarks>
    /// This acts as the abstract root for keyboard data structures of all types.
    /// </remarks>
    public interface IKeyLayout<Keyset> where Keyset : IKeyset
    {
        Keyset[] GetKeysets();
    }
}
