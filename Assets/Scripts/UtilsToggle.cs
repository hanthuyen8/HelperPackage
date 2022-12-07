using System.Collections.Generic;
using System.Linq;

namespace C18.Toggles {
    public interface IToggleable {
        void SetToggle(bool on);
    }

    public class TogglesGroup {
        private readonly Dictionary<IToggleable, bool> _toggles = new();
        private readonly bool _multiSelect;

        public TogglesGroup(bool multiSelect) {
            _multiSelect = multiSelect;
        }

        public void AddToggle(IToggleable toggle) {
            if (!_toggles.ContainsKey(toggle)) {
                _toggles[toggle] = false;
            }
        }

        public void TurnOn(IToggleable toggle) {
            if (_multiSelect) {
                MultiSelect(toggle);
            }
            else {
                SingleSelect(toggle);
            }
        }

        private void MultiSelect(IToggleable toggle) {
            if (!_toggles.ContainsKey(toggle)) {
                return;
            }

            var v = _toggles[toggle];
            _toggles[toggle] = !v;
            toggle.SetToggle(!v);
        }

        private void SingleSelect(IToggleable toggle) {
            foreach (var k in _toggles.Keys.ToList()) {
                var isOn = k == toggle;
                _toggles[k] = isOn;
                k.SetToggle(isOn);
            }
        }
    }
}