using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{

    /// <summary>
    /// IGenericRepository Interface
    /// </summary>
    /// <typeparam name="T">An Object to Type 'T'</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        T Add(T entity);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(T entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(T entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="disableProxyForUpdate">if set to <c>true</c> [disable proxy for update].</param>
        void Update(T entity, bool disableProxyForUpdate);

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();

        /// <summary>
        /// Ases the queryable.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IQueryable<T> AsQueryable(Expression<Func<T, bool>> predicate = null);
        /// <summary>
        /// Execute stored procedure
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>  //Addded by Avinash
        IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);

    }
}
