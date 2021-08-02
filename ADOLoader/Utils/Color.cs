namespace ADOLoader.Utils {
    public static class Color {
        public static UnityEngine.Color WithAlpha(this UnityEngine.Color color, float alpha) {
            color.a = alpha;
            return color;
        }
        
        public static UnityEngine.Color32 WithAlpha(this UnityEngine.Color32 color, byte alpha) {
            color.a = alpha;
            return color;
        }
    }
}