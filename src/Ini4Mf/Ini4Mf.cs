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
using System.Collections;
using System.IO;

namespace Ini4Mf
{

    #region Ini Syntax

    /// <summary>
    /// Defines the syntax for our Ini file
    /// </summary>
    public class IniSyntax
    {
        /// <summary>
        /// What characters are comment characters?
        /// default: #, ;
        /// </summary>
        public char[] CommentTokens { get; set; }

        /// <summary>
        /// What character separates the key from the value?
        /// default: =
        /// </summary>
        public char ValueSeparatorToken { get; set; }

        /// <summary>
        /// What characters defines the section header start token?
        /// default: [
        /// </summary>
        public char SectionHeaderStartToken { get; set; }

        /// <summary>
        /// What characters defines the section header end token?
        /// default: ]
        /// </summary>
        public char SectionHeaderEndToken { get; set; }

        /// <summary>
        /// Initialise with our default values
        /// </summary>
        public IniSyntax()
        {
            CommentTokens = new char[] { '#', ';' };
            ValueSeparatorToken = '=';
            SectionHeaderEndToken = ']';
            SectionHeaderStartToken = '[';
        }

    }

    #endregion

    #region IniSection

    /// <summary>
    /// A section inside a configuration file
    /// </summary>
    /// <example>
    /// [Section One]
    /// Key 1 = Value 1
    /// Key 2 = Value 2
    /// </example>
    public class IniSection : IEnumerable
    {
        /// <summary>
        /// The keys for this section 
        /// </summary>
        public KeyPairList Keys = new KeyPairList();

        /// <summary>
        /// Name of this section
        /// </summary>
        public string Name;

        /// <summary>
        /// Get the value of a key within this section
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string this[string keyName]
        {
            get
            {
                if (Keys.Contains(keyName))
                {
                    try
                    {
                        return Keys[keyName];
                    }
                    catch (Exception)
                    {
                        throw new IniKeyNotFoundException("Key contains no values");
                    }
                }
                throw new IniKeyNotFoundException("Key does not exist");
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Keys.GetEnumerator();
        }
    }
    #endregion

    #region KeyPairList

    /// <summary>
    /// KeyPairList class for holding key/pair values
    /// </summary>
    public class KeyPairList : IEnumerable
    {
        #region Private

        private Hashtable _kp = new Hashtable();

        #endregion

        #region Count

        /// <summary>
        /// How many entries in this collection?
        /// </summary>
        public int Count
        {
            get { return _kp.Count; }
        }

        #endregion

        #region this[key]

        /// <summary>
        /// Get or Set the value that for the given key
        /// </summary>
        /// <param name="key">The name of the key</param>
        /// <returns>A string for the value of this key</returns>
        public string this[string key]
        {
            get
            {
                if (Contains(key))
                {
                    return _kp[key].ToString();
                }
                throw new IniKeyNotFoundException(key);
            }
            set
            {
                if (Contains(key))
                {
                    _kp[key] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        #endregion

        #region Add

        /// <summary>
        /// Add a new key/value pair to the collection
        /// </summary>
        /// <param name="key">The name of the key</param>
        /// <param name="value">The name of the value</param>
        public void Add(string key, string value)
        {
            _kp.Add(key.Trim(), value.Trim());
        }

        #endregion

        #region Contains

        /// <summary>
        /// Searches for a given key in the collection
        /// </summary>
        /// <param name="key">The name of the key</param>
        /// <returns>TRUE if key exists, FALSE otherwise</returns>
        public bool Contains(string key)
        {
            return _kp.Contains(key.Trim());
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove a key from the collection
        /// </summary>
        /// <param name="key">The name of the key</param>
        /// <returns>TRUE if key removed, FALSE otherwise</returns>
        public bool Remove(string key)
        {
            bool rval = false;
            try
            {
                _kp.Remove(key.Trim());
                rval = true;
            } catch
            {
                rval = false;
            }
            return rval;
        }

        #endregion

        #region GetKeys

        /// <summary>
        /// Return a list of the keys in the collection
        /// </summary>
        /// <returns>A List of strings for the key names</returns>
        public ICollection GetKeys()
        {
            return _kp.Keys;
        }

        #endregion

        #region GetValues

        /// <summary>
        /// Return a list of the values in the collection
        /// </summary>
        /// <returns>A List of strings for the key values</returns>
        public ICollection GetValues()
        {
            return _kp.Values;
        }

        #endregion

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            return _kp.GetEnumerator();
        }

        #endregion

    }

    #endregion

    #region Ini

    /// <summary>
    /// This is the main class that is used to read an INI file from disk
    /// </summary>
    /// <example>
    /// // Ini File---------------------
    /// // # Example Ini File (Saved as Config.ini)
    /// // [Section 1]
    /// // Key 1 = Value 1
    /// // Key 2 = Value 2
    /// // 
    /// // [Database]
    /// // Connection String = server=(local);uid=sa;pwd=;Trusted Connection=True;
    /// // ------------------------
    /// Ini ini = new Ini();
    /// if(File.Exists("Config.ini"))
    /// {
    ///     if(ini.Read("Config.ini")) 
    ///     {
    ///         string conStr = ini["Database"]["Connection String"];
    ///         ini["Database"]["Connection String"] = "server=Remote;uid=sa;pwd=;Trusted Connection=True;";
    ///         ini.Write("NewConfig.ini",true);
    ///     }
    /// }
    /// </example>
    public class Ini : IEnumerable
    {
        internal Hashtable _sections = new Hashtable();
        public ArrayList ErrorMessages = new ArrayList();
        public IniSyntax Syntax = new IniSyntax();

        #region Properties

        /// <summary>
        /// Get a section that matches the given name
        /// </summary>
        /// <param name="sectionName">The name of the section</param>
        /// <returns>An IniSection object for the section, or NULL if not exist</returns>
        public IniSection this[string sectionName]
        {
            get
            {
                if (_sections.Contains(sectionName))
                {
                    return (IniSection) _sections[sectionName];
                }
                throw new IniSectionNotFoundException("Section " + sectionName + " not found");
            }
        }

        /// <summary>
        /// Return a list of the sections
        /// </summary>
        public ICollection Sections
        {
            get { return _sections.Values; }
        }

        /// <summary>
        /// Return a list of the section names
        /// </summary>
        public ICollection SectionNames
        {
            get { return _sections.Keys; }
        }

        #endregion

        #region Read

        /// <summary>
        /// Parse the configuration data from the given filename
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns></returns>
        public bool Read(string fileName)
        {
            ErrorMessages.Clear();
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            FileStream fs = new FileStream(fileName, FileMode.Open);
            Read(fs);
            fs.Close();
            return true;
        }

        private bool IsHeader(string s)
        {
            return String.Contains(s, Syntax.SectionHeaderEndToken.ToString()) &&
                   String.Contains(s, Syntax.SectionHeaderEndToken.ToString());
        }

        /// <summary>
        /// Is this char a comment char?
        /// </summary>
        /// <param name="ch">The character to check</param>
        /// <returns>TRUE if is a comment, FALSE otherwise</returns>
        private bool IsComment(char ch)
        {
            foreach (char c in Syntax.CommentTokens)
            {
                if (ch == c)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parse the configuration data from the array
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public bool Read(ArrayList contents)
        {
            ErrorMessages.Clear();
            /* if(Contents.Length < 1)
            {
                throw new ArgumentNullException("Contents");
            }*/
            try
            {
                _sections.Clear();
                string l;
                IniSection s = null;
                for (int i = 0; i < contents.Count; i++)
                {
                    if (contents[i] != null)
                    {
                        l = (contents[i] as string).Trim();

                        if (l.Length > 0)
                        {
                            if (!IsComment(l[0]))
                            {
                                //if (l[0] == Syntax.SectionHeaderStartToken)
                                if (IsHeader(l))
                                {
                                    if (s != null)
                                    {
                                        _sections.Add(s.Name, s);
                                    }
                                    s = new IniSection();

                                    //
                                    // ensure we do have a valid section header
                                    //
                                    s.Name = String.Contains(l, Syntax.SectionHeaderEndToken.ToString())
                                                 ? l.Substring(1, l.Length - 2)
                                                 : l.Substring(1, l.Length - 1);
                                    continue;
                                }

                                if (s != null)
                                {
                                    if (String.Contains(l, Syntax.ValueSeparatorToken.ToString()))
                                    {
                                        int pos = l.IndexOf(Syntax.ValueSeparatorToken);
                                        s.Keys.Add(l.Substring(0, pos), l.Substring(pos + 1));
                                    }
                                    else
                                    {
                                        s.Keys.Add(l, l);
                                    }
                                }
                            }
                        }
                    }
                }
                if (s != null)
                {
                    _sections.Add(s.Name, s);
                }
                return true;
            }
            catch (Exception e)
            {
                ErrorMessages.Add(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Parse the configuration data from the stream reader
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public bool Read(StreamReader sr)
        {
            ErrorMessages.Clear();
            ArrayList lines = new ArrayList();
            while (!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
            }
            sr.Close();
            sr.Dispose();
            return Read(lines);
        }

        /// <summary>
        /// Parse the configuration data from the filestream
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public bool Read(FileStream fs)
        {
            ErrorMessages.Clear();
            if (fs == null)
            {
                throw new ArgumentNullException("fs");
            }
            if (!fs.CanRead)
            {
                throw new Exception("Can't read from filestream");
            }

            try
            {
                fs.Seek(0, SeekOrigin.Begin);
                return Read(new StreamReader(fs));
            }
            catch (Exception e)
            {
                ErrorMessages.Add(e.Message);
                return false;
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Write the configuration data to disk
        /// </summary>
        /// <param name="fileName">The name of the file to write to</param>
        /// <param name="overwrite">Overwrite existing file?</param>
        public void Write(string fileName, bool overwrite)
        {
            if (File.Exists(fileName) && !overwrite)
            {
                throw new Exception("File Already Exists");
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#");
            sb.AppendLine("# AutoGenerated on " + DateTime.Now.ToString() + " " +
                          DateTime.Now.ToString());
            sb.AppendLine("#");
            sb.Append(ToString());
            File.WriteAllBytes(fileName, sb.ToBytes());
        }

        #endregion

        #region ToString

        /// <summary>
        /// Return a string version of the ini file
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IniSection s in Sections)
            {
                sb.AppendLine("[" + s.Name + "]");
                foreach (string k in s.Keys.GetKeys())
                {
                    sb.AppendLine(k + " = " + s.Keys[k]);
                }
                sb.AppendLine();
            }
            sb.AppendLine("");
            return sb.ToString();
        }

        #endregion

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            return this.Sections.GetEnumerator();
        }

        #endregion
    }

    #endregion

    #region Exceptions

    public class IniSectionNotFoundException : Exception
    {
        public IniSectionNotFoundException(string msg)
            : base(msg)
        {

        }
    }

    public class IniKeyNotFoundException : Exception
    {
        public IniKeyNotFoundException(string msg)
            : base(msg)
        {

        }
    }

    #endregion
}
