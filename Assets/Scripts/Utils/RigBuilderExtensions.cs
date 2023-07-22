using UnityEngine.Animations.Rigging;

namespace Utils
{
    public static class RigBuilderExtensions
    {
        public static void DisableLayers(this RigBuilder rigBuilder)
        {
            SetLayers(rigBuilder, false);
        }

        public static void EnableLayers(this RigBuilder rigBuilder)
        {
            SetLayers(rigBuilder, true);
        }

        public static void SetLayerByIndex(this RigBuilder rigBuilder, int index, bool value) =>
            rigBuilder.layers[index].active = value;

        private static void SetLayers(RigBuilder rigBuilder, bool value)
        {
            foreach (RigLayer layer in rigBuilder.layers)
            {
                layer.active = value;
            }
        }
    }
}