namespace NetcodeIO.NET.Encryption.Interfaces;

/// <remarks>Generic interface for objects generating random bytes.</remarks>
public interface IRandomGenerator
{
    /// <summary>Fill byte array with random values.</summary>
    /// <param name="bytes">Array to be filled.</param>
    void NextBytes(byte[] bytes);

    /// <summary>Fill byte array with random values.</summary>
    /// <param name="bytes">Array to receive bytes.</param>
    /// <param name="start">Index to start filling at.</param>
    /// <param name="len">Length of segment to fill.</param>
    void NextBytes(byte[] bytes, int start, int len);
}

