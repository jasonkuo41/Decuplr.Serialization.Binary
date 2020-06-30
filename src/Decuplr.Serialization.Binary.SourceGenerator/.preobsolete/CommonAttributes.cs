using System.Reflection;

namespace Decuplr.Serialization.Binary {
    internal static class CommonAttributes {

        internal static string GeneratedCodeAttribute => $"[GeneratedCode (\"{Assembly.GetExecutingAssembly().GetName().Name}\", \"{Assembly.GetExecutingAssembly().GetName().Version}\")]";

        internal static string HideFromEditor => "[EditorBrowsable(EditorBrowsableState.Never)]";

        internal static string Inline => "[MethodImpl(MethodImplOptions.AggressiveInlining)]";
    }
}
