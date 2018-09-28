using GenericApiLibrary.Entities;
using GenericEntityLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace GenericEntityLibrary.Repositories
{
    public partial class GenericRepository<T,C> where T : GenericEntity, new() where C : DbContext, new()
    {
        public delegate void OnSaveEventHandler(object sender, List<ChangeTracker<T>> changeTracker);
        public event OnSaveEventHandler OnSave;
        public delegate void AfterSaveEventHandler(object sender, List<ChangeTracker<T>> originalChangeTracker, List<ChangeTracker<T>> changeTracker);
        public event AfterSaveEventHandler OnAfterSave;

        internal C _context = new C();
        public C Context
        {
            get { return _context; }
            set { _context = value; }
        }
        public virtual IQueryable<T> GetAll()
        {
            try
            {
                IQueryable<T> query = Context.Set<T>();
                return query;
            }
            catch{throw;}
        }
        public virtual T GetSingle(int oid)
        {
            try
            {
                return _context.Set<T>().Where(e => e.OID == oid).FirstOrDefault();
            }
            catch { throw; }
        }
        public virtual IQueryable<E> OfType<E>() where E : class
        {
            try
            {
                IQueryable<E> query = _context.Set<T>().OfType<E>();
                return query;
            }
            catch { throw; }
        }
        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(predicate);
                return query;
            }
            catch { throw; }
        }
        public T Add(T entity)
        {
            try
            {
                _context.Set<T>().Add(entity);
                Save();
                return entity;
            }
            catch { throw; }
        }
        public List<T> AddRange(List<T> entities)
        {
            try
            {
                _context.Set<T>().AddRange(entities);
                Save();
                return entities;
            }
            catch { throw; }
        }
        public int Delete(int oid)
        {
            try
            {
                var entity = GetSingle(oid);
                if (entity != null)
                {
                    _context.Set<T>().Remove(entity);
                    return Save();
                }
                return 0;
            }
            catch { throw; }
        }
        public List<T> AddOrUpdate(List<T> entities)
        {
            try
            {
                _context.Set<T>().AddOrUpdate(entities.ToArray());
                Save();
                return entities;
            }
            catch { throw; }
        }
        public virtual int Save()
        {
            var originaltracker = Tracker;
            OnSave?.Invoke(this, originaltracker);
            var result = _context.SaveChanges();
            OnAfterSave?.Invoke(this, originaltracker, Tracker);
            return result;
        }

        private List<ChangeTracker<T>> Tracker
        {
            get
            {
                return _context.ChangeTracker.Entries<T>()
                    .Select(e => new ChangeTracker<T>
                    {
                        Entity = e.Entity,
                        EntityState = e.State,
                        OriginalValues = e.State != EntityState.Added ? e.OriginalValues : null,
                        CurrentValues = e.State != EntityState.Deleted ? e.CurrentValues : null
                    })
                    .ToList();
            }
        }

        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
        }
    }
}
