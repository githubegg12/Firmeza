using System.Threading.Tasks;

namespace Firmeza.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a database initializer service.
    /// </summary>
    public interface IDbInitializer
    {
        /// <summary>
        /// Runs the database initialization process (migrations, seeding, etc.).
        /// </summary>
        Task InitializeAsync();
    }
}
