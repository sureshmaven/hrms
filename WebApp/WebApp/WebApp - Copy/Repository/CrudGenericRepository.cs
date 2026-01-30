using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace Repository
{
    /// <summary>
    /// Generic Repository for CRUD Operations. Extends functionality provided by Generic Repository class
    /// </summary>
    /// <typeparam name="TC">The type of the c.</typeparam>
    /// <typeparam name="T">An Entity to Type 'T'</typeparam>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public abstract class CrudGenericRepository<TC, T, TId> : GenericRepository<TC, T>, ICrudGenericRepository<T, TId>
        where T : class, Entities.IDbEntity<TId>
        where TC : ContextBase, new()
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrudGenericRepository{TC, T, TId}"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        protected CrudGenericRepository(TC dataContext)
            : base(dataContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrudGenericRepository{TC, T, TId}"/> class.
        /// </summary>
        protected CrudGenericRepository()
        {
        }

        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>An Object to Type 'T'</returns>
        public T AddOrUpdate(T entity)
        {
            TId defaultValue = default(TId);
            if (entity.Id.Equals(defaultValue))
                return Add(entity);

            Update(entity);
            return entity;
        }

        /// <summary>
        /// Gets it.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>An Object to Type 'T'</returns>
        public virtual T GetIt(TId id)
        {
            //Use First Or Default with lambda Expression. Raghu 8/7/2012
            return AsQueryable().FirstOrDefault(x => x.Id.Equals(id));
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Boolean indicating if the matching object is removed from repository</returns>
        public virtual bool Delete(TId id)
        {
            Delete(GetIt(id));
            return true;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>An IEnumerable Object to Type 'T'</returns>
        public IEnumerable<T> GetAll()
        {
            return AsQueryable();
        }
    }
}
