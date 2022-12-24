using System;
using C18.Data;
using UnityEngine;

namespace Test {
    public class TestDatabaseListener : MonoBehaviour {
        public TestDatabaseDispatcher Dispatcher;

        private DatabaseListener _databaseListener;

        private void Awake() {
            _databaseListener = new DatabaseListener();
        }

        private void Start() {
            _databaseListener.Listen<MyData>(Dispatcher.MyData, OnDataChanged); 
        }

        private void OnDisable() {
            _databaseListener.Destroy();
        }

        private void OnDataChanged(MyData obj) {
            var transform1 = transform;
            transform1.position = obj.Position;
            transform1.localScale = obj.Scale;
        }
    }
}