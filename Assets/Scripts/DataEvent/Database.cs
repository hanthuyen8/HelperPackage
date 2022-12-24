﻿using System;
using System.Collections.Generic;

namespace DataEvent {
    public interface ITableData { }

    public interface IDatabaseReader {
        Binder AddListener<T>(Action<T> onDataChanged) where T : ITableData;
        void RemoveListener(Binder binder);
        T Read<T>() where T : ITableData;
    }

    public interface IDatabaseWriter {
        void Write<T>(T data) where T : ITableData;
    }
    
    public class Database : IDatabaseReader, IDatabaseWriter {
        private Dictionary<Type, ITableData> _database;
        private Dictionary<Type, Dictionary<int, Action<ITableData>>> _onDataChanged;
        private int _listenerId;

        public Binder AddListener<T>(Action<T> onDataChanged) where T : ITableData {
            var key = typeof(T);
            if (!_onDataChanged.ContainsKey(key)) {
                _onDataChanged.Add(key, new Dictionary<int, Action<ITableData>>());
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

        public T Read<T>() where T : ITableData {
            var key = typeof(T);
            return (T) _database[key];
        }

        public void Write<T>(T data) where T : ITableData {
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
        private readonly Dictionary<IDatabaseReader, List<Binder>> _binders;

        public void Listen<T>(IDatabaseReader database, Action<T> onDataChanged) where T : ITableData {
            if (!_binders.ContainsKey(database)) {
                _binders[database] = new List<Binder>();
            }
            var b = database.AddListener(onDataChanged);
            _binders[database].Add(b);
        }

        public void Dispose() {
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