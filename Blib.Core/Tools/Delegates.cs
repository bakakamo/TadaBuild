using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blib.Tools
{
    public delegate void MethodDelegate();

    public delegate R FunctionDelegate<R>();

    public delegate R OneParamFunctionDelegate<R, A>(A value);

    public delegate R TwoParamFunctionDelegate<R, A, B>(A value1, B value2);

    public delegate R ThreeParamFunctionDelegate<R, A, B, C>(A value1, B value2, C value3);

    public delegate R FourParamFunctionDelegate<R, A, B, C, D>(A value1, B value2, C value3, D value4);

    public delegate bool BooleanFunctionDelegate();

    public delegate void OneParamDelegate<A>(A value);

    public delegate void TwoParamDelegate<A, B>(A value1, B value2);

    public delegate void ThreeParamDelegate<A, B, C>(A value1, B value2, C value3);
}
