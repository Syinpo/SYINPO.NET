using System.Threading.Tasks;

namespace Syinpo.Core.Hangfire
{
    /// <summary>
    /// Defines interface of a background job.
    /// </summary>
    public interface IBackgroundJob<in TArgs>
    {
        /// <summary>
        /// Executes the job with the <see cref="args"/>.
        /// </summary>
        /// <param name="args">Job arguments.</param>
        Task Execute(TArgs args);
    }
}