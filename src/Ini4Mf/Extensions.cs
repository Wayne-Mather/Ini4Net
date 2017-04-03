// Copyright 2008-2011 Wayne Mather. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY WAYNE MATHER ``AS IS'' AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Text;

namespace Ini4Mf
{

    #region StringBuilder

    /// <summary>
    /// Really simple string builder
    /// </summary>
    public class StringBuilder : IDisposable
    {
        private char[] str;

        public StringBuilder()
        {
            str = new char[0];
        }

        public StringBuilder(string s)
        {
            str = new char[0];
            Append(s);
        }

        public void Append(char s)
        {
            Append(s.ToString());
        }

        public void Append(char[] s)
        {
            foreach (char c in s)
            {
                Append(c);
            }
        }

        public void Append(int s)
        {
            Append(s.ToString());
        }

        public void Append(long s)
        {
            Append(s.ToString());
        }

        public void Append(byte s)
        {
            Append(s.ToString());
        }

        public void Append(byte[] s)
        {
            foreach (byte b in s)
            {
                Append(b);
            }
        }

        public void Append(string s)
        {
            int t = 0;
            char[] oldStr = new char[str.Length];
            Array.Copy(str, oldStr, str.Length);
            str = new char[str.Length + s.Length];
            Array.Copy(oldStr, str, oldStr.Length);
            Array.Copy(s.ToCharArray(), 0, str, oldStr.Length, s.Length);
        }

        public void AppendLine(string s)
        {
            Append(s);
            AppendLine();
        }

        public void AppendLine()
        {
            Append('\n');
        }

        public void Clear()
        {
            Empty();
        }

        public override string ToString()
        {
            return new string(str);
        }

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public int Capacity()
        {
            if (str != null)
            {
                return str.Length;
            }
            return 0;
        }

        private void Empty()
        {
            str = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!String.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = '\0';
                }
            }
            str = null;
        }

        #endregion
    }

    #endregion

    #region String

    public static class String
    {
        public static bool IsNullOrEmpty(string s)
        {
            if (s == null)
            {
                return true;
            }
            if (s.Trim().Length < 1)
            {
                return true;
            }
            return false;
        }

        public static bool IsNullOrEmpty(char[] s)
        {
            if (s == null)
            {
                return true;
            }
            if (s.Length < 1)
            {
                return true;
            }
            return false;
        }

        public static bool Contains(string src, string search)
        {
            for(int i = 0; i < src.Length;i++)
            {
                int idx = src.IndexOf(search);
                if(idx >= 0)
                {
                    return true;
                }
            }
            return false;
        }
    }

    #endregion

}
