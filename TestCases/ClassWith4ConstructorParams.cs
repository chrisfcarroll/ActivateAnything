using System;

namespace TestCases
{
    public class ClassWith4ConstructorParams<T1, T2, T3, T4>
    {
        public readonly T1 Param1;
        public readonly T2 Param2;
        public readonly T3 Param3;
        public readonly T4 Param4;

        public ClassWith4ConstructorParams(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
        }

        public Tuple<T1, T2, T3> GetConstructorParams()
        {
            return new Tuple<T1, T2, T3>(Param1, Param2, Param3);
        }
    }
}