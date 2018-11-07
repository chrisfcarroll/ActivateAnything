using System;

namespace TestCases
{

    public interface INterfaceWhoseImplementationTakes3Params<T1,T2,T3>
    {
        Tuple<T1, T2, T3> GetConstructorParams();
    }

    public class ClassWith3ConstructorParams<T1,T2,T3> : INterfaceWhoseImplementationTakes3Params<T1,T2,T3>
    {
        public readonly T1 Param1;
        public readonly T2 Param2;
        public readonly T3 Param3;

        public ClassWith3ConstructorParams(T1 param1, T2 param2, T3 param3)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
        }

        public Tuple<T1, T2, T3> GetConstructorParams()
        {
            return new Tuple<T1, T2, T3>(Param1,Param2,Param3);
        }
    }
}