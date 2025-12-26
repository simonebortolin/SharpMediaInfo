using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Defines methods for managing a list of recently accessed file paths, including loading, adding, and clearing recent
/// files.
/// </summary>
/// <remarks>Implementations of this interface typically persist the recent files list between application
/// sessions. The maximum number of recent files is controlled by the caller via the maxItems parameter. Thread safety
/// and persistence details may vary by implementation.</remarks>
public interface IRecentFilesService
{
    /// <summary>
    /// Asynchronously loads a collection of items, up to the specified maximum number.
    /// </summary>
    /// <param name="maxItems">The maximum number of items to load. Must be greater than zero.</param>
    /// <returns>A task that represents the asynchronous load operation. The task result contains an observable collection of
    /// strings with up to the specified number of items. The collection will be empty if no items are available.</returns>
    Task<ObservableCollection<string>> LoadAsync(int maxItems);

    /// <summary>
    /// Asynchronously adds the contents of the specified file to the provided collection, up to the specified maximum
    /// number of items.
    /// </summary>
    /// <param name="filePath">The path to the file whose contents are to be added. Cannot be null or empty.</param>
    /// <param name="current">The collection to which items from the file will be added. Cannot be null.</param>
    /// <param name="maxItems">The maximum number of items to add to the collection. Must be greater than zero.</param>
    /// <returns>A task that represents the asynchronous add operation.</returns>
    Task AddAsync(string filePath, ObservableCollection<string> current, int maxItems);

    /// <summary>
    /// Asynchronously removes all items from the specified collection.
    /// </summary>
    /// <param name="current">The collection to be cleared. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous clear operation.</returns>
    Task ClearAsync(ObservableCollection<string> current);
}
