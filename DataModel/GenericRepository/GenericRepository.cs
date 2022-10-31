#region Using Namespaces...

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

#endregion

namespace DataModel.GenericRepository
{    
    /// <summary>
     /// Generic Repository class for Entity Operations
     /// </summary>
     /// <typeparam name="TEntity"></typeparam>

    public class GenericRepository<TEntity> where TEntity : class
    {
        #region Private member variables...
        internal BookingDBEntities Context;
        internal DbSet<TEntity> DbSet;
        #endregion

        #region Public Constructor...
        /// <summary>
        /// Public Constructor,initializes privately declared local variables.
        /// </summary>
        /// <param name="context"></param>
        public GenericRepository(BookingDBEntities context)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }
        #endregion

        #region Public member methods...

        /// <summary>
        /// generic Get method for Entities
        /// </summary>
        /// <returns></returns>
        public virtual void ExecuteSqlCommand(string sql, params object[] parameters)
        {
            Context.Database.ExecuteSqlCommand(sql, parameters);
        }

        /// <summary>
        /// generic Get method for Entities
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get()
        {
            IQueryable<TEntity> query = DbSet;
            return query.ToList();
        }


        /// <summary>
        /// generic Get method for Entities
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetBySQL(string sql)
        {
            return DbSet.SqlQuery(sql).ToList();
        }

        /// <summary>
        /// Generic get method on the basis of id for Entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetByID(object id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// generic Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        /// <summary>
        /// generic Insert Many method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public virtual void InsertMany(IEnumerable<TEntity> entity)
        {
            DbSet.AddRange(entity);
        }

        /// <summary>
        /// generic Insert Many method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public virtual void InsertBulk(IEnumerable<TEntity> entity)
        {
            DbSet.BulkInsert(entity);
        }

        /// <summary>
        /// generic Update Bulk method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateBulk(IEnumerable<TEntity> entity)
        {
            DbSet.BulkUpdate(entity);
        }


        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }


        /// <summary>
        /// Generic Delete All method for the entities
        /// </summary>
        /// <param name="entityToDeleteAll"></param>
        public virtual void DeleteAll()
        {
            foreach (var entity in DbSet.ToList())
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
                DbSet.Remove(entity);
            }
        }
        public virtual void DeleteMany(IEnumerable<TEntity> entity)
        {
            foreach (var ent in entity)
            {
                if (Context.Entry(ent).State == EntityState.Detached)
                {
                    DbSet.Attach(ent);
                }
            }
            DbSet.RemoveRange(entity);
        }
        public virtual void DeleteBulk(IEnumerable<TEntity> entity)
        {
            foreach (var ent in entity)
            {
                if (Context.Entry(ent).State == EntityState.Detached)
                {
                    DbSet.Attach(ent);
                }
            }
            DbSet.BulkDelete(entity);
        }

        /// <summary>
        /// Generic update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public virtual void Update(TEntity entityToUpdate)
        {
            DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        public virtual void UpdateMany(IEnumerable<TEntity> entity)
        {
            foreach (var ent in entity)
            {
                DbSet.Attach(ent);
                Context.Entry(ent).State = EntityState.Modified;
            }
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetMany(Func<TEntity, bool> where)
        {
            return DbSet.Where(where);
        }
        public virtual IEnumerable<TEntity> GetMany(Func<TEntity, bool> where, int pageSize, int pageIndex)
        {
            return DbSet.Where(where).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetManyQueryable(Func<TEntity, bool> where)
        {
            return DbSet.Where(where).AsQueryable();
        }
        /// <summary>
        /// generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public TEntity Get(Func<TEntity, Boolean> where)
        {
            return DbSet.AsNoTracking().Where(where).FirstOrDefault<TEntity>();
        }

        /// <summary>
        /// generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Delete(Func<TEntity, Boolean> where)
        {
            IQueryable<TEntity> objects = DbSet.Where<TEntity>(where).AsQueryable();
            foreach (TEntity obj in objects)
                DbSet.Remove(obj);
        }

        /// <summary>
        /// generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }
        public virtual IQueryable<TEntity> GetAllQueryable()
        {
            return DbSet.AsQueryable();
        }

        /// <summary>
        /// Inclue multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetWithInclude(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            IQueryable<TEntity> query = this.DbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate);
        }

        public IEnumerable<TEntity> GetWithIncludes(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, System.Linq.Expressions.Expression<Func<TEntity, object>> inlcude)
        {
            IQueryable<TEntity> query = this.DbSet;
            query.Include(inlcude);
            return query.Where(predicate);
        }

        /// <summary>
        /// Generic method to check if entity exists
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public bool Exists(object primaryKey)
        {
            return DbSet.Find(primaryKey) != null;
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public TEntity GetSingle(Func<TEntity, bool> predicate)
        {
            return DbSet.Single<TEntity>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public TEntity GetFirst(Func<TEntity, bool> predicate)
        {
            return DbSet.First<TEntity>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public TEntity GetFirstOrDefault(Func<TEntity, bool> predicate)
        {
            return DbSet.AsNoTracking().FirstOrDefault<TEntity>(predicate);
        }

        /// <summary>
        /// Detaches all of the DbEntityEntry objects that have been added to the ChangeTracker.
        /// </summary>
        public void DetachAll()
        {
            foreach (DbEntityEntry dbEntityEntry in this.Context.ChangeTracker.Entries())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }

        #endregion
    }
}
