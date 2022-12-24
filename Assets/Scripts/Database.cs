using System;
using System.Collections.Generic;

namespace C18.EventData {
    public interface IData { }

    public interface IDatabaseReader {
        Binder AddListener<T>(Action<T> onDataChanged) where T : IData;
        void RemoveListener(Binder binder);
        T Get<T>() where T : IData;
    }

    public interface IDatabaseWriter {
        void Set<T>(T data) where T : IData;
    }
    
    public class Database : IDatabaseReader, IDatabaseWriter {
        private readonly Dictionary<Type, IData> _database = new();
        private readonly Dictionary<Type, Dictionary<int, Action<IData>>> _onDataChanged = new();
        private int _listenerId = int.MinValue;

        public Binder AddListener<T>(Action<T> onDataChanged) where T : IData {
            var key = typeof(T);
            if (!_onDataChanged.ContainsKey(key)) {
                _onDataChanged.Add(key, new Dictionary<int, Action<IData>>());
            }

            var list = _onDataChanged[key];
            var listenerId = _listenerId++;
            list.Add(listenerId, (e => onDataChanged((T) e)));
            return new Binder(key, listenerId);
        }

        public void RemoveListener(Binder binder) {
            if (!_onDataChanged.ContainsKey(binder.Key)) {
                return;
            }

            var list = _onDataChanged[binder.Key];
            list.Remove(binder.ListenerId);
        }

        public T Get<T>() where T : IData {
            var key = typeof(T);
            return (T) _database[key];
        }

        public void Set<T>(T data) where T : IData {
            var key = typeof(T);
            _database[key] = data;
            DispatchEvent(key);
        }

        private void DispatchEvent(Type key) {
            if (!_onDataChanged.ContainsKey(key)) {
                return;
            }

            var data = _database[key];
            var list = _onDataChanged[key];
            foreach (var k in list.Keys) {
                list[k](data);
            }
        }
    }

    public class DatabaseListener {
        private readonly Dictionary<IDatabaseReader, List<Binder>> _binders = new();

        public void Listen<T>(IDatabaseReader database, Action<T> onDataChanged) where T : IData {
            if (!_binders.ContainsKey(database)) {
                _binders[database] = new List<Binder>();
            }
            var b = database.AddListener(onDataChanged);
            _binders[database].Add(b);
        }

        public void Destroy() {
            foreach (var k in _binders.Keys) {
                _binders[k].ForEach(e => k.RemoveListener(e));
            }
            _binders.Clear();
        }
    }

    public readonly struct Binder {
        public readonly Type Key;
        public readonly int ListenerId;

        public Binder(Type key, int listenerId) {
            Key = key;
            ListenerId = listenerId;
        }
    }
}