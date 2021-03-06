﻿using Microsoft.EntityFrameworkCore;
using NewsDump.Lib.Util;
using Olive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewsDump.Lib.Data
{
    public static class Repository
    {
        public static Repository<T> Of<T>() where T : BaseModel => new Repository<T>();
        public static void PerformMigration()
        {
            using (var context = new Context())
            {
                context.Database.Migrate();
            }
        }

        public static void ResetDb()
        {
            try
            {
                var appDomain = AppDomain.CurrentDomain;
                var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
                Path.Combine(basePath, "dumperdb.sqlite").AsFile().Delete();


                PerformMigration();
                EventBus.Notify("Database reset successful, DB is now in clean slate", "info");
                EventBus.Notify("پایگاه داده پاکسازی شد", "DoneOperation");

            }
            catch (Exception ex)
            {

                EventBus.Notify(ex.Message, "error");
            }

        }
    }

    public class Repository<TObject> where TObject : BaseModel
    {
        protected Context _context;
        public Repository()
        {
            _context = new Context();
        }

        public Repository(Context context)
        {
            _context = context;
        }
        public ICollection<TObject> GetAll()
        {
            return _context.Set<TObject>().ToList();
        }
        public ICollection<TObject> Take(int count)
        {
            return _context.Set<TObject>().Take(count).ToList();
        }

        public ICollection<TObject> Take(int count, int skip)
        {
            return _context.Set<TObject>().Skip(skip).Take(count).ToList();
        }

        public async Task<ICollection<TObject>> GetAllAsync()
        {
            return await _context.Set<TObject>().ToListAsync();
        }

        public TObject Get(int? id)
        {
            return _context.Set<TObject>().Find(id);
        }

        public async Task<TObject> GetAsync(int id)
        {
            return await _context.Set<TObject>().FindAsync(id);
        }

        public TObject Find(Expression<Func<TObject, bool>> match)
        {
            return _context.Set<TObject>().SingleOrDefault(match);
        }
        public TObject FindFirstOrDefault(Expression<Func<TObject, bool>> match)
        {
            return _context.Set<TObject>().FirstOrDefault(match);
        }
        public TObject FindFirstOrDefault() => _context.Set<TObject>().FirstOrDefault();

        public async Task<TObject> FindAsync(Expression<Func<TObject, bool>> match)
        {
            return await _context.Set<TObject>().SingleOrDefaultAsync(match);
        }

        public ICollection<TObject> FindAll(Expression<Func<TObject, bool>> match)
        {
            return _context.Set<TObject>().Where(match).ToList();
        }

        public async Task<ICollection<TObject>> FindAllAsync(Expression<Func<TObject, bool>> match)
        {
            return await _context.Set<TObject>().Where(match).ToListAsync();
        }

        public TObject Add(TObject t)
        {
            _context.Set<TObject>().Add(t);
            _context.SaveChanges();
            return t;
        }

        public async Task<TObject> AddAsync(TObject t)
        {
            _context.Set<TObject>().Add(t);
            await _context.SaveChangesAsync();
            return t;
        }


        public TObject Update(TObject updated)
        {
            if (updated == null)
                return null;


            var existing = _context.Set<TObject>().Find(updated.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(updated);
                _context.SaveChanges();
            }
            return existing;
        }


        public bool Exists(TObject t) => this.FindAll(o => o == t).Any();
        public bool Exists(Expression<Func<TObject, bool>> match) => this.FindAll(match).Any();

        public async Task<TObject> UpdateAsync(TObject updated)
        {
            if (updated == null)
                return null;



            var existing = await _context.Set<TObject>().FindAsync(updated.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(updated);
                await _context.SaveChangesAsync();
            }
            return existing;
        }

        public void Delete(TObject t)
        {
            _context.Set<TObject>().Remove(t);
            _context.SaveChanges();
        }

        public async Task<int> DeleteAsync(TObject t)
        {
            _context.Set<TObject>().Remove(t);
            return await _context.SaveChangesAsync();
        }

        public int Count()
        {
            return _context.Set<TObject>().Count();
        }

        public int Count(Expression<Func<TObject, bool>> expression)
        {
            return _context.Set<TObject>().Count(expression);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<TObject>().CountAsync();
        }

    }
}
