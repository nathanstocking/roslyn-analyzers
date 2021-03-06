﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Test.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.NetCore.Analyzers.Security.UnitTests
{
    public class DoNotHardCodeEncryptionKeyTests : TaintedDataAnalyzerTestBase
    {
        public DoNotHardCodeEncryptionKeyTests(ITestOutputHelper output)
            : base(output)
        {
        }

        protected override DiagnosticDescriptor Rule => DoNotHardCodeEncryptionKey.Rule;

        [Fact]
        public void Test_HardcodedInString_CreateEncryptor_NeedValueContentAnalysis_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] key = Convert.FromBase64String(""AAAAAaazaoensuth"");
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(key, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(11, 9, 9, 22, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[] Convert.FromBase64String(string s)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInStringWithVariable_CreateEncryptor_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        string someHardCodedBase64String = ""AAAAAaazaoensuth"";
        byte[] key = Convert.FromBase64String(someHardCodedBase64String);
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(key, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(12, 9, 10, 22, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[] Convert.FromBase64String(string s)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInMultilinesString_CreateEncryptor_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        string someHardCodedBase64String = ""sssdsdsdsdsdsds"" +
                                          ""sdasdsadasddsda""  + 
                                          ""sdasdsadasddsda"" ;
        byte[] key = Convert.FromBase64String(someHardCodedBase64String);
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(key, someOtherBytesForIV);

    }
}",
            GetCSharpResultAt(14, 9, 12, 22, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[] Convert.FromBase64String(string s)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInByteArray_CreateEncryptor_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] rgbKey = new byte[] {1, 2, 3};
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(rgbKey, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(11, 9, 9, 25, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInByteArray_CreateDecryptor_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] rgbKey = new byte[] {1, 2, 3};
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateDecryptor(rgbKey, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(11, 9, 9, 25, "ICryptoTransform SymmetricAlgorithm.CreateDecryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInByteArrayWithVariable_CreateEncryptor_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte b = 1;
        byte[] rgbKey = new byte[] {b};
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(rgbKey, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(12, 9, 10, 25, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInByteArray_KeyProperty_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] rgbKey = new byte[] {1, 2, 3};
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.Key = rgbKey;
    }
}",
            GetCSharpResultAt(11, 9, 9, 25, "byte[] SymmetricAlgorithm.Key", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInByteArray_CreateEncryptorFromDerivedClassOfSymmetricAlgorithm_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] rgbKey = new byte[] {1, 2, 3};
        Aes aes = Aes.Create();
        aes.CreateEncryptor(rgbKey, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(11, 9, 9, 25, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInByteArray_CreateEncryptor_Multivalues_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] rgbKey = new byte[] {1, 2, 3};
        Random r = new Random();

        if (r.Next(6) == 4)
        {
            rgbKey = new byte[] {4, 5, 6};
        }

        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(rgbKey, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(18, 9, 14, 22, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"),
            GetCSharpResultAt(18, 9, 9, 25, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_HardcodedInByteArray_CreateEncryptor_WithoutAssignment_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(new byte[] {1, 2, 3}, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(10, 9, 10, 30, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_MaybeHardcoded_CreateEncryptor_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV, byte[] rgbKey)
    {
        Random r = new Random();

        if (r.Next(6) == 4)
        {
            rgbKey = new byte[] {4, 5, 6};
        }

        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(rgbKey, someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(17, 9, 13, 22, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV, byte[] rgbKey)", "byte[]", "void TestClass.TestMethod(byte[] someOtherBytesForIV, byte[] rgbKey)"));
        }

        [Fact]
        public void Test_PassTaintedSourceInfoAsParameter_SinkMethodParameters_Interprocedual_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] key = Convert.FromBase64String(""AAAAAaazaoensuth"");
        CreateEncryptor(key, someOtherBytesForIV);
    }

    public void CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
    {
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(rgbKey, rgbIV);
    }
}",
            GetCSharpResultAt(16, 9, 9, 22, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "byte[] Convert.FromBase64String(string s)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_PassTaintedSourceInfoAsParameter_SinkProperties_Interprocedual_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod()
    {
        byte[] key = Convert.FromBase64String(""AAAAAaazaoensuth"");
        CreateEncryptor(key);
    }

    public void CreateEncryptor(byte[] rgbKey)
    {
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.Key = rgbKey;
    }
}",
            GetCSharpResultAt(16, 9, 9, 22, "byte[] SymmetricAlgorithm.Key", "void TestClass.CreateEncryptor(byte[] rgbKey)", "byte[] Convert.FromBase64String(string s)", "void TestClass.TestMethod()"));
        }

        [Fact]
        public void Test_HardcodedIn2DByteArray_CreateEncryptor_Diagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Linq;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[,] rgbKey = new byte[,] { { 1, 2, 3 }, { 4, 5, 6 } };
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(rgbKey.Cast<byte>().ToArray(), someOtherBytesForIV);
    }
}",
            GetCSharpResultAt(12, 9, 10, 26, "ICryptoTransform SymmetricAlgorithm.CreateEncryptor(byte[] rgbKey, byte[] rgbIV)", "void TestClass.TestMethod(byte[] someOtherBytesForIV)", "byte[,]", "void TestClass.TestMethod(byte[] someOtherBytesForIV)"));
        }

        [Fact]
        public void Test_NotHardcoded_CreateEncryptor_NoDiagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV, byte[] rgbKey)
    {
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(rgbKey, someOtherBytesForIV);
    }
}");
        }

        [Fact]
        public void Test_HardcodedInArrayThenOverwrite_NoDiagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV, byte[] key)
    {
        byte[] rgbKey = new byte[] {1, 2, 3};
        rgbKey = key;
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(rgbKey, someOtherBytesForIV);
    }
}");
        }

        [Fact]
        public void Test_NotHardcodedInString_CreateEncryptor_NoDiagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] key = Convert.FromBase64String(Console.ReadLine());
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(key, someOtherBytesForIV);
    }
}");
        }

        // For now, it doesn't support checking return tainted source info.
        [Fact]
        public void Test_ReturnTaintedSourceInfo_Interprocedual_NoDiagnostic()
        {
            VerifyCSharpWithDependencies(@"
using System;
using System.Security.Cryptography;

class TestClass
{
    public byte[] GetKey()
    {
        return Convert.FromBase64String(""AAAAAaazaoensuth"");
    }

    public void TestMethod(byte[] someOtherBytesForIV)
    {
        byte[] key = GetKey();
        SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
        rijn.CreateEncryptor(key, someOtherBytesForIV);
    }
}");
        }

        [Fact, WorkItem(2723, "https://github.com/dotnet/roslyn-analyzers/issues/2723")]
        public void Test_ArrayInitializerInAttribute()
        {
            VerifyCSharpWithDependencies(@"
using System;

class MyAttr : Attribute
{
    public MyAttr (byte[] array) { }
}

[MyAttr(new byte[]{ 1 })]
class C
{
}");
        }

        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return new DoNotHardCodeEncryptionKey();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DoNotHardCodeEncryptionKey();
        }
    }
}
