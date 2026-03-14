using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable MA0048 // File name must match type name

namespace Olve.TinyEXR;

public sealed class TinyExrException(int code, string? message)
    : Exception(message ?? $"TinyEXR error {code}")
{
    public int Code { get; } = code;
}

public static unsafe class TinyExr
{
    /// <summary>
    /// Extracts a managed string from a native error pointer and frees it.
    /// Returns null if the pointer is null.
    /// </summary>
    private static string? ConsumeError(byte* err)
    {
        if (err == null) return null;
        var span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(err);
        var message = Encoding.UTF8.GetString(span);
        NativeMethods.FreeEXRErrorMessage(err);
        return message;
    }

    /// <summary>
    /// Throws a <see cref="TinyExrException"/> if the return code indicates failure.
    /// Consumes and frees the native error string.
    /// </summary>
    private static void ThrowIfFailed(int ret, byte* err)
    {
        if (ret == TinyExrConstants.TINYEXR_SUCCESS) return;
        var message = ConsumeError(err);
        throw new TinyExrException(ret, message);
    }

    /// <summary>
    /// Loads an EXR image from a file. Returns pixel data as RGBA float values.
    /// The caller receives a span over natively-allocated memory; call
    /// <see cref="FreeImageData"/> with the returned pointer when done.
    /// </summary>
    public static ReadOnlySpan<float> LoadEXR(
        string filename,
        out int width,
        out int height,
        out float* nativePtr)
    {
        var ret = NativeMethods.LoadEXR(out var rgba, out width, out height, filename, out var err);
        ThrowIfFailed(ret, err);

        nativePtr = rgba;
        return new ReadOnlySpan<float>(rgba, width * height * 4);
    }

    /// <summary>
    /// Loads an EXR image with a specific layer from a file.
    /// </summary>
    public static ReadOnlySpan<float> LoadEXRWithLayer(
        string filename,
        string layerName,
        out int width,
        out int height,
        out float* nativePtr)
    {
        var ret = NativeMethods.LoadEXRWithLayer(out var rgba, out width, out height, filename, layerName, out var err);
        ThrowIfFailed(ret, err);

        nativePtr = rgba;
        return new ReadOnlySpan<float>(rgba, width * height * 4);
    }

    /// <summary>
    /// Loads an EXR image from memory. Returns pixel data as RGBA float values.
    /// </summary>
    public static ReadOnlySpan<float> LoadEXRFromMemory(
        ReadOnlySpan<byte> memory,
        out int width,
        out int height,
        out float* nativePtr)
    {
        var ret = NativeMethods.LoadEXRFromMemory(out var rgba, out width, out height, memory, out var err);
        ThrowIfFailed(ret, err);

        nativePtr = rgba;
        return new ReadOnlySpan<float>(rgba, width * height * 4);
    }

    /// <summary>
    /// Frees image data returned by LoadEXR / LoadEXRFromMemory.
    /// </summary>
    public static void FreeImageData(float* data)
    {
        NativeMemory.Free(data);
    }

    /// <summary>
    /// Saves RGBA float data to an EXR file.
    /// </summary>
    public static void SaveEXR(
        ReadOnlySpan<float> data,
        int width,
        int height,
        int components,
        bool saveAsFp16,
        string filename)
    {
        var ret = NativeMethods.SaveEXR(data, width, height, components, saveAsFp16 ? 1 : 0, filename, out var err);
        ThrowIfFailed(ret, err);
    }

    /// <summary>
    /// Saves RGBA float data to an EXR in memory. Returns a span over the
    /// natively-allocated buffer; call <see cref="FreeMemoryBuffer"/> when done.
    /// </summary>
    public static ReadOnlySpan<byte> SaveEXRToMemory(
        ReadOnlySpan<float> data,
        int width,
        int height,
        int components,
        bool saveAsFp16,
        out byte* nativePtr)
    {
        var size = NativeMethods.SaveEXRToMemory(data, width, height, components, saveAsFp16 ? 1 : 0, out var buffer, out var err);
        if ((int)size < 0)
        {
            var message = ConsumeError(err);
            throw new TinyExrException((int)size, message);
        }

        nativePtr = buffer;
        return new ReadOnlySpan<byte>(buffer, (int)size);
    }

    /// <summary>
    /// Frees a buffer returned by <see cref="SaveEXRToMemory"/>.
    /// </summary>
    public static void FreeMemoryBuffer(byte* buffer)
    {
        NativeMemory.Free(buffer);
    }

    /// <summary>
    /// Checks if a file is an EXR image.
    /// </summary>
    public static bool IsEXR(string filename)
        => NativeMethods.IsEXR(filename) == TinyExrConstants.TINYEXR_SUCCESS;

    /// <summary>
    /// Checks if memory contains an EXR image.
    /// </summary>
    public static bool IsEXRFromMemory(ReadOnlySpan<byte> memory)
        => NativeMethods.IsEXRFromMemory(memory) == TinyExrConstants.TINYEXR_SUCCESS;

    /// <summary>
    /// Parses the EXR version header from a file.
    /// </summary>
    public static EXRVersion ParseEXRVersionFromFile(string filename)
    {
        var ret = NativeMethods.ParseEXRVersionFromFile(out var version, filename);
        if (ret != TinyExrConstants.TINYEXR_SUCCESS)
            throw new TinyExrException(ret, $"Failed to parse EXR version from '{filename}'");
        return version;
    }

    /// <summary>
    /// Parses the EXR version header from memory.
    /// </summary>
    public static EXRVersion ParseEXRVersionFromMemory(ReadOnlySpan<byte> memory)
    {
        var ret = NativeMethods.ParseEXRVersionFromMemory(out var version, memory);
        if (ret != TinyExrConstants.TINYEXR_SUCCESS)
            throw new TinyExrException(ret, "Failed to parse EXR version from memory");
        return version;
    }

    /// <summary>
    /// Returns a managed string from a null-terminated native byte pointer.
    /// </summary>
    public static string? PtrToStringUtf8(byte* ptr)
    {
        if (ptr == null) return null;
        var span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr);
        return Encoding.UTF8.GetString(span);
    }
}
