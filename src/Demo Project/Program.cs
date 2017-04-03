using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ini4Net;

namespace Demo_Project
{
    class Program
    {

        /// <summary>
        /// Example for processing an INI file from the INI class
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Ini ini = new Ini();
            if(File.Exists("Demo1.ini"))
            {

                // setup the syntax
                ini.Syntax.CommentTokens = new char[] {'#', ';'};
                ini.Syntax.SectionHeaderStartToken = '[';
                ini.Syntax.SectionHeaderEndToken = ']';
                ini.Syntax.ValueSeparatorToken = '=';

                // Read the INI
                if(ini.Read("Demo1.ini"))
                {
                    Console.WriteLine(string.Format("Using Connection String {0}", ini["Database"]["Connection String"]));
                    Console.WriteLine(string.Format("Logging is {0} at a level of {1}",
                        ini["Logging"]["Enabled"],ini["Logging"]["Log Level"]));

                    //
                    // Show using IEnum
                    //
                    Console.WriteLine();
                    Console.WriteLine("Iterating over all sections and keys....");
                    Console.WriteLine();
                    foreach(IniSection section in ini)
                    {
                        Console.WriteLine(string.Format("[{0}]", section.Name));
                        foreach(KeyValuePair<string,string> kvp in section)
                        {
                            Console.WriteLine(string.Format("{0} = {1}", kvp.Key, kvp.Value));
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
                        Console.WriteLine(string.Format("Not Exists: {0}", ini["Not Exist"]));
                    } catch(IniSectionNotFoundException ex)
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
                    List<string> tries = new List<string>();
                    tries.Add("Database");
                    tries.Add("Not Exist");

                    foreach (string section in tries)
                    {

                        try
                        {
                            Console.WriteLine(string.Format("{0}: {1}", ini[section]["Not a key"]));
                        }
                        catch (IniKeyNotFoundException ex)
                        {
                            Console.WriteLine("Key not exist exception caught successfully when section does exist");
                        }
                        catch (IniSectionNotFoundException ex)
                        {
                            Console.WriteLine("Key not exist exception caught successfully when section does not exist");
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

            Console.WriteLine();
            Console.WriteLine("Press any key when ready.....");
            Console.ReadKey();
        }
    }
}
