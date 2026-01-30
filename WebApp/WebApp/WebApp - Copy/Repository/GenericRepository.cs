using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    /// <summary>
    /// Generic Repository. Allows EntityFramework based CRUD access to the Entity within the Context 
    /// </summary>
    /// <typeparam name="C">The type of the c.</typeparam>
    /// <typeparam name="T">An Entity to Type 'T'</typeparam>
    public abstract class GenericRepository<C, T> : IGenericRepository<T>
        where T : class
        where C : ContextBase, new()
    {
        /// <summary>
        /// The _entities
        /// </summary>
        private C _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{C, T}"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected GenericRepository(C entities)
        {
            _entities = entities;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{C, T}"/> class.
        /// </summary>
        protected GenericRepository()
            : this(new C())
        {
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public C Context { get; set; }

        /// <summary>
        /// As'es the queryable.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An IQueryable to Type 'T'</returns>
        public virtual IQueryable<T> AsQueryable(Expression<Func<T, bool>> predicate = null)
        {
            //Modified on 8/7/2012 - Raghu. Return the Entity based on query
            return (!ReferenceEquals(null, predicate)
                        ? _entities.Set<T>().Where(predicate)
                        : _entities.Set<T>().AsQueryable());
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>An Object to Type 'T'</returns>
        public virtual T Add(T entity)
        {
            T addedEntity = _entities.Set<T>().Add(entity);
            Save();
            return addedEntity;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(T entity)
        {
            _entities.Set<T>().Remove(entity);
            Save();
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Update(T entity)
        {
            Update(entity, true);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="disableProxyForUpdate">if set to <c>true</c> [disable proxy for update].</param>
        public virtual void Update(T entity, bool disableProxyForUpdate = false)
        {
            _entities.Entry(entity).State = EntityState.Modified;
            if (disableProxyForUpdate)
            {
                //Added on 8/2/2012 - Raghu. Disable Proxy Creation and Updates while updating data into the DB. 
                //This will ensure that dependent objects are not cascased on update. Only the IDs get updated in this case.
                _entities.Configuration.ProxyCreationEnabled = false;
            }

            Save();

            if (disableProxyForUpdate)
            {
                //Added on 8/2/2012 - Raghu. Enable Proxy Creation and Updates after updating data into the DB
                //This will ensure that dependent objects are not cascased on update. Only the IDs get updated in this case.
                _entities.Configuration.ProxyCreationEnabled = true;
            }
        }
        /// <summary>
        /// Saves this instance.
        /// </summary>
        public virtual void Save()
        {
            _entities.SaveChanges();
        }
        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
                if (disposing)
                    _entities.Dispose();
            this.disposed = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return _entities.Database.SqlQuery<T>(query, parameters);
        }
    }
}
