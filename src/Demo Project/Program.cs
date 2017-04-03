#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Ini4Net;

#endregion

namespace Demo_Project
{
    internal class Program
    {
        /// <summary>
        ///     Example for processing an INI file from the INI class
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var ini = new Ini();
            if (File.Exists("Demo1.ini"))
            {
                // setup the syntax
                ini.Syntax.CommentTokens = new[] {'#', ';'};
                ini.Syntax.SectionHeaderStartToken = '[';
                ini.Syntax.SectionHeaderEndToken = ']';
                ini.Syntax.ValueSeparatorToken = '=';

                // Read the INI
                if (ini.Read("Demo1.ini"))
                {
                    Console.WriteLine("Using Connection String {0}", ini["Database"]["Connection String"]);
                    Console.WriteLine("Logging is {0} at a level of {1}", ini["Logging"]["Enabled"],
                        ini["Logging"]["Log Level"]);

                    //
                    // Show using IEnum
                    //
                    Console.WriteLine();
                    Console.WriteLine("Iterating over all sections and keys....");
                    Console.WriteLine();
                    foreach (IniSection section in ini)
                    {
                        Console.WriteLine("[{0}]", section.Name);
                        foreach (KeyValuePair<string, string> kvp in section)
                        {
                            Console.WriteLine("{0} = {1}", kvp.Key, kvp.Value);
                        }
                        Console.WriteLine();
                    }


                    //
                    // Show exceptions
                    //
                    try
                    {
                        Console.WriteLine();
                        Console.WriteLine("Dump Complete. Attempt to read an invalid section");
                        Console.WriteLine("Not Exists: {0}", ini["Not Exist"]);
                    }
                    catch (IniSectionNotFoundException ex)
                    {
                        Console.WriteLine("Section not exist exception caught successfully");
                    }
                    catch
                    {
                        Console.WriteLine("Some other exception thrown. Error!");
                    }


                    //
                    // Show when no section or no keys
                    //
                    var tries = new List<string>();
                    tries.Add("Database");
                    tries.Add("Not Exist");

                    foreach (var section in tries)
                    {
                        try
                        {
                            Console.WriteLine("{0}: {1}", ini[section]["Not a key"]);
                        }
                        catch (IniKeyNotFoundException ex)
                        {
                            Console.WriteLine("Key not exist exception caught successfully when section does exist");
                        }
                        catch (IniSectionNotFoundException ex)
                        {
                            Console.WriteLine(
                                "Key not exist exception caught successfully when section does not exist");
                        }
                        catch
                        {
                            Console.WriteLine("Some other exception thrown. Error!");
                        }
                    }

                    //
                    // Show writing an INI
                    //
                    ini.Write("Output.ini", true);
                }
            }


            // try to create an INI file from scratch
            var syntax = new IniSyntax();
            syntax.AllowAddingSections = true;
            var myIni = new Ini(syntax);
            myIni["My Section"]["My Key"] = "One";
            myIni["Other Section"]["Key1"] = "1";
            myIni["Other Section"]["Key2"] = "2";
            myIni["Other Section"]["Key3"] = "true";
            myIni.Write("myini.ini", true);
            Console.WriteLine(myIni.Get<bool>("Other Section", "Key3"));
            myIni.Set("Other Section", "Key2", DateTime.Now);
            Console.WriteLine(myIni["Other Section"]["Key2"]);
            Console.WriteLine();
            Console.WriteLine("Press any key when ready.....");
            Console.ReadKey();
        }
    }
}