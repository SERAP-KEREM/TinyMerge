namespace _Main._Interfaces
{
    public interface ISelector
    {
        /// <summary>
        /// Selects the specified selectable object.
        /// </summary>
        void Select(ISelectable selectable);

        /// <summary>
        /// Deselects the specified selectable object.
        /// </summary>
        void DeSelect(ISelectable selectable);
    }
}
