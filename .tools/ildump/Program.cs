using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// Usage: ildump <assembly> <TypeFullName> [MemberName]
//   Dumps signatures of a type, or the IL of every method whose name contains MemberName.
class Program
{
    static void Main(string[] args)
    {
        var asm = AssemblyDefinition.ReadAssembly(args[0]);

        // Search mode: ildump <asm> --search <operandSubstring>
        if (args.Length >= 3 && args[1] == "--search")
        {
            string needle = args[2];
            foreach (var type in asm.MainModule.Types.SelectMany(AllTypes))
                foreach (var m in type.Methods.Where(m => m.HasBody))
                {
                    foreach (var i in m.Body.Instructions)
                    {
                        string op = i.Operand?.ToString() ?? "";
                        bool hit = op.Contains(needle, StringComparison.OrdinalIgnoreCase)
                                   || (i.Operand is int n && n.ToString() == needle);
                        if (hit)
                        {
                            Console.WriteLine($"{type.FullName}::{m.Name}  [{i.Offset:x4} {i.OpCode.Name} {op}]");
                            break;
                        }
                    }
                }
            return;
        }

        var all = asm.MainModule.Types.SelectMany(AllTypes).ToList();
        var t = all.FirstOrDefault(x => x.FullName == args[1])
             ?? all.FirstOrDefault(x => x.Name == args[1]);
        if (t == null) { Console.WriteLine("type not found: " + args[1]); return; }

        if (args.Length < 3)
        {
            DumpType(t, 0);
            return;
        }

        foreach (var m in t.Methods.Where(m => m.Name.Contains(args[2], StringComparison.OrdinalIgnoreCase)))
            DumpMethod(m);
    }

    static IEnumerable<TypeDefinition> AllTypes(TypeDefinition t)
    {
        yield return t;
        foreach (var n in t.NestedTypes)
            foreach (var x in AllTypes(n)) yield return x;
    }

    static void DumpType(TypeDefinition t, int depth)
    {
        string pad = new string(' ', depth * 2);
        Console.WriteLine($"{pad}type {t.FullName} : {t.BaseType?.FullName}");
        foreach (var f in t.Fields.Where(f => !f.IsPrivate || depth == 0))
            Console.WriteLine($"{pad}  field {f.FieldType.Name} {f.Name}" + (f.HasConstant ? $" = {f.Constant}" : ""));
        foreach (var p in t.Properties)
            Console.WriteLine($"{pad}  prop {p.PropertyType.Name} {p.Name}");
        foreach (var m in t.Methods.Where(m => m.IsPublic || m.IsFamily || m.Name.Contains("AI")))
            Console.WriteLine($"{pad}  method {m.ReturnType.Name} {m.Name}({string.Join(", ", m.Parameters.Select(p => p.ParameterType.Name + " " + p.Name))})");
    }

    static void DumpMethod(MethodDefinition m)
    {
        Console.WriteLine("=== " + m.FullName);
        foreach (var p in m.Parameters)
            Console.WriteLine($"  param {p.ParameterType.FullName} {p.Name}");
        if (!m.HasBody) { Console.WriteLine("  <no body>"); return; }
        foreach (var v in m.Body.Variables)
            Console.WriteLine($"  local {v.VariableType.Name} V_{v.Index}");
        foreach (var i in m.Body.Instructions)
        {
            string op = i.OpCode.Name;
            string operand;
            switch (i.Operand)
            {
                case null: operand = ""; break;
                case Instruction target: operand = "IL_" + target.Offset.ToString("x4"); break;
                case Instruction[] targets: operand = string.Join(", ", targets.Select(x => "IL_" + x.Offset.ToString("x4"))); break;
                case MethodReference mr: operand = mr.DeclaringType.Name + "::" + mr.Name; break;
                case FieldReference fr: operand = fr.DeclaringType.Name + "::" + fr.Name; break;
                case string s: operand = "\"" + s + "\""; break;
                default: operand = i.Operand.ToString(); break;
            }
            Console.WriteLine($"  IL_{i.Offset:x4}: {op,-12} {operand}");
        }
    }
}

