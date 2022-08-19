using NetcodeIO.NET.Encryption.Tools;
using NetcodeIO.NET.Encryption.Interfaces;
using NetcodeIO.NET.Encryption.Parameters;

namespace NetcodeIO.NET.Encryption;


public class AEADC20P1305
{
    private static readonly byte[] Zeroes = new byte[15];
    private static ChaCha7539Engine? cipher;

    private static IMac _mac = new Poly1305();

    private static ParametersWithIV? _temp_Params;
    private static KeyParameter? _encryptKey;
    private static KeyParameter? _decryptKey;
    private static KeyParameter? _macKey;

    private static object mutex = new object();

    public static int Encrypt(byte[] plaintext, int offset, int len, byte[] additionalData, byte[] nonce, byte[] key, byte[] outBuffer)
    {
        lock (mutex)
        {
            if (cipher == null)
                cipher = new ChaCha7539Engine();
            else
                cipher.Reset();

            _encryptKey = new KeyParameter(key);

            _temp_Params = new ParametersWithIV(_encryptKey, nonce);

            cipher.Init(true, _temp_Params);

            byte[] firstBlock = BufferPool.GetBuffer(64);
            KeyParameter macKey = GenerateRecordMacKey(cipher, firstBlock);

            cipher.ProcessBytes(plaintext, offset, len, outBuffer, 0);

            byte[] mac = BufferPool.GetBuffer(16);
            int macsize = CalculateRecordMac(macKey, additionalData, outBuffer, 0, len, mac);
            Array.Copy(mac, 0, outBuffer, len, macsize);

            BufferPool.ReturnBuffer(mac);
            BufferPool.ReturnBuffer(firstBlock);

            return len + 16;
        }
    }

    public static int Decrypt(byte[] ciphertext, int offset, int len, byte[] additionalData, byte[] nonce, byte[] key, byte[] outBuffer)
    {
        lock (mutex)
        {
            if (cipher == null)
                cipher = new ChaCha7539Engine();
            else
                cipher.Reset();

            _decryptKey = new KeyParameter(key);

            _temp_Params = new ParametersWithIV(_decryptKey, nonce);

            cipher.Init(false, _temp_Params);

            byte[] firstBlock = BufferPool.GetBuffer(64);
            KeyParameter macKey = GenerateRecordMacKey(cipher, firstBlock);

            int plaintextLength = len - 16;

            byte[] calculatedMac = BufferPool.GetBuffer(16);
            CalculateRecordMac(macKey, additionalData, ciphertext, offset, plaintextLength, calculatedMac);

            byte[] receivedMac = BufferPool.GetBuffer(16);
            Array.Copy(ciphertext, offset + plaintextLength, receivedMac, 0, receivedMac.Length);

            if (!Arrays.ConstantTimeAreEqual(calculatedMac, receivedMac))
            {
                BufferPool.ReturnBuffer(calculatedMac);
                BufferPool.ReturnBuffer(receivedMac);
                BufferPool.ReturnBuffer(firstBlock);

                throw new Exception("Bad mac record");
            }

            BufferPool.ReturnBuffer(calculatedMac);
            BufferPool.ReturnBuffer(receivedMac);
            BufferPool.ReturnBuffer(firstBlock);

            cipher.ProcessBytes(ciphertext, offset, plaintextLength, outBuffer, 0);
            return plaintextLength;
        }
    }

    protected static KeyParameter GenerateRecordMacKey(IStreamCipher cipher, byte[] firstBlock)
    {
        cipher.ProcessBytes(firstBlock, 0, firstBlock.Length, firstBlock, 0);

        _macKey = new KeyParameter(firstBlock, 0, 32);


        Arrays.Fill(firstBlock, (byte)0);
        return _macKey;
    }

    protected static int CalculateRecordMac(KeyParameter macKey, byte[] additionalData, byte[] buf, int off, int len, byte[] outMac)
    {
        _mac.Reset();
        _mac.Init(macKey);

        UpdateRecordMacText(_mac, additionalData, 0, additionalData.Length);
        UpdateRecordMacText(_mac, buf, off, len);
        UpdateRecordMacLength(_mac, additionalData.Length);
        UpdateRecordMacLength(_mac, len);

        _mac.DoFinal(outMac, 0);
        return _mac.GetMacSize();
    }

    protected static void UpdateRecordMacLength(IMac mac, int len)
    {
        byte[] longLen = BufferPool.GetBuffer(8);
        Pack.UInt64_To_LE((ulong)len, longLen);
        mac.BlockUpdate(longLen, 0, longLen.Length);
        BufferPool.ReturnBuffer(longLen);
    }

    protected static void UpdateRecordMacText(IMac mac, byte[] buf, int off, int len)
    {
        mac.BlockUpdate(buf, off, len);

        int partial = len % 16;
        if (partial != 0)
        {
            mac.BlockUpdate(Zeroes, 0, 16 - partial);
        }
    }
}

