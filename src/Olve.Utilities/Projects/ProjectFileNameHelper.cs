﻿namespace Olve.Utilities.Projects;

/// <summary>
/// Helper class for generating project file names.
/// </summary>
/// <param name="delimiter">The delimiter to use between elements in the file name.</param>
/// <param name="extension">The file extension to use.</param>
public class ProjectFileNameHelper(string? delimiter = null, string? extension = null)
{
    /// <summary>
    /// Gets the delimiter to use between elements in the file name.
    /// </summary>
    public string FileNameDelimiter { get; } = delimiter ?? string.Empty;
    
    /// <summary>
    /// Gets the file extension to use.
    /// </summary>
    public string FileExtension { get; } = extension ?? string.Empty;
    
    /// <summary>
    /// Gets the file name for a project.
    /// </summary>
    /// <param name="elements">The elements to include in the file name.</param>
    /// <returns>The file name.</returns>
    public string GetFileName(IEnumerable<string> elements)
    {
        return $"{string.Join(FileNameDelimiter, elements)}{FileExtension}";
    }
    
    /// <summary>
    /// Gets the elements from a file name.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The elements.</returns>
    public IEnumerable<string> GetElements(string fileName)
    {
        if (!fileName.EndsWith(FileExtension))
        {
            throw new ArgumentException("The file name does not have the expected extension.", nameof(fileName));
        }
        
        if (fileName.Length == FileExtension.Length)
        {
            return Array.Empty<string>();
        }

        if (string.IsNullOrWhiteSpace(FileNameDelimiter))
        {
            return [fileName[..^FileExtension.Length]];
        }
        
        return fileName[..^FileExtension.Length].Split(FileNameDelimiter);
    }
}