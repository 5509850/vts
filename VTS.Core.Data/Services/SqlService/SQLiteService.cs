using System;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using System.Threading.Tasks;
using System.Collections.Generic;
using VTS.Core.Data.Models;

namespace VTS.Core.Data.SqlService
{
    public class SQLiteService
    {
        private SQLiteAsyncConnection _db;

        public SQLiteService(ISQLitePlatform sqlitePlatform, string dbPath)
        {
            _db = new SQLiteAsyncConnection(() => CreateConnection(sqlitePlatform, dbPath));

            if (!IsExist<VacationInfoDTO>())
            {
                _db.CreateTableAsync<VacationInfoDTO>();
            }

            if (!IsExist<LoginInfoDTO>())
            {
                _db.CreateTableAsync<LoginInfoDTO>();
            }

            if (!IsExist<VacationInfoModelDTO>())
            {
                _db.CreateTableAsync<VacationInfoModelDTO>();
            }
        }

        public bool IsExist<T>() where T : class
        {
            try
            {
                Task<T> task = Task.Run(async () => await _db.Table<T>().FirstOrDefaultAsync());
                task.Wait();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public SQLiteConnectionWithLock CreateConnection(ISQLitePlatform sqlitePlatform, string dbPath)
        {
            try
            {
                SQLiteConnectionString connectionString = new SQLiteConnectionString(dbPath, false);
                SQLiteConnectionWithLock connection = new SQLiteConnectionWithLock(sqlitePlatform, connectionString);
                return connection;
            }
            catch (ArgumentException)
            {               
                throw;
            }
        }

        public async Task<T> Get<T>(string id) where T : class
        {
            try
            {
                return await _db.GetAsync<T>(id);
            }
            catch (SQLiteException ex)
            {
                var err = ex.Message;         
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<List<T>> Get<T>() where T : class
        {
            try
            {
                return await _db.Table<T>().ToListAsync();
            }
            catch (SQLiteException)
            {              
                return null;
            }
        }

        public async Task Insert<T>(T item)
        {
            try
            {
                await _db.InsertOrIgnoreAsync(item);
            }
            catch (SQLiteException ex)
            {
                var err = ex.Message;
            }
        }

        public async Task Delete<T>(string id) where T : class
        {
            try
            {
                await _db.DeleteAsync<T>(id);
            }
            catch (SQLiteException)
            {
            
            }
        }

        public async Task Update<T>(T item)
        {
            try
            {
                await _db.UpdateAsync(item);
            }
            catch (SQLiteException)
            {              
            }
        }

    }
}