using System;
using C18.EventData;
using UnityEngine;

namespace Test {
    public class MyData : IData {
        public Vector3 Position;
        public Vector3 Scale;

        public MyData(Vector3 position, Vector3 scale) {
            Position = position;
            Scale = scale;
        }
    }

    public class TestDatabaseDispatcher : MonoBehaviour {
        public Database MyData;

        private void Awake() {
            MyData = new Database();
            var transform1 = transform;
            MyData.Set(new MyData(transform1.position, transform1.localScale));
        }

        private void Update() {
            var data = MyData.Get<MyData>();
            var tr = transform;
            var changed = false;
            if (tr.position != data.Position) {
                data.Position = transform.position;
                changed = true;
            }

            if (tr.localScale != data.Scale) {
                data.Scale = transform.localScale;
                changed = true;
            }

            if (changed) {
                MyData.Set(data);
            }
        }
    }
}