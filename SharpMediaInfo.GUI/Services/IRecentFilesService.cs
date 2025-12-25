using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services
{
    public interface IRecentFilesService
    {
        Task<ObservableCollection<string>> LoadAsync(int maxItems);
        Task AddAsync(string filePath, ObservableCollection<string> current, int maxItems);
        Task ClearAsync(ObservableCollection<string> current);
    }
}
