using UnityEngine;

namespace C18 {
    public static class UtilsObject {
        public static void ClearChildren(Transform transform) {
            var childCount = transform.childCount;
            for (var i = transform.childCount - 1; i >= 0; i--) {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}