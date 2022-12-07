using UnityEngine;

namespace C18 {
    public static class UtilsObject {
        public static void ClearChildren(Transform transform) {
            while (transform.childCount > 0) {
                Object.Destroy(transform.GetChild(0).gameObject);
            }
        }
    }
}