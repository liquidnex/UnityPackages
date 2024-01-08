using System;
using UnityEngine;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// Basic CSV Unit.
    /// </summary>
    public class CSVElem
    {
        /// <summary>
        /// Type of CSV element.
        /// </summary>
        public enum Type
        {
            VOID,
            BOOL,
            CHAR,
            BYTE,
            SBYTE,
            USHORT,
            SHORT,
            UINT,
            INT,
            ULONG,
            LONG,
            FLOAT,
            DOUBLE,
            DECIMAL,
            STRING
        }

        /// <summary>
        /// Construct a CSV element from the initial string.
        /// </summary>
        /// <param name="valueStr">Inital value for CSV Elem.</param>
        public CSVElem(string valueStr)
        {
            value = valueStr;
        }

        /// <summary>
        /// Construct an empty CSV element.
        /// </summary>
        public CSVElem() { }

        /// <summary>
        /// The CSV element is interpreted as a Boolean value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        /// <param name="e"></param>
        public static implicit operator bool(CSVElem e)
        {
            return e.ExplainAsBool();
        }

        /// <summary>
        /// The CSV element is interpreted as a boolean value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator char(CSVElem e)
        {
            return e.ExplainAsChar();
        }

        /// <summary>
        /// The CSV element is interpreted as a byte value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator byte(CSVElem e)
        {
            return e.ExplainAsByte();
        }

        /// <summary>
        /// The CSV element is interpreted as a sbyte value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator sbyte(CSVElem e)
        {
            return e.ExplainAsSByte();
        }

        /// <summary>
        /// The CSV element is interpreted as a ushort value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator ushort(CSVElem e)
        {
            return e.ExplainAsUShort();
        }

        /// <summary>
        /// The CSV element is interpreted as a short value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator short(CSVElem e)
        {
            return e.ExplainAsShort();
        }

        /// <summary>
        /// The CSV element is interpreted as a uint value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator uint(CSVElem e)
        {
            return e.ExplainAsUInt();
        }

        /// <summary>
        /// The CSV element is interpreted as a int value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator int(CSVElem e)
        {
            return e.ExplainAsInt();
        }

        /// <summary>
        /// The CSV element is interpreted as a ulong value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator ulong(CSVElem e)
        {
            return e.ExplainAsULong();
        }

        /// <summary>
        /// The CSV element is interpreted as a long value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator long(CSVElem e)
        {
            return e.ExplainAsLong();
        }

        /// <summary>
        /// The CSV element is interpreted as a float value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator float(CSVElem e)
        {
            return e.ExplainAsFloat();
        }

        /// <summary>
        /// The CSV element is interpreted as a double value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator double(CSVElem e)
        {
            return e.ExplainAsDouble();
        }

        /// <summary>
        /// The CSV element is interpreted as a decimal value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        public static implicit operator decimal(CSVElem e)
        {
            return e.ExplainAsDecimal();
        }

        /// <summary>
        /// The CSV element is interpreted as a string value.
        /// Converting CSV element to a string is always legal.
        /// </summary>
        public static implicit operator string(CSVElem e)
        {
            return e.ExplainAsString();
        }

        /// <summary>
        /// Convert a boolean value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(bool b)
        {
            return new CSVElem(b.ToString());
        }

        /// <summary>
        /// Convert a char value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(char c)
        {
            return new CSVElem(c.ToString());
        }

        /// <summary>
        /// Convert a byte value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(byte b)
        {
            return new CSVElem(b.ToString());
        }

        /// <summary>
        /// Convert a sbyte value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(sbyte sb)
        {
            return new CSVElem(sb.ToString());
        }

        /// <summary>
        /// Convert a ushort value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(ushort us)
        {
            return new CSVElem(us.ToString());
        }

        /// <summary>
        /// Convert a short value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(short s)
        {
            return new CSVElem(s.ToString());
        }

        /// <summary>
        /// Convert a uint value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(uint ui)
        {
            return new CSVElem(ui.ToString());
        }

        /// <summary>
        /// Convert a int value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(int i)
        {
            return new CSVElem(i.ToString());
        }

        /// <summary>
        /// Convert a ulong value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(ulong ul)
        {
            return new CSVElem(ul.ToString());
        }

        /// <summary>
        /// Convert a long value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(long l)
        {
            return new CSVElem(l.ToString());
        }

        /// <summary>
        /// Convert a float value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(float f)
        {
            return new CSVElem(f.ToString());
        }

        /// <summary>
        /// Convert a double value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(double d)
        {
            return new CSVElem(d.ToString());
        }

        /// <summary>
        /// Convert a decimal value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(decimal d)
        {
            return new CSVElem(d.ToString());
        }

        /// <summary>
        /// Convert a string value to CSV element.
        /// </summary>
        public static implicit operator CSVElem(string s)
        {
            return new CSVElem(s.ToString());
        }

        /// <summary>
        /// The CSV element is interpreted as a boolean value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private bool ExplainAsBool()
        {
            bool b;
            Explain(out b);
            return b;
        }

        /// <summary>
        /// The CSV element is interpreted as a char value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private char ExplainAsChar()
        {
            char c;
            Explain(out c);
            return c;
        }

        /// <summary>
        /// The CSV element is interpreted as a byte value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private byte ExplainAsByte()
        {
            byte b;
            Explain(out b);
            return b;
        }

        /// <summary>
        /// The CSV element is interpreted as a sbyte value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private sbyte ExplainAsSByte()
        {
            sbyte sb;
            Explain(out sb);
            return sb;
        }

        /// <summary>
        /// The CSV element is interpreted as a ushort value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private ushort ExplainAsUShort()
        {
            ushort us;
            Explain(out us);
            return us;
        }

        /// <summary>
        /// The CSV element is interpreted as a short value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private short ExplainAsShort()
        {
            short s;
            Explain(out s);
            return s;
        }

        /// <summary>
        /// The CSV element is interpreted as a uint value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private uint ExplainAsUInt()
        {
            uint ui;
            Explain(out ui);
            return ui;
        }

        /// <summary>
        /// The CSV element is interpreted as a int value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private int ExplainAsInt()
        {
            int i;
            Explain(out i);
            return i;
        }

        /// <summary>
        /// The CSV element is interpreted as a ulong value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private ulong ExplainAsULong()
        {
            ulong ul;
            Explain(out ul);
            return ul;
        }

        /// <summary>
        /// The CSV element is interpreted as a long value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private long ExplainAsLong()
        {
            long l;
            Explain(out l);
            return l;
        }

        /// <summary>
        /// The CSV element is interpreted as a float value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private float ExplainAsFloat()
        {
            float f;
            Explain(out f);
            return f;
        }

        /// <summary>
        /// The CSV element is interpreted as a double value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private double ExplainAsDouble()
        {
            double d;
            Explain(out d);
            return d;
        }

        /// <summary>
        /// The CSV element is interpreted as a decimal value.
        /// If it fails, no error will be reported, and only the error log will be output.
        /// </summary>
        private decimal ExplainAsDecimal()
        {
            decimal dc;
            Explain(out dc);
            return dc;
        }

        /// <summary>
        /// The CSV element is interpreted as a string value.
        /// Converting CSV element to a string is always legal.
        /// </summary>
        private string ExplainAsString()
        {
            return value;
        }

        /// <summary>
        /// Set a specific value for CSV element.
        /// </summary>
        /// <param name="v">New value.</param>
        public void SetValue(string v)
        {
            if (value != null)
                value = v;
        }

        private void Explain<T>(out T v)
        {
            v = default(T);
            try
            {
                if (v is bool)
                    v = (T)Convert.ChangeType(Convert.ToBoolean(value), typeof(T));
                else if (v is char)
                    v = (T)Convert.ChangeType(Convert.ToChar(value), typeof(T));
                else if (v is byte)
                    v = (T)Convert.ChangeType(Convert.ToByte(value), typeof(T));
                else if (v is sbyte)
                    v = (T)Convert.ChangeType(Convert.ToSByte(value), typeof(T));
                else if (v is ushort)
                    v = (T)Convert.ChangeType(Convert.ToUInt16(value), typeof(T));
                else if (v is short)
                    v = (T)Convert.ChangeType(Convert.ToInt16(value), typeof(T));
                else if (v is uint)
                    v = (T)Convert.ChangeType(Convert.ToUInt32(value), typeof(T));
                else if (v is int)
                    v = (T)Convert.ChangeType(Convert.ToInt32(value), typeof(T));
                else if (v is ulong)
                    v = (T)Convert.ChangeType(Convert.ToUInt64(value), typeof(T));
                else if (v is long)
                    v = (T)Convert.ChangeType(Convert.ToInt64(value), typeof(T));
                else if (v is float)
                    v = (T)Convert.ChangeType(Convert.ToSingle(value), typeof(T));
                else if (v is double)
                    v = (T)Convert.ChangeType(Convert.ToDouble(value), typeof(T));
                else if (v is decimal)
                    v = (T)Convert.ChangeType(Convert.ToDecimal(value), typeof(T));
                else
                    v = (T)(object)value;
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to explain value: {0}. Exception: {1}", v.ToString(), e.Message);
                Debug.Log(message);
            }
        }

        private string value;
    }
}