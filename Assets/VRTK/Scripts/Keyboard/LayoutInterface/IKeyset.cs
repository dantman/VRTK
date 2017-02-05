namespace VRTK.Keyboard
{
    /// <summary>
    /// A basic key set
    /// </summary>
    /// <remarks>
    /// Implementations should use `IRowKeyset` and `IAreaKeyset`.
    /// 
    /// This interface abstracts keysets down to the name so it can be passed around by code
    /// that doesn't care what type of keyset is in use and/or only need to list names.
    /// </remarks>
    public interface IKeyset
    {
        string GetName();
    }
}
