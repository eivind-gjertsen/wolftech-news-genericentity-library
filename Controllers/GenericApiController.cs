using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Linq.Expressions;
using GenericEntityLibrary.Repositories;
using GenericEntityLibrary.Entities;
using System.Net.Http;
using System.Net;
using System.Threading;
using GenericApiLibrary.Entities;

namespace GenericEntityLibrary.Controllers
{
    /// <summary> 
    /// Generic ApiController, T Entities.Entity and C DbContext. 
    /// </summary> 
    [AllowAnonymous]
    public class GenericApiController<T, C> : ApiController where T : GenericEntity, new() where C : DbContext, new()
    {
        public Logger Logger = LogManager.GetCurrentClassLogger();
        public GenericRepository<T, C> _repository = new GenericRepository<T, C>();
        public GenericRepository<T, C> Repository
        {
            get { return _repository; }
            set { _repository = value; }
        }
        public C Context{get { return _repository.Context; }}

        public GenericApiController()
        {
            Repository.OnSave += Repository_OnSave;
            Repository.OnAfterSave += Repository_OnAfterSave;
        }

        
        public virtual void Repository_OnAfterSave(object sender, IEnumerable<ChangeTracker<T>> originalChangeTracker, IEnumerable<ChangeTracker<T>> changeTracker)
        { 
        //    if(origTracker.Count() == tracker.Count())
        //        for(int i = 0; i <= origTracker.Count(); i++)
        //            entity.Entity.OnAfterSaved(entity, entity.State);
        }
        public virtual void Repository_OnSave(object sender, IEnumerable<ChangeTracker<T>> changeTracker)
        {
        //    foreach (var entity in entities)
        //        entity.Entity.OnSaveChanges(entity, entity.State);
        }

        #region API
        /// <summary> 
        /// Gets all Entities for the inherit ApiController
        /// </summary> 
        [HttpGet]
        public virtual HttpResponseMessage GetAll()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _repository.GetAll());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed Get All for Type: " + typeof(T).GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        /// <summary> 
        /// Gets single entity with oid equal to incoming parameter oid
        /// </summary> 
        [HttpGet]
        public virtual HttpResponseMessage GetSingle(int oid)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _repository.GetSingle(oid));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed Get All for Type: " + typeof(T).GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /// <summary> 
        /// Adds entity to databse
        /// </summary> 
        [HttpPost]
        public virtual HttpResponseMessage Add(T entity)
        {
            try
            {
                AddStandardValues(entity);
                return Request.CreateResponse<T>(HttpStatusCode.OK, _repository.Add(entity));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed Get All for Type: " + typeof(T).GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /// <summary> 
        /// Add list of enitites to database
        /// </summary> 
        [HttpPost]
        public virtual HttpResponseMessage AddRange(List<T> entities)
        {
            try
            {
                foreach(var entity in entities)
                    AddStandardValues(entity);
                return  Request.CreateResponse<List<T>>(HttpStatusCode.OK, _repository.AddRange(entities));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Adding range for Type: " + typeof(T).GetType().Name);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        /// <summary> 
        /// Delete entity from databse. Returns oid value
        /// </summary> 
        [HttpGet]
        public virtual HttpResponseMessage Delete(int oid)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _repository.Delete(oid));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Could not Delete entity for type: " + typeof(T).GetType());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /// <summary> 
        /// Add or Update entity list in database, returns oid value
        /// </summary> 
        [HttpPost]
        public virtual HttpResponseMessage AddOrUpdate(List<T> entities)
        {
            try
            {
                foreach (var entity in entities)
                    AddStandardValues(entity);
                return Request.CreateResponse(HttpStatusCode.OK, _repository.AddOrUpdate(entities));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Could not Delete entity for type: " + typeof(T).GetType());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion
        #region interal
        public IQueryable<E> OfType<E>() where E : class
        {
            try
            {
                return _repository.OfType<E>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Could not get type of " + typeof(E).GetType().Name + " for type: " + typeof(T).GetType().Name);
                throw ex;
            }
        }
        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _repository.FindBy(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Could not FindBy for type: " + typeof(T).GetType().Name);
                throw ex;
            }
        }
        public void AddStandardValues(T entity)
        {
            entity.ModifiedByName = Thread.CurrentPrincipal.Identity.Name;
            entity.ModifiedOn = DateTime.UtcNow;
            if (entity.OID <= 0)
            {
                entity.CreatedByName = entity.ModifiedByName;
                entity.CreatedOn = entity.ModifiedOn;
            }
        }
        public int Save()
        {
            try { return _repository.Save(); }
            catch { throw; }
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (_repository != null)
                _repository.Dispose();
            base.Dispose(disposing);
        }
    }
}