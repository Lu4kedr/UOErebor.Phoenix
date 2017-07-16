using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.WallManager
{
    public class WallCollection
    {
        public event EventHandler Changed;
        private List<Wall> _collection;
        private readonly object _lock;

        public WallCollection()
        {
            _collection = new List<Wall>();
            _lock = new object();
        }


        public List<Wall> GetList()
        {
            List<Wall> tmp = null;
            lock (_lock)
            {
                tmp = _collection.ToList();
            }
            return tmp;
        }

        public void Remove(Wall wall)
        {
            lock (_lock)
            {
                _collection.Remove(wall);

                var temp = Changed;
                if (temp != null)
                {
                    foreach (EventHandler ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, null, null, null);
                    }
                }
            }
        }
        public void Add(Wall wall)
        {
            lock (_lock)
            {
                _collection.Add(wall);

                var temp = Changed;
                if (temp != null)
                {
                    foreach (EventHandler ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, null, null, null);
                    }
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _collection.Clear();

                var temp = Changed;
                if (temp != null)
                {
                    foreach (EventHandler ev in temp.GetInvocationList())
                    {
                        ev.BeginInvoke(this, null, null, null);
                    }
                }
            }
        }


    }
}
