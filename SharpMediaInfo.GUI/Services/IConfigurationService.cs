using SharpMediaInfo.GUI.Models;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services
{
    public interface IConfigurationService
    {
        Task<AppConfiguration> LoadConfigurationAsync();
        Task SaveConfigurationAsync(AppConfiguration configuration);
        void Attach(AppConfiguration configuration);
    }
}