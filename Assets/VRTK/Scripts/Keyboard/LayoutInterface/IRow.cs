namespace VRTK.Keyboard
{
    /// <summary>
    /// A row containing keys ordered left to right
    /// </summary>
    public interface IRow<Key> where Key : IKey
    {
        Key[] GetKeys();
    }
}
