#if UNITY_EDITOR
using UnityEngine;

namespace WheelOfFortune.UI
{
    public static class UiBind
    {
        public static T Find<T>(Component root, string childName) where T : Component
        {
            var child = FindRecursive(root.transform, childName);
            if (child == null)
            {
                Debug.LogError($"{root.GetType().Name} on '{root.name}': no child named '{childName}' found.", root);
                return null;
            }

            var component = child.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"{root.GetType().Name} on '{root.name}': child '{childName}' has no {typeof(T).Name} component.", root);
            }

            return component;
        }

        private static Transform FindRecursive(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }

                var found = FindRecursive(child, childName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
#endif
