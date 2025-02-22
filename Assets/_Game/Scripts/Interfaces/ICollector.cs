namespace _Main._Interfaces
{
    /// <summary>
    /// Interface for objects that can collect collectable items.
    /// </summary>
    public interface ICollector
    {
        /// <summary>
        /// Collects the specified collectable object.
        /// </summary>
        /// <param name="collectable">The object to collect.</param>
      
        void Collect(ICollectable collectable);
    }
}