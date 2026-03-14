using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Olve.TinyEXR;

#pragma warning disable CS0649 // Field is never assigned to
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable MA0048 // File name must match type name
#pragma warning disable MA0006 // Use string.Equals
#pragma warning disable MA0144 // Use System.OperatingSystem

// --- Constants ---

internal static partial class TinyExrConstants
{
    // Return codes
    public const int TINYEXR_SUCCESS = 0;
    public const int TINYEXR_ERROR_INVALID_MAGIC_NUMBER = -1;
    public const int TINYEXR_ERROR_INVALID_EXR_VERSION = -2;
    public const int TINYEXR_ERROR_INVALID_ARGUMENT = -3;
    public const int TINYEXR_ERROR_INVALID_DATA = -4;
    public const int TINYEXR_ERROR_INVALID_FILE = -5;
    public const int TINYEXR_ERROR_INVALID_PARAMETER = -6;
    public const int TINYEXR_ERROR_CANT_OPEN_FILE = -7;
    public const int TINYEXR_ERROR_UNSUPPORTED_FORMAT = -8;
    public const int TINYEXR_ERROR_INVALID_HEADER = -9;
    public const int TINYEXR_ERROR_UNSUPPORTED_FEATURE = -10;
    public const int TINYEXR_ERROR_CANT_WRITE_FILE = -11;
    public const int TINYEXR_ERROR_SERIALIZATION_FAILED = -12;
    public const int TINYEXR_ERROR_LAYER_NOT_FOUND = -13;
    public const int TINYEXR_ERROR_DATA_TOO_LARGE = -14;

    // Pixel types
    public const int TINYEXR_PIXELTYPE_UINT = 0;
    public const int TINYEXR_PIXELTYPE_HALF = 1;
    public const int TINYEXR_PIXELTYPE_FLOAT = 2;

    // Header limits
    public const int TINYEXR_MAX_HEADER_ATTRIBUTES = 1024;
    public const int TINYEXR_MAX_CUSTOM_ATTRIBUTES = 128;

    // Compression types
    public const int TINYEXR_COMPRESSIONTYPE_NONE = 0;
    public const int TINYEXR_COMPRESSIONTYPE_RLE = 1;
    public const int TINYEXR_COMPRESSIONTYPE_ZIPS = 2;
    public const int TINYEXR_COMPRESSIONTYPE_ZIP = 3;
    public const int TINYEXR_COMPRESSIONTYPE_PIZ = 4;
    public const int TINYEXR_COMPRESSIONTYPE_PXR24 = 5;
    public const int TINYEXR_COMPRESSIONTYPE_B44 = 6;
    public const int TINYEXR_COMPRESSIONTYPE_B44A = 7;
    public const int TINYEXR_COMPRESSIONTYPE_DWAA = 8;
    public const int TINYEXR_COMPRESSIONTYPE_DWAB = 9;
    public const int TINYEXR_COMPRESSIONTYPE_ZFP = 128;

    // ZFP compression types
    public const int TINYEXR_ZFP_COMPRESSIONTYPE_RATE = 0;
    public const int TINYEXR_ZFP_COMPRESSIONTYPE_PRECISION = 1;
    public const int TINYEXR_ZFP_COMPRESSIONTYPE_ACCURACY = 2;

    // Tile level modes
    public const int TINYEXR_TILE_ONE_LEVEL = 0;
    public const int TINYEXR_TILE_MIPMAP_LEVELS = 1;
    public const int TINYEXR_TILE_RIPMAP_LEVELS = 2;

    // Tile rounding modes
    public const int TINYEXR_TILE_ROUND_DOWN = 0;
    public const int TINYEXR_TILE_ROUND_UP = 1;

    // Spectral EXR types
    public const int TINYEXR_SPECTRUM_REFLECTIVE = 0;
    public const int TINYEXR_SPECTRUM_EMISSIVE = 1;
    public const int TINYEXR_SPECTRUM_POLARISED = 2;
}

// --- Structs ---

[StructLayout(LayoutKind.Sequential)]
public struct EXRVersion
{
    public int version;
    public int tiled;
    public int long_name;
    public int non_image;
    public int multipart;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EXRAttribute
{
    public fixed byte name[256];
    public fixed byte type[256];
    public byte* value;
    public int size;
    public int pad0;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EXRChannelInfo
{
    public fixed byte name[256];
    public int pixel_type;
    public int x_sampling;
    public int y_sampling;
    public byte p_linear;
    public fixed byte pad[3];
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EXRTile
{
    public int offset_x;
    public int offset_y;
    public int level_x;
    public int level_y;
    public int width;
    public int height;
    public byte** images;
}

[StructLayout(LayoutKind.Sequential)]
internal struct EXRBox2i
{
    public int min_x;
    public int min_y;
    public int max_x;
    public int max_y;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EXRHeader
{
    public float pixel_aspect_ratio;
    public int line_order;
    public EXRBox2i data_window;
    public EXRBox2i display_window;
    public fixed float screen_window_center[2];
    public float screen_window_width;

    public int chunk_count;

    public int tiled;
    public int tile_size_x;
    public int tile_size_y;
    public int tile_level_mode;
    public int tile_rounding_mode;

    public int long_name;
    public int non_image;
    public int multipart;
    public uint header_len;

    public int num_custom_attributes;
    public EXRAttribute* custom_attributes;

    public EXRChannelInfo* channels;

    public int* pixel_types;
    public int num_channels;

    public int compression_type;
    public int* requested_pixel_types;

    public fixed byte name[256];
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EXRMultiPartHeader
{
    public int num_headers;
    public EXRHeader* headers;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EXRImage
{
    public EXRTile* tiles;
    public EXRImage* next_level;
    public int level_x;
    public int level_y;

    public byte** images;

    public int width;
    public int height;
    public int num_channels;

    public int num_tiles;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EXRMultiPartImage
{
    public int num_images;
    public EXRImage* images;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct DeepImage
{
    public byte** channel_names;
    public float*** image;
    public int** offset_table;
    public int num_channels;
    public int width;
    public int height;
    public int pad0;
}

// --- P/Invoke declarations ---

internal static unsafe partial class NativeMethods
{
    private const string LibName = "tinyexr";

    static NativeMethods()
    {
        NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, (name, assembly, path) =>
        {
            if (name != LibName) return IntPtr.Zero;

            // Try the runtimes/{rid}/native/ layout next to the assembly
            var assemblyDir = Path.GetDirectoryName(assembly.Location) ?? ".";
            var rid = RuntimeInformation.RuntimeIdentifier;
            var candidate = Path.Combine(assemblyDir, "runtimes", rid, "native", RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? $"{name}.dll"
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? $"lib{name}.dylib"
                    : $"lib{name}.so");

            if (NativeLibrary.TryLoad(candidate, out var handle))
                return handle;

            // Fall back to default resolution
            return IntPtr.Zero;
        });
    }

    // int LoadEXR(float **out_rgba, int *width, int *height, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "LoadEXR")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int LoadEXR(
        out float* out_rgba,
        out int width,
        out int height,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // int LoadEXRWithLayer(float **out_rgba, int *width, int *height, const char *filename, const char *layer_name, const char **err);
    [LibraryImport(LibName, EntryPoint = "LoadEXRWithLayer")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int LoadEXRWithLayer(
        out float* out_rgba,
        out int width,
        out int height,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string layer_name,
        out byte* err);

    // int EXRLayers(const char *filename, const char **layer_names[], int *num_layers, const char **err);
    [LibraryImport(LibName, EntryPoint = "EXRLayers")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int EXRLayers(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte** layer_names,
        out int num_layers,
        out byte* err);

    // int IsEXR(const char *filename);
    [LibraryImport(LibName, EntryPoint = "IsEXR")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int IsEXR(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename);

    // int IsEXRFromMemory(const unsigned char *memory, size_t size);
    [LibraryImport(LibName, EntryPoint = "IsEXRFromMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int IsEXRFromMemory(
        ReadOnlySpan<byte> memory);

    // int SaveEXRToMemory(const float *data, const int width, const int height, const int components, const int save_as_fp16, unsigned char **buffer, const char **err);
    [LibraryImport(LibName, EntryPoint = "SaveEXRToMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int SaveEXRToMemory(
        ReadOnlySpan<float> data,
        int width,
        int height,
        int components,
        int save_as_fp16,
        out byte* buffer,
        out byte* err);

    // int SaveEXR(const float *data, const int width, const int height, const int components, const int save_as_fp16, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "SaveEXR")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int SaveEXR(
        ReadOnlySpan<float> data,
        int width,
        int height,
        int components,
        int save_as_fp16,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // int EXRNumLevels(const EXRImage* exr_image);
    [LibraryImport(LibName, EntryPoint = "EXRNumLevels")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int EXRNumLevels(
        EXRImage* exr_image);

    // void InitEXRHeader(EXRHeader *exr_header);
    [LibraryImport(LibName, EntryPoint = "InitEXRHeader")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void InitEXRHeader(
        EXRHeader* exr_header);

    // void EXRSetNameAttr(EXRHeader *exr_header, const char* name);
    [LibraryImport(LibName, EntryPoint = "EXRSetNameAttr")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EXRSetNameAttr(
        EXRHeader* exr_header,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

    // void InitEXRImage(EXRImage *exr_image);
    [LibraryImport(LibName, EntryPoint = "InitEXRImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void InitEXRImage(
        EXRImage* exr_image);

    // int FreeEXRHeader(EXRHeader *exr_header);
    [LibraryImport(LibName, EntryPoint = "FreeEXRHeader")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int FreeEXRHeader(
        EXRHeader* exr_header);

    // int FreeEXRImage(EXRImage *exr_image);
    [LibraryImport(LibName, EntryPoint = "FreeEXRImage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int FreeEXRImage(
        EXRImage* exr_image);

    // void FreeEXRErrorMessage(const char *msg);
    [LibraryImport(LibName, EntryPoint = "FreeEXRErrorMessage")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void FreeEXRErrorMessage(
        byte* msg);

    // int ParseEXRVersionFromFile(EXRVersion *version, const char *filename);
    [LibraryImport(LibName, EntryPoint = "ParseEXRVersionFromFile")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int ParseEXRVersionFromFile(
        out EXRVersion version,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename);

    // int ParseEXRVersionFromMemory(EXRVersion *version, const unsigned char *memory, size_t size);
    [LibraryImport(LibName, EntryPoint = "ParseEXRVersionFromMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int ParseEXRVersionFromMemory(
        out EXRVersion version,
        ReadOnlySpan<byte> memory);

    // int ParseEXRHeaderFromFile(EXRHeader *header, const EXRVersion *version, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "ParseEXRHeaderFromFile")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int ParseEXRHeaderFromFile(
        EXRHeader* header,
        in EXRVersion version,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // int ParseEXRHeaderFromMemory(EXRHeader *header, const EXRVersion *version, const unsigned char *memory, size_t size, const char **err);
    [LibraryImport(LibName, EntryPoint = "ParseEXRHeaderFromMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int ParseEXRHeaderFromMemory(
        EXRHeader* header,
        in EXRVersion version,
        ReadOnlySpan<byte> memory,
        out byte* err);

    // int ParseEXRMultipartHeaderFromFile(EXRHeader ***headers, int *num_headers, const EXRVersion *version, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "ParseEXRMultipartHeaderFromFile")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int ParseEXRMultipartHeaderFromFile(
        out EXRHeader** headers,
        out int num_headers,
        in EXRVersion version,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // int ParseEXRMultipartHeaderFromMemory(EXRHeader ***headers, int *num_headers, const EXRVersion *version, const unsigned char *memory, size_t size, const char **err);
    [LibraryImport(LibName, EntryPoint = "ParseEXRMultipartHeaderFromMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int ParseEXRMultipartHeaderFromMemory(
        out EXRHeader** headers,
        out int num_headers,
        in EXRVersion version,
        ReadOnlySpan<byte> memory,
        out byte* err);

    // int LoadEXRImageFromFile(EXRImage *image, const EXRHeader *header, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "LoadEXRImageFromFile")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int LoadEXRImageFromFile(
        EXRImage* image,
        EXRHeader* header,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // int LoadEXRImageFromMemory(EXRImage *image, const EXRHeader *header, const unsigned char *memory, const size_t size, const char **err);
    [LibraryImport(LibName, EntryPoint = "LoadEXRImageFromMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int LoadEXRImageFromMemory(
        EXRImage* image,
        EXRHeader* header,
        ReadOnlySpan<byte> memory,
        out byte* err);

    // int LoadEXRMultipartImageFromFile(EXRImage *images, const EXRHeader **headers, unsigned int num_parts, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "LoadEXRMultipartImageFromFile")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int LoadEXRMultipartImageFromFile(
        EXRImage* images,
        EXRHeader** headers,
        uint num_parts,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // int LoadEXRMultipartImageFromMemory(EXRImage *images, const EXRHeader **headers, unsigned int num_parts, const unsigned char *memory, const size_t size, const char **err);
    [LibraryImport(LibName, EntryPoint = "LoadEXRMultipartImageFromMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int LoadEXRMultipartImageFromMemory(
        EXRImage* images,
        EXRHeader** headers,
        uint num_parts,
        ReadOnlySpan<byte> memory,
        out byte* err);

    // int SaveEXRImageToFile(const EXRImage *image, const EXRHeader *exr_header, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "SaveEXRImageToFile")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int SaveEXRImageToFile(
        EXRImage* image,
        EXRHeader* exr_header,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // size_t SaveEXRImageToMemory(const EXRImage *image, const EXRHeader *exr_header, unsigned char **memory, const char **err);
    [LibraryImport(LibName, EntryPoint = "SaveEXRImageToMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nuint SaveEXRImageToMemory(
        EXRImage* image,
        EXRHeader* exr_header,
        out byte* memory,
        out byte* err);

    // int SaveEXRMultipartImageToFile(const EXRImage *images, const EXRHeader **exr_headers, unsigned int num_parts, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "SaveEXRMultipartImageToFile")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int SaveEXRMultipartImageToFile(
        EXRImage* images,
        EXRHeader** exr_headers,
        uint num_parts,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // size_t SaveEXRMultipartImageToMemory(const EXRImage *images, const EXRHeader **exr_headers, unsigned int num_parts, unsigned char **memory, const char **err);
    [LibraryImport(LibName, EntryPoint = "SaveEXRMultipartImageToMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial nuint SaveEXRMultipartImageToMemory(
        EXRImage* images,
        EXRHeader** exr_headers,
        uint num_parts,
        out byte* memory,
        out byte* err);

    // int LoadDeepEXR(DeepImage *out_image, const char *filename, const char **err);
    [LibraryImport(LibName, EntryPoint = "LoadDeepEXR")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int LoadDeepEXR(
        DeepImage* out_image,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename,
        out byte* err);

    // int LoadEXRFromMemory(float **out_rgba, int *width, int *height, const unsigned char *memory, size_t size, const char **err);
    [LibraryImport(LibName, EntryPoint = "LoadEXRFromMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int LoadEXRFromMemory(
        out float* out_rgba,
        out int width,
        out int height,
        ReadOnlySpan<byte> memory,
        out byte* err);

    // --- Spectral EXR API ---

    // int IsSpectralEXR(const char *filename);
    [LibraryImport(LibName, EntryPoint = "IsSpectralEXR")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int IsSpectralEXR(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string filename);

    // int IsSpectralEXRFromMemory(const unsigned char *memory, size_t size);
    [LibraryImport(LibName, EntryPoint = "IsSpectralEXRFromMemory")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int IsSpectralEXRFromMemory(
        ReadOnlySpan<byte> memory);

    // int EXRGetSpectrumType(const EXRHeader *exr_header);
    [LibraryImport(LibName, EntryPoint = "EXRGetSpectrumType")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int EXRGetSpectrumType(
        EXRHeader* exr_header);

    // void EXRFormatWavelength(char *buffer, size_t buffer_size, float wavelength_nm);
    [LibraryImport(LibName, EntryPoint = "EXRFormatWavelength")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EXRFormatWavelength(
        Span<byte> buffer,
        float wavelength_nm);

    // void EXRSpectralChannelName(char *buffer, size_t buffer_size, float wavelength_nm, int stokes_component);
    [LibraryImport(LibName, EntryPoint = "EXRSpectralChannelName")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EXRSpectralChannelName(
        Span<byte> buffer,
        float wavelength_nm,
        int stokes_component);

    // void EXRReflectiveChannelName(char *buffer, size_t buffer_size, float wavelength_nm);
    [LibraryImport(LibName, EntryPoint = "EXRReflectiveChannelName")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void EXRReflectiveChannelName(
        Span<byte> buffer,
        float wavelength_nm);

    // float EXRParseSpectralChannelWavelength(const char *channel_name);
    [LibraryImport(LibName, EntryPoint = "EXRParseSpectralChannelWavelength")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial float EXRParseSpectralChannelWavelength(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string channel_name);

    // int EXRGetStokesComponent(const char *channel_name);
    [LibraryImport(LibName, EntryPoint = "EXRGetStokesComponent")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int EXRGetStokesComponent(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string channel_name);

    // int EXRIsSpectralChannel(const char *channel_name);
    [LibraryImport(LibName, EntryPoint = "EXRIsSpectralChannel")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int EXRIsSpectralChannel(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string channel_name);

    // int EXRGetWavelengths(const EXRHeader *exr_header, float *wavelengths, int max_wavelengths);
    [LibraryImport(LibName, EntryPoint = "EXRGetWavelengths")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int EXRGetWavelengths(
        EXRHeader* exr_header,
        Span<float> wavelengths);

    // int EXRSetSpectralAttributes(EXRHeader *exr_header, int spectrum_type, const char *units);
    [LibraryImport(LibName, EntryPoint = "EXRSetSpectralAttributes")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int EXRSetSpectralAttributes(
        EXRHeader* exr_header,
        int spectrum_type,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string units);

    // const char* EXRGetSpectralUnits(const EXRHeader *exr_header);
    [LibraryImport(LibName, EntryPoint = "EXRGetSpectralUnits")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial byte* EXRGetSpectralUnits(
        EXRHeader* exr_header);
}

#pragma warning restore CS0649
