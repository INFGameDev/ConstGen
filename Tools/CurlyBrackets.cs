using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace ConstGen
{
    public class CurlyBrackets : IDisposable 
    {
        WrappedInt indentCount;
        StringBuilder content;

        public CurlyBrackets(StringBuilder content_, string header, WrappedInt indentCount_)
        {
            indentCount = indentCount_;
            content = content_;
            content.WriteIndentedLine(indentCount, header);
            content.WriteIndentedLine(indentCount, "{");
            ++indentCount;
        }

        /// <summary>
        /// inserts/writes an indented curly brackets
        /// </summary>
        /// <param name="content_">stringbuilder content</param>
        /// <param name="indentCount_">number of indents</param>
        public CurlyBrackets(StringBuilder content_, WrappedInt indentCount_)
        {
            indentCount = indentCount_;
            content = content_;
            content.WriteIndentedLine(indentCount, "{");
            ++indentCount;
        }

        public void Dispose()
        {
            --indentCount;
            content.WriteIndentedLine(indentCount, "}");
        }
    }

    // WrappedInt pertains to the number of indents 
    // It acts like an int variable rather than class instance with multiple properties
    // and with operator overloads we can assing and retrieve values from it like an int variable
    // alongside with increamenting and decrementing it
    public class WrappedInt
    {
        int value;

        public WrappedInt Store(int num)
        {
            value = num;
            return this;
        }

        public int Load()
        {
            return value;
        }

        /// <summary>
        /// creates the instance and sets the value of the WrappedInt then returns the created instance
        /// </summary>
        /// <param name="val">value to set</param>
        public static implicit operator WrappedInt(int val)
        {
            // create a new instance and store the value inputed
            // then return the instance created
            return new WrappedInt().Store(val);
        }

        /// <summary>
        /// get the value of the WrappedInt
        /// </summary>
        /// <param name="val">value to get</param>
        public static implicit operator int(WrappedInt val)
        {
            return val.Load();
        }

        /// <summary>
        /// Increaments WrappedInt value and returns the instance 
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static WrappedInt operator ++(WrappedInt self)
        {
            self.value++;
            return self;
        }

        public static WrappedInt operator --(WrappedInt self)
        {
            self.value--;
            return self;
        }

    }
}
