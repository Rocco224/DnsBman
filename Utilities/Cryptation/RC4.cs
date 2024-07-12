using System.Security.Cryptography;

namespace DnsBman.Utilities.Cryptation
{
    public class RC4 : IDisposable
    {
        private readonly byte[] _s = new byte[256];
        private readonly byte[] _k = new byte[256];
        private int _j;

        public RC4(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (key.Length < 1 || key.Length > 256)
                throw new ArgumentException("Key length must be between 1 and 256 bytes.");

            for (int i = 0; i < 256; i++)
            {
                _s[i] = (byte)i;
                _k[i] = key[i % key.Length];
            }

            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + _s[i] + _k[i]) % 256;
                Swap(i, j);
            }
        }

        private void Swap(int i, int j)
        {
            byte temp = _s[i];
            _s[i] = _s[j];
            _s[j] = temp;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] output = new byte[inputCount];
            int i = 0;
            for (int x = 0; x < inputCount; x++)
            {
                _j = (_j + _s[++i]) % 256;
                Swap(i, _j);
                output[x] = (byte)(inputBuffer[x + inputOffset] ^ _s[(_s[i] + _s[_j]) % 256]);
            }
            return output;
        }

        public void Dispose()
        {
            Array.Clear(_s, 0, _s.Length);
            Array.Clear(_k, 0, _k.Length);
            _j = 0;
        }
    }
}
