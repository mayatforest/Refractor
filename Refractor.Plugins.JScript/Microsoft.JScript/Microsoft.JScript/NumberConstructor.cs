//
// NumberConstructor.cs:
//
// Author: Cesar Octavio Lopez Nataren
//
// (C) 2003, Cesar Octavio Lopez Nataren, <cesar@ciencias.unam.mx>
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace Microsoft.JScript {

	public class NumberConstructor : ScriptFunction {

		internal static NumberConstructor Ctr = new NumberConstructor ();

		internal NumberConstructor ()
		{
			_prototype = NumberPrototype.Proto;
			_length = 1;
			name = "Number";
		}

		public const double MAX_VALUE = Double.MaxValue;
		public const double MIN_VALUE = Double.Epsilon;
		public const double NaN = Double.NaN;
		public const double NEGATIVE_INFINITY = Double.NegativeInfinity;
		public const double POSITIVE_INFINITY = Double.PositiveInfinity;
		
		[JSFunctionAttribute (JSFunctionAttributeEnum.HasVarArgs)]
		public new NumberObject CreateInstance (params Object [] args)
		{
			double value = 0;

			if (args.Length > 0)
				value = Convert.ToNumber (args [0]);

			return new NumberObject (value);
		}

		public double Invoke (object arg)
		{
			if (arg is Object []) {
				Object [] args = (Object []) arg;
				if (args.Length == 0)
					return 0;
				else
					return Convert.ToNumber (args [0]);
			}

			return Convert.ToNumber (arg);
		}
	}
}
