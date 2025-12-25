using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using IMSWEB.Model;
using System.Reflection;
using System.Web;
using Microsoft.AspNet.Identity;

namespace IMSWEB.Data
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private IMSWEBContext _dbContext;

        #region Properties

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected IMSWEBContext DbContext
        {
            get { return _dbContext ?? (_dbContext = DbFactory.Init()); }
        }

        public BaseRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            DbContext.Set<T>().Add(entity);
        }

        public virtual void AddMultiple(IEnumerable<T> list)
        {
            DbContext.Set<T>().AddRange(list);
        }

        public virtual void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = DbContext.Set<T>().Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                DbContext.Set<T>().Remove(obj);
        }

        public virtual IQueryable<T> GetAllByConcernID(int concernID)
        {
            PropertyInfo propInfo = typeof(T).GetProperty("ConcernID");

            return DbContext.Set<T>().Where(ExpressionBuilder.PropertyEquals<T, int>(propInfo, concernID));
        }
        public virtual IQueryable<T> GetAll()
        {
            IQueryable<T> lists = null;
            IQueryable<T> model = null;
            PropertyInfo propInfo = typeof(T).GetProperty("ConcernID");
            if (propInfo != null)
            {
                int concernId = HttpContext.Current.User.Identity.GetConcernId();

                var concern = DbContext.SisterConcerns.FirstOrDefault(i => i.ConcernID == concernId);
                IQueryable<SisterConcern> children = null;
                IQueryable<SisterConcern> Parent = null;
                if (concern.ParentID > 0)
                {
                    children = DbContext.SisterConcerns.Where(i => i.ParentID == concern.ParentID);
                    Parent = DbContext.SisterConcerns.Where(i => i.ConcernID == concern.ParentID);
                    children = children.Concat(Parent);
                }
                else
                {
                    children = DbContext.SisterConcerns.Where(i => i.ParentID == concern.ConcernID);
                    Parent = DbContext.SisterConcerns.Where(i => i.ConcernID == concern.ConcernID);
                    children = children.Concat(Parent);
                }

                foreach (var item in children)
                {
                    model = DbContext.Set<T>().Where(ExpressionBuilder.PropertyEquals<T, int>(propInfo, item.ConcernID));
                    if (model != null)
                    {
                        if (lists == null)
                            lists = model;
                        else
                            lists = lists.Concat(model);

                    }
                }

                return lists;
            }

            return DbContext.Set<T>();
        }

        public virtual IQueryable<T> GetToAll()
        {
            IQueryable<T> lists = null;
            IQueryable<T> model = null;
            PropertyInfo propInfo = typeof(T).GetProperty("ConcernID");
            if (propInfo != null)
            {
                int concernId = HttpContext.Current.User.Identity.GetConcernId();

                var concern = DbContext.SisterConcerns.FirstOrDefault(i => i.ConcernID == concernId);
                IQueryable<SisterConcern> children = null;
                IQueryable<SisterConcern> Parent = null;
                if (concern.ParentID > 0)
                {
                    children = DbContext.SisterConcerns.Where(i => i.ParentID == concern.ParentID);
                    Parent = DbContext.SisterConcerns.Where(i => i.ConcernID == concern.ParentID);
                    children = children.Concat(Parent);
                }
                else
                {
                    children = DbContext.SisterConcerns.Where(i => i.ParentID == concern.ConcernID);
                    Parent = DbContext.SisterConcerns.Where(i => i.ConcernID == concern.ConcernID);
                    children = children.Concat(Parent);
                }

                foreach (var item in children)
                {
                    model = DbContext.Set<T>().Where(ExpressionBuilder.PropertyEquals<T, int>(propInfo, item.ConcernID));
                    if (model != null)
                    {
                        if (lists == null)
                            lists = model;
                        else
                            lists = lists.Concat(model);

                    }
                }

                return lists;
            }

            return DbContext.Set<T>();
        }
        public virtual IQueryable<T> GetAllByConcern(int concernId)
        {
            IQueryable<T> lists = null;
            IQueryable<T> model = null;
            PropertyInfo propInfo = typeof(T).GetProperty("ConcernID");
            if (propInfo != null)
            {

                var concern = DbContext.SisterConcerns.FirstOrDefault(i => i.ConcernID == concernId);
                IQueryable<SisterConcern> children = null;
                IQueryable<SisterConcern> Parent = null;
                if (concern.ParentID > 0)
                {
                    children = DbContext.SisterConcerns.Where(i => i.ParentID == concern.ParentID);
                    Parent = DbContext.SisterConcerns.Where(i => i.ConcernID == concern.ParentID);
                    children = children.Concat(Parent);
                }
                else
                {
                    children = DbContext.SisterConcerns.Where(i => i.ParentID == concern.ConcernID);
                    Parent = DbContext.SisterConcerns.Where(i => i.ConcernID == concern.ConcernID);
                    children = children.Concat(Parent);
                }

                foreach (var item in children)
                {
                    model = DbContext.Set<T>().Where(ExpressionBuilder.PropertyEquals<T, int>(propInfo, item.ConcernID));
                    if (model != null)
                    {
                        if (lists == null)
                            lists = model;
                        else
                            lists = lists.Concat(model);

                    }
                }

                return lists;
            }

            return DbContext.Set<T>();
        }

        public virtual IQueryable<T> All
        {
            get
            {
                PropertyInfo propInfo = typeof(T).GetProperty("ConcernID");
                PropertyInfo propInfoNew = typeof(T).GetProperty("ConcernId");
                if (propInfo != null)
                {
                    int concernId = HttpContext.Current.User.Identity.GetConcernId();
                    return GetAll().Where(ExpressionBuilder.PropertyEquals<T, int>(propInfo, concernId));
                }
                else if (propInfoNew != null)
                {
                    int concernId = HttpContext.Current.User.Identity.GetConcernId();
                    return GetAll().Where(ExpressionBuilder.PropertyEquals<T, int>(propInfoNew, concernId));
                }
                else
                {
                    return DbContext.Set<T>();
                }
                //if (propInfo != null)
                //{
                //    int concernId = HttpContext.Current.User.Identity.GetConcernId();
                //    return DbContext.Set<T>().Where(ExpressionBuilder.PropertyEquals<T, int>(propInfo, concernId));


                //}
                //else
                //{
                //    return DbContext.Set<T>();
                //}
            }
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbContext.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return DbContext.Set<T>().Where(predicate);
        }

        public IEnumerable<T> ExecSP<T>(string sql, params object[] parameters)
        {
            return DbContext.Database.SqlQuery<T>(sql, parameters);
        }

        public IEnumerable<T> SQLQueryList<T>(string sql)
        {
            return DbContext.Database.SqlQuery<T>(sql);
        }

        public T SQLQuery<T>(string sql)
        {
            return DbContext.Database.SqlQuery<T>(sql).FirstOrDefault();
        }

        public IEnumerable<T> SQLQueryList<T>(string sql, params object[] parameters)
        {
            return DbContext.Database.SqlQuery<T>(sql, parameters);
        }

        public T SQLQuery<T>(string sql, params object[] parameters)
        {
            return DbContext.Database.SqlQuery<T>(sql, parameters).FirstOrDefault();
        }
    }
}
