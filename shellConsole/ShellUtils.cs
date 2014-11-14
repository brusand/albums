/*
 * Created by SharpDevelop.
 * User: EBDU6461
 * Date: 04/11/2014
 * Time: 18:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace ShellUtils
{
	/// <summary>
	/// Description of Class1.
	/// </summary>

	
	public class Utils
	{
		public static string MakeRelativePath(string fromPath, string toPath)
		{
		    // use Path.GetFullPath to canonicalise the paths (deal with multiple directory seperators, etc)
		    if (String.Compare(fromPath, toPath, true) == 0 ) {
		    	return "";
		    }
		    else return Path.GetFullPath(toPath).Substring(Path.GetFullPath(fromPath).Length + 1);
		}
	}
}
