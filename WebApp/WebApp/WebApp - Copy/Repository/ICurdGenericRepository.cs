using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    /// <summary>
    /// Interface ICrudGenericRepository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface ICrudGenericRepository<T, in TId> : IGenericRepository<T>
        where T : class
    {
        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        T AddOrUpdate(T entity);

        /// <summary>
        /// Gets it.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        T GetIt(TId id);

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        bool Delete(TId id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();
    }
}
