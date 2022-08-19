using System.Security.Cryptography;
using NetcodeIO.NET.Encryption.Interfaces;

namespace NetcodeIO.NET.Encryption.Tools;

/// <summary>
/// Uses RandomNumberGenerator.Create() to get randomness generator
/// </summary>
public class CryptoRandomGenerator : IRandomGenerator
{
    private readonly RandomNumberGenerator rndProv;

    public CryptoRandomGenerator() : this(RandomNumberGenerator.Create())
    {
    }

    public CryptoRandomGenerator(RandomNumberGenerator rng)
    {
        this.rndProv = rng;
    }

    public virtual void NextBytes(byte[] bytes)
    {
        rndProv.GetBytes(bytes);
    }

    public virtual void NextBytes(byte[] bytes, int start, int len)
    {
        if (start < 0)
            throw new ArgumentException("Start offset cannot be negative", "start");
        if (bytes.Length < (start + len))
            throw new ArgumentException("Byte array too small for requested offset and length");

        if (bytes.Length == len && start == 0)
        {
            NextBytes(bytes);
        }
        else
        {
            byte[] tmpBuf = new byte[len];
            NextBytes(tmpBuf);
            Array.Copy(tmpBuf, 0, bytes, start, len);
        }
    }
}


