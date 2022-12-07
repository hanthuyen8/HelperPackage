using System;

namespace C18 {
    public struct TimeCounter {
        private float _time;
        private readonly Action _timeOut;

        public TimeCounter(float time, Action timeOut) {
            _time = time;
            _timeOut = timeOut;
        }

        public void Update(float dt) {
            if (_time <= 0) {
                return;
            }

            _time -= dt;
            if (_time <= 0) {
                _timeOut.Invoke();
            }
        }
    }
}