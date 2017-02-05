namespace VRTK.Keyboard
{
    /// <summary>
    /// A key set containing rows
    /// </summary>
    public interface IRowKeyset<Row, Key> : IKeyset where Row : IRow<Key> where Key : IKey
    {
        Row[] GetRows();
    }
}
