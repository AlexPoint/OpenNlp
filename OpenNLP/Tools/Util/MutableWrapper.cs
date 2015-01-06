using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util
{
    /// <summary>
    /// A wrapper for immutable objects to update their value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MutableWrapper<T>
    {
        private T _value;

        public MutableWrapper(T val)
        {
            this._value = val;
        }

        public void SetValue(T t)
        {
            this._value = t;
        }

        public T Value()
        {
            return _value;
        }
    }
}