using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TestService
/// </summary>
public class TestService : ITest
{
    public TestService()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string getHello()
    {
        return "Hello World";
    }

    public string anotherHello()
    {
        return "Another one";
    }
}