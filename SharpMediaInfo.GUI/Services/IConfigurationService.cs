using SharpMediaInfo.GUI.Models;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services
{
    /// <summary>
    /// Defines methods for loading, saving, and managing application configuration data.
    /// </summary>
    /// <remarks>Implementations of this interface provide mechanisms to persist and retrieve application
    /// configuration settings. Methods may support asynchronous operations to accommodate various storage backends,
    /// such as files, databases, or remote services.</remarks>
    public interface IConfigurationService
    {
        /// <summary>
        /// Asynchronously loads the application configuration settings.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="AppConfiguration"/> object with the loaded configuration settings.</returns>
        Task<AppConfiguration> LoadConfigurationAsync();

        /// <summary>
        /// Asynchronously saves the specified application configuration.
        /// </summary>
        /// <param name="configuration">The application configuration to be saved. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveConfigurationAsync(AppConfiguration configuration);

        /// <summary>
        /// Associates the specified application configuration with the current instance.
        /// </summary>
        /// <param name="configuration">The application configuration to attach. Cannot be null.</param>
        void Attach(AppConfiguration configuration);
    }
}