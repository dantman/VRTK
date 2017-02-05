namespace VRTK.Keyboard
{
    /// <summary>
    /// A key set containing key areas
    /// </summary>
    public interface IAreaKeyset<KeyArea, Key> : IKeyset where KeyArea : IKeyArea<Key> where Key : IKey
    {
        KeyArea[] GetKeyAreas();
    }
}
