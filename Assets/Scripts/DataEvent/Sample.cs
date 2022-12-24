using System;
using UnityEngine;

namespace DataEvent {
    public class PlayerData : ITableData {
        public Vector3 Position;
    }

    public class GameData : ITableData {
        public bool Paused;
    }

    public class Sample {
        private readonly DatabaseListener _databaseListener;

        public Sample() {
            var database = new Database();
            _databaseListener = new DatabaseListener();
            _databaseListener.Listen<PlayerData>(database, OnDataChanged);
            _databaseListener.Listen<GameData>(database, OnDataChanged);
        }

        public void Destroy() {
            _databaseListener.Dispose();
        }
        
        private void OnDataChanged(GameData obj) {
            throw new NotImplementedException();
        }
        
        private void OnDataChanged(PlayerData obj) {
            throw new NotImplementedException();
        }
    }
}