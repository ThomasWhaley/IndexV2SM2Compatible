using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// This file is part of Model Converter for Space Marine 2
// Copyright (C) 2025 Neo_Kesha
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// See <https://www.gnu.org/licenses/> for more details.

namespace SaberTools
{
    internal static class Utils
    {
        public static void Indent(StreamWriter writer, int indent = 0)
        {
            for (int i = 0; i < indent; ++i)
            {
                Tab(writer);
            }
        }
        public static void Equal(StreamWriter writer)
        {
            Tab(writer);
            writer.Write("=");
            Tab(writer);
        }
        public static void Tab(StreamWriter writer)
        {
            writer.Write("   ");
        }
    }
    internal static class SyntaxChecker
    {
        public static void AssertEquals(string term) { if (term != "=") throw new ArgumentException("Expected equals"); }
        public static void AssertComma(string term) { if (term != ",") throw new ArgumentException("Expected comma"); }
        public static void AssertPropStart(string term) { if (term != "{") throw new ArgumentException("Expected open bracket"); }
        public static void AssertPropEnd(string term) { if (term != "}") throw new ArgumentException("Expected close bracket"); }
        public static void AssertArrayStart(string term) { if (term != "[") throw new ArgumentException("Expected open square bracket"); }
        public static void AssertArrayEnd(string term) { if (term != "]") throw new ArgumentException("Expected close square bracket"); }
        public static void AssertStartQuote(string term) { if (!term.StartsWith("\"")) throw new ArgumentException("Expected starting with quote"); }
        public static void AssertEndQuote(string term) { if (!term.EndsWith("\"")) throw new ArgumentException("Expected ending with quote"); }
    }
    public class PropertySection
    {
        public List<Field> fields = new List<Field>();
        public ValueBase this[string key]
        {
            get
            {
                Field field = fields.Where(f => f.Name == key).FirstOrDefault();
                if (field == null) return null;
                return field.Value;
            }

            set
            {
                Field field = fields.Where(f => f.Name == key).FirstOrDefault();
                if (field == null)
                {
                    field = new Field();
                    field.Name = key;
                    fields.Add(field);
                }
                field.Value = value;
            }
        }
        public static PropertySection Parse(string data)
        {
            var tmp = Regex.Split(TrimData(data), @"([,={}\[\]\n;])");
            var processedTmp = new List<string>();
            tmp = Condense(tmp);

            Queue<string> terms = new Queue<string>(tmp.Where(e => e != "" && e != "\n" && e != ";"));
            return Parse(terms);
        }

        private static string[] Condense(string[] terms)
        {
            var isStr = false;
            var result = new List<string>();
            var tmpStr = "";
            foreach (var term in terms)
            {
                if (term.StartsWith("\"") && term.EndsWith("\""))
                {
                    result.Add(term);
                }
                else if (term.StartsWith("\""))
                {
                    isStr = true;
                    tmpStr = term;
                }
                else if (term.EndsWith("\""))
                {
                    isStr = false;
                    tmpStr += term;
                    result.Add(tmpStr);
                }
                else
                {
                    if (!isStr)
                    {
                        result.Add(term);
                    }
                    else
                    {
                        tmpStr += term;
                    }

                }
            }
            return result.ToArray();
        }
        private static string TrimData(string data)
        {
            StringBuilder sb = new StringBuilder();
            var tokens = data.Split("\"");
            var removeSpaces = true;
            foreach (var token in tokens)
            {
                if (removeSpaces)
                {
                    sb.Append(token.Replace("\r\n", "\n").Replace(" ", ""));
                    sb.Append('"');
                }
                else
                {
                    sb.Append(token.Replace("\r\n", "\n"));
                    sb.Append('"');
                }
                removeSpaces = !removeSpaces;
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static PropertySection Parse(Queue<string> terms)
        {
            var section = new PropertySection();
            while (terms.Count > 0 && terms.Peek() != "}")
            {
                section.fields.Add(Field.Parse(terms));
            }

            return section;
        }

        public void Write(StreamWriter writer, int indent = 0)
        {
            foreach (var field in fields)
            {
                field.Write(writer, indent);
            }
        }

        public void Merge(PropertySection other)
        {
            if (other.fields.Count == 0)
            {
                fields.Clear();
                return;
            }
            foreach (var otherField in other.fields)
            {
                var found = false;
                foreach (var field in fields)
                {
                    if (field.Name == otherField.Name)
                    {
                        field.Merge(otherField);
                        found = true;
                        break;
                    }
                }
                if (!found) fields.Add(otherField);
            }
        }
    }

    public class Field
    {
        public override string ToString()
        {
            return Name;
        }
        public static explicit operator string(Field f) => f.ToString();
        public string Name { get; set; }
        public ValueBase Value { get; set; }
        public static Field Parse(Queue<string> terms)
        {
            var field = new Field();
            var term = terms.Dequeue();

            if (term.StartsWith("\""))
            {
                ValueString tmp = ValueString.Parse(term, terms);
                field.Name = "\"" + tmp.Value + "\"";
            }
            else
            {
                field.Name = term;
            }

            SyntaxChecker.AssertEquals(terms.Dequeue());
            field.Value = ValueBase.Parse(terms);
            return field;
        }

        public void Write(StreamWriter writer, int indent = 0)
        {
            Utils.Indent(writer, indent);
            writer.Write(Name);
            Utils.Equal(writer);
            Value.Write(writer, indent);
            writer.Write("\r\n");
        }

        public void Merge(Field other)
        {
            Value.Merge(other.Value);
        }
    }

    public abstract class ValueBase
    {
        public abstract ValueBase this[string key] { get; set; }
        public static ValueBase Parse(Queue<string> terms)
        {
            var term = terms.Dequeue();
            switch (term)
            {
                case "{":
                    {
                        var value = ValueProperty.ParseProperty(terms);
                        return value;
                    }
                case "[":
                    {
                        var value = ValueArray.ParseArray(terms);
                        return value;
                    }
                default:
                    {
                        var valueDouble = 0.0;
                        var valueInt = 0;
                        var valueBool = false;
                        if (Boolean.TryParse(term, out valueBool))
                        {
                            return new ValueBool(valueBool);
                        }
                        else if (term.Contains(".") && Double.TryParse(term, NumberStyles.Any, CultureInfo.InvariantCulture, out valueDouble))
                        {
                            return new ValueDouble(valueDouble);
                        }
                        else if (Int32.TryParse(term, NumberStyles.Any, CultureInfo.InvariantCulture, out valueInt))
                        {
                            return new ValueInt(valueInt);
                        }
                        else
                        {
                            SyntaxChecker.AssertStartQuote(term);
                            return ValueString.Parse(term, terms);
                        }
                    }
            }
        }

        public virtual void Write(StreamWriter writer, int indent = 0)
        {

        }

        public virtual void Merge(ValueBase other)
        {

        }
    }

    public class ValueProperty : ValueBase
    {
        override public ValueBase this[string key]
        {
            get
            {
                Field field = Value.fields.Where(f => f.Name == key).FirstOrDefault();
                if (field == null) return null;
                return field.Value;
            }

            set
            {
                Field field = Value.fields.Where(f => f.Name == key).FirstOrDefault();
                if (field == null)
                {
                    field = new Field();
                    field.Name = key;
                    Value.fields.Add(field);
                }
                field.Value = value;
            }
        }
        public PropertySection Value { get; set; }
        public ValueProperty()
        {
            Value = PropertySection.Parse("");
        }
        public static ValueProperty ParseProperty(Queue<string> terms)
        {
            var propValue = new ValueProperty();
            propValue.Value = PropertySection.Parse(terms);
            SyntaxChecker.AssertPropEnd(terms.Dequeue());
            return propValue;
        }

        public override void Write(StreamWriter writer, int indent = 0)
        {
            writer.Write("{\r\n");
            Value.Write(writer, indent + 1);
            Utils.Indent(writer, indent);
            writer.Write("}");
        }
        public override void Merge(ValueBase other)
        {
            var otherProperty = (ValueProperty)other;
            Value.Merge(otherProperty.Value);
        }
    }
    public class ValueBool : ValueBase
    {
        override public ValueBase this[string key]
        {
            get
            {
                return this;
            }

            set
            {
                this.Value = ((ValueBool)value).Value;
            }
        }
        public override string ToString()
        {
            return Value.ToString();
        }
        public static explicit operator string(ValueBool f) => f.ToString();
        public bool Value { get; set; }
        public ValueBool(bool value)
        {
            Value = value;
        }

        public override void Write(StreamWriter writer, int indent = 0)
        {
            writer.Write((Value) ? "True" : "False");
        }

        public override void Merge(ValueBase other)
        {
            var otherBool = (ValueBool)other;
            Value = otherBool.Value;
        }
    }
    public class ValueDouble : ValueBase
    {
        override public ValueBase this[string key]
        {
            get
            {
                return this;
            }

            set
            {
                this.Value = ((ValueDouble)value).Value;
            }
        }
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
        public static explicit operator string(ValueDouble f) => f.ToString();
        public double Value { get; set; }
        public ValueDouble(double value)
        {
            Value = value;
        }

        public override void Write(StreamWriter writer, int indent = 0)
        {
            writer.Write(Value.ToString("0.0###########", CultureInfo.InvariantCulture));
        }

        public override void Merge(ValueBase other)
        {
            var otherDouble = (ValueDouble)other;
            Value = otherDouble.Value;
        }
    }
    public class ValueInt : ValueBase
    {
        override public ValueBase this[string key]
        {
            get
            {
                return this;
            }

            set
            {
                this.Value = ((ValueInt)value).Value;
            }
        }
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
        public static explicit operator string(ValueInt f) => f.ToString();
        public int Value { get; set; }
        public ValueInt(int value)
        {
            Value = value;
        }

        public override void Write(StreamWriter writer, int indent = 0)
        {
            writer.Write(Value.ToString());
        }

        public override void Merge(ValueBase other)
        {
            var otherDouble = (ValueInt)other;
            Value = otherDouble.Value;
        }
    }
    public class ValueString : ValueBase
    {
        override public ValueBase this[string key]
        {
            get
            {
                return this;
            }

            set
            {
                this.Value = ((ValueString)value).Value;
            }
        }
        public override string ToString()
        {
            return Value;
        }
        public ValueString() { Value = ""; }
        public ValueString(string value)
        {
            Value = value;
        }

        public static explicit operator string(ValueString f) => f.Value;
        public string Value { get; set; } = "";
        public static ValueString Parse(string firstTerm, Queue<string> terms)
        {
            var str = new ValueString();
            str.Value = firstTerm.Replace("\"", "");
            if (firstTerm != "\"" && firstTerm.StartsWith("\"") && firstTerm.EndsWith("\""))
            {
                return str;
            }
            while (true)
            {
                var term = terms.Dequeue();
                str.Value += term.Replace("\"", "");
                if (term.EndsWith("\""))
                {
                    break;
                }
            }
            return str;
        }

        public override void Write(StreamWriter writer, int indent = 0)
        {
            writer.Write($"\"{Value}\"");
        }

        public override void Merge(ValueBase other)
        {
            var otherString = (ValueString)other;
            Value = otherString.Value;
        }
    }
    public class ValueArray : ValueBase
    {
        override public ValueBase this[string key]
        {
            get
            {
                var idx = Int32.Parse(key);
                if (Value.Count <= idx) return null;
                return Value[Int32.Parse(key)];
            }

            set
            {
                var idx = Int32.Parse(key);
                while (Value.Count < idx)
                {
                    Value.Add(value);
                }
                Value[idx] = value;
            }
        }
        public ValueArray() { }
        public ValueArray(List<ValueBase> value)
        {
            Value = value;
        }

        public List<ValueBase> Value { get; set; } = new List<ValueBase>();
        public static ValueArray ParseArray(Queue<string> terms)
        {
            var array = new ValueArray();
            if (terms.Peek() == "]")
            {
                terms.Dequeue();
                return array;
            }
            while (true)
            {
                array.Value.Add(ValueBase.Parse(terms));
                var term = terms.Dequeue();
                if (term == "]")
                {
                    break;
                }
                else
                {
                    SyntaxChecker.AssertComma(term);
                }
            }
            return array;
        }

        public override void Write(StreamWriter writer, int indent = 0)
        {
            writer.Write("[\r\n");
            int idx = 0;
            foreach (var value in Value)
            {
                Utils.Indent(writer, indent + 1);
                value.Write(writer, indent + 1);
                ++idx;
                if (idx < Value.Count)
                {
                    writer.Write(",");
                }
                writer.Write("\r\n");
            }
            Utils.Indent(writer, indent);
            writer.Write("]");
        }

        public override void Merge(ValueBase other)
        {
            var otherArray = (ValueArray)other;
            Value = otherArray.Value;
        }
    }
}
