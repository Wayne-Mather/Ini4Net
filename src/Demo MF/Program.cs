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

using System.Collections;
using System.IO;
using Ini4Mf;
using Microsoft.SPOT;

namespace Demo_MF
{
    public class Program
    {
        public static void Main()
        {
            Directory.SetCurrentDirectory("\\");

            Ini4Mf.Ini ini = new Ini();
            ArrayList al = new ArrayList();

            al.Add("#");
            al.Add("# Example INI File");
            al.Add("#");
            al.Add("");
            al.Add("");
            al.Add("; This is our first section");
            al.Add("[Section 1]");
            al.Add("Key 1 = Value 1");
            al.Add("Key 2 = Value 2");
            al.Add("");
            al.Add("; This is our second section");
            al.Add("[Logging]");
            al.Add("Enabled = true");
            al.Add("Log File = C:\test.txt");
            al.Add("Log Level = Verbose");
            al.Add("");
            al.Add("; This is our third section");
            al.Add("[Database]");
            al.Add("Connection String = server=(local);uid=sa;pwd=;Trusted_Connection=true;");

            ini.Read(al);

            string key1 = ini["Section 1"]["Key 1"];
            Debug.Assert(string.Compare(key1, "Value 1") == 0);

            key1 = ini["Database"]["Connection String"];
            Debug.Assert(
                string.Compare(key1, "server=(local);uid=sa;pwd=;Trusted_Connection=true;") == 0);

        }

    }
}
