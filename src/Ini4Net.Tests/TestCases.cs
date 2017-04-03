#region Using

using System.Collections.Generic;
using System.IO;
using Xunit;

#endregion

namespace Ini4Net.Tests
{
    public class TestCases
    {
        /// <summary>
        ///     Run the filesytem test using a given syntax
        /// </summary>
        /// <param name="syntax"></param>
        private void RunFS(IniSyntax syntax)
        {
            if (syntax == null)
            {
                syntax = new IniSyntax();
            }
            var tf = Path.GetTempFileName();
            using (var sw = new StreamWriter(tf))
            {
                foreach (var l in CreateIni(syntax))
                {
                    sw.WriteLine(l);
                    sw.Flush();
                }
                sw.Close();
            }
            RunTestFs(tf, syntax);
            File.Delete(tf);
        }

        // Run the tests from the filesystem
        private void RunTestFs(string fn, IniSyntax syntax = null)
        {
            // test by filename
            var i = new Ini();
            if (syntax != null)
            {
                i.Syntax = syntax;
            }
            var r = i.Read(fn);
            Assert.True(r);
            VerifyLoad(i);

            // test by streamreader
            i = new Ini();
            if (syntax != null)
            {
                i.Syntax = syntax;
            }
            var sr = new StreamReader(fn);
            r = i.Read(sr);
            sr.Close();
            sr.Dispose();
            Assert.True(r);
            VerifyLoad(i);

            // test by filestream
            i = new Ini();
            if (syntax != null)
            {
                i.Syntax = syntax;
            }
            var fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            r = i.Read(fs);
            fs.Close();
            fs.Dispose();
            Assert.True(r);
            VerifyLoad(i);
        }

        // run all the tests with this syntax
        private void RunTest(IniSyntax syntax)
        {
            var i = new Ini();
            if (syntax != null)
            {
                i.Syntax = syntax;
            }
            Assert.NotNull(i);
            var r = i.Read(CreateIni(syntax));
            Assert.True(r);
            VerifyLoad(i);
        }

        // verify the ini file was parsed correctly
        private void VerifyLoad(Ini i)
        {
            // verify the counts
            Assert.Equal(2, i.Sections[0].Count());
            Assert.Equal(3, i.Sections[1].Count());
            Assert.Equal(2, i.Sections[2].Count());

            // verify the values
            var s = i["First Section"];
            Assert.NotNull(s);
            Assert.Equal("1", s["value 1"]);
            Assert.Equal("3", s["value 3"]);
            var keyNotFound = false;
            try
            {
                // ReSharper disable once UnusedVariable
                var t = s["value 2"];
            }
            // ReSharper disable once UnusedVariable
            catch (IniKeyNotFoundException ex)
            {
                keyNotFound = true;
            }
            Assert.Equal(true, keyNotFound);


            // verify the sections
            Assert.Equal("first section", i.Sections[0].Name);
            Assert.Equal("second section", i.Sections[1].Name);
            Assert.Equal("third section", i.Sections[2].Name);

            // verify bad header in previous section
            Assert.True(
                i["third section"].Contains(string.Format("{0}Fourth Section", i.Syntax.SectionHeaderStartToken)));
            var rv = i["third section"][string.Format("{0}Fourth Section", i.Syntax.SectionHeaderStartToken)];
            Assert.False(string.IsNullOrEmpty(rv));
            Assert.Equal(string.Format("{0}Fourth Section", i.Syntax.SectionHeaderStartToken), rv);
        }

        /// <summary>
        ///     Create an example INI file with some errors in the given syntax if supplied
        /// </summary>
        /// <param name="syntax">The syntax to use, if NULL then use the default syntax</param>
        /// <returns>An array of string that can be used for tests</returns>
        private string[] CreateIni(IniSyntax syntax = null)
        {
            if (syntax == null)
            {
                syntax = new IniSyntax();
            }
            return new List<string>
            {
                $"{syntax.CommentTokens[0]} This is a test ini file",
                "",
                $"this key {syntax.ValueSeparatorToken} will go missing (not in a section)",
                "",
                $"{syntax.SectionHeaderStartToken}First Section{syntax.SectionHeaderEndToken}",
                $"value 1 {syntax.ValueSeparatorToken} 1",
                $"; value 2 {syntax.ValueSeparatorToken} 2",
                $"value 3 {syntax.ValueSeparatorToken} 3",
                "",
                $"{syntax.SectionHeaderStartToken}Second Section{syntax.SectionHeaderEndToken}",
                $"value 10 {syntax.ValueSeparatorToken} 10",
                $"value 20 {syntax.ValueSeparatorToken} 20",
                "",
                $"value 30 {syntax.ValueSeparatorToken} 30",
                "",
                $"{syntax.SectionHeaderStartToken}Third Section{syntax.SectionHeaderEndToken}",
                "",
                $"{syntax.SectionHeaderStartToken}Fourth Section",
                $"{syntax.SectionHeaderStartToken}"
            }.ToArray();
        }

        [Fact]
        public void TestIniConstructor()
        {
            var ini = new Ini();
            Assert.NotNull(ini);
        }


        [Fact]
        // run the tests via string array entry point 
        // and all the filesystem entry points
        // also test against different inisyntax
        public void TestIniParsing()
        {
            RunTest(null);
            RunFS(null);

            var s = new IniSyntax
            {
                SectionHeaderStartToken = '{',
                SectionHeaderEndToken = '}',
                ValueSeparatorToken = ':'
            };
            RunTest(s);
            RunFS(s);

            s.SectionHeaderStartToken = '+';
            s.SectionHeaderEndToken = '^';
            s.ValueSeparatorToken = '|';
            RunTest(s);
            RunFS(s);
        }

        [Fact]
        public void TestIniSectionConstructor()
        {
            var s = new IniSection();
            Assert.NotNull(s);
        }

        [Fact]
        public void TestIniSyntaxDefaultOptions()
        {
            var s = new IniSyntax();
            Assert.NotNull(s);
            Assert.Equal(2, s.CommentTokens.Length);
            Assert.Equal('#', s.CommentTokens[0]);
            Assert.Equal(';', s.CommentTokens[1]);
            Assert.Equal('=', s.ValueSeparatorToken);
            Assert.Equal('[', s.SectionHeaderStartToken);
            Assert.Equal(']', s.SectionHeaderEndToken);
            Assert.False(s.AllowAddingSections);
        }
    }
}