/*
 * Created by SharpDevelop.
 * User: V.Seminchenko
 * Date: 19.01.2015
 * Time: 13:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SharpUtils
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class MathExt
	{
		public MathExt()
		{
		}
		public static int Div(int a, int b)
		{
			return (int)(a / b);
		}
		public static int Mod(int a, int b)
		{
			return (int)(a % b);
		}
		public static UInt32 StrToDWORD(String str)
		{
			
			UInt32 d, d1;
			char ch;
			int l;
			l = str.Length;
			d1 = 1;
			d = 0;
			for (int i = 0; i < l; i++) {
				ch = str[i];
				d = d + d1 * Convert.ToByte(ch);
				if (i<3){ d1 = d1 * 256;
				}
			}
			return d;
		}
	}
}
