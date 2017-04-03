#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace Ini4Net
{
    #region Ini Syntax

    /// <summary>
    ///     Defines the syntax for our Ini file
    /// </summary>
    public class IniSyntax
    {
        /// <summary>
        ///     Initialise with our default values
        /// </summary>
        public IniSyntax()
        {
            CommentTokens = new[] {'#', ';'};
            ValueSeparatorToken = '=';
            SectionHeaderEndToken = ']';
            SectionHeaderStartToken = '[';
            AllowAddingSections = false;
        }

        /// <summary>
        ///     What characters are comment characters?
        ///     default: #, ;
        /// </summary>
        public char[] CommentTokens { get; set; }

        /// <summary>
        ///     What character separates the key from the value?
        ///     default: =
        /// </summary>
        public char ValueSeparatorToken { get; set; }

        /// <summary>
        ///     What characters defines the section header start token?
        ///     default: [
        /// </summary>
        public char SectionHeaderStartToken { get; set; }

        /// <summary>
        ///     What characters defines the section header end token?
        ///     default: ]
        /// </summary>
        public char SectionHeaderEndToken { get; set; }

        /// <summary>
        ///     Set to true if cann add sections and keys at runtime
        ///     NOTE: This will stop the throwing of IniKeyNotFoundException as a new key will be created
        ///     default: False
        /// </summary>
        public bool AllowAddingSections { get; set; }
    }

    #endregion

    #region IniSection

    /// <summary>
    ///     A section inside a configuration file
    /// </summary>
    /// <example>
    ///     [Section One]
    ///     Key 1 = Value 1
    ///     Key 2 = Value 2
    /// </example>
    public class IniSection : IEnumerable
    {
        /// <summary>
        ///     The keys for this section
        /// </summary>
        public KeyPairList Keys = new KeyPairList();

        /// <summary>
        ///     Name of this section
        /// </summary>
        public string Name;

        /// <summary>
        ///     Get the value of a key within this section
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string this[string keyName]
        {
            get
            {
                keyName = keyName.ToLower(CultureInfo.InvariantCulture);
                if (Keys.Contains(keyName))
                {
                    return Keys[keyName];
                }
                throw new IniKeyNotFoundException("Key does not exist");
            }
            set
            {
                keyName = keyName.ToLower(CultureInfo.InvariantCulture);
                if (Keys.Contains(keyName))
                {
                    Keys[keyName] = value;
                }
                else
                {
                    Keys.Add(keyName, value);
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Keys.GetEnumerator();
        }

        /// <summary>
        ///     Get the number of keys in this section
        /// </summary>
        /// <returns>The number of keys that exist</returns>
        public int Count()
        {
            return Keys.Count;
        }

        /// <summary>
        ///     Check if the given key exists in this section
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns>TRUE if exists, FALSE otherwise</returns>
        public bool Contains(string keyName)
        {
            return Keys.Contains(keyName.ToLower(CultureInfo.InvariantCulture));
        }
    }

    #endregion

    #region KeyPairList

    /// <summary>
    ///     KeyPairList class for holding key/pair values
    /// </summary>
    public class KeyPairList : IEnumerable
    {
        #region Private

        private readonly Dictionary<string, string> _kp = new Dictionary<string, string>();

        #endregion

        #region Count

        /// <summary>
        ///     How many entries in this collection?
        /// </summary>
        public int Count
        {
            get { return _kp.Count; }
        }

        #endregion

        #region this[key]

        /// <summary>
        ///     Get or Set the value that for the given key
        /// </summary>
        /// <param name="key">The name of the key</param>
        /// <returns>A string for the value of this key</returns>
        public string this[string key]
        {
            get
            {
                key = key.ToLower(CultureInfo.InvariantCulture);
                if (Contains(key.ToLower()))
                {
                    return _kp[key];
                }
                throw new IniKeyNotFoundException(key);
            }
            set
            {
                key = key.ToLower(CultureInfo.InvariantCulture);
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

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            return _kp.GetEnumerator();
        }

        #endregion

        #region Add

        /// <summary>
        ///     Add a new key/value pair to the collection
        /// </summary>
        /// <param name="key">The name of the key</param>
        /// <param name="value">The name of the value</param>
        public void Add(string key, string value)
        {
            _kp.Add(key.ToLower(), value.Trim());
        }

        #endregion

        #region Contains

        /// <summary>
        ///     Searches for a given key in the collection
        /// </summary>
        /// <param name="key">The name of the key</param>
        /// <returns>TRUE if key exists, FALSE otherwise</returns>
        public bool Contains(string key)
        {
            return _kp.ContainsKey(key.ToLower());
        }

        #endregion

        #region Remove

        /// <summary>
        ///     Remove a key from the collection
        /// </summary>
        /// <param name="key">The name of the key</param>
        /// <returns>TRUE if key removed, FALSE otherwise</returns>
        public bool Remove(string key)
        {
            return _kp.Remove(key.ToLower());
        }

        #endregion

        #region GetKeys

        /// <summary>
        ///     Return a list of the keys in the collection
        /// </summary>
        /// <returns>A List of strings for the key names</returns>
        public List<string> GetKeys()
        {
            return new List<string>(_kp.Keys);
        }

        #endregion

        #region GetValues

        /// <summary>
        ///     Return a list of the values in the collection
        /// </summary>
        /// <returns>A List of strings for the key values</returns>
        public List<string> GetValues()
        {
            return new List<string>(_kp.Values);
        }

        #endregion
    }

    #endregion

    #region Ini

    /// <summary>
    ///     This is the main class that is used to read an INI file from disk
    /// </summary>
    /// <example>
    ///     // Ini File---------------------
    ///     // # Example Ini File (Saved as Config.ini)
    ///     // [Section 1]
    ///     // Key 1 = Value 1
    ///     // Key 2 = Value 2
    ///     //
    ///     // [Database]
    ///     // Connection String = server=(local);uid=sa;pwd=;Trusted Connection=True;
    ///     // ------------------------
    ///     Ini ini = new Ini();
    ///     if(File.Exists("Config.ini"))
    ///     {
    ///     if(ini.Read("Config.ini"))
    ///     {
    ///     string conStr = ini["Database"]["Connection String"];
    ///     ini["Database"]["Connection String"] = "server=Remote;uid=sa;pwd=;Trusted Connection=True;";
    ///     ini.Write("NewConfig.ini",true);
    ///     }
    ///     }
    /// </example>
    public class Ini : IEnumerable
    {
        // ReSharper disable once InconsistentNaming
        internal Dictionary<string, IniSection> _sections = new Dictionary<string, IniSection>();

        public List<string> ErrorMessages = new List<string>();
        public IniSyntax Syntax = new IniSyntax();

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            return Sections.GetEnumerator();
        }

        #endregion

        #region Write

        /// <summary>
        ///     Write the configuration data to disk
        /// </summary>
        /// <param name="fileName">The name of the file to write to</param>
        /// <param name="overwrite">Overwrite existing file?</param>
        public void Write(string fileName, bool overwrite)
        {
            if (File.Exists(fileName) && !overwrite)
            {
                throw new FileLoadException("File Already Exists");
            }

            var sb = new StringBuilder();
            sb.AppendLine("#");
            sb.AppendLine("# AutoGenerated on " + DateTime.Now.ToShortDateString() + " " +
                          DateTime.Now.ToShortTimeString());
            sb.AppendLine("#");
            sb.Append(ToString());
            File.WriteAllText(fileName, sb.ToString());
        }

        #endregion

        #region ToString

        /// <summary>
        ///     Return a string version of the ini file
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var s in Sections)
            {
                sb.AppendLine("[" + s.Name + "]");
                foreach (var k in s.Keys.GetKeys())
                {
                    sb.AppendLine(k + " = " + s.Keys[k]);
                }
                sb.AppendLine();
            }
            sb.AppendLine("");
            return sb.ToString();
        }

        #endregion

        #region Get

        /// <summary>
        ///     Allow the ability of getting a key from a section and converting to a .NET data type
        /// </summary>
        /// <example>
        ///     Ini myIni = new Ini();
        ///     myIni["Other Section"]["Key3"] = "true";
        ///     bool b = myIni.Get&lt;bool&gt;("Other Section", "Key3");
        /// </example>
        /// <typeparam name="T">The data type to convert to</typeparam>
        /// <param name="section">The section to look for</param>
        /// <param name="key">The name of the key</param>
        /// <returns>An object of type T or IniSectionNotFoundException</returns>
        public T Get<T>(string section, string key)
        {
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException();
            }
            try
            {
                var s = this[section];
                return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(s[key]);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
            throw new IniSectionNotFoundException(section);
        }

        #endregion

        #region Set

        /// <summary>
        ///     Allow the ability to set the value of a key from a .NET data type
        /// </summary>
        /// <example>
        ///     Ini myIni = new Ini();
        ///     myIni["Other Section"]["Key3"] = "today";
        ///     myIni.Set("Other Section", DateTime.Now);
        /// </example>
        /// <typeparam name="T">The object</typeparam>
        /// <param name="section">The section</param>
        /// <param name="key">The key</param>
        /// <param name="value">The data</param>
        /// <returns>T</returns>
        public T Set<T>(string section, string key, T value)
        {
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException();
            }
            try
            {
                var s = this[section];
                if (!s.Keys.Contains(key))
                {
                    s.Keys.Add(key, string.Empty);
                }
                s[key] = value.ToString();
                return value;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
            throw new IniKeyNotFoundException(section);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Get a section that matches the given name
        /// </summary>
        /// <param name="sectionName">The name of the section</param>
        /// <returns>An IniSection object for the section, or NULL if not exist</returns>
        public IniSection this[string sectionName]
        {
            get
            {
                sectionName = sectionName.ToLower(CultureInfo.InvariantCulture);
                if (_sections.ContainsKey(sectionName))
                {
                    return _sections[sectionName];
                }
                throw new IniSectionNotFoundException(string.Format("Section {0} not found", sectionName));
            }
            set
            {
                sectionName = sectionName.ToLower(CultureInfo.InvariantCulture);
                if (!_sections.ContainsKey(sectionName))
                {
                    _sections.Add(sectionName, value ?? new IniSection());
                }
            }
        }

        /// <summary>
        ///     Return a list of the sections
        /// </summary>
        public List<IniSection> Sections
        {
            get { return new List<IniSection>(_sections.Values); }
        }

        /// <summary>
        ///     Return a list of the section names
        /// </summary>
        public List<string> SectionNames
        {
            get { return new List<string>(_sections.Keys); }
        }

        #endregion

        #region Constructors

        public Ini()
        {
        }

        public Ini(IniSyntax syntax)
        {
            Syntax = syntax;
        }

        #endregion

        #region Read

        /// <summary>
        ///     Parse the configuration data from the given filename
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns></returns>
        public bool Read(string fileName)
        {
            ErrorMessages.Clear();
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                return Read(File.ReadAllLines(fileName));
            }
            return false;
        }

        private bool IsHeader(string s)
        {
            s = s.Trim();
            return s[0] == Syntax.SectionHeaderStartToken && s[s.Length - 1] == Syntax.SectionHeaderEndToken;
            /* return s.Contains(Syntax.SectionHeaderEndToken.ToString(CultureInfo.InvariantCulture)) &&
                   s.Contains(Syntax.SectionHeaderEndToken.ToString(CultureInfo.InvariantCulture)); */
        }

        /// <summary>
        ///     Is this char a comment char?
        /// </summary>
        /// <param name="ch">The character to check</param>
        /// <returns>TRUE if is a comment, FALSE otherwise</returns>
        private bool IsComment(char ch)
        {
            foreach (var c in Syntax.CommentTokens)
            {
                if (ch == c)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Parse the configuration data from the array
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public bool Read(string[] contents)
        {
            ErrorMessages.Clear();
            /* if(Contents.Length < 1)
            {
                throw new ArgumentNullException("Contents");
            }*/
            try
            {
                Sections.Clear();
                IniSection s = null;
                for (var i = 0; i < contents.Length; i++)
                {
                    var l = contents[i].Trim();

                    if (l.Length > 0)
                    {
                        if (!IsComment(l[0]))
                        {
                            if (IsHeader(l))
                            {
                                if (s != null)
                                {
                                    _sections.Add(s.Name.ToLower(CultureInfo.InvariantCulture).Trim(), s);
                                }
                                s = new IniSection
                                {
                                    Name =
                                        l.Contains(Syntax.SectionHeaderEndToken.ToString(CultureInfo.InvariantCulture))
                                            ? l.Substring(1, l.Length - 2).ToLower(CultureInfo.InvariantCulture).Trim()
                                            : l.Substring(1, l.Length - 1).ToLower(CultureInfo.InvariantCulture).Trim()
                                };
                                continue;
                            }

                            if (s != null)
                            {
                                if (l.Contains(Syntax.ValueSeparatorToken.ToString(CultureInfo.InvariantCulture)))
                                {
                                    var pos = l.IndexOf(Syntax.ValueSeparatorToken);
                                    s.Keys.Add(l.Substring(0, pos).ToLower(CultureInfo.InvariantCulture).Trim(),
                                        l.Substring(pos + 1));
                                }
                                else
                                {
                                    s.Keys.Add(l.ToLower(CultureInfo.InvariantCulture).Trim(), l);
                                }
                            }
                        }
                    }
                }
                if (s != null)
                {
                    _sections.Add(s.Name.ToLower(CultureInfo.InvariantCulture).Trim(), s);
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
        ///     Parse the configuration data from the stream reader.
        ///     NOTE: The parent caller must take care of the dispose
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public bool Read(StreamReader sr)
        {
            var rval = false;
            if (sr != null)
            {
                ErrorMessages.Clear();
                var lines = new List<string>();
                while (!sr.EndOfStream)
                {
                    lines.Add(sr.ReadLine());
                }
                sr.Close();
                rval = Read(lines.ToArray());
            }
            return rval;
        }

        /// <summary>
        ///     Parse the configuration data from the filestream
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public bool Read(FileStream fs)
        {
            var rval = false;
            if (fs != null)
            {
                ErrorMessages.Clear();
                if (!fs.CanRead)
                {
                    throw new FileLoadException("Can't read from filestream");
                }

                try
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    rval = Read(new StreamReader(fs));
                }
                catch (Exception e)
                {
                    ErrorMessages.Add(e.Message);
                }
            }
            return rval;
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