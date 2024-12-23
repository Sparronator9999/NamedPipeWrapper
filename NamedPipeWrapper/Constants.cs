using MessagePack;

namespace NamedPipeWrapper
{
    internal static class Constants
    {
        public static readonly MessagePackSerializerOptions SerializerOptions =
            MessagePackSerializerOptions.Standard.WithSecurity(MessagePackSecurity.TrustedData);
    }
}
