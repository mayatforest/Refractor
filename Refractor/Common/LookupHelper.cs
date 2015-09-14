using System.Collections.Generic;

using Refractor.Common;

namespace Refractor
{
    /// <summary>
    /// Lookup helper for short and full ids. Full ids are those fully
    /// resolved with a filename, seperated with a '?'.
    /// </summary>
    public class LookupHelper
    {
        public BaseItem Find(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (_lookup.ContainsKey(id))
                return _lookup[id];

            if (id.Contains(BaseItem.FileSeperator.ToString()))
            {
                string shortId = id.Split('?')[1];

                if (_lookupShort.ContainsKey(shortId))
                    return _lookupShort[shortId];
            }
            else
            {
                if (_lookupShort.ContainsKey(id))
                    return _lookupShort[id];
            }

            return null;
        }
        
        internal LookupHelper(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        internal void Clear()
        {
            _lookup.Clear();
            _lookupShort.Clear();
        }

        internal int GetCount()
        {
            return _lookup.Count;
        }

        internal int GetCountShort()
        {
            return _lookupShort.Count;
        }

        internal void Add(string id, BaseItem item)
        {
            // Add full lookup, with filename.
            if (_lookup.ContainsKey(id))
            {
                _windowManager.Logger.LogStr("Warning : Lookup reports duplicate : " + id);
            }
            else
            {
                _lookup.Add(id, item);
            }


            string[] bits = id.Split(BaseItem.FileSeperator);
            string shortId = null;

            if (bits.Length > 1)
            {
                shortId = bits[1];
            }

            // Add short lookup, no filename.
            if (shortId != null)
            {
                if (_lookupShort.ContainsKey(shortId))
                {
                    _windowManager.Logger.Debug("Lookup reports duplicate shortId : " + shortId);
                }
                else
                {
                    _lookupShort.Add(shortId, item);
                }
            }
        }

        internal void Remove(string id)
        {
            // Add full lookup, with filename.
            if (_lookup.ContainsKey(id))
            {
                _lookup.Remove(id);
            }
            else
            {
                _windowManager.Logger.LogStr("Warning : Lookup reports not found to remove : " + id);
            }

            string[] bits = id.Split(BaseItem.FileSeperator);//todo
            string shortId = null;

            if (bits.Length > 1)
            {
                shortId = bits[1];
            }

            // Add short lookup, no filename.
            if (shortId != null)
            {
                if (_lookupShort.ContainsKey(shortId))
                {
                    _lookupShort.Remove(shortId);
                }
            }
        }
        
        private WindowManager _windowManager;
        private Dictionary<string, BaseItem> _lookup = new Dictionary<string, BaseItem>();
        private Dictionary<string, BaseItem> _lookupShort = new Dictionary<string, BaseItem>();
    }
}
