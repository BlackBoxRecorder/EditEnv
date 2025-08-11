using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonFlatFileDataStore;

namespace EditEnv
{
    public class Storage
    {
        private static readonly string JSONPATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "EditEnv",
            "data.json"
        );

        private static Storage? _storage;

        public static Storage Instance
        {
            get
            {
                lock (_lock)
                {
                    _storage ??= new Storage();
                }

                return _storage;
            }
        }

        private readonly DataStore store;

        private static readonly Lock _lock = new();

        private Storage()
        {
            var dir = Path.GetDirectoryName(JSONPATH) ?? throw new DirectoryNotFoundException();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            store = new DataStore(JSONPATH);
        }

        public async Task AddOrUpdateEnv(EnvModel model)
        {
            var collection = store.GetCollection<EnvModel>();

            var item = collection.AsQueryable().FirstOrDefault(ExprEnv(model));

            if (item == null)
            {
                await collection.InsertOneAsync(model);
            }
            else
            {
                item.Value = model.Value;

                await collection.UpdateOneAsync(item.Id, item);
            }
        }

        public async Task RemoveEnv(EnvModel model)
        {
            var collection = store.GetCollection<EnvModel>();
            var item = collection.AsQueryable().FirstOrDefault(ExprEnv(model));

            if (item != null)
            {
                await collection.DeleteOneAsync(item.Id);
            }
        }

        private static Func<EnvModel, bool> ExprEnv(EnvModel model)
        {
            return e =>
                e.Key.Equals(model.Key, StringComparison.OrdinalIgnoreCase)
                && e.Target == model.Target;
        }

        public async Task AddPath(PathModel model)
        {
            var collection = store.GetCollection<PathModel>();

            var item = collection.AsQueryable().FirstOrDefault(ExprPath(model));

            if (item == null)
            {
                await collection.InsertOneAsync(model);
            }
        }

        public async Task RemovePath(PathModel model)
        {
            var collection = store.GetCollection<PathModel>();
            var item = collection.AsQueryable().FirstOrDefault(ExprPath(model));

            if (item != null)
            {
                await collection.DeleteOneAsync(item.Id);
            }
        }

        private static Func<PathModel, bool> ExprPath(PathModel model)
        {
            return e =>
                e.Value.Equals(model.Value, StringComparison.OrdinalIgnoreCase)
                && e.Target == model.Target;
        }

        public List<EnvModel> GetAllEnv()
        {
            var collection = store.GetCollection<EnvModel>();

            var item = collection.AsQueryable().ToList();

            return item;
        }

        public List<PathModel> GetAllPath()
        {
            var collection = store.GetCollection<PathModel>();

            var item = collection.AsQueryable().ToList();

            return item;
        }
    }

    public class EnvModel
    {
        public int Id { get; set; }
        public required string Key { get; set; }

        public required string Value { get; set; }

        public required EnvironmentVariableTarget Target { get; set; }
    }

    public class PathModel
    {
        public int Id { get; set; }

        public required string Value { get; set; }

        public required EnvironmentVariableTarget Target { get; set; }
    }
}
