using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

public class ScriptRunner
{
    private string userScriptCode = "";      // stores the user supplied code
    private string genCodeHeader = "";       // header code the class generates
    private string genCodeFooter = "";       // footer code the class generates

    // a hashtable containing the name/value pairs for input variables
    private Hashtable scriptInputs = new Hashtable();

    /// <summary>
    /// Sets the VBScript that will be used by the scriping engine.
    /// </summary>
    /// <param name="scriptCode">a string containing the VBScript to use.</param>
    public void setScript(string scriptCode)
    {
        userScriptCode = scriptCode;
    } // setScript()

    /// <summary>
    /// Sets the VBScript that will be used by the scripting engine.
    /// </summary>
    /// <param name="VBScriptPath">The path to a VBScript file</param>
    public void setScriptFromFile(string VBScriptPath)
    {
        if (!File.Exists(VBScriptPath)) { return; }
        try
        {
            string scriptCode = File.ReadAllText(VBScriptPath);
            setScript(scriptCode);
        }
        catch
        {
            return;
        }
    } // setScriptFromFile()

    /// <summary>
    /// Returns the currently defined VBScript
    /// </summary>
    /// <returns>The currently defined VBScript</returns>
    public string getScript()
    {
        return userScriptCode;
    } // getScript()

    /// <summary>
    /// Allows the user to supply a hash with all of the name value pairs
    /// for input to the script
    /// </summary>
    /// <param name="inputs">a table containing name value pairs for variables</param>
    public void setScriptInputs(Hashtable inputs)
    {
        foreach (object key in inputs.Keys)
        {
            scriptInputs.Add(key, inputs[key]);
        }
    } // addScriptInputs()

    /// <summary>
    /// Sets a variable for input to the script
    /// </summary>
    /// <param name="varName">a string containing the variable name</param>
    /// <param name="varValue">the variable value</param>
    public void addScriptInput(string varName, object varValue)
    {
        if (!scriptInputs.ContainsKey(varName))
        {
            scriptInputs.Add(varName, varValue);
        }
    } // addScriptInput()

    /// <summary>
    /// returns a hash containing all of the defined script inputs
    /// </summary>
    /// <returns>the script inputs</returns>
    public Hashtable getInputs()
    {
        return scriptInputs;
    } // getInputs()

    /// <summary>
    /// removes all of the script inputs
    /// </summary>
    public void clearScriptInputs()
    {
        scriptInputs.Clear();
    } // clearScriptInputs()

    /// <summary>
    /// determines whether a variable is numeric or not
    /// </summary>
    /// <param name="toCheck">a variable to examine</param>
    /// <returns>true for numeric, false for non-numeric</returns>
    private bool isNumeric(object toCheck)
    {
        if (toCheck == null) { return false; }
        string check = toCheck.ToString();
        if (Regex.IsMatch(toCheck.ToString(), @"^-?[\d.]+$"))
        {
            return true;
        }
        else
        {
            return false;
        }
    } // isNumeric()

    /// <summary>
    /// Will take VBScript input and remove any user defined functions
    /// that conflict with the mScript required functions
    /// </summary>
    /// <param name="scriptCode">VBScript code</param>
    /// <returns>The cleaned VBScript code</returns>
    private string cleanUserCode(string scriptCode)
    {
        string workCode = scriptCode;
        string inpVal_re = @"Function\s+inpVal\s*?\([^)]+\)(.*?)End\s+Function";
        string iv_re = @"Function\s+iv\s*?\([^)]+\)(.*?)End\s+Function";
        string return_re = @"Sub\s+return\s*?\([^)]+\)(.*?)End\s+Sub";
        Regex re;

        // remove conflicting "inpVal" functions
        re = new Regex(inpVal_re, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (re.IsMatch(workCode))
        {
            workCode = re.Replace(workCode, "");
        }

        // remove conflicting "iv" functions
        re = new Regex(iv_re, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (re.IsMatch(workCode))
        {
            workCode = re.Replace(workCode, "");
        }

        // remove conflicting "return" subroutine
        re = new Regex(return_re, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (re.IsMatch(workCode))
        {
            workCode = re.Replace(workCode, "");
        }

        return workCode;
    } // cleanUserCode()

    /// <summary>
    /// Executes the user defined script code using the user supplied script inputs
    /// </summary>
    /// <returns>A hash containing the script's return values keyed on variable name</returns>
    public Hashtable runScript()
    {
        string retVals = "";  // the value 
        string varName = "";  // input variable name
        string varVal = "";   // input variable value

        // a unique timestamp to be used for both the script filename and the return value file
        string tStamp = String.Format("{0:00}", DateTime.Now.Hour) + "-" + String.Format("{0:00}", DateTime.Now.Minute) + "-" + String.Format("{0:00}", DateTime.Now.Second) + "-" + String.Format("{0:00000}", DateTime.Now.Millisecond);

        string outFile = tStamp + ".txt";
        string scriptFile = tStamp + ".vbs";
        string scriptVal = "";

        // reset this each time runScript() is called
        genCodeHeader = "";
        genCodeFooter = "";


        // Build the supporting code in the script header
        genCodeHeader += "'*************************************************\r\n";
        genCodeHeader += "'* VBScript generated by the ScriptRunner class  *\r\n";
        genCodeHeader += "'* for dynamic scripting of .NET applications    *\r\n";
        genCodeHeader += "'* this file can be safely deleted.              *\r\n";
        genCodeHeader += "'*                                               *\r\n";
        genCodeHeader += "'* ScriptRunner was created by Don Smith         *\r\n";
        genCodeHeader += "'* <don.c.smith@gmail.com>                       *\r\n";
        genCodeHeader += "'*************************************************\r\n\r\n";

        genCodeHeader += "outFile = \"" + outFile + "\"\r\n\r\n";

        // get our file I/O initialized.  This assumes we have write 
        // access to the current directory.
        genCodeHeader += "Set oFsObj = CreateObject(\"Scripting.FileSystemObject\")\r\n";
        genCodeHeader += "Set oFHnd = oFsObj.CreateTextFile(outFile, true)\r\n\r\n";

        // a wrapper function with a shorter name to access the supplied inputs
        genCodeHeader += "Function iv(variableName)\r\n";
        genCodeHeader += "  iv = inpVal(variableName)\r\n";
        genCodeHeader += "End Function ' iv()\r\n\r\n";

        // a function to allow the user script to access supplied inputs
        genCodeHeader += "Function inpVal(variableName)\r\n";
        genCodeHeader += "  Select Case UCase(variableName)\r\n";

        /* This takes all of the script inputs    *\
        |* and places them inside of a VBScript   *|
        |* Select Case statement in the inpVal()  *|
        |* function so the supplied VBScript can  *|
        \* retrieve the values by name.           */
        foreach (object key in scriptInputs.Keys)
        {
            if (isNumeric(scriptInputs[key]))
            { // numbers will not be quoted
                varVal = scriptInputs[key].ToString();
            }
            else
            {
                if (scriptInputs[key] == null)
                {
                    scriptVal = ""; // nulls become zero length strings
                }
                else
                {
                    scriptVal = scriptInputs[key].ToString();
                }
                // quote strings with existing appropriately quotes escaped
                varVal = "\"" + scriptVal.Replace("\"", "\"\"") + "\"";
            }

            // convert newlines appropriately
            while (varVal.Contains("\r\n"))
            {
                varVal = varVal.Replace("\r\n", "\" & vbNewLine & \"");
            }

            varName = "\"" + key.ToString().ToUpper().Replace("\"", "\"\"") + "\"";
            genCodeHeader += "    Case " + varName + "\r\n";
            genCodeHeader += "      retVal = " + varVal + "\r\n";
        } // foreach input

        // finish the case/function
        genCodeHeader += "    Case Else\r\n";
        genCodeHeader += "      retVal = \"\"\r\n";
        genCodeHeader += "  End Select\r\n";
        genCodeHeader += "  inpVal = retVal\r\n";
        genCodeHeader += "End Function ' inpVal()\r\n\r\n";

        // subroutine allowing the script to return a variable
        genCodeHeader += "Sub return(varName, retVal)\r\n";
        genCodeHeader += "  While InStr(1, retVal, vbNewLine, vbTextCompare) >= 1\r\n";
        genCodeHeader += "    retVal = replace(retVal, vbNewLine, \"\\n\")\r\n";
        genCodeHeader += "  Wend\r\n";
        genCodeHeader += "  oFHnd.WriteLine varName & vbTab & retVal\r\n";
        genCodeHeader += "End Sub ' return()\r\n\r\n";

        // close our output file handle
        genCodeFooter += "\r\n\r\noFHnd.Close\r\n";

        string suppliedCode = cleanUserCode(userScriptCode);

        // build the complete script and output it
        string allCode = genCodeHeader + suppliedCode + genCodeFooter;
        StreamWriter sw = File.CreateText(scriptFile);
        sw.Write(allCode);
        sw.Flush();
        sw.Close();

        //Create a new process info structure.
        ProcessStartInfo pInfo = new ProcessStartInfo();
        //Set the file name member of the process info structure.
        pInfo.FileName = "wscript.exe";
        pInfo.Arguments = scriptFile;
        //Start the process.
        Process p = Process.Start(pInfo);
        //Wait for the window to finish loading.
        p.WaitForInputIdle();
        //Wait for the process to end (or 60 seconds to elapse, whichever occurs first)
        p.WaitForExit(60000);


        // Assuming everything went OK and we have
        // output values, load it.
        if (File.Exists(outFile))
        {
            StreamReader sr = File.OpenText(outFile);
            retVals = sr.ReadToEnd();
            sr.Close();
        }
        else
        { // no output, something is amiss
            retVals = "";
        }

        // cleanup
        try
        {
            File.Delete(outFile);
        }
        catch
        {
            // Ack! something has gone wrong!
        }

        // cleanup
        try
        {
            File.Delete(scriptFile);
        }
        catch
        {
            // Ack! something has gone wrong!
        }

        // Split the data on line breaks.  .NET is borking about using strings
        // for splitting although there is clearly an override to the method to
        // allow it.  we convert the cr/lf sequence to lf only and then split on
        // that as a workaround.
        retVals = retVals.Replace("\r\n", "\n");
        string[] lines = retVals.Split('\n');


        string[] nvp = new string[1];           // this will hold a name/value pair
        Hashtable toReturn = new Hashtable();   // initialize our return values hash

        // go through each line of output data
        foreach (string line in lines)
        {
            // a valid name value pair is at least 3 bytes long 
            // and contains a space
            if (line.Length >= 3 && line.Contains("\t"))
            {
                nvp = line.Split('\t');
                if (!toReturn.ContainsKey(nvp[0]))
                { // if this variable wasn't already defined, set it
                    while (nvp[1].Contains("\\n")) { nvp[1] = nvp[1].Replace("\\n", "\r\n"); }
                    toReturn.Add(nvp[0], nvp[1]);
                }
            }
        }

        // returnt the hash
        return toReturn;
    } // runScript();

}