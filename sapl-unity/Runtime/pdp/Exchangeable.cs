using System;
using System.Collections.Generic;
using System.Text;

namespace Sapl.Pdp.Api
{
    public class Exchangeable<T>
    {
        public delegate void ExchangedHandler(object source);

        public event ExchangedHandler OnExchanged;

        private T val;

        public T Value
        {
            get
            {
                return val;
            }

            set
            {
                val = value;
                OnExchanged?.Invoke(this);
            }
        }

        public Exchangeable(T val)
        {
            this.val = val;
        }
    }
}
