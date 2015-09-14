using System;
using System.Collections.Generic;
using System.Text;

namespace Hwd.Serialization
{
    public interface ITuple
    {
        object GetKey();
        void SetKey(object value);        

        object GetValue();
        void SetValue(object value);
    }

    public class Tuple<TKey, TValue> : ITuple
    {
        public TKey TupleKey;
        public TValue TupleValue;
        
        public object GetKey() { return TupleKey as object; }
        public void SetKey(object key) { TupleKey = (TKey)key; }
        public object GetValue() { return TupleValue as object; }
        public void SetValue(object value) { TupleValue = (TValue)value; }
    }
}
