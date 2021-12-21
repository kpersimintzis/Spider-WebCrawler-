using System;
using Xunit;
using FsCheck.Xunit;
using Spider;
using Graph;
using Graph.Models;
using System.Collections.Generic;
using Walk;
using System.Diagnostics;

namespace Test
{
    public class UnitTest
    {
        //[Property(MaxTest = 5000)]
        //public bool TestCommutativity(int a, int b)
        //{
        //    Debug.WriteLine($"{a} {b}");
        //    return (a + b == b + a);
        //}

        
        public bool TestDfs(Tuple<int,int>[] edges)
        {
            return true;
        }
    }
}
